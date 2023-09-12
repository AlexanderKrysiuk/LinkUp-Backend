﻿using LinkUpBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkUpBackend.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public override DbSet<Role> Roles => Set<Role>();
        public DbSet<Meeting> Meetings {get;set;}
        public DbSet<MeetingOrganizator> MeetingsOrganizators {get;set;}

        public DbSet<MeetingParticipant> MeetingsParticipants { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Role>().HasData(new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid().ToString(), Name = "Client", NormalizedName = "CLIENT"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(), Name = "Contractor", NormalizedName = "CONTRACTOR"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN"
                },
            });

            builder.Entity<MeetingOrganizator>()
                .HasKey(x => new { x.OrganizatorId, x.MeetingId });

            builder.Entity<MeetingOrganizator>()
                .HasOne(x => x.Organizator)
                .WithMany()
                .HasForeignKey(x => x.OrganizatorId);

            builder.Entity<MeetingOrganizator>()
                .HasOne(x => x.Meeting)
                .WithMany()
                .HasForeignKey(x => x.MeetingId);

            builder.Entity<MeetingParticipant>().HasKey(x => new { x.ParticipantId, x.MeetingId });
            builder.Entity<MeetingParticipant>()
                .HasOne(x => x.Participant)
                .WithMany()
                .HasForeignKey(x => x.ParticipantId);

            builder.Entity<MeetingParticipant>()
                .HasOne(x => x.Meeting)
                .WithMany()
                .HasForeignKey(x => x.MeetingId);
        }
    // protected override void OnModelCreating(ModelBuilder modelBuilder){
    // modelBuilder.Entity<MeetingOrganisatorDTO>().HasNoKey();
    // }
    }
}
