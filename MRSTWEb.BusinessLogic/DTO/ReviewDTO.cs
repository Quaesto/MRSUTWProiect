using MRSTWEb.Domain.Entities;

namespace MRSTWEb.BusinessLogic.DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsFavourite { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int BookId { get; set; }
    }
}
