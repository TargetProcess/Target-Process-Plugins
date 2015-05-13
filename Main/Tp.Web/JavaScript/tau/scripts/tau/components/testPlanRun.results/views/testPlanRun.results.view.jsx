define(function(require) {
    var React = require('react'),
        _ = require('Underscore');

    return React.createClass({
        displayName: 'TestPlanRunResults',
        _createTitle() {
            var results = this.props.results;
            return _.sprintf('Passed: %d, Failed: %d, Not Run: %d, On Hold: %d / Blocked: %d',
                results.passed, results.failed, results.notrun, results.onhold, results.blocked);
        },
        render() {
            var stats = this.props.results;
            if (_.isEmpty(stats)) {
                return null;
            }
            var showExtraCounts = stats.onhold || stats.blocked, extraCounts = null;
            if (showExtraCounts) {
                extraCounts = [
                    (<div key="onhold" className="tau-board-unit__value-onhold">{stats.onhold || 0}</div>),
                    (<span key="separator" className="tau-board-unit__separate">/</span>),
                    (<div key="blocked" className="tau-board-unit__value-blocked">{stats.blocked || 0}</div>)
                ];
            }
                        
            return (
                <div className="tau-board-unit_type_testcases-detailed-counter" title={this._createTitle()}>
                    <div className="tau-board-unit_type_testcases-detailed-counter__label">{this.props.label}</div>
                    <div className="tau-board-unit__value-passed">{stats.passed}</div>
                    <div className="tau-board-unit__value-failed">{stats.failed}</div>
                    <div className="tau-board-unit__value-notrun">{stats.notrun}</div>
                    {extraCounts}
                    {this.props.children}
                </div>
            );
        }
    });
});
