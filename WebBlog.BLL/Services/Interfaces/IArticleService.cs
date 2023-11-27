using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services.Interfaces
{
    /// <summary>
    ///  CRUD операции
    /// </summary>
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(string id);
        Task<EditArticleRequest?> EditArticleRequestById(Guid id);
        Task<Article?> GetArticleByIdAsync(Guid id);

        Task<List<CheckboxViewModel>> GetTagsList();
        Task<Article?> AddAsync(NewArticleRequest reguest, BlogUser user);
        Task<Article?> EditAsync(EditArticleRequest reguest);
        Task<bool> IncCountOfViewsAsync(Guid id);

        Task<bool> DeleteAsync(Guid id);
    }
}
