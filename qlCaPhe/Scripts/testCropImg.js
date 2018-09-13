$(document).ready(function () {
    $('#btnUpload').click(function () {
        if (window.FormData !== undefined) {
            var fileUpload = $("#FileUpload1").get(0);
            var files = fileUpload.files;
            var fileData = new FormData();
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            fileData.append('username', 'Manas');
            $.ajax({
                url: '/Ajax/UploadFiles?folder=doUong',
                type: "POST",
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result != "") {
                        $('#imgCrop').attr('src', result);
                        reloadImageUploaded(result);
                    }
                    else
                        $('#hinhDD').attr('src', '/images/gallery-upload-256.png');
                },
                error: function (err) {
                    $('#hinhDD').attr('src', '/images/gallery-upload-256.png');
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });
    $('#imgCrop').hover(function () {
        $('#imgCrop').Jcrop({
            minSize: [268, 185],
            maxSize: [268, 185],
            onChange: SelectCropArea,
            onSelect: SelectCropArea
        });
    });
    $('#btnCrop').click(function () {
        var x = $('#x').val();
        var y = $('#y').val();
        var w = $('#w').val();
        var h = $('#h').val();
        if (w == 0) {
            $('#thongBao').text("Vui lòng lựa chọn vị trí cần lấy trên hình");
            return;
        }
        $.ajax({
            url: getDuongDan() + 'Ajax/CropAndSaveImage?x=' + x + '&y=' + y + '&w=' + w + '&h=' + h + '&folder=doUong',
            beforeSend: function () { },
            success: function (result) {
                $('#hinhDD').attr('src', result.split('||')[0]);
                $('#pathHinh').val(result.split('||')[1])
                ; $('#modalChonHinh').modal('hide');
            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
    });
});
function SelectCropArea(c) {
    $('#x').val(parseInt(c.x));
    $('#y').val(parseInt(c.y));
    $('#w').val(parseInt(c.w));
    $('#h').val(parseInt(c.h));
}
function reloadImageUploaded(result) {
    $('.jcrop-holder').children().children().find('img').attr('src', result);
    $('.jcrop-holder').find('img').attr('src', result);
}