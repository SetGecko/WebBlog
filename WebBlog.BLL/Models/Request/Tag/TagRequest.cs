using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Request.Tag
{
    public class TagRequest
    {
        public Guid TagId { get; set; }
  
        [Required, Display(Name = "Название"), MinLength(1, ErrorMessage = "Имя тега не заполнено"), StringLength(20, ErrorMessage = "Имя тега не может быть больше 20 символов.")]
        public string Name { get; set; } = default!;
    }
}
