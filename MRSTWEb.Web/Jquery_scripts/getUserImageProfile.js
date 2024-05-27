
var userId = $('#userId').val();
if (userId) {
    $.ajax({
        url: '/Account/GetProfileImage',
        data: { id: userId },
        success: function (profileImageUrl) {
            $('#profileImage').attr('src', profileImageUrl);
        },
    });
}
