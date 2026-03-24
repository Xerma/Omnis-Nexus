using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class CommunityStateService
    {
        public event Action? OnChange;

        public List<Community> UserCommunities { get; private set; } = new();

        public void SetCommunities(List<Community> communities)
        {
            UserCommunities = communities;
            NotifyStateChanged();
        }

        public void NotifyStateChanged() => OnChange?.Invoke();

    }
}
