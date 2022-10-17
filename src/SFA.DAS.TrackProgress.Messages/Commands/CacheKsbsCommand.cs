using System;

namespace SFA.DAS.TrackProgress.Messages.Commands
{
    public class CacheKsbsCommand
    {
        public long CourseId { get; set; }
        public Guid[] KsbIds { get; set; }
    }
}