using Microsoft.EntityFrameworkCore;

namespace LinkUpBackend.Services
{
    public class MeetingsService
    {
        private DbContext _dbContext;
        public MeetingsService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
    }
}
