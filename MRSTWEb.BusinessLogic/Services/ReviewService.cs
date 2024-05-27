using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MRSTWEb.BusinessLogic.Services
{
    public class ReviewService : IReviewService
    {
        private IUnitOfWork Database { get; set; }
        public ReviewService() { Database = new EFUnitOfWork(); }
        public void AddReview(ReviewDTO reviewDto)
        {
            var review = new Review
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                ApplicationUserId = reviewDto.ApplicationUserId,
                BookId = reviewDto.BookId,
                IsFavourite = reviewDto.IsFavourite,
            };
            Database.Reviews.Create(review);
 
        }
        public ReviewDTO GetReview(int id)
        {
            var review = Database.Reviews.Get(id);
            var reviewDto = new ReviewDTO
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                ApplicationUserId = review.ApplicationUserId,
                BookId = review.BookId,
                IsFavourite = review.IsFavourite,
            };
            return reviewDto;
        }

        public void RemoveReview(int reviewId)
        {
            var review = Database.Reviews.Get(reviewId);
            if (review != null)
            {
                Database.Reviews.Delete(reviewId);
                Database.Save();
            }
        }

        public IEnumerable<ReviewDTO> GetReviewByBookId(int bookId)
        {
            var reviews = Database.Reviews.GetByBook(bookId);
            var reviewsDto = new List<ReviewDTO>();
            foreach (var review in reviews)
            {
                var reviewDto = new ReviewDTO
                {
                    Id = review.Id,
                    BookId = review.BookId,
                    ApplicationUserId = review.ApplicationUserId,
                    Comment = review.Comment,
                    Rating = review.Rating,
                    IsFavourite = review.IsFavourite,
                };
                reviewsDto.Add(reviewDto);
            }


            return reviewsDto;
        }
        public IEnumerable<ReviewDTO> GetAllReviews()
        {
            var reviews = Database.Reviews.GetAll();
            var reviewsDTO = new List<ReviewDTO>();
            foreach (var review in reviews)
            {
                var reviewDTO = new ReviewDTO
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ApplicationUserId = review.ApplicationUserId,
                    BookId = review.BookId,
                    IsFavourite = review.IsFavourite,
                };
                reviewsDTO.Add(reviewDTO);
            }
            return reviewsDTO;
        }
        public IEnumerable<ReviewDTO> GetUserReview(string userId)
        {
            var review = Database.Reviews.GetAll().Where(x => x.ApplicationUserId == userId).ToList();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>()).CreateMapper();
            var reviewsDto = mapper.Map<IEnumerable<Review>, List<ReviewDTO>>(review);
            return reviewsDto;
        }
        public void UpdateReview(ReviewDTO reviewDto)
        {
            var review = new Review
            {
                Id = reviewDto.Id,
                BookId = reviewDto.BookId,
                ApplicationUserId = reviewDto.ApplicationUserId,
                Comment = reviewDto.Comment,
                Rating = reviewDto.Rating,
                IsFavourite = reviewDto.IsFavourite,
            };
            Database.Reviews.Update(review);

        }
        public void Dispose()
        {
            Database.Dispose();
        }

    }
}
