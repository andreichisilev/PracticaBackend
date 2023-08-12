using System.Text.Json.Serialization;
using static ProiectPractica.Enums;

namespace ProiectPractica.Data.Entities
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? DateResponded { get; set; }
        [JsonIgnore]
        public User? UserSender { get; set; }
        public int? UserSenderId { get; set; }
        [JsonIgnore]
        public User? UserReceiver { get; set; }
        public int? UserReceiverId { get; set; }
    }
}
