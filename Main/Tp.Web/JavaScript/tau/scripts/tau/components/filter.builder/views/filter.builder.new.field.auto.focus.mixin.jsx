define(function(require) {
    var $ = require('jQuery');

    return {
        getInitialState() {
            return {
                _autoFocusItemId: null
            };
        },

        componentDidUpdate(prevProps, prevState) {
            if (!prevState._autoFocusItemId && this.state._autoFocusItemId) {
                var targetSelector = '.i-role-filter-builder__field__new-filter.i-role-filter-builder__field-' +
                    this.state._autoFocusItemId + ' .i-role-focus-target';
                $(this.getDOMNode()).find(targetSelector).focus(1);
                this.setState({_autoFocusItemId: null});
            }
        },

        autoFocusOn(autoFocusItemId) {
            this.setState({_autoFocusItemId: autoFocusItemId});
        }
    };
});