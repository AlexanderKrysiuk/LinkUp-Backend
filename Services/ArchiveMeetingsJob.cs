using LinkUpBackend.Infrastructure;
using LinkUpBackend.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;

[DisallowConcurrentExecution]
public class ArchiveMeetingsJob : IJob
{
    private readonly AppDbContext dbContext;

    public ArchiveMeetingsJob(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task Execute(IJobExecutionContext context)
    {
        DateTime currentDate = DateTime.Now.ToUniversalTime();

        var meetingsToArchive = dbContext.Meetings
            .Where(m => m.DateTime.ToUniversalTime().AddMinutes(m.Duration) < currentDate)
            .ToList();

        foreach (var meeting in meetingsToArchive)
        {
            var archivedMeeting = new ArchiveMeeting
            {
                Id = meeting.Id,
                DateTime = meeting.DateTime.ToUniversalTime(),
                Description = meeting.Description
            };

            dbContext.Meetings.Remove(meeting);

            dbContext.Archive.Add(archivedMeeting);
            
        }

        dbContext.SaveChanges();

        return Task.CompletedTask;
    }
}
