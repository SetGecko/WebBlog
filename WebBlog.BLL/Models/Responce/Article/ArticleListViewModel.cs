using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.DAL.Models;

namespace WebBlog.Contracts.Models.Responce.Article
{
    public class ArticleListViewModel
    {
#pragma warning disable IDE1006 // Стили именования
        public List<ArticleViewModel> blogArticles { get; set; } = null!;
#pragma warning restore IDE1006 // Стили именования
    }
}
