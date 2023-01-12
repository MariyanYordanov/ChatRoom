﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace ChatRoom.Data
{
    using static DataConstants;

    public class ChatUser : IdentityUser
    {
        public ChatUser()
        {
            this.IsOwnerOf = new HashSet<ChatRoom>();
            this.IsMemberOf = new HashSet<ChatRoomUser>();
            this.Messages = new HashSet<Message>();
        }

        public ICollection<ChatRoom> IsOwnerOf { get; set; }

        public ICollection<ChatRoomUser> IsMemberOf { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
