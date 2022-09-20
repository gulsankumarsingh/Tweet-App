namespace User.API.Infrastructure.Repository.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using User.API.Models;

    /// <summary>
    /// Defines the <see cref="IUserRepository" />.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// The GetAllUsersAsync.
        /// </summary>
        /// <returns>The list of user profiles.</returns>
        Task<List<UserProfile>> GetAllUsersAsync();

        /// <summary>
        /// The GetUserByEmailAsync.
        /// </summary>
        /// <param name="email">The email<see cref="string"/>.</param>
        /// <returns>The user profile </returns>
        Task<UserProfile> GetUserByEmailAsync(string email);

        /// <summary>
        /// The GetUserByUserNameAsync.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The user profile</returns>
        Task<UserProfile> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// The SearchUserAsync.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The list of user profiles</returns>
        Task<List<UserProfile>> SearchUserAsync(string userName);

        /// <summary>
        /// The AddUserAsync.
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if user added successful else false.</returns>
        Task<bool> AddUserAsync(UserProfile user);

        /// <summary>
        /// The UpdateUsersAsync.
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if user udpated successful else false.</returns>
        Task<bool> UpdateUsersAsync(UserProfile user);

        /// <summary>
        /// The DeleteUsersAsync.
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if user delete successful else false.</returns>
        Task<bool> DeleteUsersAsync(UserProfile user);

        /// <summary>
        /// The ChangePasswordAsync.
        /// </summary>
        /// <param name="user">The user<see cref="UserProfile"/>.</param>
        /// <returns>true if password udpated successful else false.</returns>
        Task<bool> ChangePasswordAsync(UserProfile user);


        /// <summary>
        /// The VerifyUserAsync.
        /// </summary>
        /// <param name="email">The eamil<see cref="string"/>.</param>
        /// <param name="password">The password<see cref="string"/>.</param>
        /// <returns>The user profile</returns>
        Task<UserProfile> VerifyUserAsync(string email, string password);

        /// <summary>
        /// The SaveChangesAsync.
        /// </summary>
        /// <returns>true if records saved else false.</returns>
        Task<bool> SaveChangesAsync();
    }
}
