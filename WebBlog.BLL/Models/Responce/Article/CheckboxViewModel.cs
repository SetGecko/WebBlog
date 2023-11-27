using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Responce.Article
{
    public class CheckboxViewModel
    {
        public Guid Id { get; set; }
        public string LabelName { get; set; } = default!;
        public bool IsChecked { get; set; }
    }
}
