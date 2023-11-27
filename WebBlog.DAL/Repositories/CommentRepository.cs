using Microsoft.EntityFrameworkCore;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.DAL.Repositories
{
    /// <summary>
    /// Репозиторий модели Comment
    /// </summary>
    public class CommentRepository : ICommentRepository, IDisposable
    {
        private readonly ApplicationDbContext context;

        public CommentRepository(ApplicationDbContext context)
        { this.context = context; }

        /// <summary>
        /// Возвращает все коментарии
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            return await context.Comment
                .AsNoTracking()
                .Include(a => a.Author)
                .ToListAsync();
        }
        /// <summary>
        /// Возвращает все коментарии для  статьи с ID = articleId
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetCommentsForTheArticleAsync(Guid articleId)
        {
            return await context.Comment.Where(x=>x.ArticleId == articleId)
                .AsNoTracking()
                .Include(a => a.Author)
                .ToListAsync();
        }

        /// <summary>
        /// Ищзвращает true если коментарий с указанным Ид существует 
        /// </summary>
        /// <param name="commentId">Ид статьи</param>
        /// <returns></returns>
        public async Task<bool> CommentExistsAsync(Guid commentId)
        {
            return await context.Comment
                .AsNoTracking()
                .AnyAsync(e => e.CommentId == commentId);
        }
        /// <summary>
        /// Возвращает коментарии по указанному Ид 
        /// </summary>
        /// <param name="commentId">Ид статьи</param>
        /// <returns></returns>
        public async Task<Comment?> GetCommentByIDAsync(Guid commentId)
        {
            return await context.Comment.Include(c=>c.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(c=>c.CommentId == commentId);
        }
        /// <summary>
        /// Добавляет коментарии 
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task InsertCommentAsync(Comment comment)
        {
            await context.Comment.AddAsync(comment);
        }
        /// <summary>
        /// Удаляет коментарии с указанным Id
        /// </summary>
        /// <param name="commentId">Ид удаляемой статьи</param>
        /// <returns></returns>
        public async Task DeleteCommentAsync(Guid commentId)
        {
            Comment? Comment = await context.Comment.FindAsync(commentId);
            if (Comment != null)
                context.Comment.Remove(Comment);
        }
        /// <summary>
        /// Обновляет данные коментария
        /// </summary>
        /// <param name="comment"></param>
        public bool UpdateComment(Comment comment)
        {
            if (context.Entry(comment) is Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry ar)
                ar.State = EntityState.Modified;
            else
                return false;

            return true;

        }
        /// <summary>
        /// Сохраняет изменения
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        #region Реализация интерфейса IDisposable
        private bool disposed ;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
