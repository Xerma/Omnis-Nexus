using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class CommunityService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private CommunityStateService _communityState;

        public CommunityService(IDbContextFactory<ApplicationDbContext> dbFactory, CommunityStateService communityState)
        {
            _dbContextFactory = dbFactory;
            _communityState = communityState;
        }

        public async Task<bool> UpdateMemberRoleAsync(string userId, Guid communityId, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole)) return false;

            using var db = await _dbContextFactory.CreateDbContextAsync();
            Membership? currMembership = await db.Memberships.FirstOrDefaultAsync(m => m.UserId == userId && m.CommunityId == communityId);

            if (currMembership == null || currMembership.Role == Roles.Owner) return false;

            currMembership.Role = newRole;
            await db.SaveChangesAsync();

            return true;
        }

        public async Task SaveCommunityEditsAsync(Community com, string newComName)
        {
            if (com == null || string.IsNullOrWhiteSpace(newComName)) return;

            using var db = _dbContextFactory.CreateDbContext();
            Community? c = await db.Communities.FindAsync(com.Id);

            if (c != null && !string.IsNullOrWhiteSpace(newComName))
            {
                c.Name = newComName.Trim();
                await db.SaveChangesAsync();
                _communityState.UpdateCommunityName(c.Id, newComName.Trim());
            }
        }

        public async Task<string?> DeleteCommunityAsync(Community community, Guid activeComId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            Community? dbCom = await db.Communities.FindAsync(community.Id);
            if (dbCom == null) return null;

            db.Communities.Remove(dbCom);
            await db.SaveChangesAsync();
            _communityState.RemoveCommunity(community);

            if (community.Id == activeComId) return "";
            return null;
        }

        public async Task<string?> LeaveCommunityAsync(Community community, Guid activeComId, string userId)
        {
            using var db = _dbContextFactory.CreateDbContext();

            if (community == null) return null;

            Membership? membership = await db.Memberships.FirstOrDefaultAsync(m =>
                m.CommunityId == community.Id && m.UserId == userId);

            if (membership == null) return null;

            db.Memberships.Remove(membership);
            await db.SaveChangesAsync();
            _communityState.DecrementMemberCount(community.Id);
            if (community.Id == activeComId) return "";

            return null;
        }

        public Guid SetActiveCommunityFromUrl(string relativePath)
        {
            string[] parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2 && parts[0].Equals("channels", StringComparison.OrdinalIgnoreCase) &&
            Guid.TryParse(parts[1], out Guid communityId))
            {
                return communityId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public async Task<Guid[]> CreateCommunityAsync(ApplicationUser? user)
        {
            Guid[] guidArray = new Guid[2];
            using var db = await _dbContextFactory.CreateDbContextAsync();

            if (user == null) return guidArray;

            Community community = new()
            {
                Name = $"{user!.UserName}'s Community",
                Description = "This is a new community created at " + DateTime.Now.ToString("HH:mm:ss"),
                OwnerId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            Membership membership = new()
            {
                UserId = user.Id,
                Community = community,
                Role = Roles.Owner,
                JoinedAt = DateTime.UtcNow
            };

            db.Communities.Add(community);
            db.Memberships.Add(membership);
            await db.SaveChangesAsync();
            _communityState.AddCommunityToAllCommunities(community);

            Channel channel = new()
            {
                Name = "General",
                CommunityId = community.Id,
                Community = community,
                CreatedAt = DateTime.UtcNow
            };
            db.Channels.Add(channel);
            await db.SaveChangesAsync();

            guidArray[0] = community.Id;
            guidArray[1] = channel.Id;
            return guidArray;
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
                await db.SaveChangesAsync();
                _communityState.IncrementMemberCount(communityId);
                return communityId.ToString();
            }
            return communityId.ToString();
        }
    }
}
