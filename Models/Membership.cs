using OmnisNexus.Data;
using System.ComponentModel.DataAnnotations;

namespace OmnisNexus.Models
{
    public class Membership
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid CommunityId { get; set; }
        public Community Community { get; set; }

        [MaxLength(30)]
        public string Role { get; set; }
        // handle if too long

        public DateTime JoinedAt { get; set; }
    }
}
