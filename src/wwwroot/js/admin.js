
var editorHandler = function () {

    function cancel() {
        var confirmed = confirm("Are you sure you want to cancel and loose changes?");

        if (confirmed)
            $("#editForm").submit();
    }

    function remove() {
        var confirmed = confirm("Are you sure you want to delete this post/page?");

        if (confirmed)
            $("#editForm").submit();
    }

    return {
        cancel: cancel,
        remove: remove
    };
};

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

// Add Trumbowyg editor to div #editor
if ($("#editor").length) {
    $("#editor").trumbowyg({
        
    });
}

