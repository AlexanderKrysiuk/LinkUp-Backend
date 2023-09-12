namespace LinkUpBackend.DTOs
{
    public class RescheduleMeetingDTO
    {
        public Guid OldMeetingId { get; set; }
        public Guid NewMeetingId { get; set; }
    }
}
