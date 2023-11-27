using AutoMapper;
using Azure;
using Microsoft.Extensions.Logging;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services
{
    /// <summary>
    ///  
    /// </summary>
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;
        
        public TagService(ITagRepository tagsRepository,IMapper mapper, ILogger<TagService> logger)
        {
            _tagsRepository = tagsRepository;      
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Добавляет новый Тег в бд 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Tag> InsertTagAsync(NewTagRequest request)
        {

            var newTag = _mapper.Map<NewTagRequest, Tag>(request);
            //проверить на налицие уже тега с таким именем
            if (await _tagsRepository.TagExistsAsync(newTag.Name))
#pragma warning disable CA2208 // Правильно создавайте экземпляры исключений аргументов
                throw new ArgumentException($"Тег '{request.Name}' уже присутствует в БД. ", nameof(request.Name));
#pragma warning restore CA2208 // Правильно создавайте экземпляры исключений аргументов

            await _tagsRepository.InsertTagAsync(newTag);
            await _tagsRepository.SaveAsync();

            var name = newTag.Name;
            _logger.LogInformation("Тег '{Name}' добавлен.", name);
            return newTag;
            
        }
        /// <summary>
        /// Обновляет параметры указанного тега
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Tag> UpdateTagAsync(EditTagRequest request)
        {
            var tag = _mapper.Map<EditTagRequest, Tag>(request);
            //проверить на налицие уже тега с таким именем
            if (!await _tagsRepository.TagExistsAsync(tag.TagId))
#pragma warning disable CA2208 // Правильно создавайте экземпляры исключений аргументов
                throw new ArgumentException($"Тег '{request.Name}' отсутствует в БД", nameof(request.Name));
#pragma warning restore CA2208 // Правильно создавайте экземпляры исключений аргументов

            if (!_tagsRepository.UpdateTag(tag))
                throw new Exception("Обновление данных завершилось с ошибкой");

            await _tagsRepository.SaveAsync();

            var name = tag.Name;
            _logger.LogInformation("Тег '{Name}' добавлен.", name);
            return tag;

        }
        /// <summary>
        /// Удаляет тег по ID
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTagAsync(Guid tagId)
        {
            try
            {
                var Tag = await _tagsRepository.GetTagByIDAsync(tagId);

                if (Tag == null)
                    return false;

                await _tagsRepository.DeleteTagAsync(tagId);
                await _tagsRepository.SaveAsync();

     
                _logger.LogInformation("Тег '{TagId}' удален.", tagId);
               
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении тега.");
                return false;
            }            
        }

        public Task<Tag?> GetTagByIDAsync(Guid id)
        {
            return _tagsRepository.GetTagByIDAsync(id);
        }
        public Task<Tag?> GetByNameAsync(string name)
        {
            return _tagsRepository.GetByNameAsync(name);
        }
        public Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return _tagsRepository.GetTagsIncludeArticles();
        }
        public Task<IEnumerable<Tag>> GetAllIncludeBlogArticles()
        {
            return _tagsRepository.GetTagsIncludeArticles();
        }

    }
}
