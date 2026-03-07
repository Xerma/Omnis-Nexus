using OmnisNexus.Data;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OmnisNexus.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; }

        [MaxLength(30)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
