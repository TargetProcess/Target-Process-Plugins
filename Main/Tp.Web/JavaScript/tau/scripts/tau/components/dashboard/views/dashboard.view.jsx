define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var React = require('react');
    var ErrorLayout = require('jsx!./dashboard.error.layout.view');
    var ColumnLayout = require('jsx!./dashboard.column.layout.view');

    /**
     * @class DashboardView
     * @extend ReactComponent
     */
    return React.createClass({
        displayName: 'Dashboard',

        getDefaultProps: function() {
            return {
                _instanceEventNamespace: _.uniqueId('.dashboard-view')
            };
        },

        componentDidMount: function() {
            $(window).on('resize' + this.props._instanceEventNamespace, this._onWindowResize);
            this.props.model.widgetAdded.add(this._onWidgetAdded, this);
        },

        componentWillUnmount: function() {
            $(window).off(this.props._instanceEventNamespace);
            this.props.model.widgetAdded.remove(this);
        },

        _onWindowResize: function() {
            this.props.model.getResizeBroker().notifyResized();
        },

        _onWidgetAdded: function(event) {
            if (event.scrollToWidget) {
                var $scrollTo = $(event.placeholder).closest('.i-role-dashboard-widget');
                var $scrollParent = $scrollTo.scrollParent();
                $scrollParent.animate({
                    scrollTop: $scrollTo.offset().top - $scrollParent.offset().top + $scrollParent.scrollTop()
                }, 500);
            }
        },

        scrollToTop: function() {
            var $node = $(this.getDOMNode()).children('.tau-dashboard-columns-layout__body');

            $node.scrollTop(0);
        },

        render: function() {
            var LayoutView = this._getLayoutView();
            return <LayoutView layout={this.props.model.layoutModel} />;
        },

        _getLayoutView: function() {
            switch (this.props.model.layoutModel.layoutType) {
                case 'columns':
                    return ColumnLayout;
                default:
                    return ErrorLayout;
            }
        }
    });
});
