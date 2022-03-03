$("#RemoveTrainingToWorkerWorkersGrid").unbind('click');
$('#RemoveTrainingToWorkerWorkersGrid').click(
    function () {
        if ($('#trainingWorkersGridData').find("table>tbody>tr").find('[type=checkbox]').size() == 0)
            return;

        console.log('RemoveTrainingToWorkerWorkersGrid - after');

        var trainingWorkersGrid = {};
        var workers = [];
        trainingWorkersGrid.Workers = workers;
        trainingWorkersGrid.TrainingDate = "";
        trainingWorkersGrid.TrainingNumber = "";

        var $row = $('#trainingWorkersGridData').find("table>tbody>tr").each(function (i, row) {
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
            url: "Trainings/RemoveTrainings",
            type: "POST",
            data: JSON.stringify(trainingWorkersGrid),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                window.location.reload(true);
            },
            success: function (response) {
                window.location.reload(true);
            }
        });
    }
);

$("#SaveTrainingWorkersGrid").unbind('click');
$('#SaveTrainingWorkersGrid').click(
    function () {
        if ($('#trainingWorkersGridData').find("table>tbody>tr").find('[type=checkbox]').size() == 0)
            return;

        console.log('Save TrainingWorkersGrid - after');

        if ($('#myform').valid()) {
            var trainingWorkersGrid = {};
            var workers = [];
            trainingWorkersGrid.Workers = workers;
            trainingWorkersGrid.WrainingDate = "";
            trainingWorkersGrid.TrainingNumber = "";

            var $row = $('#trainingWorkersGridData').find("table>tbody>tr").each(function (i, row) {
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
                    //$("#refWorkerGrid").html(jqXHR.responseText);
                    //LoadGrid();

                    window.location.reload(true);
                },
                success: function (response) {
                    //$("#refWorkerGrid").html(partialViewResult);
                    //LoadGrid();

                    window.location.reload(true);
                }
            });
        } else {
            // alert('form is not valid');
        }
    }
);

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
    $('#save-new-training').click(function (e) {
        var number = $("#newTrainingNumber").val();
        var values = $("#sel").val().toString();
        $.ajax({
            url: "Trainings/AddNewTrainings",
            data: { selectedValues: values, trainingNumber: number },
            type: 'POST',
            success: function (data) {
                window.location.reload(true);
                //$('#myModal').modal('hide');
                //$.ajax({
                //    url: "Training/GetGridByTraining",
                //    type: "POST",
                //    data: {term: number}
                //})
                //    .done(function (partialViewResult) {
                //        $("#refGrid").html(partialViewResult);
                //        LoadGrid();
                //    });
            }
        });
    });
});

$("#sel").select2({
    ajax: {
        url: "/Trainings/InstructionsJsonAction",
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
    return repo.text || repo.id;
}

function ShowModal() {
    $('#myModal').modal('show');
}

$('#myModal').on('shown.bs.modal', function () {
    $("#sel").val('').trigger('change');
})