using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;
using System.Security.Claims;

namespace OmnisNexus.Services
{
    public class MessageStateService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public MessageStateService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public event Action? OnChange;

        public List<MessageDto> Messages { get; private set; } = new();

        public void SetMessages(List<MessageDto> messages)
        {
            Messages = messages;
            NotifyStateChanged();
        }

        public void AddMessage(MessageDto message)
        {
            if (Messages.Any(m => m.Id == message.Id)) return;

            Messages.Add(message);
            Messages = Messages.OrderBy(m => m.CreatedAt).ToList();
            NotifyStateChanged();
        }

        public void UpdateMessage(MessageDto message)
        {
            int index = Messages.FindIndex(m => m.Id == message.Id);
            if (index < 0) return;

            Messages[index] = message;
            NotifyStateChanged();
        }

        public void RemoveMessage(Guid messageId)
        {
            int removed = Messages.RemoveAll(m => m.Id == messageId);
            if (removed > 0)
            {
                NotifyStateChanged();
            }
        }

        public void NotifyStateChanged() => OnChange?.Invoke();

        public async Task LoadMessagesAsync(Guid currChannelId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            var messages = await db.Messages
            .Include(m => m.User)
            .Where(m => m.ChannelId == currChannelId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ChannelId = m.ChannelId,
                UserId = m.UserId,
                UserName = m.User.UserName ?? m.User.Email ?? m.UserId,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

            SetMessages(messages);
        }
    }
}
