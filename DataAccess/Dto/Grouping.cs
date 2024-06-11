using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dto
{
    public class Grouping<TKey, TElement>
    {
        public TKey Key { get; set; }
        public IEnumerable<TElement> Items { get; set; }

        public Grouping(TKey key, IEnumerable<TElement> items)
        {
            Key = key;
            Items = items;
        }
    }
}
