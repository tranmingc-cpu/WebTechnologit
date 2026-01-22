$(document).on('submit', '#newsletterForm', function (e) {
    e.preventDefault();

    var email = $('#newsletterEmail').val().trim();
    var token = $('input[name="__RequestVerificationToken"]').val();
    var $msg = $('#newsletterMessage');

    if (!email) {
        $msg.html('<span style="color:red">Vui lòng nhập email</span>');
        return;
    }

    $.ajax({
        url: '/Newsletter/Subscribe',
        type: 'POST',
        data: {
            email: email,
            __RequestVerificationToken: token
        },
        success: function (res) {
            if (res.success) {
                $msg
                    .removeClass('error')
                    .addClass('success')
                    .html('<i class="fa fa-check-circle"></i> ' + res.message);
                $('#newsletterEmail').val('');
            } else {
                $msg
                    .removeClass('success')
                    .addClass('error')
                    .html('<i class="fa fa-circle-exclamation"></i> ' + res.message);
            }
        },
        error: function () {
            $msg.html('<span style="color:red">Có lỗi xảy ra, vui lòng thử lại</span>');
        }
    });
});