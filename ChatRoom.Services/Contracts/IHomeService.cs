﻿using System.Security.Claims;
using System.Threading.Tasks;

using ChatRoom.Services.Models.Home;

namespace ChatRoom.Services.Contracts
{
    public interface IHomeService
    {
        Task<HomeServiceModel> GetUserRoomsAsync(string userId);
    }
}
