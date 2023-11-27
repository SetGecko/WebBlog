using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using WebBlog.BLL.Services;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Request.User;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Models;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{

    [Route("[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly RoleManager<BlogRole> _roleManager;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UsersController(UserManager<BlogUser> userManager, RoleManager<BlogRole> roleManager
            , IMapper mapper, ILogger<UsersController> logger, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Добавляет пользователя 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] NewUserRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _mapper.Map<NewUserRequest, BlogUser>(model); ;
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    return BadRequest(result.Errors);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        ///  GET: Users/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userService.GetUserEditViewModelAsync(id);

                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (await _userService.GetUsersViewModelAsync() is IEnumerable<UserViewModel> usersView)
                    return View(usersView);


                _logger.CommonError(null, "_userManager.Users.ToListAsync() is not IEnumerable<BlogUser>");
                return Problem("Internal server error", "", 500);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetAllUsers method");
                return Problem("Internal server error", "", 500);
            }
        }

        /// <summary>
        /// Возвращает пользователя по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var user = await _userService.GetUserEditViewModelAsync(id);

                if (user == null)
                    return NotFound();


                return View(user);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляет данные по пользователю с указанным ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [ValidateAntiForgeryToken]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Email,NewPassword,FirstName,LastName,UserRoles")] EditUserViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {

                    if (await _userService.UpdateUser(model) is IdentityResult result)
                    {
                        if (result.Succeeded)
                        {
                            var viewModel = await _userService.GetUserViewModelAsync(model.Id);
                            return View("Details", viewModel);
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Добавляет роль по ее имени для пользлователя с указанным Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost("{userId}/roles/{roleName}")]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound($"User id {userId} not found"); ;
                }

                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                {
                    return NotFound($"Role name {roleName} not found");
                }
                if (role.Name is null)
                    return NotFound();

                var result = await _userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляет казанную роль у пользователя с указанным Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound();


                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                    return NotFound();

                if (string.IsNullOrEmpty(role.Name))
                    return NotFound();

                var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

                if (result.Succeeded)
                    return Ok();


                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает список ролей для пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetRolesForUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound();


                var role = await _userManager.GetRolesAsync(user);
                if (role == null)
                    return NotFound();

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
