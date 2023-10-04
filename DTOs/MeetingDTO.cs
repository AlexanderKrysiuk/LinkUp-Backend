using LinkUpBackend.Models;

namespace LinkUpBackend.DTOs
{
    public class MeetingDTO
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public int MaxParticipants { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public bool AlreadyJoined { get; set; }

        public MeetingDTO(Meeting meeting, bool alredyJoined)
        {
            Id = meeting.Id;
            DateTime = meeting.DateTime;
            MaxParticipants = meeting.MaxParticipants;
            Duration = meeting.Duration;
            Description = meeting.Description;
            AlreadyJoined = alredyJoined;
        }
    }
}
