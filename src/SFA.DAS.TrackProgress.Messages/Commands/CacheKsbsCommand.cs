using System;

namespace SFA.DAS.TrackProgress.Messages.Commands
{
    public class CacheKsbsCommand
    {
        public string Standard{ get; set; }
        public Guid[] KsbIds { get; set; }
    }
}