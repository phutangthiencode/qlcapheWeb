//------------Hàm gọi Function Ajax lấy danh sách hóa đơn. Sau đó thực hiện phần tích và gọi hàm vẽ biểu đồ
//------------------fn: Tên hàm Ajax gửi request lên server
//------------------soHoaDons: mảng chứa số hóa đơn
//------------------tongTiens: Mảng chứa tổng tiền
//------------------datas: Dữ liệu để vẽ biểu đồ
//-----------------ts: Tham số gửi lên server để lọc dữ liệu
//-----------------idDivchart: id name div chứa biểu đồ
//-----------------title: Tên title trên biểu đồ
function layDuLieuHoaDon(fn, soHoaDons, tongTiens, datas, ts, idDivchart, title) {

    fn(function (data) {
        soHoaDons = [];
        tongTiens = [];
        datas = [];

        $.each(data, function (i, item) {
            soHoaDons.push([item.maHD]);
            tongTiens.push([item.tamTinh]);
            datas = data; // Dành cho biểu đồ cột
        });

        var data = new google.visualization.DataTable();
        data.addColumn('number', 'maHD');
        data.addColumn('number', 'tamTinh');

        //-------Phân tích dữ liệu từ json trả về
        if (datas.length > 0) {
            data.addRows(soHoaDons.length);
            for (var i = 0; i < soHoaDons.length; i++)
                data.setCell(i, 0, parseInt(tongTiens[i]));

            veBieuDoCotDoanhThu(datas, idDivchart, title);
        }
    }, ts);
}

//--------Hàm vẽ đồ thị dạng cột cho doanh thu
//-----------datas: mảng chứa dữ liệu cần vẽ biểu đồ
function veBieuDoCotDoanhThu(datas, idDivchart, title) {
    var dataArray = [['Hóa đơn', 'Tiền thanh toán']];
    $.each(datas, function (i, item) {
        dataArray.push([item.maHD, item.tamTinh]);
    });

    var data = new google.visualization.arrayToDataTable(dataArray);
    var options = {
        title: title,
        hAxis: { title: 'Hóa đơn', titleTextStyle: { color: 'black' } }
    };

    var chart = new google.visualization.ColumnChart(document.getElementById(idDivchart));

    chart.draw(data, options);

}

//-----------------Hàm vẽ biểu đồ tròn cho việc thống kê sản phẩm theo số lượng đã bán
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ tròn
//------------chartID: id của div chứa biểu đồ
function drawPieChartSanPham(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Devices');
    // Create Rows with data
    var jsonLenght = Object.keys(json).length;
    for (i = 0; i < jsonLenght; i++) {
        var sp = json[i]["tenSP"];
        var soLanBan = json[i]["soLanBan"];
        data.addRows([
            [sp, soLanBan]
        ]);
    }

    //Create option for chart
    var options = {
        title: title
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.PieChart(document.getElementById(chartID));
    //var c = new google.visualization.ColumnChart(document)
    chart.draw(data, options);
}

//-----------------Hàm vẽ biểu đồ cột cho việc thống kê doanh thu bán sản phẩm
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ cột
//------------chartID: id của div chứa biểu đồ
function drawColumnChartDoanhThuTheoSanPham(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Sản phẩm');
    // Create Rows with data
    var jsonLenght = Object.keys(json).length;
    for (i = 0; i < jsonLenght; i++) {
        var sp = json[i]["tenSP"];
        var tongTien = json[i]["tongTien"];
        data.addRows([
            [sp, tongTien]
        ]);
    }

    //Create option for chart
    var options = {
        title: title
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ColumnChart(document.getElementById(chartID));
    chart.draw(data, options);
}

//-----------------Hàm vẽ biểu đồ cột cho việc thống kê doanh thu kiếm được dựa trên người phục vụ
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ cột
//------------chartID: id của div chứa biểu đồ
function drawColumnChartDoanhThuTheoPhucVu(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Doanh số');
    // Create Rows with data
    var jsonLenght = Object.keys(json).length;
    for (i = 0; i < jsonLenght; i++) {
        var phucVu = json[i]["nguoiPhucVu"];
        var tongTien = json[i]["tongTien"];
        data.addRows([
            [phucVu, tongTien]
        ]);
    }

    //Create option for chart
    var options = {
        title: title
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ColumnChart(document.getElementById(chartID));
    chart.draw(data, options);
}


//-----------------Hàm vẽ biểu đồ cột cho việc thống kê lợi nhuận
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ cột
//------------chartID: id của div chứa biểu đồ
function drawColumnChartLoiNhuan(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Lợi nhuận');
    // Create Rows with data
    var jsonLenght = Object.keys(json).length;
    for (i = 0; i < jsonLenght; i++) {
        var thoiDiem = json[i]["time"];
        var loiNhuan = json[i]["loiNhuan"];
        data.addRows([
            [thoiDiem, loiNhuan]
        ]);
    }

    //Create option for chart
    var options = {
        title: title
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ColumnChart(document.getElementById(chartID));
    chart.draw(data, options);
}


function hienTongDoanhThuHoaDon(json) {
    $('.js-tong-doanh-thu-hoadon').html("");
    var lenghtJson = Object.keys(json).length;
    if (lenghtJson > 0) {
        var lastObject = json[lenghtJson - 1];
        var tongDoanhThu = lastObject['tongTien'];
        $('.js-tong-doanh-thu-hoadon').html("Tổng doanh thu thực tế trên hóa đơn là: " + tongDoanhThu);
    }
}

//----------Hàm hiển thị tổng doanh thu lên giao diện
//--------------json: list object json 
function hienTongDoanhThuSanPham(json) {
    $('.js-tong-doanh-thu-sanpham').html("");
    var lenghtJson = Object.keys(json).length;
    if (lenghtJson > 0) {
        var lastObject = json[lenghtJson - 1];
        var tongDoanhThu = lastObject['tongDoanhThu'];
        $('.js-tong-doanh-thu-sanpham').html("Tổng doanh thu trên sản phẩm là: " + tongDoanhThu);
    }
}
//--------Hàm hiển thị tổng lợi nhuận thu được lên giao diện
function hienTongLoiNhuan(json) {
    $('.js-tong-loinhuan').html("");
    var lenghtJson = Object.keys(json).length;
    if (lenghtJson > 0) {
        var lastObject = json[lenghtJson - 1];
        var tongLoiNhuan = lastObject['tongLoiNhuan'];
        $('.js-tong-loinhuan').html("Tổng lợi nhuận là: " + tongLoiNhuan);
    }
}

//-----------------Hàm vẽ biểu đồ cột cho việc thống kê doanh thu bán sản phẩm
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ đường
//------------chartID: id của div chứa biểu đồ
function drawLineChartDoanhThuMotSanPham(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Sản phẩm');
    // Create Rows with data
    var jsonLenght = Object.keys(json).length;
    for (i = 0; i < jsonLenght; i++) {
        var sp = json[i]["thoiDiem"];
        var tongTien = json[i]["tongTien"];
        data.addRows([
            [sp, tongTien]
        ]);
    }

    //Create option for chart
    var options = {
        title: title
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.LineChart(document.getElementById(chartID));
    chart.draw(data, options);
}