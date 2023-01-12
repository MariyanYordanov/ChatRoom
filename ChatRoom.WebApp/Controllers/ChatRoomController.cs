using System.Linq;
using System.Threading.Tasks;

using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.ChatRoom;
using ChatRoom.WebApp.Claims;
using ChatRoom.WebApp.Models.ChatRoom;
using ChatRoom.WebApp.Models.Message;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ChatRoomModel = ChatRoom.Services.Models.ChatRoom.ChatRoomModel;

namespace ChatRoom.WebApp.Controllers
{
    [Authorize]
    public class ChatRoomController : Controller
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatRoomController(IChatService chatService, IUserService userService)
        {
            this.chatService = chatService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(id))
            {
                return this.NotFound();
            }

            if (!await this.chatService.IsUserInChatRoomAsync(id, this.User.Id()))
            {
                return this.Unauthorized();
            }

            ChatRoomServiceModel serviceModel = await this.chatService.GetChatDetailsAsync(id, this.User.Id());

            ChatRoomViewModel model = new ChatRoomViewModel
            {
                Id = serviceModel.Id,
                CurrentUser = serviceModel.CurrentUser,
                OwnerId = serviceModel.OwnerId,
                Messages = serviceModel.Messages
                    .Select(m => new MessageViewModel
                    {
                        Content = m.Content,
                        Owner = m.Owner
                    })
                    .ToList(),
                Members = serviceModel.Members,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateAsync()
        {
            ChatRoomFormModel model = new ChatRoomFormModel();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ChatRoomFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            string ownerId = this.User.Id();

            await this.chatService.CreateRoomAsync(model.Name, ownerId);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(id))
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Id();
            if (!await this.chatService.IsUserChatOwnerAsync(id, currentUserId))
            {
                return this.Unauthorized();
            }

            ChatRoomModel serviceModel = await this.chatService.GetChatRoomAsync(id);

            Models.ChatRoom.ChatRoomModel model = new Models.ChatRoom.ChatRoomModel
            {
                Id = serviceModel.Id,
                Name = serviceModel.Name,
                Owner = serviceModel.Owner
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(ChatRoomModel model)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(model.Id))
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Id();
            if (!await this.chatService.IsUserChatOwnerAsync(model.Id, currentUserId))
            {
                return this.Unauthorized();
            }

            await this.chatService.RemoveRoomAsync(model.Id);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(id))
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Id();
            if (!await this.chatService.IsUserChatOwnerAsync(id, currentUserId))
            {
                return this.Unauthorized();
            }

            ChatRoomModel serviceModel = await this.chatService.GetChatRoomAsync(id);

            Models.ChatRoom.ChatRoomModel viewModel = new Models.ChatRoom.ChatRoomModel
            {
                Id = serviceModel.Id,
                Name = serviceModel.Name,
                Owner = serviceModel.Owner,
            };

            ChatRoomFormModel model = new ChatRoomFormModel
            {
                Name = viewModel.Name,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(int id, ChatRoomFormModel model)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(id))
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Id();
            if (!await this.chatService.IsUserChatOwnerAsync(id, currentUserId))
            {
                return this.Unauthorized();
            }

            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            await this.chatService.EditRoomAsync(id, model.Name);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(int chatRoomId)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(chatRoomId))
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Id();
            if (!await this.chatService.IsUserChatOwnerAsync(chatRoomId, currentUserId))
            {
                return this.Unauthorized();
            }

            UserServiceFormModel serviceModel = await this.userService.GetUsersToAddAsync(chatRoomId);

            UserFormModel model = new UserFormModel
            {
                Id = serviceModel.Id,
                RoomName = serviceModel.RoomName,
                Users = serviceModel.Users,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string userId, UserFormModel model)
        {
            await this.userService.AddUserToRoomAsync(model.Id, userId);

            return this.RedirectToAction("Details", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> RemoveUser(int chatRoomId)
        {
            if (!await this.chatService.DoesChatRoomExistAsync(chatRoomId))
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Id();
            if (!await this.chatService.IsUserChatOwnerAsync(chatRoomId, currentUserId))
            {
                return this.Unauthorized();
            }

            UserServiceFormModel serviceModel = await this.userService.GetUsersToRemoveAsync(chatRoomId);

            UserFormModel model = new UserFormModel
            {
                Id = serviceModel.Id,
                RoomName = serviceModel.RoomName,
                Users = serviceModel.Users,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(string userId, UserFormModel model)
        {
            await this.userService.RemoveUserFromRoomAsync(model.Id, userId);

            return this.RedirectToAction("Details", new { id = model.Id });
        }
    }
}
