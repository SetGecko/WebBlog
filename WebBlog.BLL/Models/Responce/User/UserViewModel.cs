using System.ComponentModel.DataAnnotations;
using WebBlog.Contracts.Models.Responce.Role;

namespace WebBlog.Contracts.Models.Query.User
{
    /// <summary>
    /// Представляет сущность BlogUser для отображения в формах 
    /// </summary>
    public class UserViewModel
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required,Display(Name ="Почта")]
        public string Email { get; set; } = null!;
        [Required, Display(Name = "Псевдоним")]
        public string UserName { get; set; } = null!;
        [Required, Display(Name = "Имя")]
        public string FirstName { get; set; } = null!;
        [Required, Display(Name = "Пароль")]
        public string LastName { get; set; } = null!;
        public List<RoleSelectViewModel> Roles { get; set; } = null!;
    }
}
