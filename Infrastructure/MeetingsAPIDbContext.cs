using LinkUpBackend.DTOs;
using Microsoft.EntityFrameworkCore;
namespace LinkUpBackend.Infrastructure;
public class MeetingsAPIDbContext : DbContext{
    public MeetingsAPIDbContext(DbContextOptions options):base(options){
    }
    public DbSet<MeetingDTO> Meetings {get;set;}
}