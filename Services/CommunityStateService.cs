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

        public List<CommunityListItem> AllCommunities { get; private set; } = new();

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

        public async Task LoadAllCommunitiesAsync()
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            AllCommunities = await db.Communities
            .Select(c => new CommunityListItem
            {
                Id = c.Id,
                Name = c.Name,
                MemberCount = c.Memberships.Count()
            })
            .ToListAsync();

            NotifyStateChanged();
        }

        public void AddCommunityToAllCommunities(Community community)
        {
            AllCommunities.Add(new CommunityListItem
            {
                Id = community.Id,
                Name = community.Name,
                MemberCount = 1
            });
            NotifyStateChanged();
        }

        public void RemoveCommunity(Community community)
        {
            AllCommunities.RemoveAll(c => c.Id == community.Id);
            NotifyStateChanged();
        }

        public void UpdateCommunityName(Guid comId, string newName)
        {
            var allCommunity = AllCommunities.FirstOrDefault(c => c.Id == comId);
            if (allCommunity is null) return;

            var userCommunity = UserCommunities.FirstOrDefault(c => c.Id == comId);
            if (userCommunity is null) return;

            allCommunity.Name = newName;
            userCommunity.Name = newName;
            NotifyStateChanged();
        }

        public void IncrementMemberCount(Guid comId)
        {
            var community = AllCommunities.FirstOrDefault(c => c.Id == comId);
            if (community is null) return;

            community.MemberCount++;
            NotifyStateChanged();
        }

        public void DecrementMemberCount(Guid comId)
        {
            var community = AllCommunities.FirstOrDefault(c => c.Id == comId);
            if (community is null) return;

            community.MemberCount--;
            NotifyStateChanged();
        }

        public Community? GetCommunity(Guid communityId)
        {
            return UserCommunities.FirstOrDefault(c => c.Id == communityId);
        }
    }
}
