$(document).ready(function () {
    function getAllBooks() {
        $.ajax({
            url: '/Search/GetAllBooks',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                var dropdownMenuAuthor = $('.dropdown-menu.dropdown-submenu.author');
                var dropdownMenuTitle = $('.dropdown-menu.dropdown-submenu.title');
                var dropdownMenuGenre = $('.dropdown-menu.dropdown-submenu.genre');
                var dropdownMenuLanguage = $('.dropdown-menu.dropdown-submenu.language');
                var uniqueTitles = [...new Set(response.books.map(book => book.Title))];
                var uniqueGenres = [...new Set(response.books.map(book => book.Genre))];
                var uniqueLanguage = [...new Set(response.books.map(book => book.Language))];
                uniqueTitles.forEach(function (title) {
                    dropdownMenuTitle.append('<li><a class="dropdown-item" href="javascript:void(0)" onclick="searchByKeyword(\'' + title + '\')">' + title + '</a></li>');
                });
                uniqueGenres.forEach(function (genre) {
                    dropdownMenuGenre.append('<li><a class="dropdown-item" href="javascript:void(0)" onclick="searchByKeyword(\'' + genre + '\')">' + genre + '</a></li>');
                });
                uniqueLanguage.forEach(function (language) {

                    dropdownMenuLanguage.append('<li><a class="dropdown-item" href="javascript:void(0)" onclick="searchByKeyword(\'' + language + '\')">' + language + '</a></li>');
                });
                response.books.forEach(function (book) {
                    dropdownMenuAuthor.append('<li><a class="dropdown-item" href="javascript:void(0)" onclick="searchByKeyword(\'' + book.Author + '\')">' + book.Author + '</a></li>');

                });

            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }


    getAllBooks();
});
$(document).ready(function () {

    $('#price-min').on('input', function () {
        $('#minPriceDisplay').text($(this).val());
    });


    $('#price-max').on('input', function () {
        $('#maxPriceDisplay').text($(this).val());
    });
});
