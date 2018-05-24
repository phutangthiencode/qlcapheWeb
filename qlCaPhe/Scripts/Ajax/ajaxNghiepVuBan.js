$(document).ready(function () {
    xuLyNghiepVuBan();
    //------Nhóm sự kiện cho danh sách bàn
    suKienLietKeDanhMucBan();
    //-----Nhóm sự kiện cho danh sách sản phẩm
    sukienTimKiemSanPham();
    
});

//--------Hàm xử lý các sư kiện xảy ra trên giao diện
function xuLyNghiepVuBan() {    

    //-----Sự kiện click vào nút gọi món sẽ reset lại dữ liệu trên modal
    $("#vungDanhSachBan").one("click", ".goiMon", function (e) {
        var maBan = $(this).attr('maBan');
        openModal(maBan, e);

    });

    //============SỰ KIỆN CHO NÚT TIẾP NHẬN BÀN HOẶC CHUYỂN ĐỔI BÀN============
    $("#vungDanhSachBan, #vungDanhSachBanModalChuyenDoi").one("click", ".tiepNhan", function (e) {
        var ts = $(this).attr('maBan') + '|' + $('#maBanCuCanDoi').val(); //ts=maBan|maBanCu
        $.ajax({
            url: getDuongDan() + 'NghiepVuBan/AjaxTiepNhanBan?param=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                //----Ẩn modal đổi bàn
                $('#modalDoiBan').modal('hide');
                //------Cập nhật lại danh sách bàn 
                layDanhSachBan(e, $('#cbbKhuVuc').val() + "|" + $('.rdbTrangThai:checked').val(), 'vungDanhSachBan');
                $('.page-loader-wrapper').attr('style', 'display:none');
                xuLyNghiepVuBan();
            },
            error: function (xhr, textStatus, errorThrown) {
                xuLyNghiepVuBan();
            }
        });
    });

    //=======================SỰ KIỆN CẬP NHẬT SESSION GIỎ HÀNG============
    //-----------------Sự kiện THÊM hàng mới vào giỏ
    $("#dsSanPhamModal").one("click", ".btnChonSP", function (e) {
        var maSP = $(this).attr('maSP');
        AjaxUpdateCartOrder('AjaxThemSanPhamVaoGio', maSP, e);
    });
    //-----------------Sự kiện XÓA BỎ 1 sản phẩm khỏi giỏ
    $("#cartGoiDoUong").on("click", ".btnRemoveCart", function (e) {
        var maSP = $(this).attr('maCt');
        AjaxUpdateCartOrder('AjaxXoaMotSanPhamKhoiGio', maSP, e);
    });
    //-----------------Sự kiện CẬP NHẬT SỐ LƯỢNG  của 1 sản phẩm trong giỏ
    $("#cartGoiDoUong").on("click", ".btnUpdateCart", function (e) {
        var maSP = $(this).attr('maCt');
        var soLuong = $('#txtSoLuongS' + maSP).val(); //-----Gán maSP để xác định textbox số lượng cần cập nhật
        var ts = maSP + '|' + soLuong;
        if (soLuong > 0)  //-------Kiểm tra số lượng trước khi cập nhật
            AjaxUpdateCartOrder('AjaxUpdateSoLuongSanPhamTrongGio', ts, e);
        else
            $('#txtSoLuong' + maSP).val("1"); //-----ngược lại thiết lập số lượng số nhỏ nhất là 1
    });
    //-------Sự kiện nhấn click vào nút Đóng lại trên modal order thì sẽ xóa bỏ session đã order trước đó
    $('#btnCancelOrder').click(function (e) {
        $.ajax({
            url: getDuongDan() + 'NghiepVuBan/AjaxXoaTatCaPhanTuCart',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                //-------Ẩn modal order
                $('#modalChonDoUong').modal('hide');

            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
    });

    //==========================Nhóm CẬP NHẬT VÀO BẢNG ctHoaDonTam DATABASE=============
    //-----------------------------Sự kiện thêm mới hóa đơn tạm----------------------
    $('#btnSave').click(function (e) {
        var maBan = $(this).attr('maBan');
        var ghiChu = $('#txtGhiChu').val();
        var ts = maBan + '|' + ghiChu;
        $.ajax({
            url: getDuongDan() + 'NghiepVuBan/AjaxThemChiTietHoaDon',
            data: 'param='+ts,
            type: 'POST',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                //---------Load lại danh sách bàn---------------
                layDanhSachBan(e, $('#cbbKhuVuc').val() + "|" + $('.rdbTrangThai:checked').val(), 'vungDanhSachBan');
                $('#modalChonDoUong').modal('hide');
                vaoTrangCoTruyenThamSoMaHoa(e);
                xuLyNghiepVuBan();
                $('.page-loader-wrapper').attr('style', 'display:none');
            },
            error: function (xhr, textStatus, errorThrown) {
                xuLyNghiepVuBan();
            }
        });
    });
    //-----Sự kiện cho nút chức năng gọi thêm món tại bàn đã order. ĐỖ DỮ LIỆU TỪ BẢNG CHI TIẾT LÊN GIAO DIỆN
    $("#vungDanhSachBan").one("click", ".goiThem", function (e) {
        var maBan = $(this).attr('maBan');
        openModal(maBan, e);
        AjaxReadAndUpdateTableCtHoaDonTam('AjaxDoChiTietLenView', maBan, e);
    });
    //-----Sự kiện cập nhật số lưởng nguyên liệu thẳng vào database.
    $("#listChiTiet").on("click", ".btnUpdateDtb", function (e) {
        var maCt = $(this).attr('maCt');
        var soLuong = $('#txtSoLuong' + maCt).val();
        var ts = maCt + '|' + soLuong;
        if (soLuong > 0)  //-------Kiểm tra số lượng trước khi cập nhật
            AjaxReadAndUpdateTableCtHoaDonTam('AjaxUpdateSoLuongSanPhamTrongDatabase',ts,e);
        else
            $('#txtSoLuong' + maSP).val("1"); //-----ngược lại thiết lập số lượng số nhỏ nhất là 1
    });
    //-----Sự kiện xóa một record trong bảng ctHoaDonTam trong DATABASE
    $("#listChiTiet").on("click", ".btnRemoveDtb", function (e) {
        var maCt = $(this).attr('maCt');
        AjaxReadAndUpdateTableCtHoaDonTam('AjaxXoaMotChiTietTam', maCt, e);
    });

    //-----Sự kiện cho nút thanh toán. HIỆN MODAL CHECKOUT
    $("#vungDanhSachBan").on("click", ".thanhToan", function (e) {
        var maBan = $(this).attr('maBan');
        //-----Gán mã bàn cho thuộc tính của nút thanh toán
        $('#btnThanhToan').attr('maBan', maBan);
        $.ajax({
            url: getDuongDan() + 'NghiepVuBan/AjaxHienCheckout',
            data: 'maBan=' + maBan,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                $('#vungCheckout').empty();
                $('#vungCheckout').html(data);
                $('#modalCheckout').modal('show');
                xuLyNghiepVuBan();
            },
            error: function (xhr, textStatus, errorThrown) {
                xuLyNghiepVuBan();
            }
        });
    });

    //-----Sự kiện đẩy hóa đơn vào thu ngân thanh toán. CẬP NHẬT TRẠNG THÁI THÀNH CHỜ THANH TOÁN
    $('#btnThanhToan').click(function (e) {
        var maBan = $(this).attr('maBan');
        $.ajax({
            url: getDuongDan() + 'NghiepVuBan/AjaxCapNhatChoThanhToan',
            data: 'maBan=' + maBan,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
                $('#btnThanhToan').unbind('click');
                //------Hiện reload
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                $('#modalCheckout').modal('hide');
                layDanhSachBan(e, $('#cbbKhuVuc').val() + "|" + $('.rdbTrangThai:checked').val(), 'vungDanhSachBan');
                xuLyNghiepVuBan();
            },
            error: function (xhr, textStatus, errorThrown) {
                xuLyNghiepVuBan();
            }
        });
    });
    //-----Sự kiện dọn bàn khi khách rời khỏi
    $("#vungDanhSachBan").on("click", ".resetBan", function (e) {
        var maBan = $(this).attr('maBan');
        $.ajax({
            url: getDuongDan() + 'NghiepVuBan/AjaxResetBan',
            data: 'maBan=' + maBan,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                layDanhSachBan(e, $('#cbbKhuVuc').val() + "|" + $('.rdbTrangThai:checked').val(), 'vungDanhSachBan');
                resetBan();
            },
            error: function (xhr, textStatus, errorThrown) {
                resetBan();
            }
        });
    });
    //===============Hàm mở modal khi có nhu cầu đổi chổ----------------
    $("#vungDanhSachBan").on("click", ".doiCho", function (e) {
        var maBan = $(this).attr('maBan');
        //------Gán dữ liệu cho textbox ẩn để lưu lại mã bàn cũ cần đổi
        $('#maBanCuCanDoi').val(maBan);
        //---------Load lại danh sách bàn---------------
        layDanhSachBan(e, $('#cbbKhuVucModal').val() +"|"+ -1, 'vungDanhSachBanModalChuyenDoi');
        $('#modalDoiBan').modal('show');
    });
}



