using System;
using System.Collections.Generic;

namespace BussinessObject.Models
{
    public partial class Category
    {
        public Category()
        {
            Films = new HashSet<Film>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Desc { get; set; } = null!;

        public virtual ICollection<Film> Films { get; set; }
    }
}
