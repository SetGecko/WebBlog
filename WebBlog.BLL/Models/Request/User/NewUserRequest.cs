using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Query.User
{
    /// <summary>
    /// Представляет сущность BlogUser для отображения в формах 
    /// </summary>
    public class NewUserRequest
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public string CustomField { get; set; } = null!;

    }
}
