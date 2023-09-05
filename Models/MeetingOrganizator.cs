using LinkUpBackend.Domain;
namespace LinkUpBackend.Models;
public class MeetingOrganizator
{
    public string? OrganizatorId { get; set; }
    public Guid MeetingId { get; set; }

    public User? Organizator { get; set; }
    public Meeting? Meeting { get; set; }
}