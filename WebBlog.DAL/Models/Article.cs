using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    [Table("Articles")]
    public record Article
    {
        [Key]
        [Comment("Первичный ключ")]
        public Guid ArticleId { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        [Comment("Название статьи")]
        public string Title { get; set; } = null!;
        [Required]
        [Column(TypeName = "varchar(1000)")]
        [Comment("Содержание статьи")]
        public string Content { get; set; } = null!;
        [Required]
        [Comment("Дата создания")]
        public DateTime Created { get; set; }

        [Comment("Количество просмотров")]
        public long ViewsCount { get; set; } = new();

        [Required]
        [Column(TypeName = "varchar(450)")]
        [ForeignKey(nameof(Author))]
        [Comment("Внешний ключ связи с таблицей AspNetUsers")]
        public string AuthorId { get; set; } = null!;
        public BlogUser Author { get; set; } = null!; 

        public virtual ICollection<Comment> Comments { get; set; } = null!;
        public virtual ICollection<Tag> Tags { get; set; } = null!;
  
    }
}
