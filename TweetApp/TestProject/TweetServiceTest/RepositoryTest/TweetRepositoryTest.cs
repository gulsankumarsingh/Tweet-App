using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tweet.API.Infrastructure.DataContext;
using Tweet.API.Infrastructure.Repository;
using Tweet.API.Infrastructure.Repository.Interface;
using Tweet.API.Models;

namespace TweetServiceTest.RepositoryTest
{
    public class TweetRepositoryTest
    {
        private static DbContextOptions<TweetDbContext> _dbContextOptions = new DbContextOptionsBuilder<TweetDbContext>()
            .UseInMemoryDatabase(databaseName: "TweetDetailDb").EnableSensitiveDataLogging().Options;

        ITweetRepository _tweetRepository;
        Mock<ILogger<TweetRepository>> _loggerMock;


        [OneTimeSetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<TweetRepository>>();
            SeedDatabase();
            _tweetRepository = new TweetRepository(new TweetDbContext(_dbContextOptions), _loggerMock.Object);

        }

        [Test]
        public async Task GetAllTweetsAsync_ListOfTweets_TweetExistInRepo()
        {
            //Arrange
            using var context = new TweetDbContext(_dbContextOptions);
            //Act
            var result = await _tweetRepository.GetAllTweetsAsync();

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<TweetDetail>>(result);
        }

        [Test]
        public async Task GetTweetsByIdAsync_Tweet_TweetNotExistInRepo()
        {
            //Arrange
            string mockTweetId = "1";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetTweetsByIdAsync(mockTweetId);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TweetDetail>(result);
        }

