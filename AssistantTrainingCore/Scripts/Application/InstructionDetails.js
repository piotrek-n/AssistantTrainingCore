(function () {


    $('.table > tbody  > tr').each(function (i, el) {

        var IsSuspend = $(this).find(":nth-child(3)").html();
        var IsDate = $(this).find(":nth-child(4)").html();

        if (IsSuspend === 'Tak') {
            $(this).css('color', 'red');
        }

        if (IsDate === '1900-01-01') {
            $(this).find(":nth-child(4)").text("");
        }
    });

}())