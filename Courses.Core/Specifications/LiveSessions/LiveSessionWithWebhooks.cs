using Courses.Core.Models.LiveSessions;

namespace Courses.Core.Specifications.LiveSessions
{
    public class LiveSessionWithWebHooks : BaseSpecifications<LiveSession>
    {
        public LiveSessionWithWebHooks(string meetingId)
            :base(x => x.ZoomMeetingId == meetingId)
        {
            
        }
    }
}
