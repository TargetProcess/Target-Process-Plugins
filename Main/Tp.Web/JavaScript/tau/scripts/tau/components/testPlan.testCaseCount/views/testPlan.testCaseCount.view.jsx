define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        mixins: [React.addons.PureRenderMixin],

        propTypes: {
            onOpenTestPlanTab: React.PropTypes.func.isRequired
        },

        displayName: 'TestCaseCount',

        render() {
            var classes = classNames({
                'tau-board-unit': true,
                'active': !this.props.isOnTestPlanTab
            });
            return (
                <div className={classes} title="Test Case Count"
                    onClick={!this.props.isOnTestPlanTab && this.props.onOpenTestPlanTab}>
                    <span className="tau-entity-icon tau-entity-icon--testcase">{this.props.testCaseIconSmall}</span>
                    <span className="tau-board-unit__value">{this.props.testCaseCount}</span>
                </div>
            );
        }
    });
});