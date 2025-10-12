
$(document).ready(function () {
    if ($('#officeCodeForm').hasClass('my-hidden')) {
        $('#Input_Password').focus();
    } else {
        $('#Input_OfficeCode').focus();
    }
});