using System.ComponentModel.DataAnnotations;

namespace OmnisNexus.Models
{
    public class Channel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CommunityId { get; set; }
        public Community Community { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
