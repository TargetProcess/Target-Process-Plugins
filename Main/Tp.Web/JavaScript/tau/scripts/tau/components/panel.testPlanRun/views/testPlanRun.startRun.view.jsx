define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TestPlanRunStartRun',

        propTypes: {
            onStartRun: React.PropTypes.func.isRequired
        },

        _handleOnClick() {
            this.props.onStartRun();
        },

        render() {
            return (
                <button
                    disabled={!this.props.isRunnable}
                    className="tau-btn tau-btn-medium tau-btn-play"
                    onClick={this._handleOnClick}
                    type="button">
                    {this.props.runAction}
                </button>
            );
        }
    });
});