//-------Hàm hiện danh sách bàn và gán lên 1 vùng trên giao diện
function layDanhSachBan(e, ts, idVungDanhSachBan) {
    $.ajax({
        url: getDuongDan() + 'NghiepVuBan/AjaxLoadDanhSachBan',
        data: "param=" + ts,
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            e.preventDefault();
            e.stopImmediatePropagation();
            $('.rdbTrangThai').unbind('change');
            $('.page-loader-wrapper').attr('style', 'display:block');
        },
        success: function (data, textStatus, xhr) {
            $('#' + idVungDanhSachBan).empty();
            $('#' + idVungDanhSachBan).html(data);
            $('.page-loader-wrapper').attr('style', 'display:none');
            suKienLietKeDanhMucBan();
        },
        error: function (xhr, textStatus, errorThrown) {
            suKienLietKeDanhMucBan();
        }
    });
}

//-------Hàm reset lại dữ liệu khi modal được mở lên
//------classItemClick: Tên class item chức năng mà người dùng click vào goiMon hoặc goiThem
function openModal(maBan, e) {4
    $('#dsSanPhamModal').empty();//----Xóa danh sách sản phẩm trên modal
    $('#cartGoiDoUong').empty();//----Xóa đồ uống đã chọn trước đó trong session
    $('#listChiTiet').empty();//----Xóa đồ uống đã chọn trước đó trong database
    $('#cbbLoaiSP').val(0);//-----Gán lại giá trị index đầu tiên cho cbb
    $('#txtTimKiemSP').val(""); //----xóa dữ liệu trên textbox tìm kiếm
    $('#btnSave').attr('maBan', maBan); //-----Gán các tham số vào thuộc tính cho nút save để xác định hóa đơn tạm để lưu
    $('#txtGhiChu').val();
    $.ajax({
        url: getDuongDan() + 'NghiepVuBan/AjaxLaySanPhamTheoLoai',
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            e.preventDefault();
            e.stopImmediatePropagation();
            $('.goiMon').unbind('click');
            $('.goiThem').unbind('click');
            $('.page-loader-wrapper').attr('style', 'display:block');
        },
        success: function (data, textStatus, xhr) {
            $('#dsSanPhamModal').empty();
            $('#dsSanPhamModal').html(data);
            $('.page-loader-wrapper').attr('style', 'display:none');
            $('#modalChonDoUong').modal('show');
            xuLyNghiepVuBan();
        },
        error: function (xhr, textStatus, errorThrown) {
            xuLyNghiepVuBan();
        }
    });
}

