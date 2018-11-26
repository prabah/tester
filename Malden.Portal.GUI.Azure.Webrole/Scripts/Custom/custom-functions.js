/// <reference path="../jquery-1.8.2.intellisense.js" />
/// <reference path="../jquery.maskedinput-1.3.1.js" />
/// <reference path="md5.js" />

$(function () {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || Globalize.parseDate(value, "dd-MMM-yyyy") !== null;
    }
});

$(document).ready(function () {
    if ($("#RegistrationCode").val() !== undefined) {
        if ($("#RegistrationCode").val().length === 0) {
            $("#RegistrationCode").disabled = "";
            $("#RegistrationCode").focus();
            strictKeys();
            customMethods();
        }
    }
});

//$(document).ready(function () {
//    $('.list .table tr').hover(function () {
//        $(this).addClass('tr-hover');
//    }, function () {
//        $(this).removeClass('tr-hover');
//    });
//});

$(document).ready(function () {
    var homePage = $('#user').length;
    if (homePage === 0) {
        $("#wrap-inner").css({
            "backgroundRepeat": "repeat-x ",
            "background-color": "rgba(0, 0, 0, 0)"
        });
    }

    //$("#userproducts tr:odd").addClass("odd-rows");
    //$(".table tr:odd").addClass("odd-rows");

    $("#DateOfExpiry").datepicker({ dateFormat: "dd-M-yy" });
    $("#releaseDate").datepicker({ dateFormat: "dd-M-yy" });
    $("#Release_DateOfRelease").datepicker({ dateFormat: "dd-M-yy" });
});

$('#RegistrationCode').bind('paste', function (e) {
    $(e.target).keyup(getInput);
});

function getInput(e) {
    var ele = $("#RegistrationCode");
    ele.val($.trim(ele.val()));
    ele.val(ele.val().replace(/ /g, ''));


    if (ele.val().length >= 11) {
        if (IsCorrectFormat(ele.val()) == null) {
            ele.val('');
        }
    }
}


function IsCorrectFormat(str) {
    var reg = "";
    switch (str.length) {
        case 4:
            reg = "^[0-9]{4}$";
            break;
        case 5:
            reg = "^[0-9]{5}$";
            break;
        case 11:
            reg = /[0-9]+.-[A-Fa-f0-9]+$/;
            break;
    }

    var result = str.match(reg);
    return result;
}

