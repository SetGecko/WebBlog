using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    public class BlogUser : IdentityUser
    {
        // Дополнительные свойства пользователя
        [Comment("Имя пользователя")]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; } = null!;
        [Comment("Фамилия пользователя")]
        [Column(TypeName = "varchar(50)")] 
        public string LastName { get; set; }= null!;

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = null!;
        public virtual ICollection<BlogUserRole> UserRoles { get; set; } = null!;
        public virtual ICollection<BlogRole> Roles { get; set; } = null!;
    }
}
