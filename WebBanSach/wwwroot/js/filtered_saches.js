
$(document).ready(function () {

    function loadSaches(page) {
        page = page || 1;

        var filters = collectFilters();
        filters.page = page;

        console.log('Các filter được chọn: ', filters);

        showLoading();

        $.ajax({
            url: '/Customer/Home/SachTheoChuDe',
            type: 'GET',
            traditional: true,
            data: filters,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                if (response.success) {
                    updateContent(response);
                    updateURL(filters);
                    scrollToResults();
                }
                else
                {
                    showError(response.message || 'Có lỗi xảy ra');
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Lỗi:", status, error);
                showError(error, xhr.responseText);
            }
        });
    }

    //===== HÀM LẤY FILTERS =====
    function collectFilters() {
        var filters = {};

        var chuDeId = $('#filterForm').data('chude-id');
        if (chuDeId) {
            filters.id = chuDeId
        }

        var selectedPrices = [];
        $('.price-filter:checked').each(function () {
            selectedPrices.push($(this).val());
        });
        if (selectedPrices.length > 0) {
            filters.priceRanges = selectedPrices;
        }

        var selectedAuthors = [];
        $('.author-filter:checked').each(function () {
            selectedAuthors.push(parseInt($(this).val()));
        });
        if (selectedAuthors.length > 0) {
            filters.tacGiaIds = selectedAuthors;
        }

        var selectedPublishers = [];
        $('.publisher-filter:checked').each(function () {
            selectedPublishers.push(parseInt($(this).val()));
        });
        if (selectedPublishers.length > 0) {
            filters.nhaXuatBanIds = selectedPublishers;
        }

        var keyword = $('#search_bar').val();
        if (keyword && keyword.trim() !== '') {
            filters.keyword = keyword.trim();
        }

        var sortBy = $('#sort-select').val();
        if (sortBy) {
            filters.sortBy = sortBy;
            $('#filterForm').data('sort-by', sortBy);
        }

        var pageSize = $('#pagesize-select').val() || 12;
        filters.pageSize = parseInt(pageSize);
        $('#filterForm').data('page-size', pageSize);

        return filters;
    }

    function showLoading() {
        $('#sach-list-section').html(`
            <style>
                #sach-list-section{
                    display: unset;
                }
            </style>
            <div class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Đang tải...</span>
                </div>
                <p class="mt-2 text-muted">Đang tải sách...</p>
            </div>
        `);

        $('#pagination-section').html('');
    }

    function updateContent(response) {
        $('#toolbar-section').html(response.toolbar);
        $('#sach-list-section').html(response.sachList);
        $('.chude_item_root').html(response.chuDeTuongUng);
        $('#nhaxuatban_filter').html(response.nhaXuatBanList);
        $('#tacgia_filter').html(response.tacGiaList);
        $('#pagination-section').html(response.pagination);

        highlightCurrentChuDe();
    }

    function highlightCurrentChuDe() {
        var currentChuDeId = $('#filterForm').data('chude-id');
        $('.chude-filter').removeClass('active fw-bold');
        if (currentChuDeId) {
            $(`.chude-filter[data-chude-id="${currentChuDeId}"]`).addClass('active fw-bold');
        }
    }

    function updateURL(filters) {
        var newUrl = '/Customer/Home/SachTheoChuDe?' + $.param(filters, true);
        history.pushState(filters, '', newUrl);
    }

    function scrollToResults() {
        $('html, body').animate({
            scrollTop: $('#sachtheochude_right').offset().top - 20
        }, 300);
    }
    function showError(error, detail) {
        $('#sachtheochude_right').html(`
            <div class="alert alert-danger">
                <strong>Có lỗi xảy ra:</strong> ${error}
                ${detail ? `
                    <hr>
                    <p><b>Chi tiết từ Server:</b></p>
                    <pre style="max-height: 200px; overflow-y: auto;">${detail}</pre>
                ` : ''}
            </div>
        `);
    }

    // ===== SỰ KIỆN KÍCH HOẠT FILTER =====
    //Lọc theo giá
    $(document).on('change', '.price-filter', function () {
        console.log('💰 Giá filter changed');
        loadSaches(1);
    });

    //Lọc theo chủ để
    $(document).on('click', '.chude-filter', function (e) {
        e.preventDefault();

        console.log('Chủ đề filter clicked');

        // Highlight chủ đề được chọn
        $('.chude-filter').removeClass('active');
        $(this).addClass('active');

        // Update data-chude-id trong form
        var newChuDeId = $(this).data('chude-id');
        $('#filterForm').data('chude-id', newChuDeId);

        loadSaches(1);
    });

    //Lọc theo tác giả
    $(document).on('change', '.author-filter', function () {
        console.log('✍️ Tác giả filter changed');
        loadSaches(1);
    });

    //Lọc theo nhà xuất bản
    $(document).on('change', '.publisher-filter', function () {
        console.log('🏢 NXB filter changed');
        loadSaches(1);
    });


    //Lọc theo thanh search
    $(document).on('click', '#search_button', function () {
        console.log('🔍 Search clicked');
        loadSaches(1);
    });

    $(document).on('keypress', '#search_bar', function (e) {
        if (e.which === 13) {
            console.log('🔍 Search by Enter');
            loadSaches(1);
        }
    });

    //Lọc theo sắp xếp
    $(document).on('change', '#sort-select', function () {
        console.log('🔀 Sort changed:', $(this).val());
        loadSaches(1);
    });

    //Thay đổi dựa vào số lượng sách mỗi trang
    $(document).on('change', '#pagesize-select', function () {
        var pageSize = $(this).val();
        console.log('📄 Page size changed:', pageSize);

        $('#filterForm').data('page-size', pageSize);

        loadSaches(1);
    });

    //Thay đổi dựa trên pagination
    $(document).on('click', '.pagination .page-link', function (e) {
        e.preventDefault();

        if ($(this).parent().hasClass('disabled')) {
            return;
        }

        var page = $(this).data('page');
        if (page) {
            console.log('📄 Page changed:', page);
            loadSaches(page);
        }
    });

    // Nút Back
    window.addEventListener('popstate', function (e) {
        location.reload();
    });

    highlightCurrentChuDe();
});