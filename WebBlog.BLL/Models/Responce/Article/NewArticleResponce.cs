using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Responce.Article
{
    public class NewArticleResponce
    {

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Заголовок", Prompt = "Введите название")]
        public string Title { get; set; } = null!;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Контент", Prompt = "Введите текст статьи")]
        public string Content { get; set; } = null!;

        public List<CheckboxViewModel> Tags { get; set; } = null!;
       

        public string AuthorId { get; set; } = null!;

        public NewArticleResponce() { }
      
    }
}
