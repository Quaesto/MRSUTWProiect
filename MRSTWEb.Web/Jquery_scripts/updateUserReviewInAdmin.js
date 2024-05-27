function updateStars(productId, userId) {

    var cacheKey = 'review_' + productId + '_' + userId;


    function renderStarsAndDetails(data) {
        var userReview = data.review;
        var book = data.book;

        if (userReview && book) {
            var rating = userReview.Rating;
            var starsHtml = '';

            for (var i = 5; i >= 1; i--) {
                if (i <= rating) {
                    starsHtml += '<i class="fa fa-star filled" data-rating="' + i + '"></i>';
                } else {
                    starsHtml += '<i class="fa fa-star" data-rating="' + i + '"></i>';
                }
            }

            $('.star-rating[data-productid="' + productId + '"][data-userid="' + userId + '"]').html(starsHtml);
            $('.total-rating_' + productId + '[data-userid="' + userId + '"]').text('Total Rating: ' + rating);
            $('.book-details_' + productId).html('<p>Book Title: ' + book.Title + '</p><img class="rounded" style="height:200px;width:100px" src="' + book.PathImage + '"/>');
        } else {
            $('.star-rating[data-productid="' + productId + '"][data-userid="' + userId + '"]').html('<p>No rating found for this user.</p>');
            $('.total-rating_' + productId + '[data-userid="' + userId + '"]').text('');
            $('.book-details_' + productId + '[data-userid="' + userId + '"]').html('');
        }
    }

    // Check if the data is in localStorage
    var cachedData = localStorage.getItem(cacheKey);
    if (cachedData) {
        // Parse the cached data and use it
        renderStarsAndDetails(JSON.parse(cachedData));
    } else {

        $('.star-rating[data-productid="' + productId + '"]').html('<i class="fa fa-spinner fa-spin"></i>');


        $.ajax({
            url: '/Home/GetUserReviews',
            type: 'GET',
            data: {
                bookId: productId,
                userId: userId
            },
            success: function (data) {

                localStorage.setItem(cacheKey, JSON.stringify(data));

                renderStarsAndDetails(data);
            },
            error: function () {
                console.log("An error occurred while fetching user reviews.");
            }
        });
    }
}

$("[data-productid]").each(function () {
    let productId = $(this).data("productid");
    let userId = $(this).data("userid");
    updateStars(productId, userId);
});