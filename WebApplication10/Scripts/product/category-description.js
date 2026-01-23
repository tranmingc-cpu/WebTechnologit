$(function () {

    const wrapper = $("#categoryDescription");
    if (!wrapper.length) return;

    function getCurrentCategoryId() {
        return wrapper.attr("data-category-id");
    }

    function destroyAllEditors() {
        if (window.CKEDITOR) {
            Object.keys(CKEDITOR.instances).forEach(key => {
                CKEDITOR.instances[key].destroy(true);
                delete CKEDITOR.instances[key];
            });
        }

        wrapper.find("textarea.category-editor").removeAttr("id");
    }

    function initEditor() {
        const textarea = wrapper.find("textarea.category-editor");
        if (!textarea.length) return;

        const editorId = "editor_" + Date.now();
        textarea.attr("id", editorId);

        const content = textarea.val();

        const editor = CKEDITOR.replace(editorId);
        editor.on("instanceReady", function () {
            editor.setData(content);
        });
    }

    function loadDescription(categoryId) {
        destroyAllEditors();

        if (!categoryId) {
            wrapper.html("");
            return;
        }

        wrapper.load(
            "/Categories/Description?id=" + categoryId + "&_=" + Date.now()
        );
    }

    function loadEdit(categoryId) {
        destroyAllEditors();

        wrapper.load(
            "/Categories/Edit?id=" + categoryId + "&_=" + Date.now(),
            function () {
                initEditor();
            }
        );
    }

    loadDescription(getCurrentCategoryId());

    $(document).on("category:changed", function (e, categoryId) {
        wrapper.attr("data-category-id", categoryId);
        loadDescription(categoryId);
    });

    $(document).on("click", "#btnEditCategory", function () {
        const id = getCurrentCategoryId();
        if (id) loadEdit(id);
    });

    $(document).on("submit", "#editCategoryForm", function (e) {
        e.preventDefault();

        const form = $(this);
        const categoryId = form.find("input[name='CategoryId']").val();
        const editorId = form.find("textarea").attr("id");
        const content = CKEDITOR.instances[editorId].getData();

        $.ajax({
            url: "/Categories/Edit",
            type: "POST",
            data: {
                __RequestVerificationToken: form.find("input[name='__RequestVerificationToken']").val(),
                CategoryId: categoryId,
                Description: content
            },
            success: function (res) {
                if (res.success) {
                    wrapper.attr("data-category-id", categoryId);
                    loadDescription(categoryId);
                }
            }
        });
    });

    $(document).on("click", "#btnCancelEdit", function () {
        loadDescription(getCurrentCategoryId());
    });

});