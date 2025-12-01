
$(document).ready(function () {

    $(document).on('click', '.chude-filter', function (e) {
        e.preventDefault();

        console.log('📚 Chủ đề clicked');

        var path = $(this).data('path');
        var newUrl = '/chude/' + path;

        // Highlight
        $('.chude-filter').removeClass('active fw-bold');
        $(this).addClass('active fw-bold');

        loadChuDe(newUrl, path);
    });

    function loadChuDe(url, path) {
        console.log('🔄 Loading chủ đề:', url);

        showLoading();

        $.ajax({
            url: url,
            type: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'  // ← Đánh dấu AJAX
            },
            success: function (response) {
                if (response.success) {
                    console.log('✅ Loaded successfully');

                    updateContent(response);

                    updateURLAndBreadcrumb(url, response, path);

                    // Scroll
                    scrollToResults();

                    // Highlight chủ đề
                    highlightCurrentChuDe(path);
                } else {
                    showError(response.message || 'Có lỗi xảy ra');
                }
            },
            error: function (xhr, status, error) {
                console.error('❌ AJAX Error:', error);
                showError(error, xhr.responseText);
            }
        });
    }

    function loadSaches(page) {
        page = page || 1;

        var filters = collectFilters();
        filters.page = page;

        var currentPath = window.location.pathname;

        showLoading();

        $.ajax({
            url: currentPath,
            type: 'GET',
            traditional: true,
            data: filters,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                if (response.success) {
                    updateContent(response);
                    updateURLWithFilters(currentPath, filters);
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

    function highlightCurrentChuDe(path) {
        $('.chude-filter').removeClass('active fw-bold');

        if (path) {
            $(`.chude-filter[data-path="${path}"]`).addClass('active fw-bold');
        } else {
            // Lấy từ URL hiện tại
            var currentPath = window.location.pathname.replace('/chude/', '');
            $(`.chude-filter[data-path="${currentPath}"]`).addClass('active fw-bold');
        }
    }

    function updateURLAndBreadcrumb(url, response, path) {
        history.pushState({ path: path }, '', url);

        if (response.breadcrumb) {
            $('nav[aria-label="breadcrumb"]').html(response.breadcrumb);
        }

        $('#filterForm').attr('data-path', path);

        if (response.pageTitle) {
            document.title = response.pageTitle;
        }

        console.log('✅ URL updated:', url);
    }

    function updateURLWithFilters(baseUrl, filters) {
        var queryString = $.param(filters, true);
        var newUrl = baseUrl + (queryString ? '?' + queryString : '');
        history.pushState({ filters: filters }, '', newUrl);

        console.log('✅ URL updated with filters:', newUrl);
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


    // ========== XỬ LÝ NÚT BACK/FORWARD ==========
    window.addEventListener('popstate', function (event) {
        console.log('⬅️ Popstate event');

        if (event.state && event.state.path) {
            // User nhấn Back/Forward sau khi click chủ đề
            loadChuDe(window.location.pathname, event.state.path);
        } else if (event.state && event.state.filters) {
            // User nhấn Back/Forward sau khi filter
            location.reload(); // Hoặc load lại với filters từ state
        } else {
            // Fallback: reload trang
            location.reload();
        }
    });

    highlightCurrentChuDe();
});