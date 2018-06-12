$(document).ready(function (e) {
    loadMucTieu(e);
    loadDanhGia(e);
    bindData();
});
//------------Hàm gọi ajax load danh sách mục tiêu chưa đánh giá
function loadMucTieu(e) {
    $.ajax({
        url: getDuongDan() + 'DanhGia/AjaxLayDanhSachMucTieu',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            //---Sự kiện unbind không click vào item nữa
            event.preventDefault();
            event.stopImmediatePropagation();
            $('#js-chuaDanhGia').attr('style', 'display:none');
            $('.page-loader-wrapper').attr('style', 'display:block');
        },
        success: function (data) {
            if (data != "") {
                $('#js-listMucTieu').html(data);
                $('#js-chuaDanhGia').attr('style', 'display:block');
            }
            $('.page-loader-wrapper').attr('style', 'display:none');
        },
        error: function (data) {
        }
    });
}

//------------Hàm gọi ajax load danh sách mục tiêu chưa đánh giá
function loadDanhGia(e) {
    $.ajax({
        url: getDuongDan() + 'DanhGia/taoBangMucTieuDaDanhGia',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            event.preventDefault();
            event.stopImmediatePropagation();
            $('#js-cartDanhGia').attr('style', 'display:none');
            $('.page-loader-wrapper').attr('style', 'display:block');
        },
        success: function (data) {
            if (data != "") {
                //--Hiện các mục tiêu đã đánh giá
                $('#js-cartDanhGia').attr('style', 'display:block');
                $('#js-listDanhGia').html(data);
            }
            $('.page-loader-wrapper').attr('style', 'display:none');
        },
        error: function (data) {
        }
    });
}

function bindData() {

    //---------Hàm danh cho sự kiện click vào nút Đánh Giá thực hiện thêm đánh giá vào giỏ DaDanhGia
    $("#js-listMucTieu").on("click", ".js-btnDanhGia", function (e) {
        var maMucTieu = $(this).attr('mamt');
        var diemSo = parseInt($('#txtDiemSo' + maMucTieu).val());
        var dienGiai = $('#txtDienGiai' + maMucTieu).val();
        var param = maMucTieu + "|" + diemSo + "|" + dienGiai;
        $.ajax({
            url: getDuongDan() + 'DanhGia/AjaxDanhGiaNhanVien',
            data: 'param=' + param,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function (xhr, opts) {
                e.preventDefault();
                e.stopImmediatePropagation();
                if (diemSo < 0 || diemSo===isNaN('NaN') || dienGiai === "") {
                    showToastNotify("#notify");
                    xhr.abort();
                    return false;
                }
                beforedSendDanhGia(e);
            },
            success: function (data, textStatus, xhr) {
                successDanhGia(data);
                bindData();

            },
            error: function (xhr, textStatus, errorThrown) {
                bindData();
            }
        });
     
    });


    //---------Hàm danh cho sự kiện click vào nút Đánh Giá lại thực hiện xóa đánh giá khỏi giỏ DaDanhGia
    $("#js-listDanhGia").on("click", ".js-btnDanhGiaLai", function (e) {
        var maMucTieu = $(this).attr('mamt');
        $.ajax({
            url: getDuongDan() + 'DanhGia/AjaxDanhGiaLai',
            data: 'param=' + maMucTieu,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function (xhr, opts) {
                beforedSendDanhGia(e);
            },
            success: function (data, textStatus, xhr) {
                successDanhGia(data);
                bindData();

            },
            error: function (xhr, textStatus, errorThrown) {
                $('#js-cartDanhGia').attr('style', 'display:none');
                bindData();
            }
        });

    });


    //---------Hàm danh cho sự kiện click vào nút Đánh Giá lại tất cả thực hiện xóa tất cả đánh giá khỏi giỏ DaDanhGia
    $('#js-btnXoaTatCa').click(function(e){
        $.ajax({
            url: getDuongDan() + 'DanhGia/AjaxXoaTatCaDanhGia',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function (xhr, opts) {
                beforedSendDanhGia(e);
            },
            success: function (data, textStatus, xhr) {
                successDanhGia(data);
                bindData();

            },
            error: function (xhr, textStatus, errorThrown) {
                $('#js-cartDanhGia').attr('style', 'display:none');
                bindData();
            }
        });

    });

}
//-------Hàm xử lý các sự kiện trước khi gửi ajax
function beforedSendDanhGia(e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    $('#js-chuaDanhGia').attr('style', 'display:none');
    $('#js-cartDanhGia').attr('style', 'display:none');
    $('.page-loader-wrapper').attr('style', 'display:block');
}

//-----------HÀM XỬ LÝ SAU KHI AJAX THÀNH CÔNG
//===============data = rowChuaDanhGia|rowDaDanhGia
function successDanhGia(data) {
    var rowsChuaDanhGia = data.split('|')[0];
    var rowsDaDanhGia = data.split('|')[1];
    $('#js-listMucTieu').html("");
    $('#js-listDanhGia').html("");
    if (rowsChuaDanhGia != "") {
        //--Hiện các mục tiêu chưa đánh giá
        $('#js-chuaDanhGia').attr('style', 'display:block');
        $('#js-listMucTieu').html(rowsChuaDanhGia);
    }
    if (rowsDaDanhGia != "") {
        //--Hiện các mục tiêu đã đánh giá
        $('#js-cartDanhGia').attr('style', 'display:block');
        $('#js-listDanhGia').html(rowsDaDanhGia);
    }
    $('.page-loader-wrapper').attr('style', 'display:none');    
    bindData();
}

