$(document).on("submit", "#contactForm", function (e) {
    e.preventDefault();

    var form = $(this);
    $(".alert").remove();

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function (res) {

            if (res.success) {
                form.prepend(`
                    <div class="alert alert-success">
                        ${res.message}
                    </div>
                `);
                form[0].reset();
            } else {
                form.prepend(`
                    <div class="alert alert-danger">
                        Vui lòng kiểm tra lại thông tin.
                    </div>
                `);
            }
        },
        error: function () {
            alert("Có lỗi xảy ra, vui lòng thử lại.");
        }
    });
});