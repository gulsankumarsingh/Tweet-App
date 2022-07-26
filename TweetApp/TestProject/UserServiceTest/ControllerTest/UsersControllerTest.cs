﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using User.API.Infrastructure.Repository.Interface;
using User.API.Infrastructure.Services.AuthenticationService.Interfaces;
using User.API.Infrastructure.Services.AzureServiceBusSender.Interface;
using User.API.Infrastructure.Services.ImageHandlerService.Interface;
using User.API.Infrastructure.Services.MessageSenderService.Interface;
using User.API.Models;
using User.API.Models.Dtos;
using UserService.API.Controllers;

namespace UserServiceTest.ControllerTest
{
    [TestFixture]
    public class UsersControllerTest
    {
        private readonly Mock<ILogger<UsersController>> _loggerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtAuthentication> _jwtAuthenticationMock;
        private readonly Mock<IMessageSender> _messageSenderMock;
        private readonly Mock<IDeleteUserMessageSender> _deleteUserMessageSenderMock;
        private readonly Mock<IHandleImage> _handleImageMock;
        private readonly IMapper _mapper;
        private readonly UsersController _usersController;
     
        public UsersControllerTest()
        {
            _loggerMock = new Mock<ILogger<UsersController>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtAuthenticationMock = new Mock<IJwtAuthentication>();
            _messageSenderMock = new Mock<IMessageSender>();
            _deleteUserMessageSenderMock = new Mock<IDeleteUserMessageSender>();
            _handleImageMock = new Mock<IHandleImage>();
            if (_mapper == null)
            {
                _mapper = UserServiceConfiguration.GetAutoMapperConfiguration();
            }
            _usersController = new UsersController(_userRepositoryMock.Object, _loggerMock.Object, _mapper, 
                                _jwtAuthenticationMock.Object, _messageSenderMock.Object, 
                                _deleteUserMessageSenderMock.Object, _handleImageMock.Object);
        }

        [Test]
        public async Task GetAllUsers_ListOfUsers_UserListExistInRepo()
        {
            //Arrange
            _userRepositoryMock.Setup(i => i.GetAllUsersAsync()).ReturnsAsync(GetListOfUserProfiles());

            //Act
            var actionResult = await _usersController.GetAllUsersAsync();
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as List<UserDto>;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<UserDto>>(value);
            Assert.AreEqual(GetListOfUserProfiles().Count, value.Count);
        }
        [Test]
        public async Task GetUserAsync_ApiResponse_UsernameExistInRepo()
        {
            //Arrange
            string fakeName = "gulsan";
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeName)).ReturnsAsync(GetListOfUserProfiles().Find(i => i.Username == fakeName));

