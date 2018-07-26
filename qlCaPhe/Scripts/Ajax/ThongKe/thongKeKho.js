
//------------Hàm gọi Function Ajax lấy danh sách hóa đơn. Sau đó thực hiện phần tích và gọi hàm vẽ biểu đồ
//------------------fn: Tên hàm Ajax gửi request lên server
//------------------phieuNhaps: mảng chứa các phiếu nhập
//------------------tongTiens: Mảng chứa tổng tiền nhập kho trên 1 phiếu
//------------------datas: Dữ liệu để vẽ biểu đồ
//-----------------ts: Tham số gửi lên server để lọc dữ liệu
//-----------------idDivchart: id name div chứa biểu đồ
//-----------------title: Tên title trên biểu đồ
function layDuLieuTongTienNhap(fn, phieuNhaps, tongTiens, datas, ts, idDivchart, title) {

    fn(function (data) {
        phieuNhaps = [];
        tongTiens = [];
        datas = [];

        $.each(data, function (i, item) {
            phieuNhaps.push([item.time]);
            tongTiens.push([item.price]);
            datas = data; // Dành cho biểu đồ cột
        });

        var data = new google.visualization.DataTable();
        data.addColumn('number', 'time');
        data.addColumn('number', 'price');

        //-------Phân tích dữ liệu từ json trả về
        if (datas.length > 0) {
            data.addRows(phieuNhaps.length);
            for (var i = 0; i < phieuNhaps.length; i++)
                data.setCell(i, 0, parseInt(tongTiens[i]));

            veBieuDoCotTienNhapKho(datas, idDivchart, title);
        }
    }, ts);

}


//--------Hàm vẽ đồ thị dạng cột cho doanh thu
//-----------datas: mảng chứa dữ liệu cần vẽ biểu đồ
function veBieuDoCotTienNhapKho(datas, idDivchart, title) {
    var dataArray = [['Thời điểm', 'Tổng tiền']];
    $.each(datas, function (i, item) {
        dataArray.push([item.time, item.price]);
    });

    var data = new google.visualization.arrayToDataTable(dataArray);
    var options = {
        title: title,
        hAxis: { title: 'Phiếu', titleTextStyle: { color: 'black' } }
    };

    var chart = new google.visualization.ColumnChart(document.getElementById(idDivchart));

    chart.draw(data, options);

}

//-----------------Hàm vẽ biểu đồ tròn cho việc thống kê nguyên liệu theo số lượng nhập
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ tròn
//------------chartID: id của div chứa biểu đồ
function drawPieChartNguyenLieu(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Devices');
    // Create Rows with data
    drawChart(json, data, 1, chartID, title);
}

//-----------------Hàm vẽ biểu đồ cột cho việc thống kê số tiền từng nguyên liệu đã nhập
//------------json: mảng json object chứa dữ liệu được lấy từ database
//------------title: Title cho biểu đồ cột
//------------chartID: id của div chứa biểu đồ
function drawColumnChartPriceOfMeterial(json, title, chartID) {
    // Create the data table.
    var data = new google.visualization.DataTable();
    // Create columns for the DataTable
    data.addColumn('string');
    data.addColumn('number', 'Tiền nhập');

    drawChart(json, data, 2, chartID, title);
}
//--------Hàm vẽ đồ thị
//---------------json: json object chứa dữ liệu cần hiện lên đồ thị
//---------------typeChart: Loại biểu đồ: 1: Tròn 2 Cột
//---------------chartID: id của elemnt div chứa chart
//---------------title: Tiêu để của biểu đồ
function drawChart(json, data, typeChart, chartID, title) {
    var jsonLenght = Object.keys(json).length;
    if (jsonLenght > 0) {
        for (i = 0; i < jsonLenght; i++) {
            var tenNguyenLieu = json[i]["tenNguyenLieu"];
            var tongTienNhap = json[i]["tongTienNguyenLieu"];
            data.addRows([
                [tenNguyenLieu, tongTienNhap]
            ]);
        }
        //Create option for chart
        var options = {
            title: title
        };
        if (typeChart == 1)
            var chart = new google.visualization.PieChart(document.getElementById(chartID));
        else
            var chart = new google.visualization.ColumnChart(document.getElementById(chartID));
        chart.draw(data, options);
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

function docTongTien(json) {
    var lenghtJson = Object.keys(json).length;
    if (lenghtJson > 0) {
        var lastObject = json[lenghtJson - 1];
        var tongTien = lastObject['tongTien'];
        return tongTien;
    }
}

function hienTongTienNhapKho(json) {
    $('.js-tong-tien').html("");
    var tongTien = docTongTien(json);
    if (typeof (tongTien) != 'undefined')
        $('.js-tong-tien').html("Tổng tiền nhập kho nguyên liệu là: " + tongTien);
}