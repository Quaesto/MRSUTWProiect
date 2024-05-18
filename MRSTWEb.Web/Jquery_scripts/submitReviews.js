$(document).ready(function () {

    function checkUserReview(productId, userId, callback) {
        $.ajax({
            url: '/Home/GetReviews',
            type: 'GET',
            data: { bookId: productId },
            success: function (data) {
                var userReview = data.reviews.find(review => review.ApplicationUserId === userId);
                if (userReview) {
                    callback(true);
                } else {
                    callback(false);
                }
            },
            error: function () {
                console.log("An error occurred while checking user review.");
                callback(false);
            }
        });
    }


    function checkAuthentication(productId) {
        $.ajax({
            url: '/Home/IsAuthenticated',
            type: 'GET',
            success: function (data) {
                if (data.isAuthenticated) {
                    $('.star-rating[data-productid="' + productId + '"] i').removeClass('disabled');
                    $('.submit[data-productid="' + productId + '"]').prop('disabled', false);
                } else {
                    $('.star-rating[data-productid="' + productId + '"] i').addClass('disabled');
                    $('.submit[data-productid="' + productId + '"]').prop('disabled', true);
                }
            },
            error: function () {
                console.log("An error occurred while checking authentication status.");
            }
        });
    }


    function updateStars(productId) {

        $('.star-rating[data-productid="' + productId + '"]').html('<i class="fa fa-spinner fa-spin"></i>');

        $.ajax({
            url: '/Home/GetReviews?bookId=' + productId,
            type: 'GET',

            success: function (data) {
                var reviews = data.reviews;
                var totalRating = 0;
                var totalReviews = reviews.length;

                for (var i = 0; i < reviews.length; i++) {
                    totalRating += reviews[i].Rating;
                }

                var averageRating = totalReviews > 0 ? (totalRating / totalReviews).toFixed(1) : 0.0;

                var roundedAverageRating = Math.floor(averageRating * 2) / 2;


                var starsHtml = '';
                for (var rating = 5; rating >= 1; rating--) {
                    if (rating <= roundedAverageRating) {
                        starsHtml += '<i class="fa fa-star filled" data-rating="' + rating + '"></i>';
                    } else {
                        starsHtml += '<i class="fa fa-star" data-rating="' + rating + '"></i>';
                    }
                }

                $('.star-rating[data-productid="' + productId + '"]').html(starsHtml);
                $('.total-rating_' + productId).text('Total Rating: ' + averageRating);
                $('.total-rated_' + productId).text('Total Rated: ' + reviews.length);
            },
            error: function () {
                console.log("An error occurred while fetching reviews.");
            }
        });
    }


    $(".swiper-slide").on("click", function () {
        var userId = $(this).data("userid");
        var productId = $(this).data("book-id");

        var review = $('#reviewText_' + productId);

        var rating = null;
        var modalWarnings = $('.modal-warnings_' + productId);
        modalWarnings.html('');
        checkUserReview(productId, userId, function (hasSubmitted) {
            if (hasSubmitted) {

                modalWarnings.html(`<p style="color: red;">You have already submitted a review for this product. </p>`);
                return;
            }
            $('#reviewModal_' + productId + ' .star-rating i').on('click', function () {
                if (!$(this).hasClass('disabled') && !$(this).hasClass("read")) {
                    rating = $(this).data('rating');

                    $(this).siblings().addBack().removeClass('filled');

                    $(this).addClass('filled');
                    $(this).nextAll('i').addClass('filled');


                    modalWarnings.html(`<p style="color: green;">Selected rating: ${rating} </p>`);
                }
            });





            $('#reviewModal_' + productId + ' .submit').off().on("click", function () {

                checkAuthentication(productId);

                $.ajax({
                    url: '/Home/PostReview',
                    type: 'POST',
                    data: {
                        Rating: rating,
                        Comment: review.val(),
                        ApplicationUserId: userId,
                        BookId: productId
                    },
                    success: function (data) {
                        if (data.success) {
                            console.log("Review successfully submitted.");

                            $('#reviewModal_' + productId).modal('hide');
                            console.log(review.val());
                            console.log(rating);
                            updateStars(productId);

                        } else {

                            modalWarnings.html(`<p style="color: red;">Failed to submit review </p>`);
                        }
                    },
                    error: function () {
                        console.log("An error occurred while submitting the review.");
                    }
                });
            });


            checkAuthentication(productId);
        });
    });
    var encounteredProductIds = {};

    $('[data-productid]').each(function () {
        var productId = $(this).data('productid');

        if (encounteredProductIds[productId]) {

            return false;
        } else {

            encounteredProductIds[productId] = true;
            updateStars(productId);

            checkAuthentication(productId);
            $('#reviewModal_' + productId).on('shown.bs.modal', function () {
                $(this).find('.star-rating i').removeClass('filled');
                $(this).find('.star-rating i').removeAttr('data-clicked');
            });
        }
    });



});
