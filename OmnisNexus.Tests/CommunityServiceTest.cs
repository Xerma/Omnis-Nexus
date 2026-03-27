using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;
using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class CommunityServiceTest
    {
        [Fact]
        public async Task UpdateMemberRoleAsync_RoleUpdates()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = "test role",
                JoinedAt = DateTime.Now
            };

            db.Memberships.Add(membership);
            await db.SaveChangesAsync();

            var service = new CommunityService(factory, communityState);
            bool updated = await service.UpdateMemberRoleAsync(membership.UserId, membership.CommunityId, "new role");

            await using var verifyDb = factory.CreateDbContext();
            var newMembership = await verifyDb.Memberships.FindAsync(membership.Id);

            Assert.True(updated);
        }

        [Fact]
        public async Task UpdateMemberRoleAsync_RoleDoesntUpdate_InvalidNewRole()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Owner,
                JoinedAt = DateTime.Now
            };

            db.Memberships.Add(membership);
            await db.SaveChangesAsync();

            var service = new CommunityService(factory, communityState);
            bool updated = await service.UpdateMemberRoleAsync(membership.UserId, membership.CommunityId, "new role");

            await using var verifyDb = factory.CreateDbContext();
            var newMembership = await verifyDb.Memberships.FindAsync(membership.Id);

            Assert.False(updated);
        }

        [Fact]
        public async Task SaveCommunityEditsAsync_NewNameSaved()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

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

            var service = new CommunityService(factory, communityState);
            await service.SaveCommunityEditsAsync(community, "New Name");

            await using var verifyDb = factory.CreateDbContext();
            var updatedCommunity = await verifyDb.Communities.FindAsync(community.Id);

            Assert.NotNull(community);
            Assert.NotNull(updatedCommunity);
            Assert.NotEqual(community.Name, updatedCommunity.Name);
        }

        [Fact]
        public async Task SaveCommunityEditsAsync_NewNameNotSaved()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

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

            var service = new CommunityService(factory, communityState);
            await service.SaveCommunityEditsAsync(community, "");

            await using var verifyDb = factory.CreateDbContext();
            var updatedCommunity = await verifyDb.Communities.FindAsync(community.Id);

            Assert.NotNull(community);
            Assert.NotNull(updatedCommunity);
            Assert.Equal(community.Name, updatedCommunity.Name);
        }

        [Fact]
        public async Task DeleteCommunityAsync_DeleteSuccess_CurrentCommunity()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

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

            var service = new CommunityService(factory, communityState);
            var result = await service.DeleteCommunityAsync(community, community.Id);

            await using var verifyDb = factory.CreateDbContext();
            var deletedCommunity = await verifyDb.Communities.FindAsync(community.Id);

            Assert.NotNull(community);
            Assert.Null(deletedCommunity);
            Assert.Equal("", result);
        }

        [Fact]
        public async Task DeleteCommunityAsync_DeleteSuccess_NotCurrentCommunity()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

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

            var service = new CommunityService(factory, communityState);
            var result = await service.DeleteCommunityAsync(community, Guid.NewGuid());

            await using var verifyDb = factory.CreateDbContext();
            var deletedCommunity = await verifyDb.Communities.FindAsync(community.Id);

            Assert.NotNull(community);
            Assert.Null(deletedCommunity);
            Assert.Null(result);
        }

        [Fact]
        public async Task LeaveCommunityAsync_Success_NotCurrentCommunity()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };
            db.Communities.Add(community);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "person",
                CommunityId = community.Id,
                Community = community,
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };
            db.Memberships.Add(membership);

            await db.SaveChangesAsync();

            var service = new CommunityService(factory, communityState);
            var result = await service.LeaveCommunityAsync(community, Guid.NewGuid(), membership.UserId);

            await using var verifyDb = factory.CreateDbContext();
            var leftCommunity = await verifyDb.Memberships
                .FirstOrDefaultAsync(m => m.Id == membership.Id);

            Assert.NotNull(community);
            Assert.Null(leftCommunity);
            Assert.Null(result);
        }

        [Fact]
        public async Task LeaveCommunityAsync_NoMembership()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

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

            var service = new CommunityService(factory, communityState);
            var result = await service.LeaveCommunityAsync(community, Guid.NewGuid(), "user2");

            await using var verifyDb = factory.CreateDbContext();
            var leftCommunity = await verifyDb.Memberships
                .FirstOrDefaultAsync(m => m.Id == Guid.NewGuid());

            Assert.NotNull(community);
            Assert.Null(leftCommunity);
            Assert.Null(result);
        }

        [Fact]
        public async Task LeaveCommunityAsync_Success_CurrentCommunity()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };
            db.Communities.Add(community);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "person",
                CommunityId = community.Id,
                Community = community,
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };
            db.Memberships.Add(membership);

            await db.SaveChangesAsync();

            var service = new CommunityService(factory, communityState);
            var result = await service.LeaveCommunityAsync(community, community.Id, membership.UserId);

            await using var verifyDb = factory.CreateDbContext();
            var leftCommunity = await verifyDb.Memberships
                .FirstOrDefaultAsync(m => m.Id == membership.Id);

            Assert.NotNull(community);
            Assert.Null(leftCommunity);
            Assert.Equal("", result);
        }

        [Fact]
        public async Task SetActiveCommunityFromUrl_SetCommunityId()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

            var result = service.SetActiveCommunityFromUrl($"channels/{Guid.NewGuid()}");

            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task SetActiveCommunityFromUrl_SetEmptyId()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

            var result = service.SetActiveCommunityFromUrl("");

            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task CreateCommunityAsync_Success()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

            ApplicationUser user = new();

            var navGuid = await service.CreateCommunityAsync(user);

            await using var verifyDb = factory.CreateDbContext();
            bool community = verifyDb.Communities.Any();
            bool membership = verifyDb.Memberships.Any();
            bool channel = verifyDb.Channels.Any();

            Assert.True(community);
            Assert.True(membership);
            Assert.True(channel);
            Assert.NotEqual(Guid.Empty, navGuid[0]);
            Assert.NotEqual(Guid.Empty, navGuid[1]);
        }

        [Fact]
        public async Task CreateCommunityAsync_Fail_NullUser()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

            var navGuid = await service.CreateCommunityAsync(null);

            await using var verifyDb = factory.CreateDbContext();
            bool community = verifyDb.Communities.Any();
            bool membership = verifyDb.Memberships.Any();
            bool channel = verifyDb.Channels.Any();

            Assert.False(community);
            Assert.False(membership);
            Assert.False(channel);
            Assert.Equal(Guid.Empty, navGuid[0]);
            Assert.Equal(Guid.Empty, navGuid[1]);
        }

        [Fact]
        public async Task JoinOrNavCommunity_NoMembership()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

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

            var result = await service.JoinOrNavCommunity(community.Id, "newUser");
            await using var verifyDb = factory.CreateDbContext();
            bool membership = verifyDb.Memberships.Any();

            Assert.Equal($"{community.Id}", result);
            Assert.True(membership);
        }

        [Fact]
        public async Task JoinOrNavCommunity_HasMembership()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };
            db.Communities.Add(community);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "person",
                CommunityId = community.Id,
                Community = community,
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };
            db.Memberships.Add(membership);

            await db.SaveChangesAsync();

            var result = await service.JoinOrNavCommunity(community.Id, membership.UserId);

            Assert.Equal($"{community.Id}", result);
        }

        [Fact]
        public async Task JoinOrNavCommunity_NullUserId()
        {
            var (db, factory) = TestDbHelper.CreateContext();
            var communityState = new CommunityStateService(factory);
            var service = new CommunityService(factory, communityState);

            var community = new Community
            {
                Id = Guid.NewGuid(),
                Name = "Test Community",
                Description = "description",
                OwnerId = "user1",
                CreatedAt = DateTime.Now,
            };
            db.Communities.Add(community);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "person",
                CommunityId = community.Id,
                Community = community,
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };
            db.Memberships.Add(membership);

            await db.SaveChangesAsync();

            var result = await service.JoinOrNavCommunity(community.Id, null);

            Assert.Null(result);
        }
    }
}
