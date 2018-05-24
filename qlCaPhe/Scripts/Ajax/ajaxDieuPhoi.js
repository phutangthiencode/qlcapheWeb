$(document).ready(function () {
    thucThi();
    //window.onbeforeunload = function (event) {
    //    return confirm("Confirm refresh");
    //};


    // slight update to account for browsers not supporting e.which
    function disableF5(e) { if ((e.which || e.keyCode) == 116) e.preventDefault(); };
    // To disable f5
    /* jQuery < 1.7 */
    $(document).bind("keydown", disableF5);
    /* OR jQuery >= 1.7 */
    $(document).on("keydown", disableF5);

    // To re-enable f5
    /* jQuery < 1.7 */
    $(document).unbind("keydown", disableF5);
    /* OR jQuery >= 1.7 */
    $(document).off("keydown", disableF5);
});

function thucThi() {
    //---------Sự kiện click vào nút Điều Phối để thêm 1 ca làm việc vào bảng công tác cho nhân viên
    $('#js-btnDieuPhoi').click(function (e) {
        var ts = $('#cbbCaLamViec').val() + "|" + $('#txtGhiChuCt').val();
        $.ajax({
            url: getDuongDan() + 'BangCongTac/AjaxThemMotCaLamViecVaoDieuPhoi',
            data: 'param=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                $('#js-btnDieuPhoi').unbind('click');
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                //--Hiện các dòng dữ liệu lên bảng
                $('#vungChiTiet').html("");
                $('#vungChiTiet').html(data);
                $('.page-loader-wrapper').attr('style', 'display:none');
                $('#txtGhiChuCt').val("");
                thucThi();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThi();
            }
        });
    });

    //-------Sự kiện click vào nút bỏ ca trên danh sách để xóa bỏ 1 ca làm việc khỏi giỏ
    $("#vungChiTiet").one("click", ".btnBoCa", function (e) {
        var ts = $(this).attr('maca');
        $.ajax({
            url: getDuongDan() + 'BangCongTac/AjaxXoaBoCaDaChon',
            data: 'param=' + ts,
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
                //--Hiện các dòng dữ liệu lên bảng
                $('#vungChiTiet').html("");
                $('#vungChiTiet').html(data);
                $('.page-loader-wrapper').attr('style', 'display:none');
                thucThi();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThi();
            }
        });
    });

    $("#js-btnXoaTatCaDieuPhoi").click(function (e) {
        $.ajax({
            url: getDuongDan() + 'BangCongTac/AjaxXoaTatCaCaLamViecDaChon',
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                $('#js-btnXoaTatCaDieuPhoi').unbind('click');
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.page-loader-wrapper').attr('style', 'display:block');
            },
            success: function (data, textStatus, xhr) {
                //--Hiện các dòng dữ liệu lên bảng
                $('#vungChiTiet').html("");
                $('#vungChiTiet').html(data);
                $('.page-loader-wrapper').attr('style', 'display:none');
                thucThi();
            },
            error: function (xhr, textStatus, errorThrown) {
                thucThi();
            }
        });
    });
}