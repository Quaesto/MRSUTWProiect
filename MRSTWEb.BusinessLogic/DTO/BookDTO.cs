using MRSTWEb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PathImage { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }

        public decimal Percentage { get; set; }
        public DateTime SetTime { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
