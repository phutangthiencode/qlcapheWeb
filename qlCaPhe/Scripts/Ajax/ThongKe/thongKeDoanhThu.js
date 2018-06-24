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