var strictKeys = function () {
    $("#RegistrationCode").keydown(function (event) {
        $.trim($("#RegistrationCode").val());
        var strLength = $("#RegistrationCode").val().length;

        var keyResult = $.inArray(event.keyCode, [8, 9, 13, 16, 17, 18, 19, 20, 32, 35, 36, 37, 38, 39, 45, 46])

        if (keyResult == -1) {
            if (strLength === 5 && keyResult == -1) {
                $("#RegistrationCode").val($("#RegistrationCode").val() + "-");
            }
            else if (strLength > 10 && keyResult == -1) {
                event.preventDefault();
            }
        }

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


function duplicatedSerial(serialText) {
    var duplicated = false;

    var serialNumber = 0;

    switch (serialText.length) {
        case 4:
            serialNumber = serialText;
            break;
        case 5:
            serialNumber = serialText.substring(2, 4);
            break;
        case 11:
            serialNumber = serialText.substring(2, 4);
            break;

    }

    $('#userproducts tr').each(function () {
        var serial = $(this).find("td").eq(1).html();

        if (serial != undefined) {
            if (serial.substring(1, 5) === serialText) {
                $(".field-validation-valid").text("Serial Number already exists.");
                $(".field-validation-valid").css({ "display": "block", "color": "red", "font-weight": "bold" });
                $(".field-validation-error").text("Serial Number already exists.");
                $(".field-validation-error").css({ "display": "block", "color": "red", "font-weight": "bold" });
                duplicated = true;
            }
        }
    });
    return duplicated;
}

var showErrorMessage = (function (errorMessage) {
    if ($(".field-validation-valid").length)
        ele = $(".field-validation-valid");
    else
        ele = $(".field-validation-error");



    ele.text(errorMessage);
    ele.css({ "display": "block", "color": "red", "font-weight": "bold" });

});

$("#RegistrationCode").bind('blur', function (e) {
    if (e.which >= 97 && e.which <= 122) {
        var newKey = e.which - 32;
        e.keyCode = newKey;
        e.charCode = newKey;
    }

    var ele = $("#RegistrationCode");

    if (ele.val().length >= 4) {
        if (IsCorrectFormat(ele.val()) == null) {
            showErrorMessage("Invalid format");
            return false;
        }
    }

    ele.val((ele.val()).toUpperCase());

    var duplicated = duplicatedSerial(ele.val());

    if (duplicated) {
        showErrorMessage("Serial number already registered");
        return false;

    }

    if (ele.val().length > 0) {
        validSerial(ele.val(), '/Home/IsValidSerial');
    }
});

var validSerial = function IsValidSerial(pid, postURL) {
    var ele;

    if ($(".field-validation-valid").length)
        ele = $(".field-validation-valid");
    else
        ele = $(".field-validation-error");

    ele.text("");
    ele.css("display", "none");

    var data = { 'id': pid };
    $.post(postURL, data,
    function (data) {
        if (data == "False") {
            ele.text("Invalid Serial Number.");
            ele.css({ "display": "block", "color": "red", "font-weight": "bold" });
            strictKeys();
            return false;
        }
        else {
            return true;
        }

    });
}

var disableToolip = $("#registerButton").click(function () {
    $("#Password").removeAttr('title');
});

var validateSerial = $("#addItem").click(function () {
    var ele;

    if ($(".field-validation-valid").length)
        ele = $(".field-validation-valid");
    else
        ele = $(".field-validation-error");
    var visible = ele.text().length;

    var serialText = $("#RegistrationCode").val();

    if (serialText.length < 4) {
        ele.text("Invalid Serial Number.");
        ele.css({ "display": "block", "color": "red", "font-weight": "bold" });
        return false;
    }

    if (serialText.length >= 11) {
        if (IsCorrectFormat(serialText) == null) {
            ele.text("Invalid serial number.");
            ele.css({ "display": "block", "color": "red", "font-weight": "bold" });
            return false;
        }
    }

    var duplicated = duplicatedSerial($("#RegistrationCode").val());

    if (duplicated) return false;


    return ele.text().length === 0;
});

var showDialog = function (dialogName, e) {
    var x = e.pageX - $(document).scrollLeft() + 30;
    var y = e.pageY - $(document).scrollTop();

    $(dialogName).css("visibility", "visible");
    $(dialogName).dialog({ width: 600, resizable: false, position: [x, y], modal: true });
    return false;
};

var customMethods = function () {
    deleteProduct;
    deleteRelease;
    deleteMaintenanceContract;
    deleteReleaseImage;
    highlightMenu;
    blockUser;
    addNotesHistory;
    validateSerial;
    disableToolip;
    validatePassword;
    confirmPassword;
};

var deleteMaintenanceContract = $(".deleteMaintenance").click(function () {
    $('#dialog-confirm')
        .data('module', 'Maintenance')
        .data('id', $(this).data('id'))
        .dialog('open');

    return false;
});

var deleteProduct = $(".deleteProduct").click(function () {
    $('#dialog-confirm')
        .data('module', 'Product')
        .data('id', $(this).data('id'))
        .dialog('open');

    return false;
});

function AddHistory(data) {
    var pathArray = window.location.href.split('/');
    var view = pathArray[4];


    if (view == null) {
        $.post('Home/AddHistory', data, null);
    }
    else if (view.toLowerCase() === "archive" || pathArray[3].toLowerCase() === "home") {
        $.post('/Home/AddHistory', data, null);
    }

    return true;
}

var addSoftwareHistory =
    $(".downloadRelease").click(function () {
        var data = { 'id': $(this).data('id'), 'userType' : $(this).data('user') };
        AddHistory(data);
        return true;
    });


var validatePassword = $(".user-inputpassword").blur(function () {
    var pattern = new RegExp(/^(?=[\x21-\x7E]*[0-9])(?=[\x21-\x7E]*[A-Z])(?=[\x21-\x7E]*[a-z])(?=[\x21-\x7E]*[\x21-\x2F|\x3A-\x40|\x5B-\x60|\x7B-\x7E])[\x21-\x7E]{8,}$/);
    var valid = pattern.test($(".user-inputpassword").val());
    if (valid) {
        $(this).css("border-color", '#e2e2e2');
    }
    else {
        $(this).css("border-color", 'red');
    }
    return valid;
});

var confirmPassword = $("#ConfirmPassword").blur(function () {
    if ($("#ConfirmPassword").val() === $(".user-inputpassword").val()) {
        $("#ConfirmPassword").css("border-color", '#e2e2e2');
    }
    else {
        $("#ConfirmPassword").css("border-color", 'red');
    }
});

var addNotesHistory = $(".releasenotes").click(function () {
    var data = { 'id': $(".releasenotes").data('id') };
    AddHistory(data);
    return true;
});

var deleteRelease = $(".deleteRelease").click(function () {
    $('#dialog-confirm')
        .data('module', 'Release')
        .data('id', $(this).data('id'))
        .dialog('open');

    return false;
});

var deleteReleaseImage = $(".deleteReleaseImage").click(function () {
    $('#dialog-confirm')
        .data('module', 'ReleaseImage')
        .data('id', $(this).data('id'))
        .dialog('open');

    return false;
});

var blockUser = $(".blockUser").click(function () {
    $('#dialog-confirm')
       .data('module', 'Account')
       .data('id', $(this).data('id'))
       .dialog('open');

    return false;
});


var showError = $(".errorTest").click(function () {
    var $flash = $('<div id="flash" style="display:none;">');
    $flash.html("XXXX error!");
    $flash.toggleClass('@flash.ClassName');
    $('body').prepend($flash);
    $flash.slideDown('slow');
    $flash.click(function () { $(this).slideToggle('highlight'); });

    $('#flash').delay(400).fadeIn('normal', function () {
        $(this).delay(2500).fadeOut();
    });
});

if ($("#dialog-reset").dialog !== undefined) {
    $('#dialog-reset').dialog({
        autoOpen: false,
        resizable: false,
        modal: true,
        buttons: [
                  {
                      text: 'OK',
                      click: function () {
                          var id = $("#dialog-reset").data('id');
                          var module = $("#dialog-confirm").data('module');
                          //Reset logic
                      }
                  },
                  {
                      text: 'Cancel',
                      click: function () {
                          $(this).dialog('close');
                      }
                  }
        ]
    });
}

if ($("dialog-confirm").dialog !== undefined) {
    $('#dialog-confirm').dialog({
        autoOpen: false,
        resizable: false,
        modal: true,

        buttons: [
                  {
                      text: 'Delete',
                      click: function () {
                          var id = $("#dialog-confirm").data('id');
                          var module = $("#dialog-confirm").data('module');
                          var postURL = "";
                          switch (module) {
                              case ("Release"):
                                  postURL = '/Release/Delete';
                                  break;
                              case ("Product"):
                                  postURL = '/Product/Delete';
                                  break;
                              case ("ReleaseImage"):
                                  postURL = '/Release/DeleteImage';
                                  break;
                              case ("Maintenance"):
                                  postURL = '/Maintenance/Delete';
                                  break;
                              case ("Account"):
                                  postURL = '/Account/Block';
                          }
                          DeleteItem(id, postURL);
                      }
                  },
                  {
                      text: 'Cancel',
                      click: function () {
                          $(this).dialog('close');
                      }
                  }
        ]
    });
}

function DeleteItem(pid, postURL) {
    var data = { 'id': pid };
    $.post(postURL, data,
    function (data) {
        if (data == "Deleted") {
            if (location !== undefined) {
                var location = encodeURIComponent(location.href);
            }

            else {
                window.location.reload();
            }
        }
        else {
            alertMessage(data);
        }
        $('#dialog-confirm').dialog('close');
    });
}

var alertMessage = (function (data) {
    var $flash = $('<div id="flash" style="display:none;">');
    $flash.html(data);
    $flash.toggleClass('flash-error');
    $('body').prepend($flash);
    $flash.slideDown('slow');
    $flash.click(function () { $(this).slideToggle('highlight'); });

    $('#flash').delay(400).fadeIn('normal', function () {
        $(this).delay(2500).fadeOut();
    });
});

function getAlldata() {
    var serialNumber = $("#SerialNumber").val();
    $.getJSON("SerialDetails/" + serialNumber, null, function (data) {
        if (data == "0") {
            alertMessage("Invalid serial number!");
            fadeout();
            return false;
        }

        if (!data.Product.IsMaintained) {
            alertMessage("Product type is not maintained!");
            fadeout();
        }
        else {
            $("#ProductName").html("Product Name: " + data.Product.Name);
            fadeIn();
        }
    });
}

var fadeout = function loadLogoutBox() {
    $("#maintain").css("display", "block");
    $('#maintain').fadeIn("slow");
    $("#maintain").css({ "opacity": "0.3" });
};

var fadeIn = function () {
    $("#DateOfExpiry").removeAttr("disabled");
    $("#maintain").css({ 'z-index': '1' });
    $("#maintain").css({ 'background-color': 'none' });
    $("#maintain").css({ 'opacity': '9999' });
};

var highlightMenu = $('ul#menuPrimary').find('li > a').each(function () {
    var path = window.location.href;
    var pathArray = window.location.href.split('/');
    var host = pathArray[3];
    var view = pathArray[4];
    var element = "";

    if ((host != "Home" & host.length > 0) || (host == "Home" & view === "Archive")) {
        breadCrumbs(pathArray);
        switch (host) {
            case "Product":
                element = $('#menu-item-450');

                break;
            case "Release":
                element = $('#menu-item-550');
                break;
            case "Account":
                if (view == "Administrators") {
                    element = $('#menu-item-650');
                }
                else {
                    element = $('#menu-item-79');
                }
                break;
            case "Maintenance":
                element = $('#menu-item-750');
                break;
        }
    } else {
        $("#breadcrumbs").html("");
        $("#breadcrumbs").hide();
    }
    $(element).addClass('active');
});

function breadCrumbs(pathArray) {
    var loc1 = pathArray[0];
    var loc2 = pathArray[1];
    var loc3 = pathArray[2];
    var host = pathArray[3];
    var view = pathArray[4];

    var url = "";

    var htmlText = "<ul>";
    url = loc1 + "//" + loc2 + "/" + loc3 + "/" + host;
    htmlText += getBreadcrumbUrl(host, url);

    if (view !== undefined) {
        url += "/" + view;
        htmlText += getBreadcrumbUrl(view, "#");
    }

    htmlText += "</ul>";
    $("#breadcrumbs").html(htmlText);
}

function getBreadcrumbUrl(linkText, url) {
    return "<li><a href=" + url + ">" + linkText + "</a><span class='divider'>></span></li>";
}

