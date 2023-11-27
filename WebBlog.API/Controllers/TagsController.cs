using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Models;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер тегов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Защита контроллера от доступа неавторизованных пользователей
    public class TagsController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="tagService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public TagsController(ITagService tagService, ILogger<TagsController> logger
            , IMapper mapper)
        {
            _tagService = tagService;
            _logger = logger;
            _mapper = mapper;
        }
        

        /// <summary>
        /// возвращает все коментарии
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<TagViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Возвращает все комментарии", Tags = new[] { "Комментарии" })]
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TagViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Возвращает комментарий по Id", Tags = new[] { "Комментарии" })]
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
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }

       

        /// <summary>
        /// Создает новый коментарий
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(typeof(TagViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Создает новый комментарий", Tags = new[] { "Комментарии" })]
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
        /// Обновляет информцию переданного коментария
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TagViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Обновляет информацию переданного комментария", Tags = new[] { "Комментарии" })]
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
                        return Ok(viewModel);
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
        /// Удаляет тег
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удаляет тег", Tags = new[] { "Теги" })]
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
