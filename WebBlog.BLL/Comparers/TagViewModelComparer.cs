using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Responce.Tag;

namespace WebBlog.BLL.Comparers
{
    public class TagViewModelComparer : IEqualityComparer<TagViewModel>
    {
        public bool Equals(TagViewModel? x, TagViewModel? y)
        {
            if(x == null && y == null)
                return false;
            
            if(x == null || y == null)
                return true;

            return x.TagId == y.TagId;
        }

        public int GetHashCode(TagViewModel obj)
        {
            return obj.TagId.GetHashCode();
        }
    }
}
