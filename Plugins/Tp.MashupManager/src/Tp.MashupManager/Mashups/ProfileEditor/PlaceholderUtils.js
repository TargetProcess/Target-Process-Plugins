tau.mashups
    .addDependency("libs/jquery/jquery")
    .addModule("MashupManager/PlaceholderUtils", function () {
        function utils(config) {
        };

        utils.prototype = {
            trim: function(str){
                return str.replace(/^\s*/, "").replace(/\s*$/, "");
            },

            placeholderToUrl: function(str){
                return str.split('_').join('/');
            },

            urlToPlaceholder: function(str){
                var withoutRoot = str.toLowerCase().indexOf('http') == 0 ? new Tp.URL(str).path : str;
                var placeholder = withoutRoot;
                
                if(withoutRoot.toLowerCase().indexOf('.aspx') >= 0)
                {
                    placeholder = withoutRoot.substr(0,withoutRoot.indexOf('.aspx'));
                }
                if(placeholder[0] == '/'){
                    placeholder = placeholder.substring(1, placeholder.length);
                }

                return placeholder.split('/').join('_');
            }
        };
        return utils;
    });