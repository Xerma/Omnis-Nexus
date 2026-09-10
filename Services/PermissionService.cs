using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public class PermissionService : IPermissionService
    {
        public bool CanDeleteMessage(Membership? membership, Message message)
        {
            return IsOwner(membership) || IsModerator(membership) || IsMessageAuthor(membership, message);
        }

        public bool CanEditMessage(Membership? membership, Message message)
        {
            return IsMessageAuthor(membership, message);
        }

        public bool CanManageChannels(Membership? membership)
        {
            return IsOwner(membership) || IsModerator(membership);
        }

        public bool CanManageCommunity(Membership? membership)
        {
            return IsOwner(membership);
        }

        public bool CanLeaveCommunity(Membership? membership)
        {
            return !IsOwner(membership);
        }

        public bool CanManageMembers(Membership? membership)
        {
            return IsOwner(membership);
        }

        private bool IsOwner(Membership? membership)
        {
            return membership?.Role == Roles.Owner;
        }

        private bool IsModerator(Membership? membership)
        {
            return membership?.Role == Roles.Moderator;
        }

        private bool IsMessageAuthor(Membership? membership, Message message)
        {
            return membership?.UserId == message.UserId;
        }

    }
}
