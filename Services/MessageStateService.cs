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

        public List<Message> Messages { get; private set; } = new();

        public void SetMessages(List<Message> messages)
        {
            Messages = messages;
            NotifyStateChanged();
        }

        public void NotifyStateChanged() => OnChange?.Invoke();

        public async Task LoadMessagesAsync(Guid currChannelId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            var messages = await db.Messages
            .Include(m => m.User)
            .Where(m => m.ChannelId == currChannelId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

            SetMessages(messages);
        }
    }
}
