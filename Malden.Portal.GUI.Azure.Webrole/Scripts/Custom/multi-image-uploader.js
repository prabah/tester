/// <reference path="jquery-1.8.2.js" />
/// <reference path="_references.js" />
var fileStatus = false;

$(document).ready(function ()
{
    $(document).on("click", "#fileUpload", beginUpload);
    ko.applyBindings(uploaders);
});

var beginUpload = function () {
    $('#fileUpload').attr('disabled', 'disabled');
    var fileControl = document.getElementById("selectFile");
    var releaseId = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);

    if (fileControl.files.length > 0) {
        var selectedFileType = $("#fileType").val();

        uploaders.uploaderCollection.removeAll();
        for (var i = 0; i < fileControl.files.length; i++) {
            checkFileStatus((fileControl.files[i].name), releaseId, selectedFileType);


            if (fileStatus) {
                cful = Object.create(chunkedFileUploader);
                cful.init(fileControl.files[i], i);
                uploaders.uploaderCollection.push(cful);

                $(".progressBar").progressbar(0);
                uploaders.uploadAll();
            }
            else {
                $("#statusMessageLabel").text("Upload canceled by user!");
            }
        }
    }
    $('#fileUpload').removeAttr("disabled");
};

var uploaders = {
    uploaderCollection: ko.observableArray([]),
    uploadAll: function () {
        for (var i = 0; i < this.uploaderCollection().length; i++) {
            var cful = this.uploaderCollection()[i];
            cful.uploadMetaData();
        }
    }
};

checkFileStatus = function (fileName, releaseId, fileType) {
    $.ajax({
        type: "GET",
        async: false,
        url: "/Release/FileStatus?fileName=" + fileName
            + "&releaseId=" + releaseId
            + "&fileType=" + fileType
    }).done(function (status) {
        //if (status = 1) {
        //    alert("File name already exists");
        //    fileStatus = false;
        //}
        //else {
        //    alert("File already uploaded for the selected type and release");
        //    fileStatus = false;
        //}
        switch (status) {
            case 0:
                fileStatus = true;
                break;
            case 1: 
            case 2:
                var result = confirm("File already exists, do you want to replace it?")
                //alert("File already uploaded for the selected type and release");
                if (result) {
                    $.ajax({
                        type: "GET",
                        async: false,
                        url: "/Release/DeleteFile?fileName=" + fileName
                            + "&releaseId=" + releaseId
                            + "&fileType=" + fileType
                    }).done(function (status) {
                        fileStatus = true;
                    });
                    
                }
                else {
                    fileStatus = false;
                }
                break;
            case 3:
                alert("Total files count cannot be greater than three for the release");
                fileStatus = true;
                break;

        }
    });
};

    var chunkedFileUploader =
    {
        maxRetries: 3,
        blockLength: 4194304,
        numberOfBlocks: 1,
        currentChunk: 1,
        retryAfterSeconds: 3,
        fileToBeUploaded: null,
        size: 0,
        fileIndex: 0,
        name: "",

        init: function (file, index)
        {
            this.fileToBeUploaded = file;
            this.size = file.size;
            this.name = file.name;
            this.fileIndex = index;
        },

        uploadMetaData: function ()
        {
            this.numberOfBlocks = Math.ceil(this.size / this.blockLength);
            this.currentChunk = 1;
            var releaseId = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);

            var locationUrl = location.pathname;

            var selectedFileType = $("#fileType").val();
            if (selectedFileType <= 0)
            {
                alert("Please select the file type");
            }

            $.ajax({
                type: "POST",
                async: false,
                url: "/Release/SetMetadata?blocksCount=" + this.numberOfBlocks
                    + "&fileName=" + this.name
                    + "&fileSize=" + this.size
                    + "&fileIndex=" + this.fileIndex
                    + "&imageType=" + selectedFileType
                    + "&releaseId=" + releaseId,
            }).done(function (state)
            {
                if (state.success == true) {
                    var cufl = uploaders.uploaderCollection()[state.index];
                    cufl.displayStatusMessage(cufl, "Starting Upload");
                    cufl.sendFile(cufl);
                }
            }).fail(function ()
            {
                this.displayStatusMessage("Failed to send MetaData");
            });

        },

        sendFile: function (uploader)
        {
            var start = 0,
                end = Math.min(uploader.blockLength, uploader.fileToBeUploaded.size),
                retryCount = 0,
                sendNextChunk, fileChunk;
            this.displayStatusMessage(uploader,"");

            var cful = uploader;

            uploader.fileToBeUploaded.fileName = "test fileName.txt";

            sendNextChunk = function () {
                fileChunk = new FormData();

                if (uploader.fileToBeUploaded.slice) {
                    fileChunk.append('Slice', uploader.fileToBeUploaded.slice(start, end));
                }
                else if (uploader.fileToBeUploaded.webkitSlice) {
                    fileChunk.append('Slice', uploader.fileToBeUploaded.webkitSlice(start, end));
                }
                else if (uploader.fileToBeUploaded.mozSlice) {
                    fileChunk.append('Slice', uploader.fileToBeUploaded.mozSlice(start, end));
                }
                else {
                    displayStatusMessage(cful, operationType.UNSUPPORTED_BROWSER);
                    return;
                }
                jqxhr = $.ajax({
                    async: true,
                    url: ('/Release/UploadChunk?id=' + uploader.currentChunk + "&fileIndex=" + uploader.fileIndex + "&imageFileType=" + $("#fileType").val()),
                    data: fileChunk,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST'
                }).fail(function (request, error) {
                    if (error !== 'abort' && retryCount < maxRetries) {
                        ++retryCount;
                        setTimeout(sendNextChunk, retryAfterSeconds * 1000);
                    }
                    if (error === 'abort') {
                        displayStatusMessage(cful, "Aborted");
                    }
                    else {
                        if (retryCount === maxRetries) {
                            displayStatusMessage(cful, "Upload timed out.");
                            resetControls();
                            uploader = null;
                        }
                        else {
                            displayStatusMessage(cful, "Resuming Upload");
                        }
                    }
                    return;
                }).done(function (state) {

                    if (state.error || state.isLastBlock) {
                        cful.displayStatusMessage(cful, state.message);
                        location.reload();
                        return;
                    }
                    ++cful.currentChunk;
                    start = (cful.currentChunk - 1) * cful.blockLength;
                    end = Math.min(cful.currentChunk * cful.blockLength, cful.fileToBeUploaded.size);
                    retryCount = 0;
                    cful.updateProgress(cful);
                    if (cful.currentChunk <= cful.numberOfBlocks) {
                        sendNextChunk();
                    }
                });
            };
            sendNextChunk();
        },

        displayStatusMessage: function (uploader, message)
        {
            $("#statusMessage" + uploader.fileIndex).text(message);
        },

        updateProgress: function (uploader)
        {
            var progress = uploader.currentChunk / uploader.numberOfBlocks * 100;
            if (progress <= 100)
            {
                $("#progressBar" + uploader.fileIndex).progressbar("option", "value", parseInt(progress));
                uploader.displayStatusMessage(uploader, "Uploaded " +  Math.round(progress) + "%");
            }
        }
    }