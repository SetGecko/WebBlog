using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Request.Role
{
    public class EditRoleRequest
    {
        [Required]
        public string Id { get; set; } = default!;

        [Required(ErrorMessage = "Заполните поле Название"), Display(Name = "Название")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Заполните поле описание"), Display(Name = "Описание")]
        public string Description { get; set; } = default!;
    }
}
