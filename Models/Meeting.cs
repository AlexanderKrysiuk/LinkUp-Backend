using static LinkUpBackend.ServiceErrors.Errors;

namespace LinkUpBackend.Models;

public static class MeetingExtensions
{
    public static List<Meeting> GetArchived(this List<Meeting> meetings) {
        return meetings.Where(meeting => meeting.DateTime.AddMinutes(meeting.Duration) < DateTime.Now.ToUniversalTime()).ToList();
    }
    public static List<Meeting> GetUpcoming(this List<Meeting> meetings)
    {
        return meetings.Where(meeting => meeting.DateTime.AddMinutes(meeting.Duration) >= DateTime.Now.ToUniversalTime()).ToList();
    }
}

public class Meeting{
    public Guid Id {get;set;}
    public DateTime DateTime {get;set;}
    public int MaxParticipants {get;set;}
    public int Duration {get;set;}
    public string? Description {get;set;}
}