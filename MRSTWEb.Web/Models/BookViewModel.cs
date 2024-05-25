using MRSTWEb.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace MRSTWEb.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string PathImage { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public string Language { get; set; }
        public decimal Percentage { get; set; }
        public DateTime SetTime { get; set; } = DateTime.Now;
        public DateTime ExpirationTime { get; set; }
    }
}