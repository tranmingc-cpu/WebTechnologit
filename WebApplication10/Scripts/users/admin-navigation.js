let currentAdminUrl = null;

$(function () {
    const isDashboardPage =
        window.location.pathname === '/Admin/Dashboard'
        || window.location.pathname === '/Admin';

    if (isDashboardPage) {
        loadMain('/Admin/Dashboard');
    }
});

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

    function bindValidation(container) {
        if (!$.validator || !$.validator.unobtrusive) return;

        const $form = $(container).find('form');

        //  FIX: clear validator cũ
        $form.removeData('validator');
        $form.removeData('unobtrusiveValidation');

        $.validator.unobtrusive.parse($form);



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
            initDashboardChart(),
            bindValidation('#mainContent');
            highlightActiveMenu(url);
            scrollToTop();
        })
        .fail(function (xhr) {
            console.error('AJAX FAIL:', url, xhr.status);
        });
}

    function showGlobalModal(type, title, message) {
        const $modal = $('#globalAlertModal');

        if (!$modal.parent().is('body')) {
            $('body').append($modal);
        }

        const $content = $modal.find('.global-modal');
        $content.removeClass('success error warning info')
            .addClass(type);

        let iconHtml = '<i class="fa-solid fa-circle-check"></i>';
        if (type === 'error') iconHtml = '<i class="fa-solid fa-circle-xmark"></i>';
        if (type === 'warning') iconHtml = '<i class="fa-solid fa-triangle-exclamation"></i>';
        if (type === 'info') iconHtml = '<i class="fa-solid fa-circle-info"></i>';

        $modal.find('.modal-icon').html(iconHtml);
        $('#globalAlertModalLabel').text(title || '');
        $('#globalAlertModalBody').text(message || '');

        bootstrap.Modal.getOrCreateInstance($modal[0]).show();
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
        if (url) loadMain(url);
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

        if (!$form.valid()) {
            return;
        }

        if (typeof CKEDITOR !== 'undefined') {
            for (let i in CKEDITOR.instances) {
                CKEDITOR.instances[i].updateElement();
            }
        }

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function (res) {

                if (typeof res === "string") {
                    $('#mainContent').html(res);
                    initCKEditor();
                    bindValidation('#mainContent');
                    scrollToTop();
                    return;
                }

                if (res.success) {
                    showGlobalModal(
                        'success',
                        'Thành công',
                        res.message || 'Thao tác thành công!'
                    );
                    loadMain('/Users/Index');
                    return;
                }

                if (res.success === false) {
                    showGlobalModal(
                        'error',
                        'Không thể thực hiện',
                        res.message || 'Có lỗi xảy ra!'
                    );
                }
            }
        });
    });
function initDashboardChart() {
    const canvas = document.getElementById('revenueChart');
    if (!canvas) {
        console.warn('❌ Không tìm thấy canvas');
        return;
    }

    const labels = JSON.parse(canvas.dataset.labels || '[]');
    const revenues = JSON.parse(canvas.dataset.revenues || '[]');

    console.log('📊 labels:', labels);
    console.log('💰 revenues:', revenues);

    //  HỦY CHART CŨ (NẾU CÓ)
    if (window.revenueChartInstance) {
        window.revenueChartInstance.destroy();
        window.revenueChartInstance = null;
    }
    const ctx = canvas.getContext('2d');

    // Gradient luxury
    const gradient = ctx.createLinearGradient(0, 0, 0, canvas.height);
    gradient.addColorStop(0, 'rgba(34,197,94,0.95)');
    gradient.addColorStop(1, 'rgba(22,163,74,0.4)');

    // Shadow plugin
    const shadowPlugin = {
        id: 'shadow',
        beforeDraw(chart) {
            const { ctx } = chart;
            ctx.save();
            ctx.shadowColor = 'rgba(0,0,0,0.35)';
            ctx.shadowBlur = 18;
            ctx.shadowOffsetY = 8;
        },
        afterDraw(chart) {
            chart.ctx.restore();
        }
    };

    window.revenueChartInstance?.destroy();

    window.revenueChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                data: revenues,
                backgroundColor: gradient,
                borderRadius: 14,
                borderSkipped: false,
                barPercentage: 0.55,
                categoryPercentage: 0.7,
                hoverBackgroundColor: 'rgba(34,197,94,1)'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 1600,
                easing: 'easeOutQuart'
            },
            interaction: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: '#020617',
                    padding: 16,
                    cornerRadius: 14,
                    titleFont: {
                        size: 13,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 14
                    },
                    displayColors: false,
                    callbacks: {
                        label(ctx) {
                            return ' 💰 ' + ctx.raw.toLocaleString('vi-VN') + ' ₫';
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        color: '#CBD5E1',
                        font: { size: 12 }
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(255,255,255,0.05)',
                        drawBorder: false
                    },
                    ticks: {
                        color: '#CBD5E1',
                        padding: 10,
                        font: { size: 12 },
                        callback: v => v.toLocaleString('vi-VN') + ' ₫'
                    }
                }
            }
        },
        plugins: [shadowPlugin]
    });
}
