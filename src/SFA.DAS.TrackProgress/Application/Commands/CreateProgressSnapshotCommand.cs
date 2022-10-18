﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands;

public record CreateProgressSnapshotCommand(long CommitmentsApprenticeshipId) : IRequest;

public class CreateProgressSnapshotCommandHandler : IRequestHandler<CreateProgressSnapshotCommand>
{
    private readonly TrackProgressContext _context;

    public CreateProgressSnapshotCommandHandler(TrackProgressContext context) => _context = context;

    public async Task<Unit> Handle(CreateProgressSnapshotCommand request, CancellationToken cancellationToken)
    {
        var events = await _context.Progress
            .Where(x => x.Approval.ApprenticeshipId == request.CommitmentsApprenticeshipId)
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

        var entity = new Snapshot(new ApprovalId(request.CommitmentsApprenticeshipId, null), allSnapshotDetails);

        _context.Snapshot.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}