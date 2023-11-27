using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Responce.Tag
{
    public class TagViewModel
    {
        public Guid TagId { get; set; }
        [Required, Display(Name = "Название"), MinLength(1, ErrorMessage = "Имя тега не заполнено"), StringLength(20, ErrorMessage = "Имя тега не может быть больше 20 символов.")]
        public string Name { get; set; } = "";
        public int ArticleCount { get; set; }
        public bool IsTagSelected { get; set; }
    }
}
