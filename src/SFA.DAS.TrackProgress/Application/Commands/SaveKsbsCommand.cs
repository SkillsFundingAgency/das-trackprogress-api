using MediatR;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands;

public record SaveKsbsCommand(Ksb[] Ksbs) : IRequest;
public record Ksb(string Id, string Description);

public class SaveKsbsCommandHandler : IRequestHandler<SaveKsbsCommand>
{
    private readonly TrackProgressContext _context;

    public SaveKsbsCommandHandler(TrackProgressContext context) => _context = context;

    public async Task<Unit> Handle(SaveKsbsCommand request, CancellationToken cancellationToken)
    {
        foreach (var x in request.Ksbs)
        {
            var ksb = _context.KsbCache.Find(x.Id);
            if (ksb == null)
            {
                ksb = new KsbName(x.Id.ToString(), x.Description);
                _context.KsbCache.Add(ksb);
            }
            else
            {
                ksb.Name = x.Description;
                _context.KsbCache.Update(ksb);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}