/// <reference path="../jquery-2.1.1.intellisense.js" />
$(document).ready(function () {
    sortUserTable();

});

var sortUserTable = (function () {
    $("#users").tablesorter({
        headers: {
            4: { sorter: false }
        }
    });
});
