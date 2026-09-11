using OmnisNexus.Models;
using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class MessageStateServiceTests
    {
        [Fact]
        public void UpdateUserName_UpdatesMatchingLoadedMessages()
        {
            var (_, factory) = TestDbHelper.CreateContext();
            var service = new MessageStateService(factory);
            int changeCount = 0;
            service.OnChange += () => changeCount++;

            service.SetMessages(new List<MessageDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ChannelId = Guid.NewGuid(),
                    UserId = "user-1",
                    UserName = "OldName",
                    Content = "First",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ChannelId = Guid.NewGuid(),
                    UserId = "user-2",
                    UserName = "OtherName",
                    Content = "Second",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ChannelId = Guid.NewGuid(),
                    UserId = "user-1",
                    UserName = "OldName",
                    Content = "Third",
                    CreatedAt = DateTime.UtcNow
                }
            });

            service.UpdateUserName("user-1", "NewName");

            Assert.Equal("NewName", service.Messages[0].UserName);
            Assert.Equal("OtherName", service.Messages[1].UserName);
            Assert.Equal("NewName", service.Messages[2].UserName);
            Assert.Equal(2, changeCount);
        }

        [Fact]
        public void UpdateUserName_DoesNotNotifyWhenNoLoadedMessagesChange()
        {
            var (_, factory) = TestDbHelper.CreateContext();
            var service = new MessageStateService(factory);
            int changeCount = 0;
            service.OnChange += () => changeCount++;

            service.SetMessages(new List<MessageDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ChannelId = Guid.NewGuid(),
                    UserId = "user-1",
                    UserName = "OldName",
                    Content = "First",
                    CreatedAt = DateTime.UtcNow
                }
            });

            service.UpdateUserName("user-2", "NewName");

            Assert.Equal("OldName", service.Messages[0].UserName);
            Assert.Equal(1, changeCount);
        }
    }
}
