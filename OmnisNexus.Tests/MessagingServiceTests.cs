using Microsoft.EntityFrameworkCore;
using OmnisNexus.Models;
using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class MessagingServiceTests
    {
        [Fact]
        public async Task SendMessageAsync_Success()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var service = new MessagingService(factory);

            await service.SendMessageAsync(Guid.NewGuid(), "text", "userId");
            await using var verifyDb = factory.CreateDbContext();
            bool message = verifyDb.Messages.Any();

            Assert.True(message);
        }

        [Fact]
        public async Task SaveEditAsync_Success()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var service = new MessagingService(factory);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "user",
                Content = "content",
                CreatedAt = DateTime.Now
            };
            db.Messages.Add(message);
            await db.SaveChangesAsync();

            bool messageEdited = await service.SaveEditAsync(message.Id, "new text");
            await using var verifyDb = factory.CreateDbContext();
            var editedMessage = await verifyDb.Messages.FindAsync(message.Id);

            Assert.NotNull(editedMessage);
            Assert.True(messageEdited);
            Assert.NotEqual(message.Content, editedMessage.Content);
        }

        [Fact]
        public async Task SaveEditAsync_SameContent()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var service = new MessagingService(factory);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "user",
                Content = "content",
                CreatedAt = DateTime.Now
            };
            db.Messages.Add(message);
            await db.SaveChangesAsync();

            bool messageEdited = await service.SaveEditAsync(message.Id, message.Content);
            await using var verifyDb = factory.CreateDbContext();
            var editedMessage = await verifyDb.Messages.FindAsync(message.Id);

            Assert.NotNull(editedMessage);
            Assert.False(messageEdited);
        }

        [Fact]
        public async Task DeleteMessageAsync_Success()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var service = new MessagingService(factory);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "user",
                Content = "content",
                CreatedAt = DateTime.Now
            };
            db.Messages.Add(message);
            await db.SaveChangesAsync();

            await service.DeleteMessageAsync(message.Id);
            await using var verifyDb = factory.CreateDbContext();
            bool messageExists = verifyDb.Messages.Any();

            Assert.False(messageExists);
        }

        [Fact]
        public async Task DeleteMessageAsync_MessageDoesntExist()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var service = new MessagingService(factory);

            await service.DeleteMessageAsync(Guid.NewGuid());
            bool messageExists = db.Messages.Any();

            Assert.False(messageExists);
        }
    }
}
