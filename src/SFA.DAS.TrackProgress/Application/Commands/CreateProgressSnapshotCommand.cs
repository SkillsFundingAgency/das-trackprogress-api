using MediatR;
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

        var es = events.SelectMany(x => x.ProgressData.Ksbs);
        var m = es.Select(x => new SnapshotDetail(x.Id, x.Value)).ToList() ?? new();

        var g = m.GroupBy(x => x.KsbId);
        var h = g.Select(x => x.Last()).ToList();

        var entity = new Snapshot(new ApprovalId(request.CommitmentsApprenticeshipId, null), h);

        _context.Snapshot.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}