using WebBlog.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс определяет методы для доступа к объектам типа Comment в базе
    /// </summary>
    public interface ICommentRepository 
    {
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<IEnumerable<Comment>> GetCommentsForTheArticleAsync(Guid articleId);
        Task<bool> CommentExistsAsync(Guid commentId);
        Task<Comment?> GetCommentByIDAsync(Guid commentId);
        Task InsertCommentAsync(Comment comment);
        Task DeleteCommentAsync(Guid commentId);
        bool UpdateComment(Comment comment);
        Task SaveAsync();
    }

    
}
