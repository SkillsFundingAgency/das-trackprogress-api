using System;

namespace SFA.DAS.TrackProgress.Messages.Commands
{
    public class CacheKsbsCommand
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public Guid[] KsbIds { get; set; }
    }
}