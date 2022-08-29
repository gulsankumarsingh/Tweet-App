using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using User.API.Infrastructure.DataContext;
using User.API.Infrastructure.Repository;
using User.API.Infrastructure.Repository.Interface;
using User.API.Models;

namespace UserServiceTest.RepositoryTest
{
    public class UserRepositoryTest
    {
        private static DbContextOptions<UserDbContext> _dbContextOptions = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "UserProfileDb").Options;

        IUserRepository _userRepository;
        Mock<ILogger<UserRepository>> _loggerMock;


        [OneTimeSetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<UserRepository>>();
            SeedDatabase();
            _userRepository = new UserRepository(new UserDbContext(_dbContextOptions), _loggerMock.Object);

        }

        [Test]
        public async Task GetAllUsers_ListOfUsers_UserListExistInRepo()
        {
            using var context = new UserDbContext(_dbContextOptions);

            var result = await _userRepository.GetAllUsersAsync();

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<UserProfile>>(result);
        }

        [Test]
        public async Task GetUserByEmailAsync_UserProfile_UserExistInRepo()
        {
            //Arrange
            string mockEmail = "gulsan@gmail.com";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.GetUserByEmailAsync(mockEmail);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserProfile>(result);
        }

        [Test]
        public async Task GetUserByUserNameAsync_UserProfile_UserExistInRepo()
        {
            //Arrange
            string mockusername = "Gulsan";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.GetUserByUserNameAsync(mockusername);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserProfile>(result);
        }

        [Test]
        public async Task GetUserByUserNameAsync_ShouldReturnNull_UserNotExistInRepo()
        {
            //Arrange
            string mockusername = "Ajith";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.GetUserByUserNameAsync(mockusername);

            Assert.IsNotNull(_userRepository);
            Assert.IsNull(result);
        }
        [Test]
        public async Task SearchUserAsync_UserProfile_UserExistInRepo()
        {
            //Arrange
            string mockusername = "Gul";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.SearchUserAsync(mockusername);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<UserProfile>>(result);
        }

