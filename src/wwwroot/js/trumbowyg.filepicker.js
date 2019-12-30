(function ($) {
    'use strict';

    // Plugin default options
    var defaultOptions = {
    };

    var isSupported = function () {
        return typeof FileReader !== 'undefined';
    };

    // Adds the language variables
    $.extend(true, $.trumbowyg, {
        langs: {
            en: {
                filepicker: 'Add existing file'
            }
        }
    });

    // Adds the extra button definition
    $.extend(true, $.trumbowyg, {
        plugins: {
            filepicker: {
                shouldInit: isSupported,
                init: function (trumbowyg) {
                    // Fill current Trumbowyg instance with the plugin default options
                    trumbowyg.o.plugins.filepicker = $.extend(true, {},
                        defaultOptions,
                        trumbowyg.o.plugins.filepicker || {}
                    );

                    var btnDef = {
                        isSupported: isSupported,
                        fn: function () {
                            trumbowyg.saveRange();

                            $('#fileSelectModal').modal('show');
                            epFileSelectSource = "editor";
                        }
                    };

                    trumbowyg.addBtnDef('filepicker', btnDef);
                }
            }
        }
    });
})(jQuery);
