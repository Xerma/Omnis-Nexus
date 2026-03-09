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

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
