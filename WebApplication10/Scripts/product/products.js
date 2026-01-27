$(function () {

    const wrapper = $("#productEditor");
    if (!wrapper.length) return;

    function getCurrentProductId() {
        return wrapper.data("product-id");
    }

    function reParseValidation() {
        const form = wrapper.find("form");
        if (!form.length) return;

        form.removeData("validator");
        form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);
    }

    function loadEdit(productId) {
        if (!productId) {
            wrapper.empty();
            return;
        }

        wrapper.load("/AdminProducts/Edit", { id: productId }, function () {
            reParseValidation();

            // ✅ init CKEditor GLOBAL
            if (typeof initCKEditor === 'function') {
                initCKEditor();
            }
        });
    }

    loadEdit(getCurrentProductId());

    $(document).on("submit", ".product-edit-form", function (e) {
        e.preventDefault();

        const $form = $(this);

        // ✅ Sync CKEditor
        if (typeof CKEDITOR !== 'undefined') {
            for (let i in CKEDITOR.instances) {
                CKEDITOR.instances[i].updateElement();
            }
        }

        if (!$form.valid()) return;

        $.ajax({
            url: $form.attr("action"),
            type: "POST",
            data: $form.serialize(),
            success: function (res) {

                if (typeof res === "string") {
                    wrapper.html(res);
                    reParseValidation();
                    initCKEditor();
                    return;
                }

                if (res.success) {
                    showGlobalModal(
                        'success',
                        'Thành công',
                        res.message || 'Cập nhật thành công'
                    );
                    loadEdit(getCurrentProductId());
                }
            },
            error: function (xhr) {
                console.error("❌ 500:", xhr.responseText);
                showGlobalModal(
                    'error',
                    'Lỗi',
                    'Lỗi server (500)'
                );
            }
        });
    });

});