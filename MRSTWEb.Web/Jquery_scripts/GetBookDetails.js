$(document).ready(function () {
    $('#bookModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var bookId = button.data('book-id');

        $.ajax({
            url: '/Home/GetBookDetails',
            type: 'GET',
            data: { id: bookId },
            success: function (response) {
                var book = response.book;

                var modalBody = $('.modal-body');
                modalBody.empty();
                modalBody.append('<h4>' + book.Title + '</h4>');
                modalBody.append('<p>Price: $' + book.Price + '</p>');
                modalBody.append('<p>Author: ' + book.Author + '</p>');
                modalBody.append('<p>Genre: ' + book.Genre + '</p>');
                modalBody.append('<p>Language: ' + book.Language + '</p>');
            },
            error: function () {
                // Handle error
                $('.modal-body').html('<p>Error loading book details.</p>');
            }
        });
    });
});