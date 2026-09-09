using OmnisNexus.Models;

namespace OmnisNexus.Services

// works with single Railway replica
// must replace (and change omnis-nexus-volume /app/keys volume setup) with a distributed cache (like Redis) for multi-replica deployments

{
    public class MessageRealtimeService
    {
        public event Action<MessageDto>? MessageCreated;
        public event Action<MessageDto>? MessageEdited;
        public event Action<DeletedMessageDto>? MessageDeleted;

        public void NotifyMessageCreated(MessageDto message)
        {
            MessageCreated?.Invoke(message);
        }

        public void NotifyMessageEdited(MessageDto message)
        {
            MessageEdited?.Invoke(message);
        }

        public void NotifyMessageDeleted(DeletedMessageDto message)
        {
            MessageDeleted?.Invoke(message);
        }
    }
}
