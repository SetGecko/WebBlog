using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.DAL.Repositories;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{
    [Route("[controller]")]
    [Authorize] // Защита контроллера от доступа неавторизованных пользователей
    public class TagsController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;


        public TagsController(ITagService tagService, ILogger<TagsController> logger
            , IMapper mapper)
        {
            _tagService = tagService;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Метод вызываемый по умолчанию
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            try
            {
                var Tags = await _tagService.GetTagsAsync();
                var viewModel = _mapper.Map<Tag[], List<TagViewModel>>(Tags.ToArray());
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetTags method");
                return StatusCode(500, "Internal server error");

            }
        }

        /// <summary>
        /// возвращает все коментарии
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTags")]
        public async Task<ActionResult<IEnumerable<TagViewModel>>> GetTags()
        {
            try
            {
                var Tags = await _tagService.GetTagsAsync();
                var viewModel = _mapper.Map<Tag[], List<TagViewModel>>(Tags.ToArray());
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetTags method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает коментарий по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<TagViewModel>> Details(Guid id)
        {
            try
            {
                var Tag = await _tagService.GetTagByIDAsync(id);

                if (Tag == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<Tag, TagViewModel>(Tag);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает представление для создания нового тега
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            try { 
                return View();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создает новый коментарий
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<TagViewModel>> CreateTag([Bind("Name")] NewTagRequest request)
        {
            if (request == null)
                return RedirectToAction("Error", "Home", new { message = "Tag is null" });



            try
            {
                if (ModelState.IsValid)
                {
                    if (await _tagService.InsertTagAsync(request) is Tag tag)
                    {
                        var viewModel = _mapper.Map<Tag, TagViewModel>(tag);
                        return RedirectToAction(nameof(Index));
                    }
                }
                return BadRequest();
            }
            catch (ArgumentException ex)
            {
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        ///  GET: Tags/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                if (await _tagService.GetTagByIDAsync((Guid)id) is Tag tag)
                {
                    var viewModel = _mapper.Map<Tag, EditTagRequest>(tag);
                    return View(viewModel);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляет информцию переданного коментария
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Tag"></param>
        /// <returns></returns>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TagId,Name")] EditTagRequest request)
        {
            if (id != request.TagId)
                return BadRequest("Tag ID mismatch");

            try
            {
                if (ModelState.IsValid)
                {

                    if (await _tagService.UpdateTagAsync(request) is Tag tag)
                    {
                        var viewModel = _mapper.Map<Tag, TagViewModel>(tag);
                        return View("Details", viewModel);
                    }
                }
                return BadRequest();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in UpdateTag method");
                return StatusCode(500, "Internal server error");
            }

        }

        /// <summary>
        /// GET: Tags/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                if (await _tagService.GetTagByIDAsync((Guid)id) is Tag tag)
                {
                    var viewModel = _mapper.Map<Tag, TagViewModel>(tag);
                    return View(viewModel);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляет тег
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (await _tagService.DeleteTagAsync(id))
                    return RedirectToAction(nameof(Index));

                return Problem($"Не удалось удалить тег {id}");
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteTag method");
                return StatusCode(500, "Internal server error");
            }


        }
    }
}
