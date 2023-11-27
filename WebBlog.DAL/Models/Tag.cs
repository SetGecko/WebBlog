using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    [Table("Tags")]
    public record Tag
    {
        [Key]
        [Comment("Первичный ключ")]
        public Guid TagId { get; set; }
        [Required]
        [Column(TypeName = "varchar(20)")]
        [Comment("Название тега")]
        public string Name { get; set; } = "";

        public virtual ICollection<Article> Articles { get; } = new List<Article>(); 
    }
}
