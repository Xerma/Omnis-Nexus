using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class CommunitySearchService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public CommunitySearchService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public async Task<Dictionary<string, List<SearchItem>>> SearchMessages(Guid communityId, string search)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            List<SearchItem> searchItems = await db.Messages
                .Where(m => m.Content.Contains(search) && m.Channel.CommunityId == communityId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new SearchItem
                {
                    MessageId = m.Id,
                    ChannelId = m.ChannelId,
                    ChannelName = m.Channel.Name,
                    AuthorName = m.User.UserName,
                    MessageContent = m.Content,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = (m.UpdatedAt == null) ? null : m.UpdatedAt
                })
                .ToListAsync();

            Dictionary<string, List<SearchItem>> searchItemDict = searchItems
                .GroupBy(item => item.ChannelName)
                .ToDictionary(group => group.Key, group => group
                .ToList());

            return searchItemDict;
        }
    }
}
