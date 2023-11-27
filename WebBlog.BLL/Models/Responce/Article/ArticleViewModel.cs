using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;

namespace WebBlog.Contracts.Models.Responce.Article
{
    public class ArticleViewModel
    {
        public Guid ArticleId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Введите название")]
        public string Title { get; set; } = null!;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Содержимое", Prompt = "Введите содержимое")]
        public string Content { get; set; } = null!;
        [Display(Name = "Создана")]
        public DateTime Created { get; set; }
        public long ViewsCount { get; set; }
        public string AuthorId { get; set; } = null!;
        [Display(Name = "Автор")]
        public string AuthorName { get; set; } = null!;

        public virtual ICollection<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        [Display(Name = "Теги")]
        public virtual List<TagViewModel> Tags { get; set; } = null!;
    }
}
