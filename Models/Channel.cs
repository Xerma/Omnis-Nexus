using System.ComponentModel.DataAnnotations;

namespace OmnisNexus.Models
{
    public class Channel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CommunityId { get; set; }
        public Community Community { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = "";
        // handle error if too long

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
    }
}
