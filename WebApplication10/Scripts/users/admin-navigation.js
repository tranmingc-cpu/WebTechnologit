let currentAdminUrl = null;

$(function () {

    function initCKEditor() {
        if (typeof CKEDITOR === 'undefined') return;

        $('.ckeditor').each(function () {
            const id = $(this).attr('id');
            if (!id) return;

            if (CKEDITOR.instances[id]) {
                CKEDITOR.instances[id].destroy(true);
            }

            CKEDITOR.replace(id, {
                height: 400,
                removeButtons: 'PasteFromWord',
                allowedContent: true,
                extraAllowedContent: 'img[*]{*}(*);',
                toolbar: [
                    { name: 'basicstyles', items: ['Bold', 'Italic'] },
                    { name: 'paragraph', items: ['NumberedList', 'BulletedList'] },
                    { name: 'insert', items: ['Image', 'Table', 'Link'] },
                    { name: 'styles', items: ['Format'] },
                    { name: 'clipboard', items: ['Undo', 'Redo'] }
                ]
            });
        });
    }

    function loadMain(url) {
        console.log('➡️ LOAD:', url);
        if (url.includes("/Admin/Dashboard") && !url.includes("partial=true")) {
            url += (url.includes("?") ? "&" : "?") + "partial=true";
        }
        currentAdminUrl = url;

        $.get(url)
            .done(function (html) {
                $('#mainContent').html(html);
                initCKEditor();
                highlightActiveMenu(url);
                scrollToTop();
            })
            .fail(function (xhr) {
                console.error('AJAX FAIL:', url, xhr.status);
            });
    }

    function showSuccess(message) {
        $('#globalAlertModalBody').text(message || 'Thay đổi thành công!');
        new bootstrap.Modal(
            document.getElementById('globalAlertModal')
        ).show();
    }

    function highlightActiveMenu(url) {
        $('#adminActionsContainer a.action-card').removeClass('active-link');
        $('#adminActionsContainer a.action-card').each(function () {
            const href = $(this).attr('href');
            if (href && url.startsWith(href)) {
                $(this).addClass('active-link');
            }
        });

        $('nav.header-actions a').removeClass('active-link');
        $('nav.header-actions a').each(function () {
            const href = $(this).attr('href');
            if (href && url.startsWith(href)) {
                $(this).addClass('active-link');
            }
        });
    }

    function scrollToTop() {
        $('html, body').animate({ scrollTop: 0 }, 300);
    }

    $(document).on('click', 'a[data-admin-partial]', function (e) {
        e.preventDefault();
        const url = $(this).attr('href');
        if (!url) return;
        loadMain(url);
    });

    $(document).on('click', 'nav.header-actions a', function (e) {
        const url = $(this).attr('href');
        if (!url || url.includes("/Account/Logout")) return;
        e.preventDefault();
        loadMain(url);
    });

    $(document).on('click', '.btn-create-user', function (e) {
        e.preventDefault();
        loadMain('/Users/Create');
    });

    $(document).on('click', '.btn-edit', function (e) {
        e.preventDefault();
        loadMain('/Users/Edit/' + $(this).data('id'));
    });

    $(document).on('click', '.btn-delete', function (e) {
        e.preventDefault();
        loadMain('/Users/Delete/' + $(this).data('id'));
    });

    $(document).on('click', '#btnBackToList', function (e) {
        e.preventDefault();
        loadMain('/Users/Index');
    });

    $(document).on('submit', 'form.ajax-form', function (e) {
        e.preventDefault();

        const $form = $(this);
        const action = $form.attr('action');

        if (typeof CKEDITOR !== 'undefined') {
            for (let i in CKEDITOR.instances) {
                CKEDITOR.instances[i].updateElement();
            }
        }

        $.post(action, $form.serialize())
            .done(function (res) {
                if (res.success) {
                    showSuccess(res.message);

                    if (action.includes('/Users')) {
                        loadMain('/Users/Index');
                    }

                    if (action.includes('/Pages')) {
                        loadMain(currentAdminUrl);
                    }

                } else if (res.errors) {
                    alert(res.errors.join('\n'));
                }
            })
            .fail(function () {
                alert('Thất bại!');
            });
    });

    $(document).on('submit', 'form#deleteForm.ajax-form', function (e) {
        e.preventDefault();
        const $form = $(this);
        const action = $form.attr('action');

        $.post(action, $form.serialize())
            .done(function (res) {
                if (res.success) {
                    showSuccess(res.message || 'Xóa thành công!');
                    loadMain('/Users/Index');
                } else if (res.errors) {
                    alert(res.errors.join('\n'));
                }
            })
            .fail(function () {
                alert('Xóa thất bại!');
            });
    });
});