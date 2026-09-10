namespace OmnisNexus.Models
{
    public class SearchItem
    {
        public Guid MessageId { get; set; }
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public string MessageContent { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