//-----Hàm dành cho các sự kiện khi người dùng chọn 1 item trên cbbKhuVuc hoặc trên cbbTrangThai
function suKienLietKeDanhMucBan() {
    //----Sự kiện chọn 1 khu vực
    $('#cbbKhuVuc, .rdbTrangThai').one('change', function (e) {
        layDanhSachBan(e, $('#cbbKhuVuc').val() + "|"+ $('.rdbTrangThai:checked').val(), 'vungDanhSachBan'); //----$('.rdbTrangThai:checked').val() lấy giá trị của radio đã select
    });

    $('#btnLietKe').click(function (e) {
        layDanhSachBan(e, $('#cbbKhuVuc').val() + "|" + $('.rdbTrangThai:checked').val(), 'vungDanhSachBan'); 
    });

    //----Sự kiện để chọn một khu vực trên modal. LẤY DANH SÁCH BÀN CÓ TRẠNG THÁI TRỐNG 
    $('#cbbKhuVucModal').one('change', function (e) {
        layDanhSachBan(e, $('#cbbKhuVucModal').val() +"|"+ -1, 'vungDanhSachBanModalChuyenDoi');
    });
    $('#btnLietKeModal').click(function (e) {
        layDanhSachBan(e, $('#cbbKhuVucModal').val() + "|" + -1, 'vungDanhSachBanModalChuyenDoi');
    });
}

