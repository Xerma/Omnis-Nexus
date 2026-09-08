using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;

namespace OmnisNexus.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public MessageHub(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task JoinChannel(Guid channelId)
        {
            if (await CanAccessChannel(channelId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetChannelGroupName(channelId));
            }
        }

        public Task LeaveChannel(Guid channelId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetChannelGroupName(channelId));
        }

        public static string GetChannelGroupName(Guid channelId)
        {
            return $"channel:{channelId}";
        }

        private async Task<bool> CanAccessChannel(Guid channelId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            await using var db = await _dbContextFactory.CreateDbContextAsync();

            return await db.Channels
                .Where(c => c.Id == channelId)
                .AnyAsync(c => c.Community.Memberships.Any(m => m.UserId == userId));
        }
    }
}
