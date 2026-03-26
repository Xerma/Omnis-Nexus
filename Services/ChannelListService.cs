using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;
using System.Security.Claims;

namespace OmnisNexus.Services
{
    public class ChannelListService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ChannelListService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public async Task<Guid> CreateChannelAsync(string name, Guid comId)
        {
            if (IsChannelNameValid(name))
            {
                using var db = await _dbContextFactory.CreateDbContextAsync();

                Community? community = await db.Communities.FirstOrDefaultAsync(c => c.Id == comId);

                if (community == null) return Guid.Empty;

                Channel channel = new()
                {
                    Name = name,
                    CommunityId = comId,
                    Community = community
                };

                db.Channels.Add(channel);
                await db.SaveChangesAsync();

                return channel.Id;
            }
            return Guid.Empty;
        }

        public async Task SaveChannelEditsAsync(Channel channel, string newChannelName)
        {
            using var db = _dbContextFactory.CreateDbContext();
            Channel? c = await db.Channels.FindAsync(channel.Id);

            if (c != null && IsChannelNameValid(newChannelName))
            {
                c.Name = newChannelName.Trim();
                await db.SaveChangesAsync();
                newChannelName = "";
            }
        }

        public async Task<int> GetChannelCount(Guid comId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return await db.Channels
                .Where(c => c.CommunityId == comId)
                .CountAsync();
        }

        public async Task<Guid> DeleteChannelAsync(Channel? target, Guid currChanId, Guid comId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            Channel? channel = await db.Channels.FindAsync(target?.Id);

            if (channel == null) return Guid.Empty;

            db.Channels.Remove(channel);
            await db.SaveChangesAsync();

            if (channel.Id == currChanId)
            {
                return comId;
            }

            return Guid.Empty;
        }

        public Guid SetActiveChannelFromUrl(string relativePath)
        {
            string[] parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 3 && parts[0].Equals("channels", StringComparison.OrdinalIgnoreCase) &&
            Guid.TryParse(parts[2], out Guid channelId))
            {
                return channelId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        private bool IsChannelNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length <= 50;
        }

    }
}
