using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Models;

namespace User.API.Infrastructure.Repository.Interface
{
    public interface IUserRepository
    {
        Task<List<UserProfile>> GetAllUsersAsync();
        Task<UserProfile> GetUserByEmailAsync(string email);
        Task<UserProfile> GetUserByUserNameAsync(string userName);
        Task<List<UserProfile>> SearchUserAsync(string userName);
        Task<bool> IsUserExistAsync(string email);
        Task<bool> IsUserNameExistAsync(string userName);
        Task<bool> AddUserAsync(UserProfile user);
        Task<bool> UpdateUsersAsync(UserProfile user);
        Task<List<UserProfile>> GetActiveUsersAsync();
        Task<UserProfile> VerifyUserAsync(string userName, string password);
        Task<bool> SaveChangesAsync();
    }
}
