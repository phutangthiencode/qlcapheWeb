$(document).ready(function () {
    thucThiSuKien();
    //-------Sự kiện click mở modal
    $('#btnModalChonNguyenLieu').click(function () {
        $('#vungDSNguyenLieu').empty();
        $('#txtTenNguyenLieuTimKiem').val("");
    });
    ajaxDoChiTietVaNguyenLieuKhiSuaCongThuc();
    doSanPhamLenViewKhiSuaCongThuc();
});




function thucThiSuKien() {
    //===================TÌM KIẾM NGUYÊN LIỆU======================
    //--Sự kiện click vào nút tìm kiếm
    $('#btnTimKiem').click(function (e) {
        thucThiTimKiem(e);
    });
    //---Sự kiện nhấn phím enter trên textbox tên sản phẩm
    $('#txtTenNguyenLieuTimKiem').keypress(function (e) {
        if (e.which == 13)
            thucThiTimKiem(e);
    });


    //==========Sự kiện click vào class itemNguyenLieu trên modal sẽ hiễn thị dữ liệu 
    $("#vungDSNguyenLieu").on("click", ".itemNguyenLieu", function (event) {
        var ts = $(this).attr('manl');
        $.ajax({
            url: getDuongDan() + 'CongThuc/layNguyenLieuModal?maNL=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không thực hiện click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                //------Gán nội dung của nguyên liệu đã chọn trên modal lên giao diện
                $('#vungNguyenLieu').html(data.split('|')[0]);
                //------Gán tổng số tiền từ các nguyên liệu
                $('#tongTienNguyenLieu').val(data.split('|')[1]);
                //-------Hiện khu vực nhập số lượng và đơn vị
                $('.showSuccess').attr('style', 'display:block');
                $('#largeModalChonNguyenLieu').modal('hide');
                thucThiSuKien();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThiSuKien();
            }
        });
    });

    //=================SỰ KIỆN CHO NÚT THÊM VÀO CÔNG THỨC==========
    $('#btnThemVaoCongThuc').click(function (event) {
        if (jQuery.trim($('#txtHanhDong').val()).length > 0) { //----Kiểm tra nếu có hành động thì mới được thêm
            //------Lấy dữ liệu từ giao diện
            var ts = $('#maNguyenLieuDaChon').val() + '|' + $('#txtBuocSo').val() + '|' + $('#txtSoLuong').val() + '|' + $('#txtDonVi').val() + '|' + $('#txtHanhDong').val() + '|' + $('#txtGhiChuCt').val();
            var buocSo = $('#txtBuocSo').val();
            $.ajax({
                url: getDuongDan() + 'CongThuc/themChiTietVaTraVeBang?duLieu=' + ts,
                type: 'GET',
                context: this,
                dataType: 'html',
                beforeSend: function () {
                    //---Sự kiện unbind không thực hiện click vào item nữa
                    event.preventDefault();
                    event.stopImmediatePropagation();
                },
                success: function (data, textStatus, xhr) {
                    capNhatCartThanhCong(data);
                    $('#txtBuocSo').val(++buocSo);
                    resetDuLieuChiTiet();
                    thucThiSuKien();
                },
                error: function (xhr, textStatus, errorThrown) {
                    thucThiSuKien();
                }
            });
        }
    });


    //================SỰ KIỆN CHO NÚT XÓA TẤT CẢ CÁC BƯỚC TRONG CÔNG THỨC==========
    $('#btnXoaTatCa').click(function () {
        $.ajax({
            url: getDuongDan() + 'CongThuc/xoaTatCaBuoc',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $('#btnXoaTatCa').unbind('click');
            },
            success: function (data, textStatus, xhr) {
                $('#vungDsChiTiet').empty();
                $('#txtBuocSo').val(1);
                $('#vungTableNguyenLieu').empty();
                $('#tongTienNguyenLieu').text(""); //--------xóa dữ liệu trên textbox tiền gốc
                resetDuLieuChiTiet();
                thucThiSuKien();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThiSuKien();
            }
        });
    });


    //================SỰ KIỆN CHO NÚT XÓA MỘT BƯỚC TRONG DANH SÁCH=================
    $("#vungDsChiTiet").on("click", ".xoaBuoc", function (event) {
        var maBuoc = $(this).attr('maCt');
        $.ajax({
            url: getDuongDan() + 'CongThuc/xoaMotBuoc?maCt=' + maBuoc,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {   
                //---Sự kiện unbind không thực hiện click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                capNhatCartThanhCong(data);
                thucThiSuKien();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThiSuKien();
            }
        });
    });

    //================SỰ  KIỆN CLICK VÀO NÚT SỬA BƯỚC TRÊN DANH SÁCH
    $("#vungDsChiTiet").on("click", ".suaBuoc", function (event) {
        var maBuoc = $(this).attr('maCt');
        $.ajax({
            url: getDuongDan() + 'CongThuc/doDuLieuChiTietLenView?maCt=' + maBuoc,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                resetDuLieuChiTiet();
                //---------Đổ dữ liệu của bước lên giao diện
                $('#txtBuocSo').val(data.split('|')[0]);
                $('#txtHanhDong').val(data.split('|')[1]);
                $('#txtGhiChuCt').val(data.split('|')[2]);
                if (data.split('|')[3] != null) { //---Kiểm tra nếu bước có nguyên liệu
                    $('#txtSoLuong').val(data.split('|')[3]);
                    $('#txtDonVi').val(data.split('|')[4]);
                    $('.showSuccess').attr('style', 'display:block');
                }
                $('#txtMaCT').val(data.split('|')[5]);  //------Gán mã chi tiết 
                $('#vungNguyenLieu').html(data.split('|')[6]);//hiện hình nguyên liệu
                //--Ẩn hiện button cho hợp lý với hành động
                $('#btnThemVaoCongThuc').attr('style', 'display:none');
                $('#btnSuaChiTiet').attr('style', 'display:block');
                thucThiSuKien();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThiSuKien();
            }
        });
    });

    //===============SỰ KIỆN CHO CHỈNH SỬA CHI TIẾT 
    $('#btnSuaChiTiet').click(function (event) {
        //------Lấy dữ liệu từ giao diện
        var ts = $('#maNguyenLieuDaChon').val() + '|' + $('#txtBuocSo').val() + '|' + $('#txtSoLuong').val() + '|' + $('#txtDonVi').val() + '|' + $('#txtHanhDong').val() + '|' + $('#txtGhiChuCt').val();
        var buocSo = $('#txtBuocSo').val();
        var maCT = $('#txtMaCT').val(); //-------Lấy mã chi tiết
        $.ajax({
            url: getDuongDan() + 'CongThuc/chinhSuaBuoc?duLieu=' + ts + '&maCt=' + maCT,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---------unbind nút sửa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                capNhatCartThanhCong(data);
                resetDuLieuChiTiet();
                thucThiSuKien();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThiSuKien();
            }
        });
    });


    //===============SỰ KIỆN CHO NÚT TÍNH TIỀN SẢN PHẨM DỰA VÀO PHẦN TRĂM LỢI NHUẬN MONG MUỐN
    $('#btnTinhPhanTram').click(function () {
        var tienNguyenLieu = parseInt($('#tongTienNguyenLieu').text(), 10);//Lấy Số tiền gốc
        var phanTram = parseInt($('#txtPhanTramLai').val(), 10);//Lấy Số phần trăm
        var kq = tienNguyenLieu + (tienNguyenLieu * phanTram / 100); //------Tính tiền sản phẩm theo tỷ lệ phần trăm mong muốn
        $('#txtDonGiaMongMuon').val(kq);
    });
}


