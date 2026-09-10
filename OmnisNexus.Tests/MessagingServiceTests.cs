using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
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
            var seed = await SeedChannelMemberAsync(db);
            var service = CreateService(factory);

            MessageDto? message = await service.SendMessageAsync(seed.ChannelId, "text", seed.UserId);
            await using var verifyDb = factory.CreateDbContext();
            bool messageExists = verifyDb.Messages.Any();

            Assert.NotNull(message);
            Assert.True(messageExists);
            Assert.Equal("text", message.Content);
            Assert.Equal(seed.ChannelId, message.ChannelId);
        }

        [Fact]
        public async Task SendMessageAsync_UserIsNotCommunityMember()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var seed = await SeedChannelMemberAsync(db);
            var service = CreateService(factory);

            MessageDto? message = await service.SendMessageAsync(seed.ChannelId, "text", "otherUser");
            await using var verifyDb = factory.CreateDbContext();
            bool messageExists = verifyDb.Messages.Any();

            Assert.Null(message);
            Assert.False(messageExists);
        }

        [Fact]
        public async Task SaveEditAsync_Success()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var seed = await SeedChannelMemberAsync(db);
            var service = CreateService(factory);
            var message = await SeedMessageAsync(db, seed.ChannelId, seed.UserId);

            MessageDto? messageEdited = await service.SaveEditAsync(message.Id, "new text", seed.UserId);
            await using var verifyDb = factory.CreateDbContext();
            var editedMessage = await verifyDb.Messages.FindAsync(message.Id);

            Assert.NotNull(messageEdited);
            Assert.NotNull(editedMessage);
            Assert.Equal("new text", editedMessage.Content);
            Assert.Equal("new text", messageEdited.Content);
            Assert.NotNull(messageEdited.UpdatedAt);
        }

        [Fact]
        public async Task SaveEditAsync_SameContent()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var seed = await SeedChannelMemberAsync(db);
            var service = CreateService(factory);
            var message = await SeedMessageAsync(db, seed.ChannelId, seed.UserId);

            MessageDto? messageEdited = await service.SaveEditAsync(message.Id, message.Content, seed.UserId);
            await using var verifyDb = factory.CreateDbContext();
            var editedMessage = await verifyDb.Messages.FindAsync(message.Id);

            Assert.Null(messageEdited);
            Assert.NotNull(editedMessage);
            Assert.Null(editedMessage.UpdatedAt);
        }

        [Fact]
        public async Task SaveEditAsync_UserIsNotAuthor()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var seed = await SeedChannelMemberAsync(db);
            var otherUserId = await SeedMemberAsync(db, seed.CommunityId, "otherUser", Roles.Member);
            var service = CreateService(factory);
            var message = await SeedMessageAsync(db, seed.ChannelId, seed.UserId);

            MessageDto? messageEdited = await service.SaveEditAsync(message.Id, "new text", otherUserId);
            await using var verifyDb = factory.CreateDbContext();
            var editedMessage = await verifyDb.Messages.FindAsync(message.Id);

            Assert.Null(messageEdited);
            Assert.NotNull(editedMessage);
            Assert.Equal("content", editedMessage.Content);
        }

        [Fact]
        public async Task DeleteMessageAsync_Success()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var seed = await SeedChannelMemberAsync(db);
            var service = CreateService(factory);
            var message = await SeedMessageAsync(db, seed.ChannelId, seed.UserId);

            DeletedMessageDto? deletedMessage = await service.DeleteMessageAsync(message.Id, seed.UserId);
            await using var verifyDb = factory.CreateDbContext();
            bool messageExists = verifyDb.Messages.Any();

            Assert.NotNull(deletedMessage);
            Assert.Equal(message.Id, deletedMessage.Id);
            Assert.Equal(seed.ChannelId, deletedMessage.ChannelId);
            Assert.False(messageExists);
        }

        [Fact]
        public async Task DeleteMessageAsync_MessageDoesntExist()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var seed = await SeedChannelMemberAsync(db);
            var service = CreateService(factory);

            DeletedMessageDto? deletedMessage = await service.DeleteMessageAsync(Guid.NewGuid(), seed.UserId);
            bool messageExists = db.Messages.Any();

            Assert.Null(deletedMessage);
            Assert.False(messageExists);
        }

        private static MessagingService CreateService(TestDbContextFactory factory)
        {
            return new MessagingService(factory, new PermissionService());
        }

        private static async Task<(Guid CommunityId, Guid ChannelId, string UserId)> SeedChannelMemberAsync(ApplicationDbContext db)
        {
            string userId = await SeedMemberAsync(db, Guid.Empty, "user", Roles.Member);
            var user = await db.Users.FindAsync(userId);

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Community",
                Description = "Description",
                OwnerId = userId,
                Owner = user!,
                CreatedAt = DateTime.UtcNow,
                Memberships = new List<Membership>(),
                Channels = new List<Channel>()
            };

            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                CommunityId = community.Id,
                Community = community,
                Name = "general",
                CreatedAt = DateTime.UtcNow,
                Messages = new List<Message>()
            };

            var membership = await db.Memberships.FirstAsync(m => m.UserId == userId);
            membership.CommunityId = community.Id;
            membership.Community = community;

            db.Communities.Add(community);
            db.Channels.Add(channel);
            await db.SaveChangesAsync();

            return (community.Id, channel.Id, userId);
        }

        private static async Task<string> SeedMemberAsync(ApplicationDbContext db, Guid communityId, string userId, string role)
        {
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = $"{userId}@example.com",
                Email = $"{userId}@example.com"
            };

            db.Users.Add(user);
            db.Memberships.Add(new Membership
            {
                UserId = user.Id,
                User = user,
                CommunityId = communityId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
            return user.Id;
        }

        private static async Task<Message> SeedMessageAsync(ApplicationDbContext db, Guid channelId, string userId)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = channelId,
                UserId = userId,
                Content = "content",
                CreatedAt = DateTime.UtcNow
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();
            return message;
        }
    }
}
