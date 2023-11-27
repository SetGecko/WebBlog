using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.DAL.Models;
using WebBlog.Extensions;


namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер для модели Article 
    /// </summary>
    [Route("[controller]")]
    [AllowAnonymous]
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<BlogUser> _userManager;

        public ArticlesController(IArticleService articleService,
            ILogger<ArticlesController> logger, UserManager<BlogUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _articleService = articleService;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? pageNumber, string sortOrder, string currentFilter, string searchString, string currentFilter1, string searchString1)
        {
            try
            {
                ////найти пользователя
                var articles = (await _articleService.GetArticlesAsync()).ToList();

                if (searchString != null || searchString1 != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString ??= currentFilter;

                    searchString1 ??= currentFilter1;
                }

                ViewData["CurrentFilter"] = searchString;
                ViewData["CurrentFilter1"] = searchString1;

                if (!String.IsNullOrEmpty(searchString))
                {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                    articles = articles.Where(s => s.Author.Email.ToUpper(CultureInfo.InvariantCulture)
                    .Contains(searchString.ToUpper(CultureInfo.InvariantCulture))).ToList();
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                }

                if (!String.IsNullOrEmpty(searchString1))
                {
                    articles = articles.Where(s => s.Tags.FirstOrDefault(o => o.Name.ToUpper(CultureInfo.InvariantCulture)
                    .Contains(searchString1.ToUpper(CultureInfo.InvariantCulture))) != null).ToList();
                }

                ViewData["CurrentSort"] = sortOrder;
                //all_blogArticles = _articleService.SortOrder(all_blogArticles, sortOrder);

                var pageSize = 5;
                List<ArticleViewModel> articleView = _mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray());

                ArticleListViewModel blogArticleListViewModel = new() { blogArticles = articleView };
                var userQueryable = blogArticleListViewModel.blogArticles.AsQueryable();

                var model = PaginatedList<ArticleViewModel>.CreateAsync(userQueryable, pageNumber ?? 1, pageSize);

                return View("Index", model);
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
                        await  _articleService.IncCountOfViewsAsync(article.ArticleId);
                        var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                        return View(articleView);
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
                    
                    if(!articles.Any())
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
        /// Начальные данные для создание новой статьи
        /// </summary>
        [Authorize]
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                //передаем в теле Id пользователя для которого будет создана статья
                if (await _userManager.GetUserAsync(HttpContext.User) is BlogUser user)
                {
                    var tags = await _articleService.GetTagsList();
                    NewArticleRequest model = new() { Tags = tags, AuthorId = user.Id };
                    return View(model);
                }
                return  Redirect("/Identity/Account/Login");
          
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Create GET method");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Создает новую статью с указанным именем
        /// </summary>
        /// <param name="reguest"></param>
        [Authorize]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("AuthorId,Title,Content,Tags")]NewArticleRequest? request)
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
                return View("Create", request);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Create POST method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Просмотр информации по указзаному в параметре Id тегу для формы редактирования
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")] 
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id is Guid lId)
                {
                    if(await _articleService.EditArticleRequestById(lId) is EditArticleRequest view)
                        return View(view);
                    
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Edit method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Редактирование параметров статьи
        /// </summary>
        /// <param name="id">ID редактируемого тега</param>
        /// <param name="reguest">класс EditTagRequest содержащий имя нового  тега</param>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken] 
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> Edit(Guid id, [Bind("ArticleId,AuthorId,Title,Content,Tags")] EditArticleRequest reguest)
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
                    return View("Details", articleView);
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
        /// Возвращает данные удаляемой статьи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>

        [HttpGet("Delete/{id}")]
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> Delete(Guid? id, bool? saveChangesError = false)
        {
            try
            {
                if (saveChangesError.GetValueOrDefault())
                {
                    ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
                }

                if (id is Guid lId)
                {
                    var article = await _articleService.GetArticleByIdAsync(lId);
                    if (article is not null)
                    {
                        var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                        return View(articleView);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Delete method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляет статью с указанным ид
        /// </summary>
        /// <param name="id">Ид тега для удаления</param>

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (await _articleService.DeleteAsync(id))
                    return RedirectToAction(nameof(Index));

                return Problem($"Не удалось удалить статью {id}");
            }
            catch (DataException dex)
            {
                _logger.CommonError(dex,$"Delete id {id}failed");
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Delete POST method");
                return StatusCode(500, "Internal server error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
