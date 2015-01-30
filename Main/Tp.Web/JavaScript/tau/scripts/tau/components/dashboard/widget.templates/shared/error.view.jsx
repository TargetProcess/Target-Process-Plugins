define(function(require) {
    var React = require('react');

    /**
     * @class WidgetErrorView
     * @extends ReactComponent
     */
    return React.createClass({
        displayName: 'WidgetErrorView',

        getDefaultProps: function() {
            return {
                header: 'Unable to display a widget content here',
                message: 'Some error occurred and we\'re not sure why'
            };
        },

        render: function() {
            return (
                <div>
                    <div>{this.props.header}</div>
                    <div
                        className="tau-widget-error-vew__message i-role-widget-error-view__message">
                        {this.props.message}
                    </div>
                </div>
            );
        }
    });
});
