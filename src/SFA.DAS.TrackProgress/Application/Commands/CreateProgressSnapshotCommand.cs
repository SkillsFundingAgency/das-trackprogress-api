using MediatR;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.Infrastructure;
using SFA.DAS.TrackProgress.Messages.Commands;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands;

public record CreateProgressSnapshotCommand(long CommitmentsApprenticeshipId) : IRequest;

public class CreateProgressSnapshotCommandHandler : IRequestHandler<CreateProgressSnapshotCommand>, IRequiresTransaction
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
        var existingSnapshot = await _context.Snapshot.FirstOrDefaultAsync(x =>
                x.Approval.ApprenticeshipId == request.CommitmentsApprenticeshipId);
        if (existingSnapshot != null)
        {
            _context.Snapshot.Remove(existingSnapshot);
        }

        var snapshot = await BuildSnapshot(
            request.CommitmentsApprenticeshipId, _context.Progress, cancellationToken);

        await SaveSnapshot(snapshot, cancellationToken);

        await CacheKsbNames(snapshot, cancellationToken);

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

        var progressEvent = events.First();

        var approval = new ApprovalId(
            commitmentsApprenticeshipId,
            progressEvent.Approval.ApprenticeshipContinuationId);

        var entity = new Snapshot(approval, allSnapshotDetails);

        return new(progressEvent.StandardUid, entity);
    }

    private async Task SaveSnapshot(CourseSnapshot snapshot, CancellationToken cancellationToken)
    {
        _context.Snapshot.Add(snapshot.Progress);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task CacheKsbNames(CourseSnapshot snapshot, CancellationToken cancellationToken)
    {
        var ksbIds = snapshot.Progress.Details.Select(x => x.KsbId).ToList();

        var numKsbsCached = await _context.KsbCache
            .Where(x => ksbIds.Contains(x.Id))
            .CountAsync(cancellationToken);

        if (numKsbsCached != ksbIds.Count)
            await SendCacheCommand(snapshot);
    }

    private async Task SendCacheCommand(CourseSnapshot snapshot)
    {
        await _messageSession.Send(new CacheKsbsCommand
        {
            StandardUid = snapshot.StandardUid,
        });
    }

    private sealed record CourseSnapshot(string StandardUid, Snapshot Progress);
}