define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        displayName: 'TestPlanRunBugCount',

        getDefaultProps() {
            return {
                openBugsCount: 0
            };
        },

        createTitle() {
            return 'Open: ' + this.props.openBugsCount + '/Total: ' + this.props.totalBugsCount;
        },

        onClick() {
            this.props.clickHandler();
        },

        render() {
            if (!this.props.totalBugsCount) {
                return null;
            }

            var unitClassName = classNames({
                'tau-board-unit tau-total-bug-unit': true,
                'active': !this.props.isOnBugsTab
            });

            return (
                <div className={unitClassName} title={this.createTitle()} onClick={!this.props.isOnBugsTab && this.onClick}>
                    <span className="tau-entity-icon tau-entity-icon--bug">{this.props.bugIconSmall}</span>
                    <div className="tau-board-unit__value-open">{this.props.openBugsCount}</div>
                    <div className="tau-board-unit__value-total">{this.props.totalBugsCount}</div>
                </div>
            );
        }
    });
});
