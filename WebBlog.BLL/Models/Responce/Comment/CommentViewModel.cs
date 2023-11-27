using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Query.User;

namespace WebBlog.Contracts.Models.Responce.Comment
{
    public class CommentViewModel
    {
        public Guid CommentId { get; set; }
        public Guid ArticleId { get; set; }
        public string Content { get; set; } = "";
        public string Title { get; set; } = null!;
        public DateTime Created { get; set; }
        public UserViewModel Author { get; set; } = null!;
    }
}
