$(document).ready(function(){
    executeBus();
});

function executeBus(){
    //-----Sự kiện click vào 1 bàn trên danh sách để mở modal chi tiết bàn
    $("#vungDanhSachBan").one("click", ".js-btn-openDetail", function (e) {
        var ts = $(this).attr('task');
        $.ajax({
            url: getDuongDan() + 'PublicPage/AjaxXemChiTietBan',
            data: 'param='+ ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                $('#js-modal-detail-ban').html("");
                $('#js-modal-detail-ban').html(data);
                $('#modal-detail-table').modal('show');
                executeBus();
            },
            error: function (xhr, textStatus, errorThrown) {
                executeBus();
            }
        });
    });

    //-----Sự kiện click chọn đặt một bàn tại danh mục bàn
    $("#vungDanhSachBan, #js-modal-detail-ban").one("click", ".js-btn-datBan", function (e) {
        var ts = $(this).attr('task');
        $.ajax({
            url: getDuongDan() + 'PublicPage/AjaxDatBan',
            data: 'param=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                $('#js-soluongDat').html(data);
                executeBus();
            },
            error: function (xhr, textStatus, errorThrown) {
                executeBus();
            }
        });
        $('#modal-detail-table').modal('hide');
    });

    //------Sự kiện click vào nút xóa trên bảng bàn đã order
    $("#js-table-ordered").one("click", ".js-btn-xoaban", function (e) {
        var ts = $(this).attr('task');
        $.ajax({
            url: getDuongDan() + 'PublicPage/AjaxXoaOrderBan',
            data: 'param=' + ts,
            type: 'GET',
            context: this,
            dataType: 'html',
            beforeSend: function () {
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                var table = data.split('&&&')[0];
                if (table === "") {
                    window.location.href = getDuongDan() + "PublicPage/DanhSachBan";
                    return;
                }
                var total = data.split('&&&')[1];
                $('#js-table-ordered').html(table);
                $('#js-total-capacity').html(total);
                executeBus();
            },
            error: function (xhr, textStatus, errorThrown) {
                executeBus();
            }
        });
    });


}
