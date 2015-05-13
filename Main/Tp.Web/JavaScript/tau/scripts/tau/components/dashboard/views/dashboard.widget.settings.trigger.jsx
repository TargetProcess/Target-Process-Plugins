define(function(require) {
    var $ = require('jQuery');
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        getInitialState: function() {
            return {
                isLoadingSettingsContent: false,
                isActive: false
            };
        },

        _onTriggerClicked: function(evt) {
            evt.preventDefault();
            evt.stopPropagation();

            if (this.state.isLoadingSettingsContent) {
                return;
            }

            this.setState({
                isLoadingSettingsContent: true
            });

            var $trigger = $(this.refs.trigger.getDOMNode());
            this.props
                .insertSettings($trigger, this._onToggleTriggerActive)
                .always(function() {
                    this.setState({
                        isLoadingSettingsContent: false
                    });
                }.bind(this));
        },

        _onToggleTriggerActive: function(isActive) {
            this.setState({
                isActive: isActive
            });
        },

        render: function() {
            var className = classNames({
                'i-role-dashboard-widget-settings-toggle': true,
                'tau-dashboard-widget__settings': true,
                'tau-dashboard-widget__settings--loading': this.state.isLoadingSettingsContent,
                'active': this.state.isActive
            });

            return (
                <button
                    ref="trigger"
                    className={className}
                    onClick={this._onTriggerClicked}>
                </button>
            );
        }
    });
});