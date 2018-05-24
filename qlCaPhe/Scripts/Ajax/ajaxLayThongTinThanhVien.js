
$(document).ready(function () {
    bindingCommand();
});
function bindingCommand() {
    $('#cbbThanhVien').change(function () {
        //Lấy mã thành viên
        var ts = $(this).val();
        $.ajax({
            url: getDuongDan() + 'ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=' + ts,
            beforeSend: function () {

            },
            success: function (data, textStatus, xhr) {
                $('#vungThongTin').html(data);
                bindingCommand();
            },
            error: function (xhr, textStatus, errorThrown) {
                bindingCommand();
            }
        });
        return false;
    });
}