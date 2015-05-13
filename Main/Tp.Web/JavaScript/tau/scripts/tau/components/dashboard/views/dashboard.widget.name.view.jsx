define(function(require) {
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');
    var ContentEditableMixin = require('./../contentEditable');

    /**
     * @class DashboardWidgetNameView
     * @extends ReactComponent
     */
    var DashboardWidgetNameView = React.createClass({
        displayName: 'WidgetName',

        statics: {
            /** @name DashboardWidgetNameView.loadingMessage */
            loadingMessage: 'Loading...'
        },

        mixins: [ContentEditableMixin],

        render: function() {
            var className = classNames({
                'tau-dashboard-widget__title': true,
                't3-edit': this.props.isEditable && this.props.contentEditable
            });
            if (this.props.isEditable && this.props.name) {
                return <div className={className}
                    onDoubleClick={this.contentEditableStart}
                    onBlur={this.contentEditableDone}
                    onKeyDown={this.contentEditableKeyDown}
                    spellCheck="false"
                    contentEditable={this.props.contentEditable}>{this.props.name}</div>;
            } else {
                return <div className={className}
                    spellCheck="false">{this.props.name || DashboardWidgetNameView.loadingMessage}</div>;
            }
        }
    });

    return DashboardWidgetNameView;
});
