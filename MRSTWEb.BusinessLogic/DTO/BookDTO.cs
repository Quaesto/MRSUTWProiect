using System;

namespace MRSTWEb.BusinessLogic.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PathImage { get; set; }
        public decimal Price { get; set; }
        public string Language { get; set; }
        public string Genre { get; set; }

    }
}
