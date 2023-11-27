using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;

namespace WebBlog.Contracts.Models.Request.Article
{

    /// <summary>
    /// Запрос для обновления свойств Тега 
    /// </summary>
    public class EditArticleRequest
    {
  
        [Required] // Указываем  параметр как обязательный
        public Guid ArticleId { get; set; }

        //Указываем параметр как обязательный с максимальныой длинной строки 100 символов
        [Required, StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = null!;

        //Указываем параметр как обязательный с максимальныой длинной строки 100 символов
        [Required, StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Content { get; set; } = null!;

        public DateTime Created { get; set; }

        [Required]
        public string AuthorId { get; set; } = null!;

       // public virtual List<CommentViewModel> Comments { get; set; } = null!;
        public virtual List<TagViewModel> Tags { get; set; } = null!;
    }
}
