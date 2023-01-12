using System.Collections.Generic;

using ChatRoom.Services.Models.Message;

namespace ChatRoom.Services.Models.ChatRoom
{
    public class ChatRoomServiceModel
    {
        public int Id { get; init; }

        public string CurrentUser { get; init; }

        public string OwnerId { get; set; }

        public IEnumerable<MessageServiceModel> Messages { get; set; } = new List<MessageServiceModel>();

        public IEnumerable<string> Members { get; set; } = new List<string>();
    }
}
