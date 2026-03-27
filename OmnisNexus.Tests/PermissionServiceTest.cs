using OmnisNexus.Models;
using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class PermissionServiceTest
    {
        [Fact]
        public void CanDeleteMessage_IsAuthorized_Author()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = "test role",
                JoinedAt = DateTime.Now
            };

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "1",
                Content = "content",
                CreatedAt = DateTime.Now
            };

            bool result = service.CanDeleteMessage(membership, message);

            Assert.True(result);
        }

        [Fact]
        public void CanDeleteMessage_NotAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = "test role",
                JoinedAt = DateTime.Now
            };

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "2",
                Content = "content",
                CreatedAt = DateTime.Now
            };

            bool result = service.CanDeleteMessage(membership, message);

            Assert.False(result);
        }

        [Fact]
        public void CanEditMessage_IsAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = "test role",
                JoinedAt = DateTime.Now
            };

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "1",
                Content = "content",
                CreatedAt = DateTime.Now
            };

            bool result = service.CanEditMessage(membership, message);

            Assert.True(result);
        }

        [Fact]
        public void CanEditMessage_NotAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = "test role",
                JoinedAt = DateTime.Now
            };

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = Guid.NewGuid(),
                UserId = "2",
                Content = "content",
                CreatedAt = DateTime.Now
            };

            bool result = service.CanEditMessage(membership, message);

            Assert.False(result);
        }

        [Fact]
        public void CanManageChannels_IsAuthorized()
        {
            var service = new PermissionService();

            var membership1 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Moderator,
                JoinedAt = DateTime.Now
            };

            var membership2 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Owner,
                JoinedAt = DateTime.Now
            };

            bool result1 = service.CanManageChannels(membership1);
            bool result2 = service.CanManageChannels(membership2);

            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        public void CanManageChannels_NotAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };

            bool result = service.CanManageChannels(membership);

            Assert.False(result);
        }

        [Fact]
        public void CanManageCommunity_IsAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Owner,
                JoinedAt = DateTime.Now
            };

            bool result = service.CanManageCommunity(membership);

            Assert.True(result);
        }

        [Fact]
        public void CanManageCommunity_NotAuthorized()
        {
            var service = new PermissionService();

            var membership1 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Moderator,
                JoinedAt = DateTime.Now
            };

            var membership2 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };

            bool result1 = service.CanManageCommunity(membership1);
            bool result2 = service.CanManageCommunity(membership2);

            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        public void CanLeaveCommunity_IsAuthorized()
        {
            var service = new PermissionService();

            var membership1 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Moderator,
                JoinedAt = DateTime.Now
            };

            var membership2 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };

            bool result1 = service.CanLeaveCommunity(membership1);
            bool result2 = service.CanLeaveCommunity(membership2);

            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        public void CanLeaveCommunity_NotAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Owner,
                JoinedAt = DateTime.Now
            };

            bool result = service.CanLeaveCommunity(membership);

            Assert.False(result);
        }

        [Fact]
        public void CanManageMembers_IsAuthorized()
        {
            var service = new PermissionService();

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Owner,
                JoinedAt = DateTime.Now
            };

            bool result = service.CanManageMembers(membership);

            Assert.True(result);
        }

        [Fact]
        public void CanManageMembers_NotAuthorized()
        {
            var service = new PermissionService();

            var membership1 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Moderator,
                JoinedAt = DateTime.Now
            };

            var membership2 = new Membership
            {
                Id = Guid.NewGuid(),
                UserId = "1",
                CommunityId = Guid.NewGuid(),
                Role = Roles.Member,
                JoinedAt = DateTime.Now
            };

            bool result1 = service.CanManageMembers(membership1);
            bool result2 = service.CanManageMembers(membership2);

            Assert.False(result1);
            Assert.False(result2);
        }
    }
}
