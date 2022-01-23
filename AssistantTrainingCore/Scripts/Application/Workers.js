(function () {

    var $row = $('.table').find("tbody>tr").each(function (i, row) {

        var IsSuspend = $(row).find('[data-name="IsSuspendDesc"]');
        if (IsSuspend[0].innerHTML === 'Tak') {
            $(row).find('[data-name="FullName"]').css('color', 'red');
        }
    });
}())