using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Responce.Role
{
    public class RoleSelectViewModel
    {
        [Required]
        public string Id { get; set; } = default!;

        [Required(ErrorMessage = "Заполните поле Название"), Display(Name = "Название")]
        public string Name { get; set; } = default!;
        [Required]
        public bool IsChecked { get; set; }

    }
}
