$(document).ready(function () {
    var flag = true;//----Biến cờ nhận thông báo
    //////======Sự kiện lấy và hiển thị danh sách thông báo
    setInterval(function () {
        if (flag == true)
            $.ajax({
                url: getDuongDan() + 'Home/Notifications',
            }).success(function (data) {
                $('#vungThongBao').html(data);
            });
    }, 1000);

    //------Sự kiện click để ẩn hộp thoại thông báo
    $('html').click(function (e) {
        var idClicked = e.target.id; //-----lấy id name của element vừa click
        if (idClicked==="js-chuong") //-----Nếu id name là js-chuong = id của icon chuông
            flag = false; //-----hạ cờ không gọi ajax thông báo
        else
            flag = true; //------bật cờ gọi ajax thông báo
    });

    xoaTatCaThongBao();
});

function getDuongDan() {
    var kq = window.location.protocol + '//' + window.location.host;
    return kq + '/';
}

//Thực hiện mở file hình và gán hình vào img có id la hinhDD
function OpenFileBrowser(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#hinhDD')
                .attr('src', e.target.result)
                .width(150)
                .height(200);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
//Thực hiện mở file hình cho view bài viết với độ dài hình lớn hơn
function OpenFileBrowserBaiViet(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#hinhDD')
                .attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
//----Thực hiện trở về trang trước cho button Trở về trên form
function troVeTrangTruoc() {
    window.history.back();
}
//-----------Hàm ajax xóa thông báo
function xoaTatCaThongBao() {
    $("#vungThongBao").on("click", "#jsBtnXoaThongBao", function (e) {
        $.ajax({
            url: getDuongDan() + 'Home/AjaxXoaTatCaThongBao',
            beforeSend: function () {
                $('#jsBtnXoaThongBao').unbind("click");
                //---Sự kiện unbind không click vào item nữa
                e.preventDefault();
                e.stopImmediatePropagation();
            },
            success: function (data, textStatus, xhr) {
                $('#vungThongBao').html("");
                xoaTatCaThongBao();
            },
            error: function (xhr, textStatus, errorThrown) {
                xuLyNghiepVuBan();
            }
        });
    });
}

//-------Hàm in mở chức năng in 1 element trên giao diện
function printContent(el) {
    var restorepage = $('body').html();
    var printcontent = $('#' + el).clone();
    $('body').empty().html(printcontent);
    window.print();
    $('body').html(restorepage);
}


//-------HÀM ĐỌC CÁC REQUEST TRUYỀN VÀO CÓ TRÊN URL
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}
//-----Hàm lấy tham số trang hiện tại có trên url
function getCurrentPageInRequest() {
    var res = getUrlVars()["page"];
    if (typeof (res) === 'undefined')
        return 1;
    return res;
}

//-----Hàm hiện thông báo Toast
function showToastNotify(element) {
    $(element).fadeIn("slow").delay(5000);//--------Hiện thông báo trong 5s
    $(element).slideUp(300).fadeOut("slow");
}

//----Hàm thực hiện trượt giao diện xuống section có className
//---------className: tên class cần trượt đến 
function slideToNextSection(className) {
    $('html,body').animate({
        scrollTop: $("." + className).offset().top
    },
              'slow');
}



//-------HÀM CHUYỂN ĐỔI GIÁ TRỊ THEO TIỀN TỆ. VD 1,235,000 vnd
//------------value: giá trị cần chuyển đổi
function formatTienTe(value, decSeperate, groupSeperate) {
    value += '';
    x = value.split(decSeperate);
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + groupSeperate + '$2');
    }
    return x1 + x2;
}

//--------Hàm chuyển đổi Date thành string
function convertDateToString(date) {
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var today = date.getFullYear() + "-" + (month) + "-" + (day);
    return today;
}

//-------Hàm lấy ngày hiện tại
//----------Trả về chuỗi ngày hiện tại theo đạng dd/MM/yyyy
function getDateCurrent() {
    var now = new Date();
    var today = convertDateToString(now);
    return today;
}
//----------Hàm lấy ngày của của tháng tiếp theo
//--------------startDate ngày bắt đầu
//--------------return ngày của tháng tiếp theo
function getDateOfNextMonth(startDate) {
    var start = new Date(startDate);
    start.setMonth(start.getMonth() + 1);
    return start;
}

//---------Hàm lấy ngày của tuần tiếp theo
//--------------startDate ngày bắt đầu
//--------------return Ngày của tuần tiếp theo
function getDateOfNextWeek(startDate) {
    var firstDay = new Date(startDate);
    var nextWeek = new Date(firstDay.getTime() + 7 * 24 * 60 * 60 * 1000);
    return nextWeek;
}
//-------Hàm lấy số năm hiện tại
function getYearCurrent() {
    return (new Date()).getFullYear()
}
//-------Hàm lấy tháng hiện tại
function getMonthCurrent() {
    return (new Date()).getMonth() + 1;
}
//------Hàm lấy quý hiện tại
function getQuarterCurrent() {
    var today = new Date();
    return Math.floor((today.getMonth() + 3) / 3);
}