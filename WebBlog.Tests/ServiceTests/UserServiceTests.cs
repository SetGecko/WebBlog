using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WebBlog.BLL.Services;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.User;
using WebBlog.DAL.Models;

namespace WebBlog.Tests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<BlogUser>> _userManagerMock;
        private readonly Mock<RoleManager<BlogRole>> _roleManagerMock;
        private readonly Mock<ILogger<ArticleService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = new Mock<UserManager<BlogUser>>(new Mock<IUserStore<BlogUser>>().Object,
                null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<BlogRole>>(new Mock<IRoleStore<BlogRole>>().Object,
                null, null, null, null);
            _loggerMock = new Mock<ILogger<ArticleService>>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userManagerMock.Object, _roleManagerMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetUsersViewModelAsync_ReturnsUsersViewModel()
        {
            // Arrange
            var users = new List<BlogUser>
        {
            new BlogUser { Id = "1", UserName = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
            new BlogUser { Id = "2", UserName = "user2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" }
        };

            var roles = new List<string> { "Role1", "Role2" };

            _userManagerMock.Setup(manager => manager.Users).Returns(users.AsQueryable());
            _userManagerMock.Setup(manager => manager.GetRolesAsync(It.IsAny<BlogUser>())).ReturnsAsync(roles);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<UserViewModel>>(It.IsAny<List<BlogUser>>())).Returns(new List<UserViewModel>());

            // Act
            var result = await _userService.GetUsersViewModelAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserViewModel>>(result);
           
        }
       
        [Fact]
        public async Task GetUserViewModelAsync_ReturnsUserViewModel()
        {
            // Arrange
            var userId = "1";
            var roles = new List<string> { "Role1", "Role2" };
            var user = new BlogUser
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _userManagerMock.Setup(manager => manager.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(manager => manager.GetRolesAsync(user)).ReturnsAsync(roles);
            _mapperMock.Setup(mapper => mapper.Map<BlogUser, UserViewModel>(user)).Returns(new UserViewModel());

            // Act
            var result = await _userService.GetUserViewModelAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserViewModel>(result);
          
        }


    }
}
