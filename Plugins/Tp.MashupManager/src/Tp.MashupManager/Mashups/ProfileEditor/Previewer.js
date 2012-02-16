tau.mashups
    .addDependency("MashupManager/PlaceholderUtils")
    .addDependency("libs/jquery/jquery")
    .addModule("MashupManager/Previewer", function (utils) {
        function previewer(config) {
            this._create(config);
        };

        previewer.prototype = {
            _create: function(config) {
                this.utils = new utils();
            },

            preview: function(placeholders) {
                var w = window.open(this._getUrl(placeholders));

//                $(w).load(function() {
//                    w.eval(mashup.Script);
//                });
            },

            _getUrl: function(placeholders) {
                var relativeUrl = '/Default.aspx';
                
                if (placeholders && placeholders.toLowerCase().indexOf('footerplaceholder'.toLowerCase()) < 0) {
                    var first = this.utils.trim(placeholders.split(',')[0]);
                    relativeUrl = '/' + this.utils.placeholderToUrl(first) + '.aspx';
                }

                return new Tp.WebServiceURL(relativeUrl).url;
            }
        };
        return previewer;
    });