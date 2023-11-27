using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Request.Tag
{
    /// <summary>
    /// Запрос для добавления нового Тега 
    /// </summary>
    public class NewTagRequest
    {
        //Указываем параметр как обязательный с максимальныой длинной строки 20 символов
        [Required, Display(Name = "Название"), MinLength(1, ErrorMessage = "Имя тега не заполнено"), StringLength(20, ErrorMessage = "Имя тега не может быть больше 20 символов.")]
        public string? Name { get; set; }
    }
}
