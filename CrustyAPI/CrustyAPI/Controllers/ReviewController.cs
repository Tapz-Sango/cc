using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Customer submits a review
        [HttpPost("customer/{customerId:int}")]
        public async Task<IActionResult> CreateReview(
            int customerId,
            [FromBody] CreateReviewRequest request)
        {
            var review = await _reviewService.CreateReview(customerId, request);

            if (review == null)
                return BadRequest(new { message = "Could not create the review. Check the customer and review details." });

            return Ok(review);
        }

        // Admin portal retrieves all reviews
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviews();
            return Ok(reviews);
        }
        // Customer retrieves their own reviews and admin replies
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetCustomerReviews(int customerId)
        {
            var reviews = await _reviewService.GetCustomerReviews(customerId);
            return Ok(reviews);
        }

        // Admin responds to a review
        [HttpPut("{reviewId:int}/response/{adminId:int}")]
        public async Task<IActionResult> RespondToReview(
            int reviewId,
            int adminId,
            [FromBody] AdminReviewResponseRequest request)
        {
            var review = await _reviewService.RespondToReview(
                reviewId,
                adminId,
                request);

            if (review == null)
                return NotFound(new { message = "Review or admin account not found." });

            return Ok(review);
        }
    }
}