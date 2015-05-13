define(function(require) {
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');
    var _ = require('Underscore');
    var $ = require('jQuery');

    var dependencies = [];

    return React.defineClass(dependencies, function() {
        return {
            STATUSES: {
                enabled: 'enabled',
                disabled: 'disabled',
                inProgress: 'inProgress'
            },

            getInitialState: function() {
                return {
                    inProgress: false
                };
            },

            componentWillReceiveProps: function(nextProps) {
                if (this.props.isEnabled != nextProps.isEnabled) {
                    this.setState({ inProgress: false });
                }
            },

            _toggleStatus: function() {
                if (!this.state.inProgress) {
                    this.props.onStatusToggled();
                    this.setState({ inProgress: true });
                }
            },

            _getButtonLabel: function() {
                var label = (this.props.isEnabled) ? 'Deactivate' : 'Activate';
                if (this.state.inProgress) {
                    label = '';
                }

                return label;
            },

            render: function() {
                var buttonClassName = classNames({
                    'tau-btn': true,
                    'tau-btn-big': true,
                    'tau-btn-loader': this.state.inProgress,
                    'tau-success': !this.state.inProgress && !this.props.isEnabled,
                    'tau-cancel': !this.state.inProgress && this.props.isEnabled
                });

                return (
                    <button onClick={this._toggleStatus} className={buttonClassName} type="button">
                        {this._getButtonLabel()}
                    </button>
                );
            }
        }
    });

});