            //Act
            var actionResult = await _usersController.GetUserAsync(fakeName);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNotNull(value.ResponseValue);
            Assert.IsInstanceOf<UserProfileDto>(value.ResponseValue);
        }

        [Test]
        public async Task GetUserAsync_shouldReturnNotFound_UsernameNotExistInRepo()
        {
            //Arrange
            string fakeName = "Ajit";
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeName)).ReturnsAsync(GetListOfUserProfiles().Find(i => i.Username == fakeName));

            //Act
            var actionResult = await _usersController.GetUserAsync(fakeName);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task SearchByUsernameAsync_ApiResponse_UsernameExistInRepo()
        {
            //Arrange
            string fakeName = "sou";
            var list = GetListOfUserProfiles().FindAll(i => i.Username.Contains(fakeName));
            _userRepositoryMock.Setup(i => i.SearchUserAsync(fakeName)).ReturnsAsync(list);
            //Act
            var actionResult = await _usersController.SearchByUserNameAsync(fakeName);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as List<UserDto>;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<UserDto>>(value);
            Assert.AreEqual(list.Count, value.Count);

        }

        [Test]
        public async Task SearchByUsernameAsync_shouldReturnEmptyObject_UsernameNotExistInRepo()
        {
            //Arrange
            string fakeName = "ajit";
            var list = GetListOfUserProfiles().FindAll(i => i.Username.Contains(fakeName));
            _userRepositoryMock.Setup(i => i.SearchUserAsync(fakeName)).ReturnsAsync(list);

            //Act
            var actionResult = await _usersController.SearchByUserNameAsync(fakeName);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as List<UserDto>;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<UserDto>>(value);
            Assert.AreEqual(0, value.Count);
        }
        [Test]
        public async Task LoginAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeName = "gulsan";
            UserLoginModel fakeModel = new UserLoginModel
            {
                Email = "gulsan@gmail.com",
                Password = "Gulsan@123"
            };
            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Email == fakeModel.Email && i.Password == fakeModel.Password);
            _userRepositoryMock.Setup(i => i.VerifyUserAsync(fakeModel.Email, fakeModel.Password)).ReturnsAsync(userProfile);
            _userRepositoryMock.Setup(i => i.UpdateUsersAsync(userProfile)).ReturnsAsync(true);
            _jwtAuthenticationMock.Setup(i => i.GenerateToken(fakeName)).Returns(new TokenDetail()
            {
                Username = fakeName,
                Token = "Thisisatesttokenforunittesting",
                Expiration = new DateTime().AddMinutes(30)
            });

            //Act
            var actionResult = await _usersController.LoginAsync(fakeModel);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNotNull(value.ResponseValue);
            Assert.IsInstanceOf<TokenDetail>(value.ResponseValue);

        }

        [Test]
        public async Task LoginAsync_shouldReturnBadRequest_InvalidInfoEntered()
        {
            //Arrange
            UserLoginModel fakeModel = new UserLoginModel
            {
                Email = "ajit@gmail.com",
                Password = "Ajit@123"
            };
            _userRepositoryMock.Setup(i => i.VerifyUserAsync(fakeModel.Email, fakeModel.Password)).ReturnsAsync(GetListOfUserProfiles().Find(i => i.Email == fakeModel.Email && i.Password == fakeModel.Password));

            //Act
            var actionResult = await _usersController.LoginAsync(fakeModel);
            var result = actionResult.Result as BadRequestObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);

        }

        [Test]
        public async Task LoginAsync_shouldReturnBadRequest_InvalidPasswordEntered()
        {
            //Arrange
            UserLoginModel fakeModel = new UserLoginModel
            {
                Email = "gulsan@gmail.com",
                Password = "Password@123"
            };
            _userRepositoryMock.Setup(i => i.VerifyUserAsync(fakeModel.Email, fakeModel.Password)).ReturnsAsync(GetListOfUserProfiles().Find(i => i.Email == fakeModel.Email && i.Password == fakeModel.Password));

            //Act
            var actionResult = await _usersController.LoginAsync(fakeModel);
            var result = actionResult.Result as BadRequestObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);

        }

        [Test]
        public async Task LogoutAsync_ApiResponse_Success()
        {
            //Arrange
            string fakeName = "gulsan";
            var userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeName);
            userProfile.IsActive = false;
            userProfile.LogoutAt = DateTime.Now;
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeName)).ReturnsAsync(userProfile);
            _userRepositoryMock.Setup(i => i.UpdateUsersAsync(userProfile)).ReturnsAsync(true);

            //Act
            var actionResult = await _usersController.LogoutAsync(fakeName);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNull(value.ResponseValue);
        }
        [Test]
        public async Task RegisterAsync_ApiResponse_ValidInputEntered()
        {
            //Arrange
            CreateUserProfileDto createUserProfile = new CreateUserProfileDto()
            {
                Username = "soni",
                Email = "soni@gmail.com",
                FirstName = "Soni",
                LastName = "Singh",
                Gender = "Female",
                DateOfBirth = new DateTime(1993, 11, 18),
                Password = "Soni@123",
                ContactNumber = 8776567894,
            };
            _userRepositoryMock.Setup(i => i.GetUserByEmailAsync(createUserProfile.Email)).ReturnsAsync((UserProfile)null);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(createUserProfile.Username)).ReturnsAsync((UserProfile)null);
            _userRepositoryMock.Setup(i => i.AddUserAsync(It.IsAny<UserProfile>())).ReturnsAsync(true);

            //Act
            var actionResult = await _usersController.RegisterAsync(createUserProfile);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNull(value.ResponseValue);
        }
        [Test]
        public async Task RegisterAsync_shouldReturnBadRequest_EmailAlreadyExist()
        {
            //Arrange
            CreateUserProfileDto createUserProfile = new CreateUserProfileDto()
            {
                Username = "anil_deb",
                Email = "anil@gmail.com",
                FirstName = "Anil",
                LastName = "Debata",
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 2),
                Password = "Anil@12345",
                ContactNumber = 7679987899,
            };
            UserProfile user = GetListOfUserProfiles().Find(i => i.Email == createUserProfile.Email.ToLower());
            _userRepositoryMock.Setup(i => i.GetUserByEmailAsync(createUserProfile.Email)).ReturnsAsync(user);

            //Act
            var actionResult = await _usersController.RegisterAsync(createUserProfile);
            var result = actionResult.Result as BadRequestObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("EmailExist", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task RegisterAsync_shouldReturnBadRequest_UsernameAlreadyExist()
        {
            //Arrange
            CreateUserProfileDto createUserProfile = new CreateUserProfileDto()
            {
                Username = "gulsan",
                Email = "gulsan123@gmail.com",
                FirstName = "Gulsan",
                LastName = "Sharma",
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 2),
                Password = "Sharma@123",
                ContactNumber = 8887777989,
            };
            UserProfile user = GetListOfUserProfiles().Find(i => i.Username.ToLower() == createUserProfile.Username.ToLower());
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(createUserProfile.Username)).ReturnsAsync(user);


            //Act
            var actionResult = await _usersController.RegisterAsync(createUserProfile);
            var result = actionResult.Result as BadRequestObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("UsernameExist", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task UpdateProfileAsync_ApiResponse_ValidInput()
        {
            //Arrange
            string fakeUsername = "sourav";
            UpdateUserProfileDto updateUserProfile = new UpdateUserProfileDto()
            {
                Email = "sourav@gmail.com",
                FirstName = "Sourav",
                LastName = "Singh",
                Gender = "Male",
                DateOfBirth = new DateTime(1997, 10, 16),
                ContactNumber = 7678787966,
            };
            UserProfile user = GetListOfUserProfiles().Find(i => i.Username.ToLower() == fakeUsername.ToLower());
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(user);
            _userRepositoryMock.Setup(i => i.UpdateUsersAsync(It.IsAny<UserProfile>())).ReturnsAsync(true);

            //Act
            var actionResult = await _usersController.UpdateProfileAsync(fakeUsername,updateUserProfile);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task UpdateProfileAsync_shouldReturnNotFound_UsernameNotExist()
        {
            //Arrange
            string fakeUsername = "ajit";
            UpdateUserProfileDto updateUserProfile = new UpdateUserProfileDto()
            {
                Email = "ajit@gmail.com",
                FirstName = "Ajit",
                LastName = "Singh",
                Gender = "Male",
                DateOfBirth = new DateTime(1997, 10, 16),
                ContactNumber = 7678787966,
            };
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync((UserProfile)null);

            //Act
            var actionResult = await _usersController.UpdateProfileAsync(fakeUsername, updateUserProfile);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }
        
        [Test]
        public async Task DeleteProfileAsync_ApiResponse_UsernameExist()
        {
            //Arrange
            string fakeUsername = "pooja";
            string fileName = "abc.png";

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);
            _userRepositoryMock.Setup(i => i.DeleteUsersAsync(userProfile)).ReturnsAsync(true);
            _handleImageMock.Setup(i => i.DeleteImageFileAsync(fileName)).ReturnsAsync(true);

            //Act
            var actionResult = await _usersController.DeleteProfileAsync(fakeUsername);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task DeleteProfileAsync_shouldReturnNotFound_UsernameNotExist()
        {
            //Arrange
            string fakeUsername = "Ajit";
            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);

            //Act
            var actionResult = await _usersController.DeleteProfileAsync(fakeUsername);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task ForgotPasswordAsync_ApiResponse_ValidInput()
        {
            //Arrange
            string fakeUsername = "sanjana";
            ChangePasswordDto passwordDto = new ChangePasswordDto
            {
                Email = "sanjana@gmail.com",
                Password = "Sanjana@12345",
                ContactNumber = 8796784293
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);
            userProfile.Password = passwordDto.Password;
            _userRepositoryMock.Setup(i => i.ChangePasswordAsync(userProfile)).ReturnsAsync(true);

            //Act
            var actionResult = await _usersController.ForgotPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task ForgotPasswordAsync_shouldReturnNotFound_UserNotExist()
        {
            //Arrange
            string fakeUsername = "Anil123";
            ChangePasswordDto passwordDto = new ChangePasswordDto
            {
                Email = "anil@gmail.com",
                Password = "Anil@12345",
                ContactNumber = 8770345642
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);


            //Act
            var actionResult = await _usersController.ForgotPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task ForgotPasswordAsync_shouldReturnNotFound_InvalidEmailEntered()
        {
            //Arrange
            string fakeUsername = "anil";
            ChangePasswordDto passwordDto = new ChangePasswordDto
            {
                Email = "anil12@gmail.com",
                Password = "Anil@12345",
                ContactNumber = 8770345642
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);


            //Act
            var actionResult = await _usersController.ForgotPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task ForgotPasswordAsync_shouldReturnNotFound_InvalidContactNumberEntered()
        {
            //Arrange
            string fakeUsername = "anil";
            ChangePasswordDto passwordDto = new ChangePasswordDto
            {
                Email = "anil@gmail.com",
                Password = "Anil@12345",
                ContactNumber = 8772342345
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);


            //Act
            var actionResult = await _usersController.ForgotPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task ResetPasswordAsync_ApiResponse_ValidInput()
        {
            //Arrange
            string fakeUsername = "gulsan";
            ResetPasswordDto passwordDto = new ResetPasswordDto
            {
                OldPassword = "Gulsan@123",
                NewPassword = "Gulsan@1234"
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);
            _userRepositoryMock.Setup(i => i.ChangePasswordAsync(It.IsAny<UserProfile>())).ReturnsAsync(true);

            //Act
            var actionResult = await _usersController.ResetPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
            Assert.IsNull(value.ResponseValue);
        }

        [Test]
        public async Task ResetPasswordAsync_shouldReturnNotFound_UserNotExist()
        {
            //Arrange
            string fakeUsername = "gulsan123";
            ResetPasswordDto passwordDto = new ResetPasswordDto
            {
                OldPassword = "Gulsan@123",
                NewPassword = "Gulsan@1234"
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);


            //Act
            var actionResult = await _usersController.ResetPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }
        [Test]
        public async Task ResetPasswordAsync_shouldReturnNotFound_IncorrectOldPassword()
        {
            //Arrange
            string fakeUsername = "sourav";
            ResetPasswordDto passwordDto = new ResetPasswordDto
            {
                OldPassword = "Password@123",
                NewPassword = "Sourav@1234"
            };

            UserProfile userProfile = GetListOfUserProfiles().Find(i => i.Username == fakeUsername);
            _userRepositoryMock.Setup(i => i.GetUserByUserNameAsync(fakeUsername)).ReturnsAsync(userProfile);


            //Act
            var actionResult = await _usersController.ResetPasswordAsync(fakeUsername, passwordDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_usersController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
            Assert.IsNull(value.ResponseValue);
        }
        private static List<UserProfile> GetListOfUserProfiles()
        {
            List<UserProfile> userProfiles = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Username= "gulsan",
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
                    Username= "sourav",
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
                    Username= "anil",
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
                    Username= "pooja",
                    Email="pooja@gmail.com",
                    FirstName="Pooja",
                    LastName="Roy",
                    Gender="Female",
                    DateOfBirth= new DateTime(1991, 05, 25),
                    Password="Pooja@123",
                    ContactNumber=6789234324,
                    ProfileImage = "http//somewhere/abc.png",
                    ImageName="abc.png"
                },
                new UserProfile()
                {
                    Username= "sanjana",
                    Email="sanjana@gmail.com",
                    FirstName="Sanjana",
                    LastName="Prasad",
                    Gender="Female",
                    DateOfBirth= new DateTime(1993, 7, 7),
                    Password="Sanjana@123",
                    ContactNumber=8796784293,
                },
            };
            return userProfiles;
        }
    }
   
}
