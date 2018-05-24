$(document).ready(function () {
    //------Ẩn khu vực nhập số lượng và đơn vị sử dụng
    $('.showSuccess').attr('style', 'display:none');
    ajaxTimNguyenLieu();
    $('#btnModalChonNguyenLieu').click(function () {
        $('#vungDSNguyenLieu').empty();
        $('#txtTenNguyenLieuTimKiem').val("");
        $('#txtTenNguyenLieuTimKiem').attr('autofocus');
    });

    ajaxChonNguyenLieu();
    ajaxThemChiTiet();
    ajaxXoaTatCaChiTiet();
    ajaxXoaMotChiTiet();
});
//-----Hàm thực hiện bắt các sự kiện
function ajaxTimNguyenLieu() {
    //--Sự kiện click vào nút tìm kiếm
    $('#btnTimKiem').click(function (e) {
        thucThiTimKiem(e);
    });
    //---Sự kiện nhấn phím enter trên textbox tên sản phẩm
    $('#txtTenNguyenLieuTimKiem').keypress(function (e) {
        if (e.which == 13)
            thucThiTimKiem(e);
    });
}
//Hàm thực hiện ajax 
function thucThiTimKiem(e) {
    var ts = $('#txtTenNguyenLieuTimKiem').val();
    $.ajax({
        url: getDuongDan() + 'XuatKho/AjaxTimKiemNguyenLieuTonKho?tenNL=' + ts,
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            $('#btnTimKiem').unbind('click');
            e.preventDefault();
            e.stopImmediatePropagation();
        },
        success: function (data, textStatus, xhr) {
            $('#vungDSNguyenLieu').html(data);
            ajaxTimNguyenLieu();
        },
        error: function (xhr, textStatus, errorThrown) {
            ajaxTimNguyenLieu();
        }
    });
}
//------Thực hiện ajax lấy thông tin nguyên liệu khi đã chọn trên modal
function ajaxChonNguyenLieu() {
    //---Sự kiện cho class click itemNguyenLieu sẽ hiễn thị dữ liệu
    $("#vungDSNguyenLieu").on("click", ".itemNguyenLieu", function (event) {
        var ts = $(this).attr('manl');
        $.ajax({
            url: getDuongDan() + 'NhapKho/layNguyenLieuModal?maNL=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('.itemNguyenLieu, media-left').unbind('click');
                //---Sự kiện unbind không click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                $('#vungNguyenLieu').html(data);
                //-------Hiện khu vực nhập số lượng và đơn vị
                $('.showSuccess').attr('style', 'display:block');
                $('#largeModalChonNguyenLieu').modal('hide');
                ajaxChonNguyenLieu();
            },
            error: function (xhr, textStatus, errorThrown) {
                ajaxChonNguyenLieu();
            }
        });
    });

}
//------Thực hiện ajax thêm mới chi tiết phiếu xuất kho khi click vào nút Thêm vào phiếu
function ajaxThemChiTiet() {
    $('#btnThemHangVaoPhieu').click(function () {
        //------Lấy dữ liệu từ giao diện
        var ts = $('#maNguyenLieuDaChon').val() +'|' + $('#txtSoLuongNhap').val() + '|' + $('#txtDonGiaNhap').val() + '|' + $('#txtGhiChuCt').val();
        $.ajax({
            url: getDuongDan() + 'XuatKho/AjaxThemChiTietVaTraVeBang?duLieu=' + ts,
            type: 'POST',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('#btnThemHangVaoPhieu').unbind('click');
            },
            success: function (data, textStatus, xhr) {
                $('#vungDsChiTiet').empty();
                $('#vungDsChiTiet').html(data.split('|')[0]);
                $('#txtTongTien').val(data.split('|')[1]);
                $('#btnXoaTatCa').attr('style', 'display:block');
                resetDuLieuChiTiet();
                ajaxThemChiTiet();
            },
            error: function (xhr, textStatus, errorThrown) {
                ajaxThemChiTiet();
            }
        });
    });
}
//---hàm thực hiện reset lại dữ liệu cho form chi tiết
function resetDuLieuChiTiet() {
    $('#maNguyenLieuDaChon').val("");
    $('#txtSoLuongNhap').val("");
    $('#txtDonGiaNhap').val("");
    $('#txtGhiChuCt').val("");
    $('#vungNguyenLieu').empty();
}

//------Thực hiện ajax thêm mới chi tiết công thức khi click vào nút Thêm vào công thức
function ajaxXoaTatCaChiTiet() {
    $('#btnXoaTatCa').click(function () {
        $.ajax({
            url: getDuongDan() + 'XuatKho/AjaxXoaTatCaChiTiet',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('#btnXoaTatCa').unbind('click');
            },
            success: function (data, textStatus, xhr) {
                $('#vungDsChiTiet').empty();
                $('#btnXoaTatCa').attr('style', 'display:none');
                resetDuLieuChiTiet();
                ajaxXoaTatCaChiTiet();
            },
            error: function (xhr, textStatus, errorThrown) {
                ajaxXoaTatCaChiTiet();
            }
        });
    });
}
//-----Hàm thực hiện ajax xóa một nguyên liệu đã chọn trong session
function ajaxXoaMotChiTiet() {
    $("#vungDsChiTiet").on("click", ".xoaChiTiet", function (event) {
        var param = $(this).attr('maCt');
        $.ajax({
            url: getDuongDan() + 'XuatKho/AjaxXoaMotChitiet?maCt=' + param,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('.xoaChiTiet').unbind('click');
            },
            success: function (data, textStatus, xhr) {
                $('#vungDsChiTiet').empty();
                $('#vungDsChiTiet').html(data.split('|')[0]);
                ajaxXoaMotChiTiet();
            },
            error: function (xhr, textStatus, errorThrown) {
                ajaxXoaMotChiTiet();
            }
        });
    });
}
//-------Thực hiện ajax lấy danh sách chi tiết có trong session và hiện lên giao diện phục vụ cho chức năng chỉnh sửa công thức
function ajaxDoChiTietKhiSua() {
    var maCT = $('#txtMaPhieu').val();
    if (maCT > 0) {
        $.ajax({
            url: getDuongDan() + 'NhapKho/taoBangChiTietTuSession', //--Lấy tất cả dữ liệu có trong session. Vì trong session là danh sách chi tiết công thức 
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $(document).unbind('ready');
            },
            success: function (data, textStatus, xhr) {
                $('#vungDsChiTiet').empty();
                $('#vungDsChiTiet').html(data);
                resetDuLieuChiTiet();
            },
            error: function (xhr, textStatus, errorThrown) {
                ajaxDoChiTietKhiSuaCongThuc();
            }
        });
    }
    else return;
}