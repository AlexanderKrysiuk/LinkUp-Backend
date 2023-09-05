namespace LinkUpBackend.Models;
public class MeetingParticipant{
    public Guid ParticipantId {get;set;}
    public Guid MeetingId {get;set;}
    public string? Description {get;set;}
}