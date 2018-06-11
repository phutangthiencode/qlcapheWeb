$(document).ready(function (e) {
    vaoTrangCoTruyenThamSoMaHoa(e);
    vaoTrangCoThamSo();

});

function vaoTrangCoTruyenThamSoMaHoa(e) {
    $("body").on("click", ".guiRequest", function (event) {
        var ts = $(this).attr('task');
        $.ajax({
            url: getDuongDan() + 'Ajax/xulyRequestVaGuiRequest',
            data: 'param=' + ts,
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data) {
                window.location.href = data;
            }
        });
    });
}

function vaoTrangCoThamSo() {
    $('.vaoTrang').click(function () {
        var param = $(this).attr('param');
        var href = $(this).attr('href');
        var value = 'page=' + href + '|request=' + param;
        $.ajax({
            url: getDuongDan() + 'Ajax/addSessionRequest',
            data: 'param=' + value,
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data) {
                window.location.href = data;
            }
        });
    });
}

function addRequestToSession(e) {
    $('.addSession').click(function (e) {
        var ts = $(this).attr('task');
        $.ajax({
            url: getDuongDan() + 'Ajax/AddRequestToSession',
            data: 'param=' + ts,
            context: this,
            dataType: 'html',
            beforeSend: function () {
                //---Sự kiện unbind không click vào item nữa
                event.preventDefault();
                event.stopImmediatePropagation();
            },
            success: function (data) {
                
            }
        });
    });
}