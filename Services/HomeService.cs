using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class HomeService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public HomeService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public async Task<HomeStats> GetStatsAsync(string userId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();
            return new HomeStats
            {
                CommunitiesJoined = await db.Communities
                .Where(c => c.Memberships.Any(m => m.UserId == userId))
                .CountAsync(),

                MessagesSent = await db.Messages
                    .Where(m => m.UserId == userId)
                    .CountAsync(),

                CommunitiesOwned = await db.Communities
                    .Where(c => c.OwnerId == userId)
                    .CountAsync(),

                MessagesEdited = await db.Messages
                    .Where(m => m.UpdatedAt != null)
                    .Where(m => m.UserId == userId)
                    .CountAsync()
            };
        }
    }
}
