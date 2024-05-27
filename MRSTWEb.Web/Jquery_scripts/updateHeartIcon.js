$(document).ready(function () {

    async function getWishlistItems() {
        try {
            const response = await $.ajax({
                url: '/Home/getWishList',
                type: 'GET'
            });

            return response.books;
        } catch (error) {
            console.error(error);
            throw error;
        }
    }

    async function checkIfBookIsInWishlist(wishlistItems, bookId) {
        return wishlistItems.some(item => item.Id === bookId);
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
        $('#wishItemCount').text(cartWishCount);
    }

    async function updateHeartIconColor(wishlistItems) {
        $('.addToWishlistLink').each(async function () {
            const bookId = $(this).data('book-id');
            const isInWishlist = await checkIfBookIsInWishlist(wishlistItems, bookId);
            $(this).toggleClass('heart-red', isInWishlist);
        });
    }

    async function handleAddToWishlist(e) {
        e.preventDefault();

        var form = $(this).closest('form');
        var bookId = $(this).data('book-id');
        var heartIcon = $(this);

        var formData = new FormData(form[0]);
        formData.append('bookId', bookId);

        try {
            const response = await $.ajax({
                url: '/Home/AddToWishList',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false
            });

            heartIcon.toggleClass('heart-red');

            const wishlistItems = JSON.parse(localStorage.getItem('wishlistItems')) || [];
            const index = wishlistItems.findIndex(item => item.Id === bookId);

            if (index !== -1) {
                await $.ajax({
                    url: '/Home/RemoveFromWishList',
                    type: 'POST',
                    data: { bookId: bookId }
                });
                wishlistItems.splice(index, 1);
                UpdateWishItemCounter('remove');
            } else {
                wishlistItems.push({ Id: bookId });
                UpdateWishItemCounter('add');
            }

            localStorage.setItem('wishlistItems', JSON.stringify(wishlistItems));
        } catch (error) {
            console.error(error);
            // Handle error
        }
    }

    async function init() {
        const wishlistItems = await getWishlistItems();
        updateHeartIconColor(wishlistItems);
        displayWishItemCount();

        $('.addToWishlistLink').off('click').on('click', handleAddToWishlist);
    }

    init();
});
