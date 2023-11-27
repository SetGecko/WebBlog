using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер для модели Article 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<BlogUser> _userManager;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="articleService"></param>
        /// <param name="logger"></param>
        /// <param name="userManager"></param>
        /// <param name="mapper"></param>
        public ArticlesController(IArticleService articleService,
            ILogger<ArticlesController> logger, UserManager<BlogUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _articleService = articleService;
            _userManager = userManager;
        }

        /// <summary>
        /// Возвращает указанную страницу с статьями. если номер не указан вернется страница номер 1
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            try
            {
                ////найти пользователя
                var articles = (await _articleService.GetArticlesAsync()).ToList();


                var pageSize = 5;
                List<ArticleViewModel> articleView = _mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray());

                ArticleListViewModel blogArticleListViewModel = new() { blogArticles = articleView };
                var userQueryable = blogArticleListViewModel.blogArticles.AsQueryable();

                var model = PaginatedList<ArticleViewModel>.CreateAsync(userQueryable, pageNumber ?? 1, pageSize);

                return Ok( model);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Просмотр информации о статье с указанным Id 
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {

                if (id is Guid lId)
                {
                    var article = await _articleService.GetArticleByIdAsync(lId);

                    if (article is not null)
                    {
                        await _articleService.IncCountOfViewsAsync(article.ArticleId);
                        var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                        return Ok(articleView);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Возвращает список статей для указанного автора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetArticleByAuthor/{id}")]
        public async Task<IActionResult> GetArticleByAuthor(string? id)
        {
            try
            {
                if (id is string authorId)
                {
                    var articles = await _articleService.GetArticlesByAuthorIdAsync(authorId);
                    if (articles == null)
                        return NotFound();

                    if (!articles.Any())
                        return NotFound();

                    List<ArticleViewModel> articleView = _mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray());
                    return Ok(articleView.ToList());

                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetArticleByAuthor method");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Создает новую статью с указанным именем
        /// </summary>
        /// <param name="request"></param>
        [Authorize]
        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,Title,Content,Tags")] NewArticleRequest? request)
        {

            if (request == null)
                return BadRequest(request);

            try
            {
                if (ModelState.IsValid)
                {
                    ////найти пользователя
                    if (await _userManager.GetUserAsync(HttpContext.User) is BlogUser user)
                    {
                        if (await _articleService.AddAsync(request, user) is Article article)
                            return RedirectToAction(nameof(Index));
                    }
                }
                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Create POST method");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Редактирование параметров статьи
        /// </summary>
        /// <param name="id">ID редактируемого тега</param>
        /// <param name="reguest">класс EditTagRequest содержащий имя нового  тега</param>
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> Update(Guid id, [Bind("ArticleId,AuthorId,Title,Content,Tags")] EditArticleRequest reguest)
        {
            if (reguest is null)
                return NotFound();

            if (id != reguest.ArticleId)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();


            try
            {

                if (await _articleService.EditAsync(reguest) is Article article)
                {
                    var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                    return Ok(articleView);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Edit POST method");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Удаляет статью с указанным ид
        /// </summary>
        /// <param name="id">Ид тега для удаления</param>

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (await _articleService.DeleteAsync(id))
                    return RedirectToAction(nameof(Index));

                return Problem($"Не удалось удалить статью {id}");
            }
            catch (DataException dex)
            {
                _logger.CommonError(dex, $"Delete id {id}failed");
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Delete POST method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
