$(document).on("click", "#newsletterSection #btnSubscribe", function (e) {
    e.preventDefault();

    var $section = $("#newsletterSection");
    var token = $section.find("input[name='__RequestVerificationToken']").val();

    $.ajax({
        url: "/Newsletter/Subscribe",
        type: "POST",
        data: { __RequestVerificationToken: token },
        success: function (html) {
            $section.replaceWith(html);

            reloadNewsletterFooter();
        }
    });
});

$(document).on("click", "#newsletterSection #btnUnsubscribe", function (e) {
    e.preventDefault();

    var $section = $("#newsletterSection");
    var token = $section.find("input[name='__RequestVerificationToken']").val();

    $.ajax({
        url: "/Newsletter/Unsubscribe",
        type: "POST",
        data: { __RequestVerificationToken: token },
        success: function (html) {
            $section.replaceWith(html);

            reloadNewsletterFooter();
        }
    });
});

$(document).on("submit", "#newsletterForm", function (e) {
    e.preventDefault();

    var $form = $(this);
    var email = $form.find("#newsletterEmail").val().trim();
    var token = $form.find("input[name='__RequestVerificationToken']").val();

    if (!email) return;

    $.ajax({
        url: "/Newsletter/SubscribeFooter",
        type: "POST",
        data: {
            email: email,
            __RequestVerificationToken: token
        },
        success: function (res) {
            if (res.success) {
                reloadNewsletterFooter();
            }
        }
    });
});
function reloadNewsletterFooter() {
    $.ajax({
        url: "/Newsletter/FooterStatus",
        type: "GET",
        success: function (html) {
            $("#footerNewsletterContent").html(html);
        }
    });
}