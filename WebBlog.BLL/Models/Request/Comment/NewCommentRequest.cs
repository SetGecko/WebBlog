using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Query.User;

namespace WebBlog.Contracts.Models.Request.Comment
{
    public class NewCommentRequest
    {
        [Required]
        public Guid ArticleId { get; set; } 
        [Required, MinLength(1, ErrorMessage = "Content is empty."), StringLength(200, ErrorMessage = "Content cannot exceed 200 characters.")]
        public string Content { get; set; } = "";
    }
}
