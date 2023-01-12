using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.ChatRoom;
using ChatRoom.Services.Models.Message;

using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext dbContext;

        public ChatService(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public async Task<bool> DoesChatRoomExistAsync(int id)
        {
            return await this.dbContext.ChatRooms.FindAsync(id) != null;
        }

        public async Task<bool> IsUserInChatRoomAsync(int id, string userId)
        {
            List<string> chatRoomUsers = await this.dbContext.ChatRoomsUsers
                .Where(x => x.ChatRoomId == id)
                .Select(x => x.ChatUserId)
                .ToListAsync();

            ChatUser user = await this.dbContext.Users.FindAsync(userId);
            return chatRoomUsers.Contains(user.Id);
        }

        public async Task<ChatRoomServiceModel> GetChatDetailsAsync(int id, string userId)
        {
            ChatUser user = await this.dbContext.Users.FindAsync(userId);
            string username = user.UserName;

            return await this.dbContext
                .ChatRooms
                .Include(c => c.Members)
                .ThenInclude(cu => cu.ChatUser)
                .Where(c => c.Id == id)
                .Select(c => new ChatRoomServiceModel
                {
                    Id = c.Id,
                    CurrentUser = username,
                    OwnerId = c.OwnerId,
                    Messages = c.Messages.Select(m => new MessageServiceModel
                    {
                        Content = m.Content,
                        Owner = m.Owner.UserName,
                    }),
                    Members = c.Members.Select(m => m.ChatUser.UserName)
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateRoomAsync(string name, string ownerId)
        {
            Data.ChatRoom room = new Data.ChatRoom
            {
                Name = name,
                OwnerId = ownerId,
            };

            await this.dbContext.ChatRooms.AddAsync(room);
            await this.dbContext.SaveChangesAsync();

            ChatRoomUser map = new ChatRoomUser
            {
                ChatRoomId = room.Id,
                ChatUserId = ownerId,
            };

            await this.dbContext.ChatRoomsUsers.AddAsync(map);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsUserChatOwnerAsync(int chatId, string userId)
        {
            Data.ChatRoom room = await this.dbContext
                .ChatRooms
                .Where(cr => cr.Id == chatId)
                .FirstOrDefaultAsync();

            return room.OwnerId == userId;
        }

        public async Task<ChatRoomModel> GetChatRoomAsync(int id)
        {
            Data.ChatRoom room = await this.dbContext.ChatRooms
                .Where(cr => cr.Id == id)
                .Include(cr => cr.Owner)
                .FirstOrDefaultAsync();

            return room == null ? null : new ChatRoomModel
            {
                Id = room.Id,
                Name = room.Name,
                Owner = room.Owner.Id,
            };
        }

        public async Task RemoveRoomAsync(int id)
        {
            Data.ChatRoom room = await this.dbContext
                .ChatRooms
                .Where(cr => cr.Id == id)
                .FirstOrDefaultAsync();

            this.dbContext.ChatRooms.Remove(room);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditRoomAsync(int id, string name)
        {
            Data.ChatRoom room = await this.dbContext
                .ChatRooms
                .Where(cr => cr.Id == id)
                .FirstOrDefaultAsync();

            room.Name = name;

            await this.dbContext.SaveChangesAsync();
        }
    }
}
