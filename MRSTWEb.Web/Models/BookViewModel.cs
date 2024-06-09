using MRSTWEb.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace MRSTWEb.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The Title Is Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The Author Is Required")]
        public string Author { get; set; }
        [Required(ErrorMessage = "The Image is Required")]
        [Display(Name = "Image")]
        public string PathImage { get; set; }
        [Required(ErrorMessage = "The Price Is Required")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The Genre Is Required")]
        public string Genre { get; set; }
        [Required(ErrorMessage = "The Language Is Required")]
        public string Language { get; set; }
        public decimal Percentage { get; set; }
        public DateTime SetTime { get; set; } = DateTime.Now;
        public DateTime ExpirationTime { get; set; }
    }
}