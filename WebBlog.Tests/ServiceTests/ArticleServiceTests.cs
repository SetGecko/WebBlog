using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using System.Security.Claims;
using WebBlog.BLL.Services;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.Tests.ServiceTests
{
    public class ArticleServiceTests
    {
        private readonly Mock<IArticleRepository> _articleRepositoryMock;
        private readonly Mock<UserManager<BlogUser>> _userManagerMock;
        private readonly Mock<ILogger<ArticleService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITagRepository> _tagRepositoryMock;

        private readonly ArticleService _articleService;

        public ArticleServiceTests()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            
            _loggerMock = new Mock<ILogger<ArticleService>>();
            _mapperMock = new Mock<IMapper>();
            _tagRepositoryMock = new Mock<ITagRepository>();

            var store = new Mock<IUserStore<BlogUser>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<BlogUser>>();
            var userValidators = new List<IUserValidator<BlogUser>>();
            var passwordValidators = new List<IPasswordValidator<BlogUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<BlogUser>>>();

            
            _userManagerMock = new Mock<UserManager<BlogUser>>(
                store.Object,
                optionsAccessor.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object);


            _articleService = new ArticleService(
                _userManagerMock.Object,//Substitute.For<UserManager<BlogUser>>(new object[] { "x","x"}),
                _articleRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _tagRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_ValidRequest_ReturnsArticle()
        {
            // Arrange
            var request = new NewArticleRequest
            {
                AuthorId = "authorId",
                Title = "Test Article",
                Content = "Test content",
                Tags = new List<CheckboxViewModel> { new CheckboxViewModel { LabelName = "Tag1", IsChecked = true } }
            };
            var user = new BlogUser();
            var tag = new Tag { Name = "Tag1" };
            var article = new Article();

            _tagRepositoryMock.Setup(x => x.GetByNameAsync("Tag1")).ReturnsAsync(tag);
            _tagRepositoryMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            _articleRepositoryMock.Setup(x => x.InsertArticleAsync(It.IsAny<Article>())).Returns(Task.CompletedTask);
            _articleRepositoryMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            _userManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<BlogUser>(), It.IsAny<Claim>())).Returns(Task.FromResult(IdentityResult.Success));


            // Act
            var result = await _articleService.AddAsync(request, user);

            // Assert
            Assert.Equal(article.ArticleId, result?.ArticleId);
            
            
        }

        [Fact]
        public async Task DeleteAsync_ExistingArticle_DeletesArticle()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var article = new Article();

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(articleId)).ReturnsAsync(article);
            _articleRepositoryMock.Setup(x => x.DeleteArticleAsync(articleId)).Returns(Task.CompletedTask);
            _articleRepositoryMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            bool r = await _articleService.DeleteAsync(articleId);

            // Assert
           Assert.True(r);
           
        }
        [Fact]
        public async Task EditAsync_ExistingArticle_ValidRequest_ReturnsUpdatedArticle()
        {
            // Arrange
            var request = new EditArticleRequest
            {
                ArticleId = Guid.NewGuid(),
                Title = "Updated Article",
                Content = "Updated content",
                Tags = new List<TagViewModel> { new TagViewModel { Name = "Tag1", IsTagSelected = true } }
            };
            var article = new Article() { ArticleId = request.ArticleId, Tags = new List<Tag>()  };

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(request.ArticleId)).ReturnsAsync(article);
            _tagRepositoryMock.Setup(x => x.GetByNameAsync("Tag1")).ReturnsAsync(new Tag());
            _tagRepositoryMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            _articleRepositoryMock.Setup(x => x.UpdateArticle(article)).Returns(true);
            _articleRepositoryMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _articleService.EditAsync(request);

            // Assert
            Assert.Equal(article.ArticleId, result?.ArticleId);
            

        }

        [Fact]
        public async Task EditAsync_NonExistingArticle_ReturnsNull()
        {
            // Arrange
            var request = new EditArticleRequest
            {
                ArticleId = Guid.NewGuid(),
                Title = "Updated Article",
                Content = "Updated content",
                Tags = new List<TagViewModel> { new TagViewModel { Name = "Tag1", IsTagSelected = true } }
            };

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(request.ArticleId)).ReturnsAsync((Article?)null);

            // Act
            var result = await _articleService.EditAsync(request);

            // Assert
            Assert.Null(result);
           

        }

        [Fact]
        public async Task EditAsync_UpdateArticleFails_ReturnsNull()
        {
            // Arrange
            var request = new EditArticleRequest
            {
                ArticleId = Guid.NewGuid(),
                Title = "Updated Article",
                Content = "Updated content",
                Tags = new List<TagViewModel> { new TagViewModel { Name = "Tag1", IsTagSelected = true } }
            };
            var article = new Article() { Tags = new List<Tag>(), ArticleId = request.ArticleId};

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(request.ArticleId)).ReturnsAsync(article);
            _tagRepositoryMock.Setup(x => x.GetByNameAsync("Tag1")).ReturnsAsync(new Tag());
            _tagRepositoryMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            _articleRepositoryMock.Setup(x => x.UpdateArticle(article)).Returns(false);

            // Act
            var result = await _articleService.EditAsync(request);

            // Assert
            Assert.Null(result);
           

        }

        [Fact]
        public async Task GetArticleByIdAsync_ExistingArticle_ReturnsArticle()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var article = new Article { ArticleId = articleId };

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(articleId)).ReturnsAsync(article);

            // Act
            var result = await _articleService.GetArticleByIdAsync(articleId);

            // Assert
            Assert.Equal(article, result);
            
        }

        [Fact]
        public async Task GetArticleByIdAsync_NonExistingArticle_ReturnsNull()
        {
            // Arrange
            var articleId = Guid.NewGuid();

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(articleId)).ReturnsAsync((Article?)null);

            // Act
            var result = await _articleService.GetArticleByIdAsync(articleId);

            // Assert
            Assert.Null(result);
            
        }

        [Fact]
        public async Task EditArticleRequestById_ExistingArticle_ReturnsEditArticleRequest()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var article = new Article { ArticleId = articleId, Content="xxxx", AuthorId = "dd" , Title = "dddd", Author = new BlogUser() };
            var articleR = new EditArticleRequest 
            { ArticleId = articleId, Content = "xxxx", AuthorId = "dd", Title = "dddd", Tags = new List<TagViewModel>() };
            var tags = new List<Tag> { new Tag { TagId = Guid.NewGuid(), Name = "Tag1" } };

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(articleId)).ReturnsAsync(article);
            _tagRepositoryMock.Setup(x => x.GetTagsAsync()).ReturnsAsync(tags);
            _mapperMock.Setup(m => m.Map<Article, EditArticleRequest>(article)).Returns(articleR);
            // Act
            var result = await _articleService.EditArticleRequestById(articleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(articleId, result.ArticleId);
            Assert.Equal(tags.Count, result.Tags.Count);
            
        }

        [Fact]
        public async Task EditArticleRequestById_NonExistingArticle_ReturnsNull()
        {
            // Arrange
            var articleId = Guid.NewGuid();

            _articleRepositoryMock.Setup(x => x.GetArticleByIDAsync(articleId)).ReturnsAsync((Article?)null);

            // Act
            var result = await _articleService.EditArticleRequestById(articleId);

            // Assert
            Assert.Null(result);
            
        }

        [Fact]
        public async Task GetArticlesAsync_ReturnsListOfArticles()
        {
            // Arrange
            var articles = new List<Article> { new Article(), new Article() };

            _articleRepositoryMock.Setup(x => x.GetArticlesAsync()).ReturnsAsync(articles);

            // Act
            var result = await _articleService.GetArticlesAsync();

            // Assert
            Assert.Equal(articles, result);
           
        }

        [Fact]
        public async Task GetTagsList_ReturnsListOfCheckboxViewModels()
        {
            // Arrange
            var tags = new List<Tag> { new Tag { TagId = Guid.NewGuid(), Name = "Tag1" } };

            _tagRepositoryMock.Setup(x => x.GetTagsAsync()).ReturnsAsync(tags);

            // Act
            var result = await _articleService.GetTagsList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tags.Count, result.Count);
            
        }

        [Fact]
        public async Task GetAllIncludeTags_ReturnsListOfArticles()
        {
            // Arrange
            var articles = new List<Article> { new Article(), new Article() };

            _articleRepositoryMock.Setup(x => x.GetArticlesAsync()).ReturnsAsync(articles);

            // Act
            var result = await _articleService.GetAllIncludeTags();

            // Assert
            Assert.Equal(articles, result);
            
        }

        [Fact]
        public async Task GetArticlesByAuthorIdAsync_ExistingAuthorId_ReturnsListOfArticles()
        {
            // Arrange
            var authorId = "authorId";
            var articles = new List<Article> { new Article(), new Article() };

            _articleRepositoryMock.Setup(x => x.GetArticlesByAuthorAsync(authorId)).ReturnsAsync(articles);

            // Act
            var result = await _articleService.GetArticlesByAuthorIdAsync(authorId);

            // Assert
            Assert.Equal(articles, result);
           
        }

        [Fact]
        public async Task IncCountOfViewsAsync_ExistingArticleId_ReturnsTrue()
        {
            // Arrange
            var articleId = Guid.NewGuid();

            _articleRepositoryMock.Setup(x => x.IncCountOfViewsAsync(articleId)).Returns(Task.FromResult(true));

            // Act
            var result = await _articleService.IncCountOfViewsAsync(articleId);

            // Assert
            Assert.True(result);
            
        }

        [Fact]
        public void SortOrder_SortByTitle_ReturnsSortedArticlesByTitle()
        {
            // Arrange
            var articles = new List<Article>
    {
        new Article { Title = "Article C" },
        new Article { Title = "Article A" },
        new Article { Title = "Article B" }
    };

            var sortOrder = "Title";

            // Act
            var result = ArticleService.SortOrder(articles, sortOrder);

            // Assert
            var expectedOrder = new List<string> { "Article A", "Article B", "Article C" };
            Assert.Equal(expectedOrder, result.Select(a => a.Title));
        }

        [Fact]
        public void SortOrder_SortByAuthor_ReturnsSortedArticlesByAuthorEmail()
        {
            // Arrange
            var articles = new List<Article>
    {
        new Article { Author = new BlogUser { Email = "author2@example.com" } },
        new Article { Author = new BlogUser { Email = "author3@example.com" } },
        new Article { Author = new BlogUser { Email = "author1@example.com" } }
    };

            var sortOrder = "Author";

            // Act
            var result = ArticleService.SortOrder(articles, sortOrder);

            // Assert
            var expectedOrder = new List<string> { "author1@example.com", "author2@example.com", "author3@example.com" };
            Assert.Equal(expectedOrder, result.Select(a => a.Author.Email));
        }

        [Fact]
        public void SortOrder_SortByDateCreation_ReturnsSortedArticlesByDateCreationDescending()
        {
            // Arrange
            var articles = new List<Article>
    {
        new Article { Created = new DateTime(2021, 1, 1) },
        new Article { Created = new DateTime(2021, 2, 1) },
        new Article { Created = new DateTime(2021, 3, 1) }
    };

            var sortOrder = "DateCreation";

            // Act
            var result = ArticleService.SortOrder(articles, sortOrder);

            // Assert
            var expectedOrder = new List<DateTime>
    {
        new DateTime(2021, 3, 1),
        new DateTime(2021, 2, 1),
        new DateTime(2021, 1, 1)
    };
            Assert.Equal(expectedOrder, result.Select(a => a.Created));
        }

        [Fact]
        public void SortOrder_InvalidSortOrder_ReturnsSortedArticlesByDateCreationDescending()
        {
            // Arrange
            var articles = new List<Article>
    {
        new Article { Created = new DateTime(2021, 1, 1) },
        new Article { Created = new DateTime(2021, 2, 1) },
        new Article { Created = new DateTime(2021, 3, 1) }
    };

            var sortOrder = "InvalidSortOrder";

            // Act
            var result = ArticleService.SortOrder(articles, sortOrder);

            // Assert
            var expectedOrder = new List<DateTime>
    {
        new DateTime(2021, 3, 1),
        new DateTime(2021, 2, 1),
        new DateTime(2021, 1, 1)
    };
            Assert.Equal(expectedOrder, result.Select(a => a.Created));
        }


    }
}
