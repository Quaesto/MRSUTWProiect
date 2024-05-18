function getFavouriteReview() {
    var successAlert = $(".alert-success");
    var warningAlert = $(".alert-warning");

    $('.swiper-wrapper').on('click', '.favouriteButton', function () {
        let reviewId = $(this).data("review");

        $.ajax({
            url: "/Home/AddReviewToMainPage",
            type: "POST",
            data: { reviewId: reviewId },
            success: function (response) {
                if (response.success) {
                    console.log("Review updated successfully!");
                    successAlert.fadeIn();
                    setTimeout(function () {
                        successAlert.fadeOut();
                    }, 3000);
                } else {
                    console.log("The review was already added!");
                    warningAlert.fadeIn();
                    setTimeout(function () {
                        warningAlert.fadeOut();
                    }, 3000);
                }
            },
            error: function () {
                console.log("An error occurred while updating the review!");
                warningAlert.text("An error occurred while updating the review.").fadeIn();
                setTimeout(function () {
                    warningAlert.fadeOut();
                }, 3000);
                successAlert.hide(); 
            }
        });
    });
}

getFavouriteReview();
