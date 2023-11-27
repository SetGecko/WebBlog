using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.User;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>?> GetUsersViewModelAsync();
        Task<EditUserViewModel?> GetUserEditViewModelAsync(string id);
        Task<IdentityResult?> UpdateUser(EditUserViewModel request);
        Task<UserViewModel?> GetUserViewModelAsync(string id);

    }
}
