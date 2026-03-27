using OmnisNexus.Services;

namespace OmnisNexus.Tests
{
    public class ErrorServiceTests
    {
        [Fact]
        public async Task ShowErrorAsync_SetsAndClearsMessage()
        {
            ErrorService errorService = new();
            await errorService.ShowErrorAsync("Test", 100);
            Assert.Null(errorService.ErrorMessage);
        }

        [Fact]
        public async Task ShowErrorAsync_DoesntOverride()
        {
            ErrorService errorService = new();
            var task1 = errorService.ShowErrorAsync("First", 200);
            await Task.Delay(50);
            var task2 = errorService.ShowErrorAsync("Second", 200);
            await Task.WhenAll(task1, task2);
            Assert.Null(errorService.ErrorMessage);
        }
    }
}
