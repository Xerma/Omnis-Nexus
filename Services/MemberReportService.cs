using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class MemberReportService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public MemberReportService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<MemberReportItem>> GetMemberReport(Guid communityId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            return await db.Memberships
                .Where(m => m.CommunityId == communityId)
                .OrderByDescending(m => m.JoinedAt)
                .Select(m => new MemberReportItem
                {
                    UserId = m.UserId,
                    Username = m.User.UserName,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt,
                    MessageCount = db.Messages
                    .Count(msg => msg.UserId == m.UserId
                        && msg.Channel.CommunityId == communityId)
                })
                .ToListAsync();
        }
    }
}