//--------Hàm thực hiện thiết lập các sự kiện để tìm kiếm sản phẩm
function sukienTimKiemSanPham() {
    $('#btnSearch').click(function (e) {
        AjaxLayDanhSachSanPham('AjaxTimKiemSanPham', 'txtTimKiemSP');
    });

    //---Sự kiện nhấn phím enter trên textbox tên sản phẩm
    $('#txtTimKiemSP').keypress(function (e) {
        if (e.which == 13)
            AjaxLayDanhSachSanPham('AjaxTimKiemSanPham', 'txtTimKiemSP');
    });

    $('#cbbLoaiSP').one('change', function (e) {
        AjaxLayDanhSachSanPham('AjaxLaySanPhamTheoLoai', 'cbbLoaiSP');
    });
}

//---------Ajax lấy danh sách sản phẩm theo LOẠI hoặc theo TÊN SP
//==================action: Action thực hiện lấy danh sách trong Controller===============
//==================idValue: id của tag lấy dữ liệu tìm kiếm
function AjaxLayDanhSachSanPham(action, idValue) {
    var ts = $('#' + idValue).val();
    $.ajax({
        url: getDuongDan() + 'NghiepVuBan/' + action,
        data: 'param=' + ts,
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            //---Sự kiện unbind không click vào item nữa
            event.preventDefault();
            event.stopImmediatePropagation();
        },
        success: function (data, textStatus, xhr) {
            $('#dsSanPhamModal').empty();
            $('#dsSanPhamModal').html(data);
            sukienTimKiemSanPham();
        },
        error: function (xhr, textStatus, errorThrown) {
            sukienTimKiemSanPham();
        }
    });
}


//==============Hàm ajax cho nghiệp vụ cập nhật giỏ hàng order của khách==========
//------------------action: Tên phương thức thực hiện trong controller----------------
//------------------ts: Giá trị tham số truyên vào action phương thức-----------------
//------------------e: Event...........
function AjaxUpdateCartOrder(action, ts, e) {
    $.ajax({
        url: getDuongDan() + 'NghiepVuBan/' + action,
        data: 'param=' + ts,
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            //---Sự kiện unbind không click vào item nữa
            e.preventDefault();
            e.stopImmediatePropagation();
        },
        success: function (data, textStatus, xhr) {
            $('#cartGoiDoUong').empty();
            $('#cartGoiDoUong').html(data);//------Lấy chuỗi html tạo nên bảng
            xuLyNghiepVuBan();
        },
        error: function (xhr, textStatus, errorThrown) {
            xuLyNghiepVuBan();
        }
    });
}


//======================HÀM ĐỌC DỮ LIỆU VÀ CẬP NHẬT DỮ LIỆU VÀO BẢNG ctHoaDonTam TRONG DATABASE
//------------------action: Tên phương thức thực hiện có trong controller
//------------------ts: Tham số truyền vào phương thức controller
//------------------e: Sự kiện..........
function AjaxReadAndUpdateTableCtHoaDonTam(action, ts, e) {
    $.ajax({
        url: getDuongDan() + 'NghiepVuBan/' + action,
        data: 'param=' + ts,
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            //---Sự kiện unbind không click vào item nữa
            e.preventDefault();
            e.stopImmediatePropagation();
            
        },
        success: function (data, textStatus, xhr) {
            $('#listChiTiet').empty();
            $('#listChiTiet').html(data.split('|')[0]);
            $('#txtGhiChu').val(data.split('|')[1]);
            $('#modalChonDoUong').modal('show');

            xuLyNghiepVuBan();
        },
        error: function (xhr, textStatus, errorThrown) {
            xuLyNghiepVuBan();
        }
    });
}
