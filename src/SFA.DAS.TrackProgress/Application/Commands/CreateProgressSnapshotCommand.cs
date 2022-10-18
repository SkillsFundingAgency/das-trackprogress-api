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
            .ToListAsync(cancellationToken);

        var e = events.FirstOrDefault()?.ProgressData;
        var m = e?.Ksbs.Select(x => new SnapshotDetail(x.Id, x.Value)).ToList() ?? new();

        var entity = new Snapshot(new ApprovalId(request.CommitmentsApprenticeshipId, null), m);

        _context.Snapshot.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}