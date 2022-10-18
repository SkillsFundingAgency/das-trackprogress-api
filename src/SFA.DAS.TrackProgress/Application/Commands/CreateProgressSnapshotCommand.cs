using MediatR;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Application.Commands;

public record CreateProgressSnapshotCommand(long CommitmentsApprenticeshipId) : IRequest;

public class CreateProgressSnapshotCommandHandler : IRequestHandler<CreateProgressSnapshotCommand>
{
    private readonly TrackProgressContext _context;

    public CreateProgressSnapshotCommandHandler(TrackProgressContext context) => _context = context;

    public async Task<Unit> Handle(CreateProgressSnapshotCommand request, CancellationToken cancellationToken)
    {
        _context.Snapshot.Add(new Models.Snapshot(new Models.ApprovalId(request.CommitmentsApprenticeshipId, null)));
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}