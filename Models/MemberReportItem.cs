namespace OmnisNexus.Models
{
    public class MemberReportItem
    {
        public string UserId { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = Roles.Member;
        public DateTime JoinedAt { get; set; }
        public int MessageCount { get; set; } = 0;
    }
}
