using Microsoft.EntityFrameworkCore;
using WebBlog.DAL;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.DAL.Repositories
{
    /// <summary>
    /// Репозиторий модели Tag
    /// </summary>
    public class TagRepository : ITagRepository, IDisposable
    {
        private readonly ApplicationDbContext context;

        public TagRepository(ApplicationDbContext context)
        { this.context = context;}
        /// <summary>
        /// Возвращает массив всех имеющихся тегов
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await context.Tag.AsNoTracking().ToListAsync();
        }
        /// <summary>
        ///  возвращает все теги с списком статей в которых они применялись
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tag>> GetTagsIncludeArticles()
        {
            return await context.Tag.Include(x=>x.Articles).AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// Возвращает массив всех имеющихся тегов для статьи
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tag>> GetTagsForTheArticleAsync(Guid articleId)
        {
            if( await context.Article.Include(t=>t.Tags)
                .FirstOrDefaultAsync(a=>a.ArticleId == articleId) is Article art)
                return art.Tags;

            return new List<Tag>();
        }
        
        /// <summary>
        /// Возвращает тег с указанным именем 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await context.Tag.FirstOrDefaultAsync(x=>x.Name == name);
        }
        /// <summary>
        /// возвращает true если есть тег с таким  ИД
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<bool> TagExistsAsync(Guid tagId)
        {
            return await context.Tag.AsNoTracking().AnyAsync(e => e.TagId == tagId);
        }
        /// <summary>
        /// возвращает true если есть тег с таким  ИД
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<bool> TagExistsAsync(string tagName)
        {
            return await context.Tag.AsNoTracking().AnyAsync(e => e.Name == tagName);
        }
        /// <summary>
        /// возвращает тег для указанного ИД
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<Tag?> GetTagByIDAsync(Guid tagId)
        {
            return await context.Tag.FindAsync(tagId);
        }
        /// <summary>
        /// добавляет тег 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task InsertTagAsync(Tag tag)
        {
           await context.Tag.AddAsync(tag);
        }
        /// <summary>
        /// удаляет тег
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task DeleteTagAsync(Guid tagId)
        {
            Tag? Tag = await context.Tag.FindAsync(tagId);
            if (Tag != null) 
                context.Tag.Remove(Tag);
        }
        /// <summary>
        /// обновляет данные для переданного тега.Если такого тега нет в БД возвращает false
        /// </summary>
        /// <param name="tag"></param>
        public bool  UpdateTag(Tag tag)
        {
            
            if (context.Entry(tag) is Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry ar)
                ar.State = EntityState.Modified;
            else
                return false;

            return true;
        }
        /// <summary>
        /// сохраняет изменения
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
