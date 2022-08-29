namespace User.API.Infrastructure.Repository
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using User.API.Infrastructure.DataContext;
    using User.API.Infrastructure.Repository.Interface;
    using User.API.Models;

    /// <summary>
    /// Defines the User repository.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Defines the _dbContext.
        /// </summary>
        private readonly UserDbContext _dbContext;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<UserRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="UserDbContext"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{UserRepository}"/>.</param>
        public UserRepository(UserDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Method for Get All Users
        /// </summary>
        /// <returns>The list of user profiles.</returns>
        public async Task<List<UserProfile>> GetAllUsersAsync()
        {
            List<UserProfile> userList = null;
            try
            {
                userList = await _dbContext.UserProfiles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError( "An error occured while fetching all users!", ex.Message);
            }
            return userList;
        }

        /// <summary>
        /// Method for Get User By Email
        /// </summary>
        /// <param name="email">The email<see cref="string"/>.</param>
        /// <returns>The user profile </returns>
        public async Task<UserProfile> GetUserByEmailAsync(string email)
        {
            UserProfile user = null;
            try
            {
                user = await _dbContext.UserProfiles.FirstOrDefaultAsync(e => e.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching user by email!", ex.Message);
            }
            return user;
        }

        /// <summary>
        /// Method for Get User By User Name Async.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The user profile</returns>
        public async Task<UserProfile> GetUserByUserNameAsync(string userName)
        {
            UserProfile user = null;
            try
            {
                user = await _dbContext.UserProfiles.FirstOrDefaultAsync(e => e.Username == userName);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching user by user name!", ex.Message);
            }
            return user;
        }

        /// <summary>
        /// Method for Search User By User Name.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The list of user profiles</returns>
        public async Task<List<UserProfile>> SearchUserAsync(string userName)
        {
            List<UserProfile> user = null;
            try
            {
                var userProfiles = await _dbContext.UserProfiles.ToListAsync();
                user = userProfiles.Where(e => e.Username.ToLower().Contains(userName.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching user by username!", ex.Message);
            }
            return user;
        }

        

        /// <summary>
        /// Method for Check if User Exist
        /// </summary>
        /// <param name="email">The email<see cref="string"/>.</param>
        /// <returns>The true if user exist else false.</returns>
        public async Task<bool> IsEmailExistAsync(string email)
        {
            bool isUserExist = false;
            try
            {
                isUserExist = await _dbContext.UserProfiles.AnyAsync(e => e.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while checking user exist or not!", ex.Message);
            }
            return isUserExist;
        }

        /// <summary>
        /// Method for Check if User Name Exist
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The true if user exist else false.</returns>
        public async Task<bool> IsUserNameExistAsync(string userName)
        {
            bool isUserExist = false;
            try
            {
                isUserExist = await _dbContext.UserProfiles.AnyAsync(e => e.Username == userName);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while checking user exist or not!", ex.Message);
            }
            return isUserExist;
        }

        /// <summary>
        /// Method for Verify User Info
        /// </summary>
        /// <param name="email">The email<see cref="string"/>.</param>
        /// <param name="password">The password<see cref="string"/>.</param>
        /// <returns>The user profile</returns>
        public async Task<UserProfile> VerifyUserAsync(string email, string password)
        {
            UserProfile user = null;
            try
            {
                user = await _dbContext.UserProfiles.FirstOrDefaultAsync(e => (e.Email == email) && e.Password == password);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while verifying user detail!", ex.Message);
            }
            return user;
        }

        /// <summary>
        /// Method for adding new user
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if user added successful else false.</returns>
        public async Task<bool> AddUserAsync(UserProfile user)
        {
            bool isUserAdded = false;
            try
            {
                await _dbContext.UserProfiles.AddAsync(user);
                isUserAdded = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while adding user!", ex.Message);
            }
            return isUserAdded;
        }

        /// <summary>
        /// Method for Update User Information.
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if user udpated successful else false.</returns>
        public async Task<bool> UpdateUsersAsync(UserProfile user)
        {
            bool isUpadateSuccess = false;
            try
            {
                _dbContext.UserProfiles.Update(user);
                isUpadateSuccess = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while updating user!", ex.Message);
            }
            return isUpadateSuccess;
        }

        /// <summary>
        /// Method for Delete User Information.
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if user delete successful else false.</returns>
        public async Task<bool> DeleteUsersAsync(UserProfile user)
        {
            bool isDeleteSuccess = false;
            try
            {
                _dbContext.UserProfiles.Remove(user);
                isDeleteSuccess = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while removing user!", ex.Message);
            }
            return isDeleteSuccess;
        }

        /// <summary>
        /// Method for Change Password
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if password udpated successful else false.</returns>
        public async Task<bool> ChangePasswordAsync(UserProfile user)
        {
            bool isUpadateSuccess = false;
            try
            {
                _dbContext.UserProfiles.Attach(user);
                _dbContext.Entry(user).Property(x => x.Password).IsModified = true;
                isUpadateSuccess = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError( "An error occured while updating user!", ex.Message);
            }
            return isUpadateSuccess;
        }

        /// <summary>
        /// Method for Save Changes to database
        /// </summary>
        /// <returns>true if records saved else false.</returns>
        public async Task<bool> SaveChangesAsync()
        {
            bool isRecordSaved = false;
            try
            {
                isRecordSaved = await _dbContext.SaveChangesAsync() >= 0;
            }
            catch (Exception ex)
            {
                _logger.LogError( "An error occured while saving the record to db!", ex.Message);
            }
            return isRecordSaved;
        }
    }
}
