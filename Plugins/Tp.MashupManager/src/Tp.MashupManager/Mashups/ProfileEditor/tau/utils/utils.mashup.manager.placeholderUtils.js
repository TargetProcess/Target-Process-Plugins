/*global Tp*/
define([
], function() {

    var PlaceholderUtils = function() {
    };

    PlaceholderUtils.prototype = {
        trim: function(str) {
            return str.replace(/^\s*/, '').replace(/\s*$/, '');
        },

        placeholderToUrl: function(str) {
            return str.split('_').join('/');
        },

        urlToPlaceholder: function(str) {
            var withoutRoot = str.toLowerCase().indexOf('http') === 0 ? new Tp.URL(str).path : str;
            var placeholder = withoutRoot;

            var indexOfAspx = withoutRoot.toLowerCase().indexOf('.aspx');
            if (indexOfAspx >= 0) {
                placeholder = withoutRoot.substr(0, indexOfAspx);
            }
            if (placeholder[0] == '/') {
                placeholder = placeholder.substring(1, placeholder.length);
            }

            return placeholder.split('/').join('_');
        }
    };

    return PlaceholderUtils;
});
