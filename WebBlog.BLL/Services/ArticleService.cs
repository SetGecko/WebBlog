using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WebBlog.BLL.Comparers;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services
{
    /// <summary>
    /// Сервис CRUD операций для сущности Article
    /// </summary>
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository; 
        private readonly UserManager<BlogUser> _userManager;
        private readonly ILogger<ArticleService> _logger;
        private readonly IMapper _mapper;
        private readonly ITagRepository _tagRepository;

        public ArticleService(UserManager<BlogUser> userManager, IArticleRepository articlesRepository,
            ILogger<ArticleService> logger, IMapper mapper, ITagRepository tagRepository) 
        {
            _userManager = userManager;
            _articleRepository = articlesRepository;
            _logger = logger;
            _mapper = mapper;
            _tagRepository = tagRepository;
        }

        public async Task<Article?> AddAsync(NewArticleRequest request, BlogUser user)
        {
            try
            {
                //удалить все не выбранные теги
                request.Tags.RemoveAll(x => !x.IsChecked);

                Article article = new()
                {
                    AuthorId = request.AuthorId,
                    Title = request.Title,
                    Created = DateTime.Now,
                    Content = request.Content
                };
                

                //добавить теги
                foreach (var newTag in request.Tags)
                {
                    //  получение существующего из базы данных
                    if( await _tagRepository.GetByNameAsync(newTag.LabelName) is Tag tag)
                    {
                        // добавление тега к статье
                        article.Tags ??= new List<Tag>();
                        article.Tags.Add(tag);
                    }
                   
                }
                await _tagRepository.SaveAsync();


                article.Created = DateTime.Now;
                await _articleRepository.InsertArticleAsync(article);
                await _articleRepository.SaveAsync();

                //добавить пользователю утверждение ArticleOwner
                var claim = new Claim("ArticleOwner", article.ArticleId.ToString());
                await _userManager.AddClaimAsync(user, claim);
                var title = request.Title;
                _logger.LogInformation("Статья '{Title}' добавлена.", title);
                return article;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении статьи.");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                
                if (await _articleRepository.GetArticleByIDAsync(id) is Article article)
                {
                    await _articleRepository.DeleteArticleAsync(id);
                    await _articleRepository.SaveAsync();

                    var title = article.Title;
                    _logger.LogInformation("Статья '{Title}' удалена.", title);

                  
                    return true;
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении статьи {Id}.", id) ;
                
            }
            return false;
        }

        public async Task<Article?> EditAsync(EditArticleRequest request)
        {
            try
            {

                var article = await _articleRepository.GetArticleByIDAsync(request.ArticleId);
                if (article == null)
                    return null;

                try
                {
                    article.Title = request.Title;
                    article.Content = request.Content;

                    //добавить \ удалить теги
                    foreach (var newTag in request.Tags)
                    {
                        var tg = article.Tags.FirstOrDefault(x => x.Name == newTag.Name);

                        if (newTag.IsTagSelected && tg == null)
                        {
                            if (await _tagRepository.GetByNameAsync(newTag.Name) is Tag t)
                                article.Tags.Add(t);
                        }
                        else if (!newTag.IsTagSelected && tg != null)
                            article.Tags.Remove(tg);
                    }
                    await _tagRepository.SaveAsync();

                    if (_articleRepository.UpdateArticle(article))
                        await _articleRepository.SaveAsync();
                    else
                        return null;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _articleRepository.ArticleExistsAsync(request.ArticleId))
                    {
                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
                var title = request.Title;
                _logger.LogInformation("Статья '{Title}' изменена.", title);

                
                return article;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при изменении статьи.");
                return null;
            }
        }

        public Task<Article?> GetArticleByIdAsync(Guid id)
        {
            return _articleRepository.GetArticleByIDAsync(id);
        }
        public async Task<EditArticleRequest?> EditArticleRequestById(Guid id)
        {
            if (await _articleRepository.GetArticleByIDAsync(id) is Article article)
            {
                if( _mapper.Map<Article, EditArticleRequest>(article) is not EditArticleRequest articleView)
                    return null;

                if (articleView.Tags is  null)
                    return null;

                //проставить стус тегов
                foreach (var t in articleView.Tags)
                        t.IsTagSelected = true;
                
                //добавить полный список тегов
                var tags = await _tagRepository.GetTagsAsync();
                var tagsView = tags.Select(r => new TagViewModel
                {
                    TagId = r.TagId,
                    Name = r.Name
                }).ToList();

                articleView.Tags = articleView.Tags.Union(tagsView, new TagViewModelComparer()).ToList();
                return articleView;

            }
            return null;
        }
        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            return await _articleRepository.GetArticlesAsync();

        }
        public async Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(string id)
        {
            return await _articleRepository.GetArticlesByAuthorAsync(id);

        }
        public async Task<List<CheckboxViewModel>> GetTagsList()
        {
            //список тегов
            var tags = await _tagRepository.GetTagsAsync();
            return tags.Select(r => new CheckboxViewModel
            {
                Id = r.TagId,
                LabelName = r.Name
            }).ToList();
        }
        public async Task<IEnumerable<Article>> GetAllIncludeTags()
        {
            return await _articleRepository.GetArticlesAsync();
        }

        public static string SetTagsInModel(List<Tag> tags)
        {
            bool first = true;
            string tagStr = "";
            foreach (var tag in tags)
            {
                if (first)
                {
                    tagStr += tag.Name;
                    first = false;
                }
                else
                {
                    tagStr += (", " + tag.Name);
                }
            }

            return tagStr;
        }
        /// <summary>
        /// Увеличивает значение счетчика просмотров для указанной статьи 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<bool> IncCountOfViewsAsync(Guid id)
        {
                try
                {
                     await _articleRepository.IncCountOfViewsAsync(id);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при просмотре статьи {Id}",id);
                    return false;
                }               
            
        }

        public static IEnumerable<Article> SortOrder(IEnumerable<Article> blogArticle, string sortOrder)
        {
            blogArticle = sortOrder switch
            {
                "Title" => blogArticle.OrderBy(s => s.Title).ToList(),
                "Author" => blogArticle.OrderBy(s => s.Author?.Email).ToList(),
                "DateCreation" => blogArticle.OrderByDescending(s => s.Created).ToList(),
                _ => blogArticle.OrderByDescending(s => s.Created).ToList(),
            };
            return blogArticle;
        }
    }
}
