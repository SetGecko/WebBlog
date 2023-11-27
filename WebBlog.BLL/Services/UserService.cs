using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.User;
using WebBlog.Contracts.Models.Responce.Role;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services
{
    public  class UserService : IUserService
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly RoleManager<BlogRole> _roleManager;
        private readonly ILogger<ArticleService> _logger;
        private readonly IMapper _mapper;

        public UserService(UserManager<BlogUser> userManager, RoleManager<BlogRole> roleManager,
           ILogger<ArticleService> logger, IMapper mapper)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Возвращает всех пользователей с массивом только его ролей
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserViewModel>?> GetUsersViewModelAsync()
        {
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
            List<UserViewModel> models = new(_userManager.Users.Count());
            foreach (BlogUser u in _userManager.Users)
            {
                UserViewModel uvm = new ()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                };
                uvm.Roles = (await _userManager.GetRolesAsync(u))
                    .Select(r => new RoleSelectViewModel()
                    { Name = r }).ToList();

                models.Add(uvm);
            }
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
            return models;
        }
        /// <summary>
        /// Возвращает пользователя с массивом всех доступных ролей
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<EditUserViewModel?> GetUserEditViewModelAsync(string Id)
        {
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
            var allRoles = await _roleManager.Roles.Select(r => new RoleSelectViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToListAsync();

            if (await _userManager.FindByIdAsync(Id) is BlogUser user)
            {
                EditUserViewModel model = new()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Id = user.Id,
                };
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.



                model.UserRoles = allRoles.Select(r => new RoleSelectViewModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsChecked = _userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList();
                return model;
            }
            return null;
        }
        /// <summary>
        /// Возвращает  пользователя с массивом только его ролей
        /// </summary>
        /// <returns></returns>
        public async Task<UserViewModel?> GetUserViewModelAsync(string id)
        {

            if (await _userManager.FindByIdAsync(id) is not BlogUser u)
                return null;

            var uvm = _mapper.Map<BlogUser, UserViewModel>(u);
            
                uvm.Roles = (await _userManager.GetRolesAsync(u))
                    .Select(r => new RoleSelectViewModel()
                    { Name = r }).ToList();

            return uvm;
        }
        /// <summary>
        /// Обновляет все поля пользоватея указанные в запросе
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IdentityResult?> UpdateUser(EditUserViewModel request)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
                return null;


            var result = await _userManager.SetUserNameAsync(user, request.Email);
            if (!result.Succeeded)
            {
                _logger.LogError("UpdateUser. SetUserNameAsync failed: {Result}", result);
                return result;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
               string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                result = await _userManager.ResetPasswordAsync(user,token,request.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.LogError("UpdateUser. ResetPasswordAsync failed: {Result}", result);
                    return result;
                }
            }

            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("UpdateUser. UpdateAsync failed: {Result}", result);
                return result;
            }
            //обновление ролей
            foreach (var r in request.UserRoles)
            {
                bool roleActive = await _userManager.IsInRoleAsync(user, r.Name);

                if (r.IsChecked && !roleActive)
                {
                    result = await _userManager.AddToRoleAsync(user, r.Name);
                    _logger.LogError("UpdateUser. AddToRoleAsync failed: {Result}", result);
                    return result;
                }
                else if (!r.IsChecked && roleActive)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, r.Name);
                    _logger.LogError("UpdateUser. RemoveFromRoleAsync failed: {Result}", result);
                    return result;
                }
                
            }
          
            return result;
            
        }
    }
}
