

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