        [Test]
        public async Task GetTweetsByIdAsync_ShouldReturnNull_TweetNotExistInRepo()
        {
            //Arrange
            string mockTweetId = "-1";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetTweetsByIdAsync(mockTweetId);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetTweetsByUserAsync_Tweet_TweetExistInRepo()
        {
            //Arrange
            string mockUsername = "Gulsan";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetTweetsByUserAsync(mockUsername);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<TweetDetail>>(result);
        }

        [Test]
        public async Task GetTweetsByUserAsync_ShouldReturnEmpty_TweetNotExistInRepo()
        {
            //Arrange
            string mockUsername = "Ajith";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetTweetsByUserAsync(mockUsername);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task AddTweetAsync_TrueIfSaved_Valid()
        {
            //Arrange
            TweetDetail mockDetail = new TweetDetail()
            {
                Id = "15",
                Message = "Adding new tweet",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserName = "Abcd e"
            };
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.AddTweetAsync(mockDetail);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task UpdateTweetAsync_TrueIfSaved_Valid()
        {
            //Arrange
            TweetDetail mockTweetDetail = GetListOfAllTweets()[4];
            mockTweetDetail.Message = "Updating the tweet";
            mockTweetDetail.UpdatedAt = DateTime.Now;
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.UpdateTweetAsync(mockTweetDetail);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task DeleteTweetAsync_TrueIfSaved_Valid()
        {
            //Arrange
            TweetDetail mockTweetDetail = GetListOfAllTweets()[2];
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.DeleteTweetAsync(mockTweetDetail);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task DeleteTweetByUsernameAsync_TrueIfSaved_Valid()
        {
            //Arrange
            string mockUsername = "Rani";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.DeleteTweetByUserNameAsync(mockUsername);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task GetReplyByIdAsync_Reply_ReplyExistInRepo()
        {
            //Arrange
            string mockId = "1";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetReplyByIdAsync(mockId);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Reply>(result);
        }

        [Test]
        public async Task GetAllReplyByTweetIdAsync_ListOfReply_ReplyExistInRepo()
        {
            //Arrange
            string mockTweetId = "1";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetAllReplyByTweetIdAsync(mockTweetId);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Reply>>(result);
        }

        [Test]
        public async Task AddReplyAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            Reply mockReply = new Reply
            {
                Id = "14",
                Message = "Adding a reply",
                TweetId = "8",
                UserName = "GUlsan"
            };
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.AddReplyAsync(mockReply);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task UpdateReplyAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            Reply mockReply = GetListOfAllReply()[11];
            mockReply.Message = "Updating my reply";
            mockReply.UpdatedAt = DateTime.Now;
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.UpdateReplyAsync(mockReply);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task DeleteReplyAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            Reply mockReply = GetListOfAllReply()[3];
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.DeleteReplyAsync(mockReply);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task DeleteReplyByUsernameAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            string mockUsername = "Anil";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.DeleteReplyByUsernameAsync(mockUsername);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task GetTweetLikesAsync_ListOfLikes_LikesExistInDb()
        {
            //Arrange
            string mockTweetId = "3";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetTweetLikesAsync(mockTweetId);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Like>>(result);
        }

        [Test]
        public async Task GetLikesByUserNameAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            string mockTweetId = "2";
            string mockUsername = "Gulsan";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.GetLikesByUserNameAsync(mockTweetId, mockUsername);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Like>(result);
        }

        [Test]
        public async Task LikeATweetAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            Like mockLike = new Like
            {
                Id = "14",
                TweetId = "10",
                UserName = "Andrew"
            };
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.LikeATweetAsync(mockLike);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task UnlikeATweetAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            Like mockLike = GetListOfAllLikes()[9];
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.UnlikeATweetAsync(mockLike);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task DeleteLikeByUsernameAsync_TrueIfSaved_ValidData()
        {
            //Arrange
            string mockUsername = "Ravi";
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.DeleteLikeByUsernameAsync(mockUsername);

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task SaveChangesAsync_TrueIfSuccessfully_ValidCheck()
        {
            //Arrange
            using var context = new TweetDbContext(_dbContextOptions);

            //Act
            var result = await _tweetRepository.SaveChangesAsync();

            Assert.IsNotNull(_tweetRepository);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }
        private void SeedDatabase()
        {
            using var context = new TweetDbContext(_dbContextOptions);
            context.TweetDetails.AddRange(GetListOfAllTweets());
            context.Likes.AddRange(GetListOfAllLikes());
            context.Replies.AddRange(GetListOfAllReply());
            context.SaveChanges();
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
                    UserName = "Binod"
                },
                new TweetDetail()
                {
                    Id= "6",
                    Message="Tweet message 6",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Pooja"
                },
                new TweetDetail()
                {
                    Id= "7",
                    Message="Tweet message 7",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Raghav"
                },
                new TweetDetail()
                {
                    Id= "8",
                    Message="Tweet message 8",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Rani"
                },
              
                new TweetDetail()
                {
                    Id= "9",
                    Message="Tweet message 9",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Ravi"
                },
                new TweetDetail()
                {
                    Id= "10",
                    Message="Tweet message 10",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserName = "Sandeep"
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
                 new Like
                {
                    Id = "7",
                    TweetId = "7",
                    UserName = "Rani"
                },
                new Like
                {
                    Id = "8",
                    TweetId = "8",
                    UserName = "Raghav"
                },
                new Like
                {
                    Id = "9",
                    TweetId = "8",
                    UserName = "Rani"
                },
                new Like
                {
                    Id = "10",
                    TweetId = "5",
                    UserName = "Atanu"
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
                    TweetId = "3",
                    UserName = "Ranjeet"
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
                    TweetId = "1",
                    UserName = "Gulsan"
                },
                 new Reply
                {
                    Id = "7",
                    Message = "Reply message 7",
                    TweetId = "7",
                    UserName = "Rani"
                },
                new Reply
                {
                    Id = "8",
                    Message = "Reply message 8",
                    TweetId = "7",
                    UserName = "Raghav"
                },
                new Reply
                {
                    Id = "9",
                    Message = "Reply message 9",
                    TweetId = "8",
                    UserName = "Raghav"
                },
                new Reply
                {
                    Id = "10",
                    Message = "Reply message 10",
                    TweetId = "8",
                    UserName = "Ravi"
                },
                new Reply
                {
                    Id = "11",
                    Message = "Reply message 11",
                    TweetId = "6",
                    UserName = "Abhijeet"
                },
                new Reply
                {
                    Id = "12",
                    Message = "Reply message 12",
                    TweetId = "7",
                    UserName = "Abhijeet"
                }

            };
            return replies;
        }
    }
}
