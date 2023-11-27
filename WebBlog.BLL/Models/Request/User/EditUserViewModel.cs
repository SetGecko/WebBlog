using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBlog.Contracts.Models.Responce.Role;

namespace WebBlog.Contracts.Models.Request.User
{
    public  class EditUserViewModel
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required, Display(Name ="Почта")]
        public string Email { get; set; } = null!;
        //[Required, Display(Name = "Полное имя пользователя")]
        //public string UserName { get; set; } = null!;
        [Display(Name = "Пароль")]
        public string NewPassword { get; set; } = null!;

        [Required,StringLength(50, ErrorMessage = "CustomField cannot exceed 100 characters."), Display(Name = "Имя")]
        public string FirstName { get; set; } = null!;
        [Required,StringLength(50, ErrorMessage = "CustomField cannot exceed 100 characters."), Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        public List<RoleSelectViewModel> UserRoles { get; set; } = null!;

    }
}
