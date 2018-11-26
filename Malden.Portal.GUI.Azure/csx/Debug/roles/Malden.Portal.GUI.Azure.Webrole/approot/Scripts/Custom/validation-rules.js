/// <reference path="../jquery-1.8.2.intellisense.js" />

$(document).ready(function () {
    var validator = $("form").data("validator");
    //alert($("form[name='register']"));
    if (validator) {
        validator.settings.onkeyup = false; // disable validation on keyup
    }

    var isValidEmail = (function (emailAddress) {
        var pattern = new RegExp(/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}([0-9])?))$/);
        return pattern.test(emailAddress);
    });

    //var dateOfRelease = document.getElementById("Release_DateOfRelease");

    //if (dateOfRelease !== null )
    //{
    //   dateOfRelease.validate({ rules: { maldenDate: true } });
    //}
});

//if (jQuery.validator !== null && jQuery.validator !== undefined) {
//    jQuery.validator.addMethod("maldenDate", function (value, element) {
//        return Date.parseExact(value, "d-M-yy");
//    });
//}

