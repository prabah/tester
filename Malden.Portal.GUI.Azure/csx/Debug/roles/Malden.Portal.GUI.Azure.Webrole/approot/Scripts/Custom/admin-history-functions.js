/// <reference path="../jquery-2.1.1.intellisense.js" />
var enableLoadRecords = "loadRecords";
var disableLoadRecords = "disableLoadRecords";

$(document).ready(function () {
    refreshRowLabel();
    $("#check-status").hide();
    //loadMoreRecords();
    sortTable();
    
    $(".wrapper").hide();
});

$("#load-more-history").click(function () {
    
    loadMoreRecords();
    
    });

var sortTable = (function () {
    $('#admin').tablesorter({
        headers: {
            0: { sorter: false },
            3: { sorter: "digit" },
            6: { sorter: "digit" }
        },
    });
});

var refreshRowLabel = (function () {
    $("#rowsLoaded").html($(".table tr").length - 1 + " history records of " + $("#totalRecords").val() + " loaded");
});

var loadMoreRecords = (function () {
    $("#check-status").show();
    var lastRowNumber = $("#admin tr").length -1;
    var rowNumber = {
'lastRowNumber': lastRowNumber
    };

    if (lastRowNumber == 0 || lastRowNumber > parseInt($("#totalRecords").val())) {
        return false;
    } else {

        var url = getURL();

        $.get(url + 'Home/HistoryRecords', rowNumber, (function (data) {
            $(".wrapper").show();

            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                var dataRow =
                    "<tr>" +
                    "<td>" + item.DateTimeStr + "</td>" +
                    "<td>" + item.UserEmail + "</td>" +
                    "<td>" + item.Product + "</td>" +
                    "<td>" + item.SerialNumber + "</td>" +
                    "<td>" + item.ImageFileTypeStr + "</td>" +
                    "<td>" + item.UserTypeStr + "</td>" +
                    "<td class='last-cell'>" + item.VersionStr + "</td>" +
                    "</tr>";

                var tbody = $('#admin').children('tbody');
                tbody.append(dataRow);
                $('#admin').trigger("update");
                sortTable();
                refreshRowLabel();
            };

            $(".wrapper").hide();
        })
        ).done(function () {
            $("#check-status").hide();
        });
    }
});

var getURL = (function () {
    var url = $(location).attr('href');
    return url.substring(url.length - 1, url.length) == "/" ? url : url + "/";
});
