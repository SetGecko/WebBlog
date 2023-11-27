using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebBlog.Contracts.Models.Request.Role;

namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер для управления ролями пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="mapper"></param>
        public RoleController(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает все имеющиеся роли
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Возвращает все имеющиеся роли")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает все имеющиеся роли", typeof(IEnumerable<IdentityRole>))]
        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }
        /// <summary>
        /// Возвращает роль по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Возвращает роль по Id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает роль по Id", typeof(IdentityRole))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректный Id роли")]
        public async Task<IActionResult> GetRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is IdentityRole role)
                return Ok(role);
            
            return BadRequest($"Role Id:{id} not found");
        }

        /// <summary>
        /// Добавление роли
        /// </summary>   
        [HttpPost]
        [SwaggerOperation(Summary = "Добавление роли")]
        [SwaggerResponse(StatusCodes.Status200OK, "Роль успешно создана")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации данных")]
        public async Task<IActionResult> CreateRole([FromBody] NewRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<NewRoleRequest, IdentityRole>(request);
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors.ToArray());
            }
            return BadRequest();
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>    
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Редактирование роли")]
        [SwaggerResponse(StatusCodes.Status200OK, "Роль успешно отредактирована")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректный Id роли или ошибка валидации данных")]
        public async Task<IActionResult> EditRole([FromBody] EditRoleRequest request)
        {

            if (ModelState.IsValid)
            {
                if (await _roleManager.FindByIdAsync(request.Id) is IdentityRole role)
                {
                    await _roleManager.UpdateAsync(role);
                    return Ok();
                }
                return BadRequest($"Role Id:{request.Id} not found");
            }
            return BadRequest();
        }

        /// <summary>
        /// Удаление роли
        /// </summary>       
        [HttpDelete("{id}")]
        [SwaggerOperation( Summary = "Удаление роли")]
        [SwaggerResponse(StatusCodes.Status200OK, "Роль успешно удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректный Id роли")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is IdentityRole role)
            {
                await _roleManager.DeleteAsync(role);
                return Ok();
            }
            return BadRequest($"Role Id:{id} not found");
        }
    }
}
