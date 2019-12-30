

// Admin menu Tooltip - this option will toggle the placement of the Tooltip on Desktop and Mobile
$(".admmenu-item-link").tooltip({
    placement: function (ctx, src) {
        if ($(window).width() > 991) {
            return "right";
        } else {
            return "bottom";
        }
    }
});

function ConvertToValidXHTML() {
    if ($("#editor").length) {
        parsedDOM = new DOMParser().parseFromString($("#editor").trumbowyg('html'), 'text/html');
        parsedDOM = new XMLSerializer().serializeToString(parsedDOM);
        /<body>(.*)<\/body>/im.exec(parsedDOM);
        parsedDOM = RegExp.$1;
        $("#editor").trumbowyg('html', parsedDOM);
    }
}

function readURL(input) {
    var url = input.value;
    var ext = url.substring(url.lastIndexOf('.') + 1).toLowerCase();
    if (input.files && input.files[0] && (ext === "png" || ext === "jpeg" || ext === "jpg" || ext === "gif" || ext === "svg")) {
        reader.readAsDataURL(input.files[0]);
    } else {
        $("#coverimageupload").val('');
        alert('Only Images Are Allowed!');
    }
}

// Get Font Awesome icon from extension
function getFaIconFromExt(ext) {
    switch (ext) {
        case "doc":
        case "docx":
            return "fa-file-word";
        case "pdf":
            return "fa-file-pdf";
        case "zip":
            return "file-archive";
        case "xls":
        case "xlsx":
            return "fa-file-excel";
        case "txt":
            return "fa-file-alt";
        default:
            return "fa-question";
    }
}

// Edit page global variables
var epFileSelectSource = "";

// Are we one the edit page?
if ($("#editor").length) {
    // Add Trumbowyg editor to div #editor
    $("#editor").trumbowyg({
        btnsDef: {
            // Create a new dropdown
            image: {
                dropdown: ['insertImage', 'base64'],
                ico: 'insertImage'
            }
        },
        // Redefine the button pane
        btns: [
            ['viewHTML'],
            ['formatting'],
            ['strong', 'em', 'del'],
            ['superscript', 'subscript'],
            ['link'],
            ['image'], 
            ['filepicker'],
            ['justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull'],
            ['unorderedList', 'orderedList'],
            ['horizontalRule'],
            ['removeformat'],
            ['fullscreen']
        ],
        imageWidthModalEdit: false,
        semantic: false,
        autogrow: false
    });

    $("#btnPublish").on("click", function () {
        ConvertToValidXHTML();
        return true;
    });

    $("#btnDraft").on("click", function () {
        ConvertToValidXHTML();
        return true;
    });

    $("#btnCoverImageUpload").on("click", function () {
        coverimageupload.click();
    });

    $("#btnCoverImageSelect").on("click", function () {
        $('.fileSelectModalFiles').hide();
        epFileSelectSource = "coverimage";
    });

    $("#btnFileSelectCancel").on("click", function () {
        epFileSelectSource = "";
    });

    $('#fileSelectModal').on('hidden.bs.modal', function (e) {
        epFileSelectSource = "";
        $('.fileSelectModalFiles').show();
    });

    //$("#fileSelectModal .modal-body div div").on("click", function () {
    $("#fileSelectModal .fileSelectModalImages div, #fileSelectModal .fileSelectModalFiles ul a").on("click", function () {
        var fileName = $(this).data("filename");
        var ext = $(this).data("filetype");

        // File was selected for cover image
        if (epFileSelectSource === "coverimage") {
            $('#coverimageupload').removeAttr('style');
            $('#coverimagepreview').css("background-image", "url(" + fileName + ")");
            $('#coverimageselectbuttons').hide();
            $('#coverimageremovebutton').show();
            $('#Post_CoverImage').val(fileName);

            $('#fileSelectModal').modal('hide');
            epFileSelectSource = "";
        }

        // File was selected from editor
        if (epFileSelectSource === "editor") {
            // Add image/file to editor
            var attachment;

            // Image file
            if (ext === "png" || ext === "jpeg" || ext === "jpg" || ext === "gif" || ext === "svg") {
                attachment = $('<img>');
                attachment.attr('src', fileName);
            }
            // Other file type
            else {
                // TODO: A href around image, with filename and size(?)
                // link(FA fileName (size))
                attachment = $('<span />', { class: 'attachment' })
                    .append($('<a />', { href: fileName, title: fileName })
                        .html($('<i />', { class: 'fa ' + getFaIconFromExt(ext) }))
                        .append(' ' + fileName));
                
            }
            // Restore the previous position
            $('#editor').trumbowyg('restoreRange');

            // Insert text at the current position
            $('#editor').trumbowyg('execCmd', {
                cmd: 'insertHtml',
                param: attachment.prop('outerHTML'),
                forceCss: false
            });

            $('#fileSelectModal').modal('hide');
            epFileSelectSource = "";
        }
    });

    /*
    setTimeout(function () {
        $.trumbowyg.langs.en.base64 = 'Upload image';
    }, 1000);
    */

    var reader = new FileReader();
    reader.onload = function (e) {
        $('#coverimageupload').removeAttr('style');
        $('#coverimagepreview').css("background-image", "url(" + e.target.result + ")");
        $('#coverimageselectbuttons').hide();
        $('#coverimageremovebutton').show();
    };

    // Set the newly uploaded as background on the cover image
    $("#coverimageupload").change(function () {
        readURL(this);
    });

    // Remove cover image
    $('#coverimageremovebutton a[role=button]').click(function () {
        $('#coverimageselectbuttons').show();
        $('#coverimageremovebutton').hide();
        $('#coverimagepreview').removeAttr('style');
        $('#coverimageupload').val('');
        $('#Post_CoverImage').val('');
    });

    // If editing existing post and it has a cover image, show it.
    var coverimage = $('#Post_CoverImage').val();
    if (coverimage !== '') {
        $('#coverimageupload').removeAttr('style');
        $('#coverimageupload').val('');
        $('#coverimagepreview').css("background-image", "url(" + coverimage + ")");
        $('#coverimageselectbuttons').hide();
        $('#coverimageremovebutton').show();
    }
}

// Attach event on delete buttons on the Files page
if ($('#filesForm').length) {
    $('#filesForm .card button').on('click', function() {
        $('#filesForm #SelectedFile').val($(this).attr('data-filepath'));
    });

    $('#filesForm #deleteModal button[data-dismiss=modal]').on('click', function () {
        $('#filesForm #SelectedFile').val('');
    });

    $('#filesForm #UploadFile').on('change', function () {
        //get the file name
        var fileName = $(this).val().replace(/^.*[\\\/]/, '');
        //replace the "Choose a file" label
        $(this).next('.custom-file-label').html(fileName);
    });
}

