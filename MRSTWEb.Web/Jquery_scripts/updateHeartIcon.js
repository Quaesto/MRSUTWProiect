$(document).ready(function () {

    function checkIfBookIsInWishlist(bookId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/Home/getWishList',
                type: 'GET',
                success: function (response) {
                    const wishlistItems = response.books;
                    const isInWishlist = wishlistItems.some(item => item.Id === bookId);

                    localStorage.setItem('wishlistItems', JSON.stringify(wishlistItems));

                    resolve(isInWishlist);
                },
                error: function (xhr, status, error) {
                    reject(error);
                }
            });
        });
    }

    function UpdateWishItemCounter(action) {
        var cartWishCount = localStorage.getItem("wishItemCount") || 0;
        cartWishCount = parseInt(cartWishCount);

        if (action === "add") {
            cartWishCount++;
        } else if (action === "remove") {
            cartWishCount--;

            if (cartWishCount < 0) {
                cartWishCount = 0;
            }
        }

        $('#wishItemCount').text(cartWishCount);
        localStorage.setItem("wishItemCount", cartWishCount);
    }

    function displayWishItemCount() {
        var cartWishCount = localStorage.getItem("wishItemCount") || 0;

        if (cartWishCount !== null && cartWishCount !== undefined) {
            $('#wishItemCount').text(cartWishCount);
        } else {
            cartWishCount = 0;
        }

        $('#wishItemCount').text(cartWishCount);
    }

    function updateHeartIconColor() {
        $('.addToWishlistLink').each(async function () {
            const bookId = $(this).data('book-id');
            const isInWishlist = await checkIfBookIsInWishlist(bookId);
            $(this).toggleClass('heart-red', isInWishlist);
        });
    }

    updateHeartIconColor();
    displayWishItemCount();

    $('.addToWishlistLink').off('click').on('click', function (e) {
        e.preventDefault();

        var form = $(this).closest('form');
        var bookId = $(this).data('book-id');
        var heartIcon = $(this);

        var formData = new FormData(form[0]);
        formData.append('bookId', bookId);

        $.ajax({
            url: 'Home/AddToWishList',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                heartIcon.toggleClass('heart-red');


                const wishlistItems = JSON.parse(localStorage.getItem('wishlistItems')) || [];
                const index = wishlistItems.findIndex(item => item.Id === bookId);

                if (index !== -1) {
                    $.ajax({
                        url: '/Home/RemoveFromWishList',
                        type: 'POST',
                        data: { bookId: bookId },
                        success: function () {
                            wishlistItems.splice(index, 1);
                            localStorage.setItem('wishlistItems', JSON.stringify(wishlistItems));
                            UpdateWishItemCounter('remove');
                            console.log("items is removing from wish list");
                        },
                        error: function (xhr, status, error) {

                        }
                    });
                } else {
                    wishlistItems.push({ Id: bookId });
                    localStorage.setItem('wishlistItems', JSON.stringify(wishlistItems));
                    console.log("items is adding to wish list");

                    UpdateWishItemCounter('add');
                }
            },
            error: function (xhr, status, error) {
                // Handle error
            }
        });
    });
});
