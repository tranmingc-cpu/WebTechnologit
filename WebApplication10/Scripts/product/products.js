$(function () {

    const wrapper = $("#productEditor");
    if (!wrapper.length) return;

    function getCurrentProductId() {
        return wrapper.data("product-id");
    }

    function destroyAllEditors() {
        if (window.CKEDITOR) {
            for (let name in CKEDITOR.instances) {
                CKEDITOR.instances[name].destroy(true);
            }
        }
        wrapper.find("textarea.product-editor").removeAttr("id");
    }

    function initEditor() {
        const textarea = wrapper.find("textarea.product-editor");
        if (!textarea.length) return;

        const editorId = "product_editor_" + Date.now();
        textarea.attr("id", editorId);

        CKEDITOR.replace(editorId);
    }

    function loadEdit(productId) {
        destroyAllEditors();

        if (!productId) {
            wrapper.empty();
            return;
        }

        wrapper.load("/Products/Edit", { id: productId }, function () {
            initEditor();
        });
    }

    // INIT
    loadEdit(getCurrentProductId());

    // CLICK EDIT
    $(document).on("click", "#btnEditProduct", function () {
        const id = getCurrentProductId();
        if (id) loadEdit(id);
    });

    // SUBMIT EDIT PRODUCT
    $(document).on("submit", ".product-edit-form", function (e) {
        e.preventDefault();

        const form = $(this);
        const textareaId = form.find("textarea.product-editor").attr("id");

        if (CKEDITOR.instances[textareaId]) {
            CKEDITOR.instances[textareaId].updateElement();
        }

        $.ajax({
            url: form.attr("action") || "/Products/Edit",
            type: "POST",
            data: form.serialize(),
            success: function (res) {

                // SERVER TRẢ VIEW (VALIDATION FAIL)
                if (typeof res === "string") {
                    wrapper.html(res);
                    initEditor();
                    return;
                }

                // SUCCESS
                if (res.success) {
                    wrapper.data("product-id", res.productId);
                    loadEdit(res.productId);
                }
            }
        });
    });

    // CANCEL
    $(document).on("click", "#btnCancelEditProduct", function () {
        loadEdit(getCurrentProductId());
    });

});
