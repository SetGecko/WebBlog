using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Request.Tag
{

    /// <summary>
    /// Запрос для обновления свойств Тега 
    /// </summary>
    public class EditTagRequest
    {
        [Required] // Указываем  параметр как обязательный
        public Guid TagId { get; set; }

        //Указываем параметр как обязательный с максимальныой длинной строки 20 символов
        [Required, Display( Name ="Название"),  MinLength(1, ErrorMessage = "Имя тега не заполнено"), StringLength(20, ErrorMessage = "Имя тега не может быть больше 20 символов.")]
        public string? Name { get; set; }
    }
}
