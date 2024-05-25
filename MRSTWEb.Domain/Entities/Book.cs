using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PathImage { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
        public ICollection<Review> Review { get; set; }
        public virtual Discount Discount { get; set; }

    }
}
