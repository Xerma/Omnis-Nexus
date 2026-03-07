using OmnisNexus.Data;
using System.ComponentModel.DataAnnotations;

namespace OmnisNexus.Models
{
    public class Community
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Membership> Memberships { get; set; }

        public ICollection<Channel> Channels { get; set; }
    }
}
