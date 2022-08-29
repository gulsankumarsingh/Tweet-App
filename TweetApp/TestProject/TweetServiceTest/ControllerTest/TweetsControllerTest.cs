using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tweet.API.Controllers;
using Tweet.API.Infrastructure.Repository.Interface;
using Tweet.API.Models;
using Tweet.API.Models.Dtos;

namespace TweetServiceTest.ControllerTest
{
    [TestFixture]
    public class TweetsControllerTest
    {
        private readonly Mock<ILogger<TweetsController>> _loggerMock;
        private readonly Mock<ITweetRepository> _tweetRepositoryMock;
        private readonly IMapper _mapper;
        private readonly TweetsController _tweetController;

        public TweetsControllerTest()
        {
            _loggerMock = new Mock<ILogger<TweetsController>>();
            _tweetRepositoryMock = new Mock<ITweetRepository>();
            if(_mapper == null)
            {
                _mapper = TweetServiceConfiguration.GetAutoMapperConfiguration();
            }
            _tweetController = new TweetsController(_tweetRepositoryMock.Object, _loggerMock.Object, _mapper);
        }


        [Test]
        public async Task GetAllTweetsAsync_ListOfTweets_TweetListExistInRepo()
        {
            //Arrange
            _tweetRepositoryMock.Setup(i => i.GetAllTweetsAsync()).ReturnsAsync(GetListOfAllTweets());
            _tweetRepositoryMock.Setup(i => i.GetTweetLikesAsync(It.IsAny<string>())).ReturnsAsync(GetListOfAllLikes().FindAll(i => i.TweetId == It.IsAny<string>()));
            _tweetRepositoryMock.Setup(i => i.GetAllReplyByTweetIdAsync(It.IsAny<string>())).ReturnsAsync(GetListOfAllReply().FindAll(i => i.TweetId == It.IsAny<string>()));

            //Act
            var actionResult = await _tweetController.GetAllTweetsAsync();
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as List<TweetInfoDto>;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<TweetInfoDto>>(value);
            Assert.AreEqual(GetListOfAllTweets().Count, value.Count);
            
        }
        [Test]
        public async Task GetTweetByTweetIdAsync_TweetsInfo_TweetListExistInRepo()
        {
            //Arrange
            string fakeTweetId = "1";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId))
                .ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.GetTweetLikesAsync(fakeTweetId))
                .ReturnsAsync(GetListOfAllLikes().FindAll(i => i.TweetId == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.GetAllReplyByTweetIdAsync(fakeTweetId))
                .ReturnsAsync(GetListOfAllReply().FindAll(i => i.TweetId == fakeTweetId));

            //Act
            var actionResult = await _tweetController.GetTweetByTweetIdAsync(fakeTweetId);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as TweetInfoDto;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<TweetInfoDto>(value);

        }
        [Test]
        public async Task GetTweetByTweetIdAsync_ShouldReturnNotFound_EnteredTweetIdNotExistInRepo()
        {
            //Arrange
            string fakeTweetId = "8";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            

            //Act
            var actionResult = await _tweetController.GetTweetByTweetIdAsync(fakeTweetId);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);

        }

        [Test]
        public async Task GetTweetsByUserAsync_TweetsInfo_TweetExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByUserAsync(fakeUsername)).ReturnsAsync(GetListOfAllTweets().FindAll(i => i.UserName == fakeUsername));
            _tweetRepositoryMock.Setup(i => i.GetTweetLikesAsync(It.IsAny<string>())).ReturnsAsync(GetListOfAllLikes().FindAll(i => i.TweetId == It.IsAny<string>()));
            _tweetRepositoryMock.Setup(i => i.GetAllReplyByTweetIdAsync(It.IsAny<string>())).ReturnsAsync(GetListOfAllReply().FindAll(i => i.TweetId == It.IsAny<string>()));

            //Act
            var actionResult = await _tweetController.GetTweetsByUserAsync(fakeUsername);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as List<TweetInfoDto>;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<TweetInfoDto>>(value);

        }
        [Test]
        public async Task GetTweetByTweetIdAsync_ShouldReturnNull_NoTweetExistForEnteredUsername()
        {
            //Arrange
            string fakeUsername = "Anil";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByUserAsync(fakeUsername)).ReturnsAsync(GetListOfAllTweets().FindAll(i => i.UserName == fakeUsername));
            
            //Act
            var actionResult = await _tweetController.GetTweetsByUserAsync(fakeUsername);
            var result = actionResult.Result as OkObjectResult;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
        [Test]
        public async Task AddTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            MessageDto messageDto = new MessageDto
            {
                Message = "Posting a tweet from test controller"
            };
            _tweetRepositoryMock.Setup(i => i.AddTweetAsync(It.IsAny<TweetDetail>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.AddTweetAsync(fakeUsername, messageDto);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }

        [Test]
        public async Task UpdateTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "1";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Updating my tweet 1"
            };
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.UpdateTweetAsync(It.IsAny<TweetDetail>())).ReturnsAsync(true);

            //Act
            var actionResult = await _tweetController.UpdateTweetAsync(fakeUsername, fakeTweetId, fakeMessageDto);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
        }

        [Test]
        public async Task UpdateTweetAsync_ShouldReturnNotFound_TweetNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "-1";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Updating my tweet -1"
            };
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            
            //Act
            var actionResult = await _tweetController.UpdateTweetAsync(fakeUsername, fakeTweetId, fakeMessageDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task UpdateTweetAsync_ShouldReturnUnauthorized_TweetUpdatedByDifferentUser()
        {
            //Arrange
            string fakeUsername = "Sourav";
            string fakeTweetId = "1";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Updating my tweet 1"
            };
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));

            //Act
            var actionResult = await _tweetController.UpdateTweetAsync(fakeUsername, fakeTweetId, fakeMessageDto);
            var result = actionResult.Result as UnauthorizedObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task DeleteTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "1";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.DeleteTweetAsync(It.IsAny<TweetDetail>())).ReturnsAsync(true);

            //Act
            var actionResult = await _tweetController.DeleteTweetAsync(fakeUsername, fakeTweetId);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);
        }

        [Test]
        public async Task DeleteTweetAsync_ShouldReturnNotFound_TweetNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "-1";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));

            //Act
            var actionResult = await _tweetController.DeleteTweetAsync(fakeUsername, fakeTweetId);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task DeleteTweetAsync_ShouldReturnUnauthorized_TweetDeletedByDifferentUser()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "5";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));

            //Act
            var actionResult = await _tweetController.DeleteTweetAsync(fakeUsername, fakeTweetId);
            var result = actionResult.Result as UnauthorizedObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task LikeTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "2";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.LikeATweetAsync(It.IsAny<Like>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.LikeTweetAsync(fakeTweetId, fakeUsername);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }

        [Test]
        public async Task LikeTweetAsync_ShouldReturnNotFound_TweetNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "11";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
           
            //Act
            var actionResult = await _tweetController.LikeTweetAsync(fakeTweetId, fakeUsername);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task UnlikeTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "2";
            _tweetRepositoryMock.Setup(i => i.GetLikesByUserNameAsync(fakeTweetId, fakeUsername))
                .ReturnsAsync(GetListOfAllLikes().Find(i => i.TweetId == fakeTweetId && i.UserName == fakeUsername));
            _tweetRepositoryMock.Setup(i => i.UnlikeATweetAsync(It.IsAny<Like>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.UnlikeTweetAsync(fakeTweetId, fakeUsername);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }

        [Test]
        public async Task UnlikeTweetAsync_ShouldReturnNotFound_TweetNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "11";
            _tweetRepositoryMock.Setup(i => i.GetLikesByUserNameAsync(fakeTweetId, fakeUsername))
                .ReturnsAsync(GetListOfAllLikes().Find(i => i.TweetId == fakeTweetId && i.UserName == fakeUsername));
            //Act
            var actionResult = await _tweetController.UnlikeTweetAsync(fakeTweetId, fakeUsername);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }
        [Test]
        public async Task ReplyToTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "2";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Adding a reply to tweet 2"
            };
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.AddReplyAsync(It.IsAny<Reply>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.ReplyToTweetAsync(fakeTweetId, fakeUsername, fakeMessageDto);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }
        [Test]
        public async Task ReplyToTweetAsync_ShouldReturnNotFound_TweetNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeTweetId = "12";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Adding a reply to tweet 12"
            };
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(It.IsAny<string>())).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == It.IsAny<string>()));
            _tweetRepositoryMock.Setup(i => i.AddReplyAsync(It.IsAny<Reply>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.ReplyToTweetAsync(fakeTweetId, fakeUsername, fakeMessageDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task UpdateReplyToTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeReplyId = "2";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Updating a reply to tweet 2"
            };
            _tweetRepositoryMock.Setup(i => i.GetReplyByIdAsync(fakeReplyId)).ReturnsAsync(GetListOfAllReply().Find(i => i.Id == fakeReplyId));
            _tweetRepositoryMock.Setup(i => i.UpdateReplyAsync(It.IsAny<Reply>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.UpdateReplyToTweetAsync(fakeReplyId, fakeUsername, fakeMessageDto);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }
        [Test]
        public async Task UpdateReplyToTweetAsync_ShouldReturnNotFound_ReplyNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeReplyId = "12";
            MessageDto fakeMessageDto = new MessageDto
            {
                Message = "Updating a reply to tweet 12"
            };
            _tweetRepositoryMock.Setup(i => i.GetReplyByIdAsync(fakeReplyId))
                .ReturnsAsync(GetListOfAllReply().Find(i => i.Id == fakeReplyId));

            //Act
            var actionResult = await _tweetController.UpdateReplyToTweetAsync(fakeReplyId, fakeUsername, fakeMessageDto);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task DeleteReplyToTweetAsync_ApiResponse_ValidInfoEntered()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeReplyId = "1";
            
            _tweetRepositoryMock.Setup(i => i.GetReplyByIdAsync(fakeReplyId)).ReturnsAsync(GetListOfAllReply().Find(i => i.Id == fakeReplyId));
            _tweetRepositoryMock.Setup(i => i.DeleteReplyAsync(It.IsAny<Reply>())).ReturnsAsync(true);
            //Act
            var actionResult = await _tweetController.DeleteReplyToTweetAsync(fakeReplyId, fakeUsername);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }
        [Test]
        public async Task DeleteReplyToTweetAsync_ShouldReturnNotFound_ReplyNotExistInRepo()
        {
            //Arrange
            string fakeUsername = "Gulsan";
            string fakeReplyId = "12";
            _tweetRepositoryMock.Setup(i => i.GetReplyByIdAsync(fakeReplyId)).ReturnsAsync(GetListOfAllReply().Find(i => i.Id == fakeReplyId));
            
            //Act
            var actionResult = await _tweetController.DeleteReplyToTweetAsync(fakeReplyId, fakeUsername);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        [Test]
        public async Task GetAllReplyAsync_ApiResponse_ReplyExistInRepo()
        {
            //Arrange
            string fakeTweetId = "1";

            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.GetAllReplyByTweetIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllReply().FindAll( i => i.TweetId == fakeTweetId));
            
            //Act
            var actionResult = await _tweetController.GetAllReplyAsync(fakeTweetId);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }
        [Test]
        public async Task GetAllReplyAsync_ApiResponseNoCommentFound_ReplyNotExistInRepo()
        {
            //Arrange
            string fakeTweetId = "4";

            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));
            _tweetRepositoryMock.Setup(i => i.GetAllReplyByTweetIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllReply().FindAll(i => i.TweetId == fakeTweetId));

            //Act
            var actionResult = await _tweetController.GetAllReplyAsync(fakeTweetId);
            var result = actionResult.Result as OkObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Success", value.Status);

        }
        [Test]
        public async Task GetAllReplyAsync_ShouldReturnNotFound_TweetNotExistInRepo()
        {
            //Arrange
            string fakeTweetId = "-1";
            _tweetRepositoryMock.Setup(i => i.GetTweetsByIdAsync(fakeTweetId)).ReturnsAsync(GetListOfAllTweets().Find(i => i.Id == fakeTweetId));

            //Act
            var actionResult = await _tweetController.GetAllReplyAsync(fakeTweetId);
            var result = actionResult.Result as NotFoundObjectResult;
            var value = result.Value as ApiResponse;


            //Assert
            Assert.IsNotNull(_tweetController);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.IsInstanceOf<ApiResponse>(value);
            Assert.AreEqual("Error", value.Status);
        }

        private List<TweetDetail> GetListOfAllTweets()
        {
            List<TweetDetail> tweetDetails = new List<TweetDetail>()
            {
                new TweetDetail()
                {
                    Id = "1",
                    Message="Tweet message 1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Gulsan"
                },
                new TweetDetail()
                {
                    Id = "2",
                    Message="Tweet message 2",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Sourav"
                },
                new TweetDetail()
                {
                    Id = "3",
                    Message ="Tweet message 3",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Gulsan"
                },
                new TweetDetail()
                {
                    Id = "4",
                    Message ="Tweet message 4",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Gulsan"
                },
                new TweetDetail()
                {
                    Id= "5",
                    Message="Tweet message 5",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Sourav"
                },
                new TweetDetail()
                {
                    Id= "6",
                    Message="Tweet message 6",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Pooja"
                },
            };
            return tweetDetails;
        }

        private List<Like> GetListOfAllLikes()
        {
            List<Like> likes = new List<Like>()
            {
                new Like
                {
                    Id = "1",
                    TweetId = "1",
                    UserName = "Sourav"
                },
                new Like
                {
                    Id = "2",
                    TweetId = "1",
                    UserName = "Anil"

                },
                new Like
                {
                    Id = "3",
                    TweetId = "2",
                    UserName = "Gulsan"
                },
                new Like
                {
                    Id = "4",
                    TweetId = "2",
                    UserName = "Pooja"
                },
                new Like
                {
                    Id = "5",
                    TweetId = "5",
                    UserName = "Anil"
                },
                new Like
                {
                    Id = "6",
                    TweetId = "4",
                    UserName = "Sourav"
                },
            };
            return likes;
        }

        private List<Reply> GetListOfAllReply()
        {
            List<Reply> replies = new List<Reply>()
            {
                new Reply
                {
                    Id = "1",
                    Message = "Reply message 1",
                    TweetId = "1",
                    UserName = "Pooja"
                },
                new Reply
                {
                    Id = "2",
                    Message = "Reply message 2",
                    TweetId = "2",
                    UserName = "Sourav"
                },
                new Reply
                {
                    Id = "3",
                    Message = "Reply message 3",
                    TweetId = "1",
                    UserName = "Sourav"
                },
                new Reply
                {
                    Id = "4",
                    Message = "Reply message 4",
                    TweetId = "3",
                    UserName = "Gulsan"
                },
                new Reply
                {
                    Id = "5",
                    Message = "Reply message 5",
                    TweetId = "5",
                    UserName = "Anil"
                },
                new Reply
                {
                    Id = "6",
                    Message = "Reply message 6",
                    TweetId = "6",
                    UserName = "Gulsan"
                },

            };
            return replies;
        }
    }
}
