using System.Threading.Tasks;

using ChatRoom.Services.Models.ChatRoom;

namespace ChatRoom.Services.Contracts
{
    public interface IUserService
    {
        Task<UserServiceFormModel> GetUsersToAddAsync(int id);

        Task AddUserToRoomAsync(int chatId, string userId);

        Task<UserServiceFormModel> GetUsersToRemoveAsync(int id);

        Task RemoveUserFromRoomAsync(int chatId, string userId);
    }
}
