using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using OmnisNexus.Models;
using System.Security.Claims;

namespace OmnisNexus.Services
{
    public class CommunityStateService
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public CommunityStateService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public event Action? OnChange;

        public List<Community> UserCommunities { get; private set; } = new();

        public void SetCommunities(List<Community> communities)
        {
            UserCommunities = communities;
            NotifyStateChanged();
        }

        public void NotifyStateChanged() => OnChange?.Invoke();

        public async Task LoadUserCommunitiesAsync(AuthenticationStateProvider AuthStateProvider)
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return;

            using var db = await _dbContextFactory.CreateDbContextAsync();

            var communities = await db.Memberships
                .Where(m => m.UserId == userId)
                .Select(m => m.Community)
                .ToListAsync();

            SetCommunities(communities);
        }
    }
}
