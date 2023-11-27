using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBlog.Contracts.Models.Request.Comment;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{

    [Route("[controller]")]
    [Authorize] // Защита контроллера от доступа неавторизованных пользователей
    public class CommentsController : Controller
    {
      
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentsController> _logger;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ICommentRepository commentRepository, ILogger<CommentsController> logger
            , IMapper mapper, UserManager<BlogUser> userManager)
        {
            _commentRepository = commentRepository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// возвращает все коментарии
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetComments")]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetComments()
        {
            try
            {
                var comments = await _commentRepository.GetCommentsAsync();
                var viewModel = _mapper.Map<Comment[], List<CommentViewModel>>(comments.ToArray());
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComments method");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Возвращает все коментарии для статью с указанным Ид
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet("GetCommentsForTheArticle")]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetCommentsForTheArticle(Guid articleId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsForTheArticleAsync(articleId);
                var viewModel = _mapper.Map<Comment[], List<CommentViewModel>>(comments.ToArray());
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComments method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает коментарий по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<CommentViewModel>> Details(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIDAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<Comment, CommentViewModel>(comment);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создает новый коментарий
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ActionResult<CommentViewModel>> Create([Bind("ArticleId,Content")] NewCommentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Comment is null");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var comment = _mapper.Map<NewCommentRequest, Comment>(request);
                    if (comment is null)
                        return BadRequest();

                    //передаем в теле Id пользователя для которого будет создана статья
                    if (await _userManager.GetUserAsync(HttpContext.User) is BlogUser user)
                    {
                        comment.Title = "";
                        comment.Author = user;
                        comment.AuthorID = user.Id;

                        await _commentRepository.InsertCommentAsync(comment);
                        await _commentRepository.SaveAsync();
                        return RedirectToAction("Details", "Articles", new { id = request.ArticleId.ToString() });
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает страницу с коментарием по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit/{id}")]
        public async Task<ActionResult<CommentViewModel>> Edit(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIDAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<Comment, CommentViewModel>(comment);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComment method");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Обновляет информцию переданного коментария
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        [Authorize(Policy = "RuleAdministratorOrModerator")]
        public async Task<IActionResult> UpdateComment([Bind("CommentId,ArticleId,Content")] EditCommentRequest request)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    if (await _commentRepository.GetCommentByIDAsync(request.CommentId) is Comment comment)
                    {
                        comment.Content = request.Content;
                        if (_commentRepository.UpdateComment(comment))
                        {
                            await _commentRepository.SaveAsync();
                            return RedirectToAction("Index", "Articles");
                        }
                        return BadRequest("Update failed");
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in UpdateComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// GET: Comment/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIDAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<Comment, CommentViewModel>(comment);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляет комментарий
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RuleAdministratorOrModerator")]


        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIDAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                await _commentRepository.DeleteCommentAsync(id);
                await _commentRepository.SaveAsync();

                return RedirectToAction("Index", "Articles");

            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
