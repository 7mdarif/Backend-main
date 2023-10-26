using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password, string email);
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<bool> UserExists(string text);
    }
}