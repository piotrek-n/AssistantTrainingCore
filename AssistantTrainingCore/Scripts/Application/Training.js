(function () {
    LoadGrid();
}())

$(document).ajaxSuccess(function () {
    LoadGrid();
});
function LoadGrid() {
    //var gridName = 'TrainingGrid';
    //var pagingUrl = 'Training/GridPager';

    //$('.grid-mvc').gridmvc();
    //pageGrids[gridName].ajaxify({
    //    getData: pagingUrl,
    //    getPagedData: pagingUrl
    //});

    var gridName2 = 'TrainingWorkersGrid';
    var pagingUrl2 = 'Training/GridWorkerPager';

    $('#refWorkerGrid > .grid-mvc').gridmvc();
    pageGrids[gridName2].ajaxify({
        getData: pagingUrl2,
        getPagedData: pagingUrl2
    });

    $('#sandbox-container input').datepicker({
        todayBtn: "linked",
        language: "pl",
        autoclose: true
    });

    $('[data-gridname="TrainingWorkersGrid"]').children('tbody').unbind('click');
    $('[data-gridname="TrainingWorkersGrid"]').children('tbody').click(function () {
    });

    $('#myform').validate({ // initialize the plugin
        rules: {
            field1: {
                required: true
            }
        },
        submitHandler: function (form) { // for demo
            //alert('valid form submitted'); // for demo
            return false; // for demo
        }
    });

    $("#RemoveTrainingToWorkerWorkersGrid").unbind('click');
    $('#RemoveTrainingToWorkerWorkersGrid').click(
        function () {
            if ($('[data-gridname="TrainingWorkersGrid"]').find("table>tbody>tr").find('[type=checkbox]').size() == 0)
                return;

            var trainingWorkersGrid = {};
            var workers = [];
            trainingWorkersGrid.Workers = workers;
            trainingWorkersGrid.TrainingDate = "";
            trainingWorkersGrid.TrainingNumber = "";

            var $row = $('[data-gridname="TrainingWorkersGrid"]').find("table>tbody>tr").each(function (i, row) {
                var checked = $(row).find('[type=checkbox]').prop('checked');
                var workerID = $(row).find('[data-name="WorkerID"]').html();
                var trainingNameId = $(row).find('[data-name="TrainingNameId"]').html();

                var worker = {
                    "WorkerID": workerID,
                    "TrainingNameId": trainingNameId,
                    "Checked": checked
                };
                trainingWorkersGrid.Workers.push(worker);
            });
            var trainingDate = $('#TrainingDate').val();
            var trainingNumber = $('#TrainingNumber').val();

            trainingWorkersGrid.TrainingDate = trainingDate;
            trainingWorkersGrid.TrainingNumber = trainingNumber;

            console.log(JSON.stringify(trainingWorkersGrid));

            $.ajax({
                url: "Training/RemoveTrainings",
                type: "POST",
                data: JSON.stringify(trainingWorkersGrid),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (jqXHR, textStatus, errorThrown) {
                    $("#refWorkerGrid").html(jqXHR.responseText);
                    LoadGrid();
                },
                success: function (response) {
                    $("#refWorkerGrid").html(partialViewResult);
                    LoadGrid();
                }
            });
        }
    );
    $("#SaveTrainingWorkersGrid").unbind('click');
    $('#SaveTrainingWorkersGrid').click(
        function () {
            if ($('[data-gridname="TrainingWorkersGrid"]').find("table>tbody>tr").find('[type=checkbox]').size() == 0)
                return;
            if ($('#myform').valid()) {
                var trainingWorkersGrid = {};
                var workers = [];
                trainingWorkersGrid.Workers = workers;
                trainingWorkersGrid.WrainingDate = "";
                trainingWorkersGrid.TrainingNumber = "";

                var $row = $('[data-gridname="TrainingWorkersGrid"]').find("table>tbody>tr").each(function (i, row) {
                    var checked = $(row).find('[type=checkbox]').prop('checked');
                    var workerID = $(row).find('[data-name="WorkerID"]').html();
                    var trainingNameId = $(row).find('[data-name="TrainingNameId"]').html();

                    var worker = {
                        "WorkerID": workerID,
                        "TrainingNameId": trainingNameId,
                        "Checked": checked
                    };
                    trainingWorkersGrid.Workers.push(worker);
                });
                var trainingDate = $('#TrainingDate').val();
                var trainingNumber = $('#TrainingNumber').val();

                trainingWorkersGrid.TrainingDate = trainingDate;
                trainingWorkersGrid.TrainingNumber = trainingNumber;

                console.log(JSON.stringify(trainingWorkersGrid));

                $.ajax({
                    url: "Training/UpdateTrainings",
                    type: "POST",
                    data: JSON.stringify(trainingWorkersGrid),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    error: function (jqXHR, textStatus, errorThrown) {
                        $("#refWorkerGrid").html(jqXHR.responseText);
                        LoadGrid();
                    },
                    success: function (response) {
                        $("#refWorkerGrid").html(partialViewResult);
                        LoadGrid();
                    }
                });
            } else {
                // alert('form is not valid');
            }
        }
    );

    $("span > a").unbind('click');
    $("span > a").click(function (event) {
        event.preventDefault();
        var value = $(this).attr("href");

        var id = $(this).attr("id");

        if (id === 'untrained') {
            $.ajax({
                url: "Training/GetWorkerGrid",
                type: "POST",
                data: { term: value, type: 'untrained' }
            })
                .done(function (partialViewResult) {
                    $("#refWorkerGrid").html(partialViewResult);
                    LoadGrid();
                });
            //return false; //for good measure
        }
        else if (id === 'trained') {
            $.ajax({
                url: "Training/GetWorkerGrid",
                type: "POST",
                data: { term: value, type: 'trained' }
            })
                .done(function (partialViewResult) {
                    $("#refWorkerGrid").html(partialViewResult);
                    LoadGrid();
                });
            //return false; //for good measure
        }
    });

    //$('#TrainingNumber').val('nr');
}
//
$('#srch-term-instruction').typeahead(
    {
        source: function (query, process) {
            return $.get('/Training/GetInstructionsByQuery', { query: query }, function (data) {
                console.log(data);
                return process(data);
            });
        }
    });