        [Test]
        public async Task SearchUserAsync_ShouldReturnNull_UserNotExistInRepo()
        {
            //Arrange
            string mockusername = "skjdf";

            //Act
            var result = await _userRepository.SearchUserAsync(mockusername);

            Assert.IsNotNull(_userRepository);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task IsEmailExistAsync_TrueIfExist_UserExistInRepo()
        {
            //Arrange
            string mockEmail = "gulsan@gmail.com";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.IsEmailExistAsync(mockEmail);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task IsEmailExistAsync_ShouldReturnFalse_UserNotExistInRepo()
        {
            //Arrange
            string mockEmail = "ajith@gmail.com";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.IsEmailExistAsync(mockEmail);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(false, result);
        }
        [Test]
        public async Task IsUserNameExistAsync_TrueIfExist_UserExistInRepo()
        {
            //Arrange
            string mockUsername = "Gulsan";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.IsUserNameExistAsync(mockUsername);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task IsUserNameExistAsync__ShouldReturnFalse_UserNotExistInRepo()
        {
            //Arrange
            string mockUsername = "Abcd";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.IsUserNameExistAsync(mockUsername);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(false, result);
        }

        [Test]
        public async Task VerifyUserAsync_UserProfile_UserExistInRepo()
        {
            //Arrange
            string mockEmail = "gulsan@gmail.com";
            string mockPassword = "Gulsan@123";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.VerifyUserAsync(mockEmail, mockPassword);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserProfile>(result);
        }
        [Test]
        public async Task VerifyUserAsync_ShouldReturnNull_UserNotExistInRepo()
        {
            //Arrange
            string mockEmail = "ajith@gmail.com";
            string mockPassword = "Ajith@123";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.VerifyUserAsync(mockEmail, mockPassword);

            Assert.IsNotNull(_userRepository);
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task AddUserAsync_TrueIfAddedSuccessfully_ValidData()
        {
            //Arrange
            UserProfile mockProfile = new UserProfile()
            {
                Username = "Sandeep",
                Email = "sandeep@gmail.com",
                FirstName = "Sandeep",
                LastName = "Jha",
                Gender = "Male",
                DateOfBirth = new DateTime(1992, 1, 5),
                Password = "Sandeep@123",
                ContactNumber = 8787983299,
            };
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.AddUserAsync(mockProfile);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task UpdateUserAsync_TrueIfUpdatedSuccessfully_ValidData()
        {
            //Arrange
            UserProfile mockProfile = GetListOfUserProfiles()[7];
            mockProfile.FirstName = "Ankur";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.UpdateUsersAsync(mockProfile);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
        }

        [Test]
        public async Task DeleteUserAsync_TrueIfDeletedSuccessfully_ValidData()
        {
            //Arrange
            UserProfile mockProfile = GetListOfUserProfiles()[3];
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.DeleteUsersAsync(mockProfile);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task ChangePasswordAsync_TrueIfSuccessfully_UserExistInRepo()
        {
            //Arrange
            UserProfile mockProfile = GetListOfUserProfiles()[4];
            mockProfile.Password = "NewPassword@123";
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.ChangePasswordAsync(mockProfile);

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task SaveChangesAsync_TrueIfSuccessfully_ValidCheck()
        {
            //Arrange
            using var context = new UserDbContext(_dbContextOptions);

            //Act
            var result = await _userRepository.SaveChangesAsync();

            Assert.IsNotNull(_userRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }
        private void SeedDatabase()
        {
            using var context = new UserDbContext(_dbContextOptions);
            context.UserProfiles.AddRange(GetListOfUserProfiles());
            context.SaveChanges();
        }

        private static List<UserProfile> GetListOfUserProfiles()
        {
            List<UserProfile> userProfiles = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Username= "Gulsan",
                    Email="gulsan@gmail.com",
                    FirstName="Gulsan",
                    LastName="Singh",
                    Gender="Male",
                    DateOfBirth= new DateTime(1996, 11, 25),
                    Password="Gulsan@123",
                    ContactNumber=7974113407,
                },
                new UserProfile()
                {
                    Username= "Sourav",
                    Email="sourav@gmail.com",
                    FirstName="Sourav",
                    LastName="Singh",
                    Gender="Male",
                    DateOfBirth= new DateTime(1997, 10, 16),
                    Password="Sourav@123",
                    ContactNumber=9876543210,
                },
                new UserProfile()
                {
                    Username= "Anil",
                    Email="anil@gmail.com",
                    FirstName="Anil",
                    LastName="Debata",
                    Gender="Male",
                    DateOfBirth= new DateTime(1995, 1, 2),
                    Password="Anil@1234",
                    ContactNumber=8770345642,
                },
                new UserProfile()
                {
                    Username= "Pooja",
                    Email="pooja@gmail.com",
                    FirstName="Pooja",
                    LastName="Roy",
                    Gender="Female",
                    DateOfBirth= new DateTime(1991, 05, 25),
                    Password="Pooja@123",
                    ContactNumber=6789234324,
                },
                new UserProfile()
                {
                    Username= "Sanjana",
                    Email="sanjana@gmail.com",
                    FirstName="Sanjana",
                    LastName="Prasad",
                    Gender="Female",
                    DateOfBirth= new DateTime(1993, 7, 7),
                    Password="Sanjana@123",
                    ContactNumber=8796784293,
                },
                new UserProfile()
                {
                    Username= "Ranjeet",
                    Email="ranjeet@gmail.com",
                    FirstName="Ranjeet",
                    LastName="Thakur",
                    Gender="Male",
                    DateOfBirth= new DateTime(1993, 7, 7),
                    Password="Ranjeet@123",
                    ContactNumber=8796784293,
                },
                new UserProfile()
                {
                    Username= "Kumar",
                    Email="kumar@gmail.com",
                    FirstName="Kumar",
                    LastName="Prasad",
                    Gender="Male",
                    DateOfBirth= new DateTime(1993, 7, 7),
                    Password="Kumar@123",
                    ContactNumber=998234833,
                },
                new UserProfile()
                {
                    Username= "Abhishek",
                    Email="abhishek@gmail.com",
                    FirstName="Abhishek",
                    LastName="Prasad",
                    Gender="Male",
                    DateOfBirth= new DateTime(1993, 7, 7),
                    Password="Abhishek@123",
                    ContactNumber=998234833,
                },
            };
            return userProfiles;
        }

    }
}
