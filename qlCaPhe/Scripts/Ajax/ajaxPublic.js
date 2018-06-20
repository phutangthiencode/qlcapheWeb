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
                $('#js-vung-modal').html("");
                $('#js-vung-modal').html(data);
                $('#modal-detail-table').modal('show');
                executeBus();
            },
            error: function (xhr, textStatus, errorThrown) {
                executeBus();
            }
        });
    });
}