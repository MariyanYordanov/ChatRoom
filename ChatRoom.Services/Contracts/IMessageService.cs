using System.Threading.Tasks;

namespace ChatRoom.Services.Contracts
{
    public interface IMessageService
    {
        Task SaveMessage(string content, int chatId, string ownerId);
    }
}
