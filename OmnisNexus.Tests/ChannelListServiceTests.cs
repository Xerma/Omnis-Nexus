using OmnisNexus.Models;
using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class ChannelListServiceTests
    {
        [Fact]
        public async Task CreateChannelAsync_ValidNameAndCommunity_CreatesChannel()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var result = await service.CreateChannelAsync("general", community.Id);

            await using var verifyDb = factory.CreateDbContext();
            var createdChannel = await verifyDb.Channels.FindAsync(result);

            Assert.NotNull(createdChannel);
            Assert.Equal("general", createdChannel.Name);
            Assert.Equal(community.Id, createdChannel.CommunityId);
        }

        [Fact]
        public async Task CreateChannelAsync_InvalidName_ReturnsEmptyGuid()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var result = await service.CreateChannelAsync("", community.Id);

            await using var verifyDb = factory.CreateDbContext();
            var createdChannel = await verifyDb.Channels.FindAsync(result);

            Assert.Equal(Guid.Empty, result);
            Assert.Null(createdChannel);
        }

        [Fact]
        public async Task SaveChannelEdits_SuccessfulRename()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var chanId = await service.CreateChannelAsync("general", community.Id);

            await using var verifyDb1 = factory.CreateDbContext();
            var createdChannel = await verifyDb1.Channels.FindAsync(chanId);

            Assert.NotNull(createdChannel);
            await service.SaveChannelEditsAsync(createdChannel, "new");
            await using var verifyDb2 = factory.CreateDbContext();
            var renamedChannel = await verifyDb2.Channels.FindAsync(chanId);

            Assert.NotEqual(Guid.Empty, chanId);
            Assert.NotNull(renamedChannel);
            Assert.Equal("new", renamedChannel.Name);
        }

        [Fact]
        public async Task GetChannelCount_GreaterThanZero()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var chanId = await service.CreateChannelAsync("general", community.Id);
            var count = await service.GetChannelCount(community.Id);

            Assert.NotEqual(Guid.Empty, chanId);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetChannelCount_Zero()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var count = await service.GetChannelCount(community.Id);

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task DeleteChannelAsync_DeletesChannel()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var chanId = await service.CreateChannelAsync("general", community.Id);

            await using var verifyDb1 = factory.CreateDbContext();
            var createdChannel = await verifyDb1.Channels.FindAsync(chanId);

            Assert.NotNull(createdChannel);
            var deletedId = await service.DeleteChannelAsync(createdChannel, Guid.NewGuid(), community.Id);
            await using var verifyDb2 = factory.CreateDbContext();
            var count = await service.GetChannelCount(community.Id);

            Assert.Equal(0, count);
            Assert.Equal(Guid.Empty, deletedId);
        }

        [Fact]
        public async Task DeleteChannelAsync_InvalidChannel()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);

            await using var verifyDb1 = factory.CreateDbContext();
            var deletedId = await service.DeleteChannelAsync(null, Guid.NewGuid(), community.Id);

            await using var verifyDb2 = factory.CreateDbContext();
            var count = await service.GetChannelCount(community.Id);

            Assert.Equal(0, count);
            Assert.Equal(Guid.Empty, deletedId);
        }

        [Fact]
        public async Task DeleteChannelAsync_DeleteCurrentChannel()
        {
            var (db, factory) = TestDbHelper.CreateContext();

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };

            db.Communities.Add(community);
            await db.SaveChangesAsync();

            var service = new ChannelListService(factory);
            var chanId = await service.CreateChannelAsync("general", community.Id);

            await using var verifyDb1 = factory.CreateDbContext();
            var createdChannel = await verifyDb1.Channels.FindAsync(chanId);

            Assert.NotNull(createdChannel);
            var deletedId = await service.DeleteChannelAsync(createdChannel, createdChannel.Id, community.Id);
            await using var verifyDb2 = factory.CreateDbContext();
            var count = await service.GetChannelCount(community.Id);

            Assert.Equal(0, count);
            Assert.Equal(community.Id, deletedId);
        }
    }
}
