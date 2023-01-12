using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services.Contracts;

namespace ChatRoom.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext dbContext;

        public MessageService(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public async Task SaveMessage(string content, int chatId, string ownerId)
        {
            Message message = new Message
            {
                Content = content,
                ChatRoomId = chatId,
                OwnerId = ownerId,
            };

            await this.dbContext.Messages.AddAsync(message);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
