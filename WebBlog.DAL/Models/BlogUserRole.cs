using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.DAL.Models
{

    public class BlogUserRole : IdentityUserRole<string>
    {
        //[ForeignKey("UserId")]
        //public new string UserId { get; set; } = null!;
       //public virtual BlogUser User { get; set; } = null!;
       // public virtual BlogRole Role { get; set; } = null!;
    }
}