$('#srch-term-training').typeahead(
    {
        source: function (query, process) {
            return $.get('/Training/GetTrainingNamesByQuery', { query: query }, function (data) {
                console.log(data);
                return process(data);
            });
        }
    });

$(document).ready(function () {
    $("#idInstructionForm").submit(function (e) {
        var val = $('#srchterminstruction').val();
        $.ajax({
            url: "Training/GetGridByInstruction",
            type: "POST",
            data: { term: val }
        })
            .done(function (partialViewResult) {
                $("#refGrid").html(partialViewResult);
                LoadGrid();
            });
        e.preventDefault(); //STOP default action
    });
    $("#idTrainingForm").submit(function (e) {
        var val = $('#srchtermtraining').val();
        $.ajax({
            url: "Training/GetGridByTraining",
            type: "POST",
            data: { term: val }
        })
            .done(function (partialViewResult) {
                $("#refGrid").html(partialViewResult);
                LoadGrid();
            });
        e.preventDefault(); //STOP default action
    });
    $('span > a').click(function (event) {
        event.preventDefault();
        var value = $(this).attr("href");

        var id = $(this).attr("id");

        if (id === 'untrained') {
            $.ajax({
                url: "Training/GetWorkerGrid",
                type: "POST",
                data: { term: value, type: 'untrained' }
            })
                .done(function (partialViewResult) {
                    $("#refWorkerGrid").html(partialViewResult);
                    LoadGrid();
                });
            //return false; //for good measure
        }
        else if (id === 'trained') {
            $.ajax({
                url: "Training/GetWorkerGrid",
                type: "POST",
                data: { term: value, type: 'trained' }
            })
                .done(function (partialViewResult) {
                    $("#refWorkerGrid").html(partialViewResult);
                    LoadGrid();
                });
            //return false; //for good measure
        }
    });
    $('#save-new-training').click(function (e) {
        var number = $("#newTrainingNumber").val();
        var values = $("#sel").val().toString();
        $.ajax({
            url: "Trainings/AddNewTrainings",
            data: { selectedValues: values, trainingNumber: number },
            type: 'POST',
            success: function (data) {
                $('#myModal').modal('hide');
                $.ajax({
                    url: "Training/GetGridByTraining",
                    type: "POST",
                    data: { term: number }
                })
                    .done(function (partialViewResult) {
                        $("#refGrid").html(partialViewResult);
                        LoadGrid();
                    });
            }
        });
    });
});

$("#sel").select2({
    ajax: {
        url: "/Training/InstructionsJsonAction",
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return {
                q: params.term, // search term
                t: $("#searchTermCheckbox").is(":checked")
                //,page: params.page
            };
        },
        processResults: function (data, params) {
            params.page = params.page || 1;

            console.log(data.items);

            return {
                results: data.items,
                pagination: {
                    more: (params.page * 30) < data.total_count
                }
            };
        },
        cache: false
    },
    escapeMarkup: function (markup) {
        return markup;
    }, // let our custom formatter work
    minimumInputLength: 1,
    templateResult: formatRepo, // omitted for brevity, see the source of this page
    templateSelection: formatRepoSelection // omitted for brevity, see the source of this page
});

$("#sel").on("select2:unselect", function (e) {
    $('#remainders').children().each(function () {
        if ($(this)[0].innerText.includes('Wersja Papierowa:' + e.params.data.text)) {
            $(this)[0].remove();
        }
    });
});

function formatRepo(repo) {
    if (repo.loading) return repo.text;

    var markup = "<div class='select2-result-repository clearfix'>" +
        "<div class='select2-result-repository__meta'>" +
        "<div class='select2-result-repository__title'>" + repo.text + ' v.' + repo.version + "</div>";

    if (repo.name) {
        markup += "<div class='select2-result-repository__description'>" + repo.name + "</div>";
    }
    return markup;
}

function formatRepoSelection(repo) {
    if (repo.reminder) {
        var name = repo.text || repo.id;
        var contains = false;
        var div = $('<div></div>').addClass('reminder-row').text('Wersja Papierowa:' + repo.text || repo.id);

        $('#remainders').children().each(function () {
            if ($(this)[0].innerText.includes('Wersja Papierowa:' + name)) {
                contains = true;
            }
        });
        if (contains == false) {
            $(div).appendTo($('#remainders'));
        }
    }
    return repo.text || repo.id;
}

function ShowModal() {
    $('#myModal').modal('show');
}

$('#myModal').on('shown.bs.modal', function () {
    $("#sel").val('').trigger('change');
})