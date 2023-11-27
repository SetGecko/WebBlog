using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebBlog.Contracts.Models.Request.Role
{
    public class NewRoleRequest
    {
        [Required(ErrorMessage = "Заполните поле Название"), Display(Name = "Название")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Заполните поле описание"), Display(Name = "Описание")]
        public string Description { get; set; } = default!;
    }
}
