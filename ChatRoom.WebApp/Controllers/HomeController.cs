using System.Linq;
using System.Threading.Tasks;

using ChatRoom.Services.Contracts;
using ChatRoom.Services.Models.Home;
using ChatRoom.WebApp.Claims;
using ChatRoom.WebApp.Models.ChatRoom;
using ChatRoom.WebApp.Models.Home;

using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService homeService)
        {
            this.homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.View();
            }

            HomeServiceModel serviceModel = await this.homeService.GetUserRoomsAsync(this.User.Id());

            HomeViewModel model = new HomeViewModel
            {
                ChatRooms = serviceModel.ChatRooms
                    .Select(cr => new ChatRoomModel
                    {
                        Id = cr.Id,
                        Name = cr.Name,
                        Owner = cr.Owner
                    })
                    .ToList(),
            };

            return View(model);
        }

        public IActionResult Error401()
        {
            return this.View();
        }

        public IActionResult Error404()
        {
            return this.View();
        }
    }
}
