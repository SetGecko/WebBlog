using WebBlog.DAL.Models;

namespace WebBlog.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс определяет методы для доступа к объектам типа Tag в базе
    /// </summary>
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<bool> ArticleExistsAsync(Guid articleId);
        Task<Article?> GetArticleByIDAsync(Guid articleId);
        Task<bool> IncCountOfViewsAsync(Guid articleId);
        // Task<bool> RemoveTagFromArticle(Guid articleId, Guid tagId);
        
        Task<IEnumerable<Article>> GetArticlesByAuthorAsync(string authorId);
        Task InsertArticleAsync(Article article);
        Task DeleteArticleAsync(Guid articleId);
        bool UpdateArticle(Article article);
        Task SaveAsync();
    }

    
}
