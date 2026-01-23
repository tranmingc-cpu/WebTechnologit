$(function () {

    const left = '#auth-left-container';
    const right = '#auth-right-container';
    const wrapper = '.auth-container';

    function updateFromHtml(html) {
        const $html = $('<div>').html(html);

        const fullLeft = $html.find('.auth-left').html();
        const fullRight = $html.find('.auth-right').html();

        $(wrapper).css('opacity', 0);

        requestAnimationFrame(() => {

            if (fullLeft !== undefined) {
                $(left).html(fullLeft);
            } else {
                $(left).html(html);
            }

            if (fullRight !== undefined) {
                $(right).html(fullRight);
            }

            $(wrapper).css('opacity', 1);
        });
    }

    function loadAuth(url, push = true) {
        $.ajax({
            url: url,
            type: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (html) {
                updateFromHtml(html);

                if (push) {
                    history.pushState({ url }, '', url);
                }
            },
            error: function () {
                alert('Không thể tải trang. Vui lòng thử lại.');
            }
        });
    }

    $(document).on('click', '[data-auth-link]', function (e) {
        e.preventDefault();
        loadAuth($(this).attr('href'));
    });

    $(document).on('submit', `${left} form`, function (e) {
        e.preventDefault();

        const $form = $(this);

        $.ajax({
            url: $form.attr('action') || window.location.pathname,
            type: 'POST',
            data: $form.serialize(),
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (res) {

                if (typeof res === 'string') {
                    updateFromHtml(res);
                    return;
                }

                if (res.success && res.redirect) {
                    window.location.href = res.redirect;
                }
            },
            error: function () {
                alert('Có lỗi xảy ra. Vui lòng thử lại.');
            }
        });
    });

    window.onpopstate = function (e) {
        if (e.state && e.state.url) {
            loadAuth(e.state.url, false);
        }
    };

});