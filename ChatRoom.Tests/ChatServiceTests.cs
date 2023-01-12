using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using ChatRoom.Data;
using ChatRoom.Services;
using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.ChatRoom;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace ChatRoom.Tests
{
    [TestFixture]
    public class ChatServiceTests : UnitTestsBase
    {
        private IChatService chatService;

        [SetUp]
        public void SetUp()
        {
            this.chatService = new ChatService(this.dbContext);
        }

        [Test]
        public async Task Test_GetChatDetails_ReturnsCorrectInfo()
        {
            // Arrange: get guest user, london room from test db
            ChatUser user = this.testDb.GuestUser;
            Data.ChatRoom room = this.testDb.LondonChatRoom;

            // Act: call service method
            ChatRoomServiceModel serviceModel = await this.chatService.GetChatDetailsAsync(1, user.Id);

            // Assert: room details match
            Assert.That(serviceModel.Id, Is.EqualTo(room.Id));
            Assert.That(serviceModel.CurrentUser, Is.EqualTo(user.UserName));
            Assert.That(serviceModel.OwnerId, Is.EqualTo(user.Id));
        }

        [Test]
        public async Task Test_SaveChatRoom_AddsCorrectly()
        {
            // Arrange: get guest user from test db
            ChatUser guestUser = this.testDb.GuestUser;

            // Arrange: create data for a new chat room
            string name = $"New Create Chat Room {DateTime.Now.Ticks}";
            string ownerId = guestUser.Id;

            // Act: call service method
            await this.chatService.CreateRoomAsync(name, ownerId);

            // Assert: chat room has been added
            Data.ChatRoom room = this.dbContext.ChatRooms
                .LastOrDefault();

            Assert.That(room!.Name, Is.EqualTo(name));
            Assert.That(room.OwnerId, Is.EqualTo(ownerId));
        }

        [Test]
        public async Task Test_DeleteChatRoom_RemovesCorrectly()
        {
            // Arrange: get guest user from test db
            ChatUser guestUser = this.testDb.GuestUser;

            // Arrange: create data for a new chat room
            string name = $"New Create Chat Room {DateTime.Now.Ticks}";
            string ownerId = guestUser.Id;

            // Arrange: create and save new room
            await this.chatService.CreateRoomAsync(name, ownerId);

            // Act: get the room and then delete it with service method
            Data.ChatRoom room = await this.dbContext.ChatRooms
                .LastOrDefaultAsync();

            await this.chatService.RemoveRoomAsync(room!.Id);

            // Assert: room is deleted
            room = await this.dbContext.ChatRooms
                .FirstOrDefaultAsync(cr => cr.Id == room.Id);
            Assert.That(room, Is.Null);
        }

        [Test]
        public async Task Test_EditChatRoom_ChangesCorrectly()
        {
            // Arrange: get guest user from test db
            ChatUser guestUser = this.testDb.GuestUser;

            // Arrange: create data for a new chat room
            string name = $"New Create Chat Room {DateTime.Now.Ticks}";
            string ownerId = guestUser.Id;

            // Arrange: create and save new room
            await this.chatService.CreateRoomAsync(name, ownerId);

            // Act: get the room and then edit it with service method
            Data.ChatRoom room = await this.dbContext.ChatRooms
                .LastOrDefaultAsync();

            string newName = "New Edited Room Name";
            await this.chatService.EditRoomAsync(room!.Id, newName);

            // Assert: room is edited
            room = await this.dbContext.ChatRooms
                .FirstOrDefaultAsync(cr => cr.Id == room.Id);
            Assert.That(room, Is.Not.Null);
            Assert.That(room.Name, Is.EqualTo(newName));
        }
    }
}
