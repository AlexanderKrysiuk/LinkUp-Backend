namespace LinkUpBackend.DTOs;
public class AddMeetingRequestDTO{
    public Guid OrganizatorId {get;set;}
    public DateTime DateTime {get;set;}
    public int MaxParticipants {get;set;}
    public int Duration {get;set;}
    public string? Description {get;set;}
}