using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    [Table("Comments")]
    public record Comment
    {
        [Key]
        [Comment("Первичный ключ")]
        public Guid CommentId { get; set; }

        [ForeignKey("Article")]
        [Comment("Внешний ключ связи с таблицей Articles")]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(200)")]
        [Comment("Содержание коментария")]
        public string Content { get; set; } = "";
        
        [Column(TypeName = "varchar(100)")]
        [Comment("Заголовок коментария")]
        public string Title { get; set; } = null!;

        [Required]
        [Comment("Дата создания")]
        public DateTime Created { get; set; }

        [Required]
        [Column(TypeName = "varchar(450)")]
        [ForeignKey(nameof(Author))]
        [Comment("Внешний ключ связи с таблицей пользователей AspNetUsers")]
        public string AuthorID { get; set; } = null!;
        public BlogUser Author { get; set; } = null!;
    }
}
