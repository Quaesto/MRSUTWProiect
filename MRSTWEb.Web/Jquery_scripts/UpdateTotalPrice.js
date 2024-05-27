function updateTotalPrice() {
    var subtotal = 0.0;
    var totalDelivery = 0.0;

    // Synchronous AJAX call to get the shipping cost
    var deliveryCost = getShippingCost();

    $('.product').each(function () {
        var quantityInput = $(this).find('.quantity-input');
        var priceText = $(this).find('.price').text().replace('$', '');

        if (quantityInput.val() !== "" && !isNaN(quantityInput.val())) {
            var quantity = parseInt(quantityInput.val());
            var price = parseFloat(priceText.replace(',', '.'));

            subtotal += (quantity * price);
        }
    });

    totalDelivery = parseFloat(deliveryCost);

    var totalPrice = subtotal + totalDelivery;

    $('.summary-item .price').first().text('$' + subtotal.toFixed(2));
    $('.summary-item .delivery').text('$' + totalDelivery.toFixed(2));
    $('#totalPrice').text('$' + totalPrice.toFixed(2));
}

function getShippingCost() {
    var cost = 0.0;
    $.ajax({
        url: '/Buy/getShippingCost',
        type: "GET",
        dataType: 'json',
        async: false, // Makes the call synchronous
        success: function (response) {
            if (response.delivery) {
                cost = response.delivery.Cost;
            }
        }
    });
    return cost;
}
updateTotalPrice();


$(document).ready(function () {
    updateTotalPrice();
    displayCartItemCount();


});
function displayCartItemCount() {
    var cartItemCount = localStorage.getItem('cartItemCount') | 0;

    if (cartItemCount !== null && cartItemCount !== undefined) {
        $('#cartItemCount').text(cartItemCount);
    } else {
        cartItemCount = 0;
    }

    $('#cartItemCount').text(cartItemCount);
}
function updateCartItemCount(action) {
    var cartItemCount = localStorage.getItem("cartItemCount") | 0;


    cartItemCount = parseInt(cartItemCount);

    if (action === 'add') {
        cartItemCount++;
    } else if (action === 'remove') {
        cartItemCount--;

        if (cartItemCount < 0) {
            cartItemCount = 0;
        }
    }

    $('#cartItemCount').text(cartItemCount);

    localStorage.setItem('cartItemCount', cartItemCount);
}



function updateTotalPriceAndItemCount(action) {
    updateTotalPrice();
    updateCartItemCount(action);
}
