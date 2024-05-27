
using MRSTWEb.BusinessLogic.DTO;
using System.Collections.Generic;

namespace MRSTWEb.BusinessLogic.Interfaces
{
    public interface IReviewService
    {
        void AddReview(ReviewDTO reviewDto);
        ReviewDTO GetReview(int id);
        IEnumerable<ReviewDTO> GetAllReviews();
        IEnumerable<ReviewDTO> GetReviewByBookId(int bookId);
        IEnumerable<ReviewDTO> GetUserReview(string userId);
        void UpdateReview(ReviewDTO reviewDto);
        void RemoveReview(int reviewId);
        void Dispose();
    }
}
