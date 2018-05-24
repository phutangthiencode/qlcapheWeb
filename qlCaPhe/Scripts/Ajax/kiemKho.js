
$(document).ready(function () {
    bindData();
});

function displayContentIntabs(elementClick , element) {

    //-----Hủy các tab không chọn
    $('.js-tab-item').removeClass('active');

    //------Kích hoạt tab đã chọn
    $(elementClick).parent().addClass('active');

    //-----Ản tất cả các tab
    $('.js-tabs').attr('style', 'display:none');
    $('.js-tabs').removeClass('active in ');
    //---Hiện tab đã chọn
    $(element).attr('style', 'display:block');
    $(element).addClass(' active in ');
    bindData();
}


function bindData() {
    //-------Sự kiện chọn vào tab in danh sách và lưu 
    $('#js-tab-luudulieu').click(function () {
        displayContentIntabs('js-tab-indanhsach', '#luudulieu');
    });

    $('#js-tab-indanhsach').click(function () {
        displayContentIntabs('#js-tab-luudulieu', '#indanhsach');
    });




    //---------Hàm danh cho sự kiện click vào nút IN DANH SÁCH NGUYÊN LIỆU tại tab IN DANH SÁCH
    $('#btnModalNguyenLieuKiemKho').click(function (e) {
        $.ajax({
            url: getDuongDan() + 'TonKho/AjaxLayDanhSachNguyenLieuCanKiem',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
                $('#btnModalNguyenLieuKiemKho').unbind('click');
            },
            success: function (data) {
                $('#phieuNguyenLieuKiem').html(data);
                $('#modalInNguyenLieuKiem').modal('show');
                bindData();
            },
            error: function (data) {
                bindData();
            }
        });
    });


    //------SỰ KIỆN LẤY DANH SÁCH CÁC NGUYÊN LIỆU KIỂM KÊ CHO VÙNG NHẬP SỐ LIỆU
    $('#js-tab-nhapsolieu').click(function (e) {
        $.ajax({
            url: getDuongDan() + 'TonKho/AjaxLayNguyenLieuKiemKho',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('#js-tab-nhapsolieu').unbind('click');
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                //--Hiện các dòng dữ liệu lên bảng
                $('#danhSachTruocKiem').html("");
                $('#danhSachTruocKiem').html(data);
                $('.page-loader-wrapper').attr('style', 'display:none');
                displayContentIntabs('#js-tab-nhapsolieu', '#nhapsolieu');
            },
            error: function (xhr, textStatus, errorThrown) {
                bindData();
            }
        });
    });




    //--------HÀM XỬ LÝ NGHIỆP VỤ NHẬP SỐ LƯỢNG NGUYÊN LIỆU KIỂM KHO
    $("#danhSachTruocKiem").on("click", ".btnKiemHang", function (event) {
        var maNguyenLieu = $(this).attr('manl');
        var soLuongThucTe = $('#txtSoLuongThucTe' + maNguyenLieu).val();
        var nguyenNhan = $('#txtNguyenNhan' + maNguyenLieu).val();
        var ts = maNguyenLieu + "|" + soLuongThucTe + "|" + nguyenNhan;

        $.ajax({
            url: getDuongDan() + 'TonKho/AjaxSuKienKiemHang',
            data: 'param=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (rows, textStatus, xhr) {
                //--Hiện các dòng dữ liệu lên bảng
                $('#danhSachTruocKiem').html("");
                $('#danhSachTruocKiem').html(rows);
                $('.page-loader-wrapper').attr('style', 'display:none');
                bindData();
            },
            error: function (xhr, textStatus, errorThrown) {
                bindData();
            }
        });
    });




    //------Hàm lấy danh sách các nguyên liệu đã kiểm
    $('#js-tab-doichieu').click(function (e) {
        $.ajax({
            url: getDuongDan() + 'TonKho/AjaxDoiChieuSoLuong',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('#js-tab-doichieu').unbind('click');
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                //--Hiện các dòng dữ liệu lên bảng
                $('#danhSachDaKiem').html("");
                $('#danhSachDaKiem').html(data);
                $('.page-loader-wrapper').attr('style', 'display:none');
                displayContentIntabs('#js-tab-doichieu', '#doichieu');

            },
            error: function (xhr, textStatus, errorThrown) {
                bindData();
            }
        });
    });




    //--------HÀM XỬ LÝ NGHIỆP VỤ YÊU CẦU KIỂM LẠI NGUYÊN LIỆU
    $("#danhSachDaKiem").on("click", ".btnKiemLai", function (event) {
        var ts = $(this).attr('manl');
        $.ajax({
            url: getDuongDan() + 'TonKho/AjaxSuKienKiemLai',
            data: 'param=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (rows, textStatus, xhr) {
                //--Hiện các dòng dữ liệu lên bảng
                $('#danhSachDaKiem').html("");
                $('#danhSachDaKiem').html(rows);
                $('.page-loader-wrapper').attr('style', 'display:none');
                bindData();
            },
            error: function (xhr, textStatus, errorThrown) {
                bindData();
            }
        });
    });
}