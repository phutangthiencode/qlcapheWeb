$(document).ready(function () {
    validateNgayNhoHonHienTai();
});
 
function validateNgayNhoHonHienTai() {
    //------Script thực hiện chỉ nhận ngày tháng nhỏ hơn ngày hiện tại.
    //------Dành riêng cho các đối tượng có class là ngayValidate
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd
    }
    if (mm < 10) {
        mm = '0' + mm
    }
    today = yyyy + '-' + mm + '-' + dd;
    $('.ngayValidate').attr('max', today);
}
