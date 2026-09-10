using OmnisNexus.Models;

namespace OmnisNexus.Services
{
    public interface IPermissionService
    {
        bool CanDeleteMessage(Membership? membership, Message message);
        bool CanEditMessage(Membership? membership, Message message);
        bool CanManageChannels(Membership? membership);
        bool CanManageCommunity(Membership? membership);
        bool CanLeaveCommunity(Membership? membership);
        bool CanManageMembers(Membership? membership);
    }
}
