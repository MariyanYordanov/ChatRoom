using System.Linq;
using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services;
using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.ChatRoom;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace ChatRoom.Tests
{
    public class UserServiceTests : UnitTestsBase
    {
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            this.userService = new UserService(this.dbContext);
        }

        [Test]
        public async Task Test_GetUsersToAdd_ReturnsCorrectUsers()
        {
            // Arrange: get london chat room from db
            Data.ChatRoom room = this.testDb.LondonChatRoom;

            // Act: call service method
            UserServiceFormModel serviceModel = await this.userService.GetUsersToAddAsync(room.Id);

            // Assert: correct users are returned
            Assert.That(serviceModel.Id, Is.EqualTo(room.Id));
            Assert.That(serviceModel.RoomName, Is.EqualTo(room.Name));

            ChatUser mariaUser = this.testDb.UserMaria;
            Assert.That(serviceModel.Users.FirstOrDefault(u => u.Id == mariaUser.Id), Is.Not.Null);
        }

        [Test]
        public async Task Test_AddUserToRoom_AddsCorrectly()
        {
            // Arrange: get london chat room, maria user from db
            Data.ChatRoom room = this.testDb.LondonChatRoom;
            ChatUser user = this.testDb.UserMaria;

            // Act: call service method
            await this.userService.AddUserToRoomAsync(room.Id, user.Id);

            // Assert: map exists in database
            ChatRoomUser dbMap = await this.dbContext.ChatRoomsUsers
                .Where(cru => cru.ChatRoomId == room.Id && cru.ChatUserId == user.Id)
                .FirstOrDefaultAsync();
            Assert.That(dbMap, Is.Not.Null);
        }

        [Test]
        public async Task Test_GetUsersToRemove_ReturnsCorrectUsers()
        {
            // Arrange: get london chat room from db
            Data.ChatRoom room = this.testDb.LondonChatRoom;

            // Act: call service method
            UserServiceFormModel serviceModel = await this.userService.GetUsersToRemoveAsync(room.Id);

            // Assert: correct users are returned
            Assert.That(serviceModel.Id, Is.EqualTo(room.Id));
            Assert.That(serviceModel.RoomName, Is.EqualTo(room.Name));

            ChatUser guestUserTwo = this.testDb.GuestUserTwo;
            Assert.That(serviceModel.Users.FirstOrDefault(u => u.Id == guestUserTwo.Id), Is.Not.Null);
        }

        [Test]
        public async Task Test_RemoveUserFromRoom_RemovesCorrectly()
        {
            // Arrange: get london chat room, guest user from db
            Data.ChatRoom room = this.testDb.LondonChatRoom;
            ChatUser user = this.testDb.GuestUser;

            // Act: call service method
            await this.userService.RemoveUserFromRoomAsync(room.Id, user.Id);

            // Assert: map disappeared in database
            ChatRoomUser dbMap = await this.dbContext.ChatRoomsUsers
                .Where(cru => cru.ChatRoomId == room.Id && cru.ChatUserId == user.Id)
                .FirstOrDefaultAsync();
            Assert.That(dbMap, Is.Null);
        }
    }
}
