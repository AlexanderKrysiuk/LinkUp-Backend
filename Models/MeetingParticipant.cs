namespace LinkUpBackend.Models;
public class MeetingParticipant{
    public string? ParticipantId { get; set; }
    public Guid MeetingId { get; set; }

    public User? Participant { get; set; }
    public Meeting? Meeting { get; set; }
}