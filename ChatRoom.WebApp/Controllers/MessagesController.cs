using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using ChatRoom.Services.Contracts;
using ChatRoom.WebApp.Claims;
using ChatRoom.WebApp.Models.Message;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChatRoom.WebApp.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int chatRoomId, MessageFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                foreach (ModelError error in this.ModelState.Values.SelectMany(entry => entry.Errors))
                {
                    this.TempData["Error"] = error.ErrorMessage;
                }

                return this.RedirectToAction("Details", "ChatRoom", new { id = chatRoomId });
            }

            await this.messageService.SaveMessage(
                model.NewMessage,
                chatRoomId,
                this.User.Id());

            return this.RedirectToAction("Details", "ChatRoom", new { id = chatRoomId });
        }
    }
}
