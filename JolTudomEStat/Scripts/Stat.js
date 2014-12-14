var StatChart;
var chartoptions = {
  ///Boolean - Whether grid lines are shown across the chart
  scaleShowGridLines: true,
  //String - Colour of the grid lines
  scaleGridLineColor: "rgba(0,0,0,.05)",
  //Number - Width of the grid lines
  scaleGridLineWidth: 1,
  //Boolean - Whether the line is curved between points
  bezierCurve: false,
  //Number - Tension of the bezier curve between points
  bezierCurveTension: 0.4,
  //Boolean - Whether to show a dot for each point
  pointDot: true,
  //Number - Radius of each point dot in pixels
  pointDotRadius: 4,
  //Number - Pixel width of point dot stroke
  pointDotStrokeWidth: 1,
  //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
  pointHitDetectionRadius: 20,
  //Boolean - Whether to show a stroke for datasets
  datasetStroke: true,
  //Number - Pixel width of dataset stroke
  datasetStrokeWidth: 2,
  //Boolean - Whether to fill the dataset with a colour
  datasetFill: true,
  //String - A legend template
  legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
};

var testdata = {
  labels: ["January", "February", "March", "April", "May", "June", "July"],
  datasets: [
      {
        label: "My First dataset",
        fillColor: "rgba(220,220,220,0.2)",
        strokeColor: "rgba(220,220,220,1)",
        pointColor: "rgba(220,220,220,1)",
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: "rgba(220,220,220,1)",
        data: [65, 59, 80, 81, 56, 55, 40]
      },
      {
        label: "My Second dataset",
        fillColor: "rgba(151,187,205,0.2)",
        strokeColor: "rgba(151,187,205,1)",
        pointColor: "rgba(151,187,205,1)",
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: "rgba(151,187,205,1)",
        data: [28, 48, 40, 19, 86, 27, 90]
      }
  ]
};


function Init() {
  var ctx = document.getElementById("statchart").getContext("2d");
  StatChart = new Chart(ctx);
}

function GetData() {
  $.ajax({
    type: "GET",
    dataType: "json",
    url: "http://joltudomeapi.azurewebsites.net/api/actstat/get"
    //url: "http://localhost:1854/api/actstat/get"
  }).done(function (data) {
    //alert("data");
    var chartdata = {};
    chartdata.labels = data.Labels;
    chartdata.datasets = new Array();
    var datavalues = new Array();
    for (var i in data.Data) {
      datavalues.push(data.Data[i].HowMany);
    }
    chartdata.datasets.push({
      label: "Login",
      fillColor: "rgba(220,220,220,0.2)",
      strokeColor: "rgba(220,220,220,1)",
      pointColor: "rgba(220,220,220,1)",
      pointStrokeColor: "#fff",
      pointHighlightFill: "#fff",
      pointHighlightStroke: "rgba(220,220,220,1)",
      data: datavalues
    });

    StatChart.Line(chartdata, chartoptions);

  }).fail(function (data) {
    alert("fail");
  });
}