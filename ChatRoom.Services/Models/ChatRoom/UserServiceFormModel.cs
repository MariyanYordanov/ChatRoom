using System.Collections.Generic;

using ChatRoom.Data;

namespace ChatRoom.Services.Models.ChatRoom
{
    public class UserServiceFormModel
    {
        public int Id { get; set; }

        public string RoomName { get; set; }

        public ICollection<ChatUser> Users { get; set; }
    }
}
