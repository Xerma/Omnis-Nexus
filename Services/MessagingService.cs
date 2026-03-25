using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class MessagingService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public MessagingService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public async Task SendMessageAsync(Guid currChannelId, string messageText, string userId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            Message message = new()
            {
                ChannelId = currChannelId,
                UserId = userId,
                Content = messageText.Trim()
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();
        }

        public async Task<bool> SaveEditAsync(Guid messageId, string editedMessageText)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();
            Message? m = await db.Messages.FindAsync(messageId);

            string newText = editedMessageText.Trim();
            DateTime now = DateTime.UtcNow;

            if (m is null || string.IsNullOrWhiteSpace(newText)) return false;
            if (m.Content == newText) return false;

            m.Content = newText;
            m.UpdatedAt = now;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();
            Message? m = await db.Messages.FindAsync(messageId);

            if (m is null) return;

            db.Remove(m);
            await db.SaveChangesAsync();
        }
    }
}
