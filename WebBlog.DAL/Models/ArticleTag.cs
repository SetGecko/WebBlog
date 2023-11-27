using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    /// <summary>
    /// Класс отнощений многие ко многим между таблицами Articles и Tags
    /// </summary>
    [PrimaryKey(nameof(ArticleId), nameof(TagId))]
    public record ArticleTag
    {
        [Required]
        [Comment("Внешний ключ связи с таблицей Articles")]
        [ForeignKey("Article")]
        public Guid ArticleId { get; set; }
        public Article Post { get; set; } = null!;
        [Required]
        [Comment("Внешний ключ связи с таблицей Tags")]
        [ForeignKey("Tag")]
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
