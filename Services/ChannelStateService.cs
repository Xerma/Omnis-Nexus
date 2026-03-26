using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;
using System.Security.Claims;

namespace OmnisNexus.Services
{
    public class ChannelStateService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ChannelStateService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public event Action? OnChange;

        public List<Channel> Channels { get; private set; } = new();

        public void SetChannels(List<Channel> channels)
        {
            Channels = channels;
            NotifyStateChanged();
        }

        public void NotifyStateChanged() => OnChange?.Invoke();

        public async Task LoadChannelsAsync(AuthenticationStateProvider authProvider, Guid comId)
        {
            var authState = await authProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return;

            using var db = await _dbContextFactory.CreateDbContextAsync();

            var channels = await db.Channels
                .Where(c => c.CommunityId == comId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            SetChannels(channels);
        }

        public Channel? GetChannel(Guid channelId)
        {
            return Channels.FirstOrDefault(c => c.Id == channelId);
        }

        public async Task UpdateChannelNameAsync(Guid channelId, string newName)
        {
            var channel = GetChannel(channelId);
            if (channel is null) return;

            channel.Name = newName;
            NotifyStateChanged();
        }
    }
}
