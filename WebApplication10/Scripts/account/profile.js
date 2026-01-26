$(document).on("click", "#btnEditProfile", function (e) {
    e.preventDefault();
    $("#editProfileModal").addClass("show");
});

$(document).on("click", "#btnChangePassword", function (e) {
    e.preventDefault();
    $("#changePasswordModal").addClass("show");
});

$(document).on("click", ".profile-close", function () {
    $(this).closest(".profile-modal").removeClass("show");
});

$(document).on("click", "#btnCloseSuccess", function () {
    $("#successModal").removeClass("show");
});


$(document).on("submit", "#editProfileForm", function (e) {
    e.preventDefault();

    $.ajax({
        url: "/Account/EditProfile",
        type: "POST",
        data: $(this).serialize(),
        success: function (res) {
            if (res && res.success) {

                $("#sidebarFullName, #fullnameDisplay")
                    .text($("input[name='FullName']").val());

                $("#phoneDisplay")
                    .text($("input[name='Phone']").val() || "Chưa cập nhật");

                $("#addressDisplay")
                    .text($("input[name='Address']").val() || "Chưa cập nhật");

                $("#editProfileModal").removeClass("show");

                $("#successMessage").text("Thông tin đã được cập nhật thành công!");
                $("#successModal").addClass("show");
            } else {
                alert(res?.message || "Cập nhật thất bại");
            }
        },
        error: function () {
            alert("Có lỗi xảy ra, vui lòng thử lại");
        }
    });
});

$(document).on("submit", "#changePasswordForm", function (e) {
    e.preventDefault();

    $.ajax({
        url: "/Account/ChangePassword",
        type: "POST",
        data: $(this).serialize(),
        success: function (res) {
            if (res && res.success) {
                $("#changePasswordModal").removeClass("show");
                $("#successMessage").text("Mật khẩu đã được thay đổi thành công!");
                $("#successModal").addClass("show");
            } else {
                alert(res?.message || "Có lỗi xảy ra");
            }
        },
        error: function () {
            alert("Có lỗi xảy ra, vui lòng thử lại");
        }
    });
});