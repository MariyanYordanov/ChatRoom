using ChatRoom.Data;
using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.ChatRoom;
using ChatRoom.Services.Models.Home;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoom.Services
{
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext dbContext;

        public HomeService(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public async Task<HomeServiceModel> GetUserRoomsAsync(string userId)
        {
            ChatUser user = await this.dbContext.Users.FindAsync(userId);
            List<int> userChatRooms = this.dbContext.ChatRoomsUsers
                .Where(cru => cru.ChatUserId == user.Id)
                .Select(cru => cru.ChatRoomId)
                .ToList();

            return new HomeServiceModel
            {
                ChatRooms = this.dbContext.ChatRooms
                    .Where(cr => userChatRooms.Contains(cr.Id))
                    .Select(c => new ChatRoomModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Owner = c.Owner.UserName
                    })
                    .ToList()
            };
        }
    }
}
