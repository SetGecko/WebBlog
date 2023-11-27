using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.BLL.Services;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.Tests.ServiceTests
{
    /// <summary>
    /// Класс тестов для сервиса TagService
    /// </summary>
    public class TagServiceTests
    {
        private readonly Mock<ITagRepository> _tagRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TagService>> _loggerMock;
        private readonly TagService _tagService;

        public TagServiceTests()
        {
            _tagRepositoryMock = new Mock<ITagRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TagService>>();
            _tagService = new TagService(_tagRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task InsertTagAsync_WithValidRequest_ReturnsInsertedTag()
        {
            // Arrange
            var request = new NewTagRequest { Name = "TestTag" };
            var tag = new Tag { TagId = Guid.NewGuid(), Name = "TestTag" };

            _mapperMock.Setup(mapper => mapper.Map<NewTagRequest, Tag>(request)).Returns(tag);
            _tagRepositoryMock.Setup(repo => repo.TagExistsAsync(tag.Name)).ReturnsAsync(false);

            // Act
            var result = await _tagService.InsertTagAsync(request);

            // Assert
            Assert.Equal(tag, result);
           
        }

        [Fact]
        public async Task InsertTagAsync_WithExistingTag_ThrowsArgumentException()
        {
            // Arrange
            var request = new NewTagRequest { Name = "TestTag" };
            var tag = new Tag { TagId = Guid.NewGuid(), Name = "TestTag" };

            _mapperMock.Setup(mapper => mapper.Map<NewTagRequest, Tag>(request)).Returns(tag);
            _tagRepositoryMock.Setup(repo => repo.TagExistsAsync(tag.Name)).ReturnsAsync(true);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _tagService.InsertTagAsync(request));
            
        }

        [Fact]
        public async Task UpdateTagAsync_WithValidRequest_ReturnsUpdatedTag()
        {
            // Arrange
            var request = new EditTagRequest { TagId = Guid.NewGuid(), Name = "UpdatedTag" };
            var tag = new Tag { TagId = request.TagId, Name = request.Name };

            _mapperMock.Setup(mapper => mapper.Map<EditTagRequest, Tag>(request)).Returns(tag);
            _tagRepositoryMock.Setup(repo => repo.TagExistsAsync(tag.TagId)).ReturnsAsync(true);
            _tagRepositoryMock.Setup(repo => repo.UpdateTag(tag)).Returns(true);

            // Act
            var result = await _tagService.UpdateTagAsync(request);

            // Assert
            Assert.Equal(tag, result);
          
        }

        [Fact]
        public async Task UpdateTagAsync_WithNonExistingTag_ThrowsArgumentException()
        {
            // Arrange
            var request = new EditTagRequest { TagId = Guid.NewGuid(), Name = "UpdatedTag" };
            var tag = new Tag { TagId = request.TagId, Name = request.Name };

            _mapperMock.Setup(mapper => mapper.Map<EditTagRequest, Tag>(request)).Returns(tag);
            _tagRepositoryMock.Setup(repo => repo.TagExistsAsync(tag.TagId)).ReturnsAsync(false);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _tagService.UpdateTagAsync(request));
            
        }

        [Fact]
        public async Task DeleteTagAsync_WithExistingTag_ReturnsTrue()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var tag = new Tag { TagId = tagId, Name = "TestTag" };

            _tagRepositoryMock.Setup(repo => repo.GetTagByIDAsync(tagId)).ReturnsAsync(tag);

            // Act
            var result = await _tagService.DeleteTagAsync(tagId);

            // Assert
            Assert.True(result);
           
        }

        [Fact]
        public async Task DeleteTagAsync_WithNonExistingTag_ReturnsFalse()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            _tagRepositoryMock.Setup(repo => repo.GetTagByIDAsync(tagId)).ReturnsAsync(null as Tag);

            // Act
            var result = await _tagService.DeleteTagAsync(tagId);

            // Assert
            Assert.False(result);
           
        }

        [Fact]
        public async Task DeleteTagAsync_WithException_ReturnsFalse()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var tag = new Tag { TagId = tagId, Name = "TestTag" };

            _tagRepositoryMock.Setup(repo => repo.GetTagByIDAsync(tagId)).ReturnsAsync(tag);
            _tagRepositoryMock.Setup(repo => repo.DeleteTagAsync(tagId)).ThrowsAsync(new Exception());

            // Act
            var result = await _tagService.DeleteTagAsync(tagId);

            // Assert
            Assert.False(result);
            
        }
    }
}
