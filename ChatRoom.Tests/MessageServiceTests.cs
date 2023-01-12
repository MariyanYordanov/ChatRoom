using System.Linq;
using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services;
using ChatRoom.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ChatRoom.Tests
{
    public class MessageServiceTests : UnitTestsBase
    {
        private IMessageService messageService;

        [SetUp]
        public void SetUp()
        {
            this.messageService = new MessageService(this.dbContext);
        }

        [Test]
        public async Task Test_SaveComment_AddsCorrectly()
        {
            // Arrange: get guest user, london chat from test db
            ChatUser guestUser = this.testDb.GuestUser;
            Data.ChatRoom londonRoom = this.testDb.LondonChatRoom;

            // Arrange: create valid data for new comment
            string content = "Some text";
            int chatId = londonRoom.Id;
            string ownerId = guestUser.Id;

            // Act: call service method with valid data
            await this.messageService.SaveMessage(content, chatId, ownerId);

            // Assert: message exists in db
            Message message = await this.dbContext.Messages
                .LastOrDefaultAsync();

            Assert.That(message, Is.Not.Null);
            Assert.That(message.Content, Is.EqualTo(content));
            Assert.That(message.ChatRoomId, Is.EqualTo(chatId));
            Assert.That(message.OwnerId, Is.EqualTo(ownerId));
        }
    }
}
