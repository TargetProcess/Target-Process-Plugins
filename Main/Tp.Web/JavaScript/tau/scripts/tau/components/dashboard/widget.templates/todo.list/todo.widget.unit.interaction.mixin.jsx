define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var EVENT_NAMESPACE = '.todo-widget-unit-interaction-mixin';

    return {
        componentDidMount() {
            this._getWidgetContainer().on(
                'click' + EVENT_NAMESPACE,
                '.tau-board-unit--editable',
                this._onUnitClick);
        },

        componentWillUnmount() {
            this._getWidgetContainer().off(EVENT_NAMESPACE);
        },

        _getWidgetContainer: function() {
            return $(this.getDOMNode()).closest('.i-role-dashboard-widget-content');
        },

        _onUnitClick(evt) {
            if (!_.isFunction(this.props.handleUnitInteraction)) {
                return;
            }

            var $unit = $(evt.currentTarget);
            var $card = $unit.closest('.i-role-card');

            this.props.handleUnitInteraction({
                $unit: $unit,
                $card: $card,
                cardId: $card.data('id').toString(),
                unitId: $unit.data('unit-id').toString()
            });

            evt.stopPropagation();
        }
    };
});