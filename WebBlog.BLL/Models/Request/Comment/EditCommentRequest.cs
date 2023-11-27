using System.ComponentModel.DataAnnotations;
using WebBlog.Contracts.Models.Query.User;

namespace WebBlog.Contracts.Models.Request.Comment
{
    public class EditCommentRequest
    {
        [Required]
        public Guid CommentId { get; set; }
        [Required]
        public Guid ArticleId { get; set; }
        [Required, MinLength(1, ErrorMessage = "Content is empty."), StringLength(200, ErrorMessage = "Content cannot exceed 200 characters.")]
        public string Content { get; set; } = "";

    }
}
