using Microsoft.EntityFrameworkCore;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.DAL.Repositories
{
    /// <summary>
    /// Репозиторий модели Article
    /// </summary>
    public class ArticleRepository : IArticleRepository, IDisposable
    {
        private readonly ApplicationDbContext context;

        public ArticleRepository(ApplicationDbContext context)
        { this.context = context; }

        /// <summary>
        /// Возвращает все статьи
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            return await context.Article
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.Comments)
                .Include(a => a.Author)
                .ToListAsync();
        }
        /// <summary>
        /// Возвращает статью для автора 
        /// </summary>
        /// <param name="authorId">Ид автора по которому будет фильтр отбора</param>
        /// <returns></returns>
        public async Task<IEnumerable<Article>> GetArticlesByAuthorAsync(string authorId)
        {
            return await context.Article.Where(x => x.AuthorId == authorId)
                .AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.Comments)
                .Include(a => a.Author)
                .ToListAsync();
        }
        /// <summary>
        /// Ищзвращает true если статья с указанным Ид существует 
        /// </summary>
        /// <param name="articleId">Ид статьи</param>
        /// <returns></returns>
        public async Task<bool> ArticleExistsAsync(Guid articleId)
        {
            return await context.Article.AnyAsync(e => e.ArticleId == articleId);
        }
        /// <summary>
        /// Возвращает статью по указанному Ид 
        /// </summary>
        /// <param name="articleId">Ид статьи</param>
        /// <returns></returns>
        public async Task<Article?> GetArticleByIDAsync(Guid articleId)
        {
            return await context.Article.AsNoTracking()
                .Include(a => a.Tags)
                .Include(a=>a.Author)
                .Include(a=>a.Comments)
                .ThenInclude(a=>a.Author)
                .FirstOrDefaultAsync(t => t.ArticleId == articleId);
        }
        public async Task<bool> IncCountOfViewsAsync(Guid articleId)
        {
            if(context.Article.FirstOrDefault(x=>x.ArticleId == articleId) is Article a)
            {
                a.ViewsCount++;
                context.Article.Update(a);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Добавляет статью 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public async Task InsertArticleAsync(Article article)
        {
            await context.Article.AddAsync(article);
        }
        /// <summary>
        /// Удаляет статью с указанным Id
        /// </summary>
        /// <param name="articleId">Ид удаляемой статьи</param>
        /// <returns></returns>
        public async Task DeleteArticleAsync(Guid articleId)
        {
            Article? article = await context.Article.FindAsync(articleId);
            if (article != null)
                context.Article.Remove(article);
        }
        /// <summary>
        /// Обновляет данные статьи
        /// </summary>
        /// <param name="article"></param>
        public bool UpdateArticle(Article article)
        {
            if (context.Entry(article) is Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry ar)
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
        private bool disposed;
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
