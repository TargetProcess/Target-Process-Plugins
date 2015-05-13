define(function(require) {
    var $ = require('jQuery');
    var Class = require('tau/core/class');

    function loadTableau() {
        var def = $.Deferred();

        require(['libs/tableau/tableau'], () => {
            def.resolve(window.tableauSoftware);
        });

        return def.promise();
    }

    return Class.extend({
        init({placeholder, reportUrl}) {
            this._placeholder = placeholder;
            this._reportUrl = reportUrl;
        },

        render() {
            return loadTableau().then(this._renderWith.bind(this));
        },

        _renderWith(tableau) {
            var container = document.createElement('div');
            container.style.width = this._placeholder.clientWidth + 'px';
            container.style.height = this._placeholder.clientHeight + 'px';
            $(this._placeholder).empty().append(container);

            var initDef = $.Deferred();

            try {
                var viz;

                var options = {
                    width: this._placeholder.clientWidth,
                    height: this._placeholder.clientHeight,
                    hideTabs: true,
                    hideToolbar: true,
                    onFirstInteractive: () => {
                        initDef.resolve({
                            resize: () => viz.setFrameSize(this._placeholder.clientWidth, this._placeholder.clientHeight),
                            destroy: viz.dispose.bind(viz)
                        });
                    }
                };

                viz = new tableau.Viz(container, this._reportUrl, options);

            } catch(e) {
                initDef.reject(e);
            }

            return initDef;
        }
    });
});