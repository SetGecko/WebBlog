using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.DAL.Models
{
    /// <summary>
    /// Кастомизауия стандартных возможностей класса IdentityRole
    /// </summary>
    public class BlogRole : IdentityRole
    {
        public string Description { get; set; } = null!;

        public virtual ICollection<BlogUserRole> UserRoles { get; set; } = null!;

    }
}
