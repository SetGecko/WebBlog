using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetTagsAsync();
        Task<Tag?> GetTagByIDAsync(Guid tagId);
        Task<Tag?> GetByNameAsync(string name);
        Task<Tag> InsertTagAsync(NewTagRequest request);
        Task<bool> DeleteTagAsync(Guid request);
        Task<Tag> UpdateTagAsync(EditTagRequest request);        
    }
}