//Hàm thực hiện ajax 
function thucThiTimKiem(e) {
    var ts = $('#txtTenNguyenLieuTimKiem').val();
    $.ajax({
        url: getDuongDan() + 'NguyenLieu/layDanhSachNguyenLieuTimKiem?tenNL=' + ts,
        type: 'GET',
        context: this,
        dataType: 'html',
        beforeSend: function () {
            e.preventDefault;
            e.stopImmediatePropagation();
        },
        success: function (data, textStatus, xhr) {
            $('#vungDSNguyenLieu').html(data);
            thucThiSuKien();
        },
        error: function (xhr, textStatus, errorThrown) {
            thucThiSuKien();
        }
    });
}

//---hàm thực hiện reset lại dữ liệu cho form chi tiết
function resetDuLieuChiTiet() {
    $('#maNguyenLieuDaChon').val("");
    $('#txtSoLuong').val("");
    $('#txtDonVi').val("");
    $('#txtHanhDong').val("");
    $('#txtGhiChuCt').val("");
    $('#vungNguyenLieu').empty();
    //-------Ẩn khu vực nhập số lượng và đơn vị
    $('.showSuccess').attr('style', 'display:none');
    //--Ẩn hiện button cho hợp lý với hành động
    $('#btnThemVaoCongThuc').attr('style', 'display:block');
    $('#btnSuaChiTiet').attr('style', 'display:none');
}
//----Hàm GÁN DỮ LIỆU lên giao diện sau khi THÊM, XÓA, SỬA một Chi tiết thành công
//---data: Chuỗi kết quả trả về sau khi đã ajax
function capNhatCartThanhCong(data) {
    $('#vungDsChiTiet').empty();
    $('#vungDsChiTiet').html(data.split('|')[0]); //-------Hiện danh sách các bước lên bảng
    $('#vungTableNguyenLieu').html(data.split('|')[1]); //----Hiện danh sách nguyên liệu lên bảng
    $('#tongTienNguyenLieu').text(data.split('|')[2]); //-----Hiển tổng tiền nguyên liệu
    $('#txtTongTienNguyenLieu').val(data.split('|')[2]);//---Gán tổng tiền gốc để thêm vào thuộc tính tienGoc của bảng  lichSuGa
}



//-------Thực hiện ajax lấy danh sách chi tiết có trong session và hiện lên giao diện phục vụ cho chức năng chỉnh sửa công thức
function ajaxDoChiTietVaNguyenLieuKhiSuaCongThuc() {
    var maCT = $('#txtMaCongThuc').val();
    if (maCT > 0) {
        $.ajax({
            url: getDuongDan() + 'CongThuc/taoBangChiTietVaNguyenLieuSuDungTuSession', //--Lấy tất cả dữ liệu có trong session. Vì trong session là danh sách chi tiết công thức 
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $(document).unbind('ready');
            },
            success: function (data, textStatus, xhr) {
                capNhatCartThanhCong(data);
                resetDuLieuChiTiet();
            },
            error: function (xhr, textStatus, errorThrown) {
                ajaxDoChiTietKhiSuaCongThuc();
            }
        });
    }
    else return;
}



//========Hàm đổ  thông tin của sản phẩm đã chọn để tạo công thức lên giao diện
function doSanPhamLenViewKhiSuaCongThuc() {
    var ts = $('#cbbDoUong').val();
    if (ts > 0)
        $.ajax({
            url: getDuongDan() + 'DoUong/getInfoDoUongForCreateCongThuc?maDoUong=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                $(document).unbind('ready');
            },
            success: function (data, textStatus, xhr) {
                $('#vungInfoDoUong').html(data);
            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
}
