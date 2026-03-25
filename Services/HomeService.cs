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

        public async Task<List<CommunityListItem>> GetAllCommunitiesAsync()
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            return await db.Communities
            .Select(c => new CommunityListItem
            {
                Id = c.Id,
                Name = c.Name,
                MemberCount = c.Memberships.Count()
            })
            .ToListAsync();
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

        public async Task<string?> JoinOrNavCommunity(Guid communityId, string? userId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            Membership? membership = await db.Memberships.FirstOrDefaultAsync(m => m.UserId == userId && m.CommunityId == communityId);

            if (membership == null)
            {
                Community? community = await db.Communities.FirstOrDefaultAsync(c => c.Id == communityId);

                if (userId == null || community == null) return null;

                Membership m = new()
                {
                    UserId = userId,
                    Community = community,
                    Role = Roles.Member,
                    JoinedAt = DateTime.UtcNow
                };

                await db.Memberships.AddAsync(m);
                db.SaveChanges();
                return communityId.ToString();
            }
            return communityId.ToString();
        }
    }
}
