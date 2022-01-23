jQuery(document).ready(function () {
    var dataObject = {
        columns: [{
            title: "NAME"
        }, {
            title: "COUNTY"
        }],
        data: [
          ["John Doe", "Fresno"],
          ["Billy", "Fresno"],
          ["Tom", "Kern"],
          ["King Smith", "Kings"]
        ]
    };
    var columns = [];


    $.ajax({
        cache: false,
        url: "/Reports/JsonAction",
        type: "GET",
        data: { 'q': 0 },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        error: function (response) {
            alert(response.responseText);
        },
        success: function (response) {
            table = $('#example').dataTable({
                destroy: true,
                "data": response.data,
                "columns": response.columns
            });
        }
    });

    var table = null;

    //var table = $('#example').dataTable({
    //    aoData: [{}]
    //});
    $("#SelectedId").change(function () {
        var selectedElement = $(this).val();
        $.ajax({
         cache: false,
         url: "/Reports/JsonAction",
         type: "GET",
         data: { 'q': selectedElement },
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         error: function (response) {
            alert(response.responseText);
        },
         success: function (response) {
             if (table.length) {
                 $('#example_wrapper').remove();
                 $("#example-container").prepend("<table cellpadding='0' cellspacing='0' border='0' class='table table-striped table-bordered center-table' id='example'></table>");
             }
             table = $('#example').dataTable({
                 destroy: true,
                "data": response.data,
                "columns": response.columns               
             });
        }
        });


    });

});

MVCDataTableJqueryBootStrap = {

    init: function () {
        this.initDataTable();
    },

    initDataTable: function () {
        var table = $('#tableContract').DataTable({
            "language": {
                "sProcessing": "Przetwarzanie...",
                "sLengthMenu": "Pokaż _MENU_ pozycji",
                "sZeroRecords": "Nie znaleziono pasujących pozycji",
                "sInfoThousands": " ",
                "sInfo": "Pozycje od _START_ do _END_ z _TOTAL_ łącznie",
                "sInfoEmpty": "Pozycji 0 z 0 dostępnych",
                "sInfoFiltered": "(filtrowanie spośród _MAX_ dostępnych pozycji)",
                "sInfoPostFix": "",
                "sSearch": "Szukaj:",
                "sUrl": "",
                "oPaginate": {
                    "sFirst": "Pierwsza",
                    "sPrevious": "Poprzednia",
                    "sNext": "Następna",
                    "sLast": "Ostatnia"
                }
            }
        });

        MVCDataTableJqueryBootStrap.returnDataTable = function () {
            return table;
        }
    },
};

$(function () {
    MVCDataTableJqueryBootStrap.init();
});
var childWindow = null;

function incompleteTraining() {
    childWindow = window.open("/Training", "childWindow");
    childWindow.focus();
}
function workersWithoutTraining() {
    childWindow = window.open("/Training", "childWindow");
    childWindow.focus();
}
function instructionsWithoutTraining() {
    childWindow = window.open("/Training", "childWindow");
    childWindow.focus();
    window.setTimeout(function () { if ((childWindow != null) && (childWindow.closed == false)) childWindow.ShowModal(); }, 3000);
}
