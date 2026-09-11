using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class UserRealtimeService
    {
        private readonly object _lock = new();
        private readonly Dictionary<Guid, List<Action<UserProfileUpdatedDto>>> _communityHandlers = new();

        public IDisposable SubscribeToCommunity(Guid communityId, Action<UserProfileUpdatedDto> handler)
        {
            lock (_lock)
            {
                if (!_communityHandlers.TryGetValue(communityId, out List<Action<UserProfileUpdatedDto>>? handlers))
                {
                    handlers = new List<Action<UserProfileUpdatedDto>>();
                    _communityHandlers[communityId] = handlers;
                }

                handlers.Add(handler);
            }

            return new Subscription(() => UnsubscribeFromCommunity(communityId, handler));
        }

        public void NotifyUserProfileUpdated(UserProfileUpdatedDto user, IEnumerable<Guid> communityIds)
        {
            List<Action<UserProfileUpdatedDto>> handlers;

            lock (_lock)
            {
                handlers = communityIds
                    .Distinct()
                    .Where(_communityHandlers.ContainsKey)
                    .SelectMany(communityId => _communityHandlers[communityId])
                    .Distinct()
                    .ToList();
            }

            foreach (Action<UserProfileUpdatedDto> handler in handlers)
            {
                handler.Invoke(user);
            }
        }

        private void UnsubscribeFromCommunity(Guid communityId, Action<UserProfileUpdatedDto> handler)
        {
            lock (_lock)
            {
                if (!_communityHandlers.TryGetValue(communityId, out List<Action<UserProfileUpdatedDto>>? handlers))
                {
                    return;
                }

                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    _communityHandlers.Remove(communityId);
                }
            }
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Action _dispose;
            private bool _disposed;

            public Subscription(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                if (_disposed) return;

                _dispose();
                _disposed = true;
            }
        }
    }
}
