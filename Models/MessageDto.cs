namespace OmnisNexus.Models
{
    public class MessageDto
    {
        public Guid Id { get; set; }

        public Guid ChannelId { get; set; }

        public string UserId { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
