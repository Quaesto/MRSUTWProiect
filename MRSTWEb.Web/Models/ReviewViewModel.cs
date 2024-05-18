namespace MRSTWEb.Models
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public string ApplicationUserId { get; set; }
        public bool IsFavourite { get; set; }
        public int BookId { get; set; }
    }
}