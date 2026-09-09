namespace OmnisNexus.Services
{
    public class ErrorService
    {
        public event Action? OnChange;

        public string? ErrorMessage { get; private set; }

        public async Task ShowErrorAsync(string message, int mil = 5000)
        {
            ErrorMessage = message;
            NotifyStateChanged();

            var currMessage = message;
            await Task.Delay(mil);

            if (ErrorMessage == currMessage)
            {
                Clear();
            }
        }

        public void Clear()
        {
            ErrorMessage = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
