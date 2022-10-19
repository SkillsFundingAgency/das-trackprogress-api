﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.Messages.Commands;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands;

public record CreateProgressSnapshotCommand(long CommitmentsApprenticeshipId) : IRequest;

public class CreateProgressSnapshotCommandHandler : IRequestHandler<CreateProgressSnapshotCommand>
{
    private readonly TrackProgressContext _context;
    private readonly IMessageSession _messageSession;

    public CreateProgressSnapshotCommandHandler(TrackProgressContext context, IMessageSession messageSession)
    {
        _context = context;
        _messageSession = messageSession;
    }

    public async Task<Unit> Handle(CreateProgressSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await BuildSnapshot(
            request.CommitmentsApprenticeshipId, _context.Progress, cancellationToken);

        _context.Snapshot.Add(snapshot.Progress);
        await _context.SaveChangesAsync(cancellationToken);

        await _messageSession.Publish(new CacheKsbsCommand
        {
            CommitmentsApprenticeshipId = request.CommitmentsApprenticeshipId,
            StandardUid = snapshot.StandardUid,
        });

        return Unit.Value;
    }

    private static async Task<CourseSnapshot> BuildSnapshot(
        long commitmentsApprenticeshipId,
        IQueryable<Progress> progress,
        CancellationToken cancellationToken)
    {
        var events = await progress
            .Where(x => x.Approval.ApprenticeshipId == commitmentsApprenticeshipId)
            .OrderBy(x => x.CreatedOn)
            .ToListAsync(cancellationToken);

        var allKsbSubmissions = events
            .SelectMany(x => x.ProgressData.Ksbs)
            .GroupBy(x => x.Id);

        var latestKsbSubmissions = allKsbSubmissions
            .Select(x => x.Last());

        var allSnapshotDetails = latestKsbSubmissions
            .Select(x => new SnapshotDetail(x.Id, x.Value))
            .ToList();

        var progress1 = events.FirstOrDefault();

        var approval = new ApprovalId(
            commitmentsApprenticeshipId,
            progress1?.Approval.ApprenticeshipContinuationId);

        var entity = new Snapshot(approval, allSnapshotDetails);

        return new(progress1!.StandardUid, entity);
    }

    private record CourseSnapshot(string StandardUid, Snapshot Progress);
}