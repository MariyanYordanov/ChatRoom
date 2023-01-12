using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.ChatRoom;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;

        public UserService(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public async Task<UserServiceFormModel> GetUsersToAddAsync(int id)
        {
            Data.ChatRoom room = await this.dbContext.ChatRooms
                .Where(cr => cr.Id == id)
                .FirstOrDefaultAsync();

            ICollection<string> chatRoomMembers =
                await this.dbContext.ChatRoomsUsers
                    .Where(cru => cru.ChatRoomId == id)
                    .Select(cru => cru.ChatUserId)
                    .ToListAsync();

            ICollection<ChatUser> usersNotInRoom = this.dbContext.Users
                .Where(u => !chatRoomMembers.Contains(u.Id))
                .ToList();

            return new UserServiceFormModel
            {
                Id = id,
                RoomName = room.Name,
                Users = usersNotInRoom,
            };
        }

        public async Task AddUserToRoomAsync(int chatId, string userId)
        {
            await this.dbContext.ChatRoomsUsers.AddAsync(new ChatRoomUser
            {
                ChatRoomId = chatId,
                ChatUserId = userId,
            });

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<UserServiceFormModel> GetUsersToRemoveAsync(int id)
        {
            Data.ChatRoom room = await this.dbContext.ChatRooms
                .Where(cr => cr.Id == id)
                .FirstOrDefaultAsync();

            ICollection<string> chatRoomMembers =
                await this.dbContext.ChatRoomsUsers
                    .Where(cru => cru.ChatRoomId == id && cru.ChatUserId != room.OwnerId)
                    .Select(cru => cru.ChatUserId)
                    .ToListAsync();

            ICollection<ChatUser> usersInRoom = this.dbContext.Users
                .Where(u => chatRoomMembers.Contains(u.Id))
                .ToList();

            return new UserServiceFormModel
            {
                Id = id,
                RoomName = room.Name,
                Users = usersInRoom,
            };
        }

        public async Task RemoveUserFromRoomAsync(int chatId, string userId)
        {
            ChatRoomUser user = await this.dbContext.ChatRoomsUsers
                .FirstOrDefaultAsync(cru => cru.ChatUserId == userId
                                        && cru.ChatRoomId == chatId);

            this.dbContext.ChatRoomsUsers.Remove(user);

            await this.dbContext.SaveChangesAsync();
        }
    }
}
