using Microsoft.AspNetCore.SignalR;

namespace Demo.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public long UserId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Application? Application { get; set; }

    }
}
