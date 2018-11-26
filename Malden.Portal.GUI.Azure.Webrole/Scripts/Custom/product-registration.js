/// <reference path="../jquery-1.8.2.intellisense.js" />
/// <reference path="../jquery.maskedinput-1.3.1.js" />
/// <reference path="md5.js" />
$(document).ready(function () {
    if ($("#RegistrationCode").val() !== undefined) {
        if ($("#RegistrationCode").val().length === 0) {
            $("#RegistrationCode").mask("99999-*****");
            $("#RegistrationCode").disabled = "";
            $("#RegistrationCode").focus();
            strictKeys();

        }
    }
});

var strictKeys = function () {
    $("#RegistrationCode").keydown(function (event) {

        if ($.inArray(event.keyCode, [46, 8, 9, 27, 13, 190, 65, 66, 67, 68, 69, 70, 17, 86, 67]) !== -1 ||
            (event.keyCode == 65 && event.ctrlKey === true) ||
            (event.keyCode >= 35 && event.keyCode <= 39)) {
            return;
        }
        else {
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });
};

$("#RegistrationCode").bind('blur', function (e) {

    if (e.which >= 97 && e.which <= 122) {
        var newKey = e.which - 32;

        e.keyCode = newKey;
        e.charCode = newKey;
    }

    $("#RegistrationCode").val(($("#RegistrationCode").val()).toUpperCase());
});