using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services;
using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.Home;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace ChatRoom.Tests
{
    [TestFixture]
    public class HomeServiceTests : UnitTestsBase
    {
        private IHomeService homeService;

        [SetUp]
        public void SetUp()
        {
            this.homeService = new HomeService(this.dbContext);
        }

        [Test]
        public async Task Test_GetUserRooms_ReturnsCorrectChatRooms()
        {
            // Arrange: get guest user from test db
            ChatUser guestUser = this.testDb.GuestUser;

            // Arrange: get user chat room count from db
            int userChatRoomsDb = await this.dbContext.ChatRoomsUsers
                .CountAsync(cru => cru.ChatUserId == guestUser.Id);

            // Act: invoke service method
            HomeServiceModel serviceModel = await this.homeService.GetUserRoomsAsync(guestUser.Id);

            // Assert: user chat rooms are the same
            int userChatRoomsService = serviceModel.ChatRooms.Count;
            Assert.That(userChatRoomsService, Is.EqualTo(userChatRoomsDb));
        }
    }
}
