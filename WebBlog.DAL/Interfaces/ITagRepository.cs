using WebBlog.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс определяет методы для доступа к объектам типа Tag в базе
    /// </summary>
    public interface ITagRepository 
    {
        Task<IEnumerable<Tag>> GetTagsAsync();
        Task<bool> TagExistsAsync(Guid tagId);
        Task<bool> TagExistsAsync(string tagName);
        Task<Tag?> GetTagByIDAsync(Guid tagId);
        Task<IEnumerable<Tag>> GetTagsIncludeArticles();
        Task<Tag?> GetByNameAsync(string name);
        Task InsertTagAsync(Tag tag);
        Task DeleteTagAsync(Guid tagId);
        bool UpdateTag(Tag tag);
        Task SaveAsync();
    }

    
}
