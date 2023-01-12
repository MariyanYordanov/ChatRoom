using ChatRoom.Services.Models.ChatRoom;
using System.Threading.Tasks;

namespace ChatRoom.Services.Contracts
{
    public interface IChatService
    {
        Task<bool> DoesChatRoomExistAsync(int id);

        Task<bool> IsUserInChatRoomAsync(int id, string userId);

        Task<ChatRoomServiceModel> GetChatDetailsAsync(int id, string userId);

        Task CreateRoomAsync(string name, string ownerId);

        Task<bool> IsUserChatOwnerAsync(int chatId, string userId);

        Task<ChatRoomModel> GetChatRoomAsync(int id);

        Task RemoveRoomAsync(int id);

        Task EditRoomAsync(int id, string name);
    }
}
