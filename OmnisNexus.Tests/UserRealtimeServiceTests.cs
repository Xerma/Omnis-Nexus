using OmnisNexus.Models;
using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class UserRealtimeServiceTests
    {
        [Fact]
        public void NotifyUserProfileUpdated_OnlyInvokesAffectedCommunitySubscribers()
        {
            var service = new UserRealtimeService();
            Guid communityA = Guid.NewGuid();
            Guid communityB = Guid.NewGuid();
            Guid communityC = Guid.NewGuid();
            int communityACount = 0;
            int communityBCount = 0;
            int communityCCount = 0;

            service.SubscribeToCommunity(communityA, _ => communityACount++);
            service.SubscribeToCommunity(communityB, _ => communityBCount++);
            service.SubscribeToCommunity(communityC, _ => communityCCount++);

            service.NotifyUserProfileUpdated(CreateUpdate(), new[] { communityA, communityB });

            Assert.Equal(1, communityACount);
            Assert.Equal(1, communityBCount);
            Assert.Equal(0, communityCCount);
        }

        [Fact]
        public void SubscriptionDispose_RemovesCommunitySubscriber()
        {
            var service = new UserRealtimeService();
            Guid communityId = Guid.NewGuid();
            int updateCount = 0;

            IDisposable subscription = service.SubscribeToCommunity(communityId, _ => updateCount++);
            subscription.Dispose();

            service.NotifyUserProfileUpdated(CreateUpdate(), new[] { communityId });

            Assert.Equal(0, updateCount);
        }

        private static UserProfileUpdatedDto CreateUpdate()
        {
            return new UserProfileUpdatedDto
            {
                UserId = "user-1",
                UserName = "NewName",
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
