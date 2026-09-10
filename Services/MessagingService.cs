using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class MessagingService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IPermissionService _permissionService;

        public MessagingService(IDbContextFactory<ApplicationDbContext> dbFactory, IPermissionService permissionService)
        {
            _dbContextFactory = dbFactory;
            _permissionService = permissionService;
        }

        public async Task<MessageDto?> SendMessageAsync(Guid currChannelId, string messageText, string userId)
        {
            string newText = messageText.Trim();
            if (string.IsNullOrWhiteSpace(newText) || string.IsNullOrWhiteSpace(userId)) return null;

            await using var db = await _dbContextFactory.CreateDbContextAsync();

            Channel? channel = await db.Channels
                .Include(c => c.Community)
                .FirstOrDefaultAsync(c => c.Id == currChannelId);

            if (channel is null) return null;

            Membership? membership = await db.Memberships.FirstOrDefaultAsync(m =>
                m.UserId == userId &&
                m.CommunityId == channel.CommunityId);

            if (membership is null) return null;

            Message message = new()
            {
                ChannelId = currChannelId,
                UserId = userId,
                Content = newText
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            ApplicationUser? user = await db.Users.FindAsync(userId);
            return ToDto(message, user);
        }

        public async Task<MessageDto?> SaveEditAsync(Guid messageId, string editedMessageText, string userId)
        {
            string newText = editedMessageText.Trim();
            if (string.IsNullOrWhiteSpace(newText) || string.IsNullOrWhiteSpace(userId)) return null;

            await using var db = await _dbContextFactory.CreateDbContextAsync();
            Message? m = await db.Messages
                .Include(m => m.Channel)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (m is null) return null;
            if (m.Content == newText) return null;

            Membership? membership = await db.Memberships.FirstOrDefaultAsync(mem =>
                mem.UserId == userId &&
                mem.CommunityId == m.Channel.CommunityId);

            if (!_permissionService.CanEditMessage(membership, m)) return null;

            m.Content = newText;
            m.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return ToDto(m, m.User);
        }

        public async Task<DeletedMessageDto?> DeleteMessageAsync(Guid messageId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            await using var db = await _dbContextFactory.CreateDbContextAsync();
            Message? m = await db.Messages
                .Include(m => m.Channel)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (m is null) return null;

            Membership? membership = await db.Memberships.FirstOrDefaultAsync(mem =>
                mem.UserId == userId &&
                mem.CommunityId == m.Channel.CommunityId);

            if (!_permissionService.CanDeleteMessage(membership, m)) return null;

            DeletedMessageDto deletedMessage = new()
            {
                Id = m.Id,
                ChannelId = m.ChannelId
            };

            db.Remove(m);
            await db.SaveChangesAsync();
            return deletedMessage;
        }

        private static MessageDto ToDto(Message message, ApplicationUser? user)
        {
            return new MessageDto
            {
                Id = message.Id,
                ChannelId = message.ChannelId,
                UserId = message.UserId,
                UserName = user?.UserName ?? user?.Email ?? message.UserId,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }
    }
}
