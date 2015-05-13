define(function(require) {
    var React = require('react');
    var _ = require('Underscore');

    return React.createClass({
        displayName: 'TestStepRun.ActiveStatusSelector',

        render() {
            var pass = _.partial(this.props.run.changeStatus, true);
            var fail = _.partial(this.props.run.changeStatus, false);
            return (
                <div className="tau-inline-group">
                    <button className="tau-btn tau-btn-green" onClick={pass} data-result='passed'>Pass</button>
                    <button className="tau-btn tau-btn-red" onClick={fail} data-result='failed'>Fail</button>
                </div>
            );
        }
    });
});