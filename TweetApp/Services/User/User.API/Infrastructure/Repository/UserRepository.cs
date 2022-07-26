using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Infrastructure.DataContext;
using User.API.Models;
using User.API.Infrastructure.Repository.Interface;

namespace User.API.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Users
        public async Task<List<UserProfile>> GetAllUsersAsync()
        {
            List<UserProfile> userList = null;
            try
            {
                userList = await _dbContext.UserProfiles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching all users!");
            }
            return userList;
        }
        public async Task<UserProfile> GetUserByEmailAsync(string email)
        {
            UserProfile user = null;
            try
            {
                user = await _dbContext.UserProfiles.FirstOrDefaultAsync(e => e.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching user by email!");
            }
            return user;
        }
        public async Task<UserProfile> GetUserByUserNameAsync(string userName)
        {
            UserProfile user = null;
            try
            {
                user = await _dbContext.UserProfiles.FirstOrDefaultAsync(e => e.LoginId == userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching user by user name!");
            }
            return user;
        }
        public async Task<List<UserProfile>> SearchUserAsync(string userName)
        {
            List<UserProfile> user = null;
            try
            {
                user = await _dbContext.UserProfiles.Where(e => e.LoginId.Contains(userName)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching user by username!");
            }
            return user;
        }
        public async Task<List<UserProfile>> GetActiveUsersAsync()
        {
            List<UserProfile> userList = null;
            try
            {
                userList = await _dbContext.UserProfiles.Where(i => i.IsActive).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching active users!");
            }
            return userList;
        }
        public async Task<bool> IsUserExistAsync(string email)
        {
            bool isUserExist = false;
            try
            {
                isUserExist = await _dbContext.UserProfiles.AnyAsync(e => e.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while checking user exist or not!");
            }
            return isUserExist;
        }
        public async Task<bool> IsUserNameExistAsync(string userName)
        {
            bool isUserExist = false;
            try
            {
                isUserExist = await _dbContext.UserProfiles.AnyAsync(e => e.LoginId == userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while checking user exist or not!");
            }
            return isUserExist;
        }
        public async Task<UserProfile> VerifyUserAsync(string userName, string password)
        {
            UserProfile user = null;
            try
            {
                user = await _dbContext.UserProfiles.FirstOrDefaultAsync(e => (e.Email == userName || e.LoginId == userName) && e.Password == password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while verifying user detail!");
            }
            return user;
        }
        public async Task<bool> AddUserAsync(UserProfile user)
        {
            bool isUserAdded = false;
            try
            {
                await _dbContext.UserProfiles.AddAsync(user);
                isUserAdded = await SaveChangesAsync();
                _logger.LogInformation("User added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while adding user!");
            }
            return isUserAdded;
        }
        public async Task<bool> UpdateUsersAsync(UserProfile user)
        {
            bool isUpadateSuccess = false;
            try
            {
                _dbContext.UserProfiles.Update(user);
                isUpadateSuccess = await SaveChangesAsync();
                _logger.LogInformation("User updated successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while updating user!");
            }
            return isUpadateSuccess;
        }
        public async Task<bool> SaveChangesAsync()
        {
            bool isRecordSaved = false;
            try
            {
                isRecordSaved = await _dbContext.SaveChangesAsync() >= 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while saving the record to db!");
            }
            return isRecordSaved;
        }

        #endregion
    }
}
