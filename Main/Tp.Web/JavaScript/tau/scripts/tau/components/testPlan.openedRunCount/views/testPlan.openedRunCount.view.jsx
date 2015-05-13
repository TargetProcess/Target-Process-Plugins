define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        mixins: [React.addons.PureRenderMixin],
        displayName: 'TestPlanOpenedRunCount',
        render() {
            var unitClassName = classNames({
                'tau-board-unit': true,
                'active': !this.props.isOnTestPlanRunsTab
            });
            return (
                <div className={unitClassName} title="Open Test Plan Runs"
                    onClick={!this.props.isOnTestPlanRunsTab && this.props.onOpenTestPlanRunsTab}>
                    <span className="tau-entity-icon tau-entity-icon--testrun">{this.props.testPlanRunIconSmall}</span>
                    <span className="tau-board-unit__value">{this.props.openedRunCount}</span>
                </div>
            );
        }
    });
});
