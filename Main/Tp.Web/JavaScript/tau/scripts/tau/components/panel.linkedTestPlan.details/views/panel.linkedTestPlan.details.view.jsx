define(function(require) {
    var React = require('react'),
        TestPlanManageRunView = require('jsx!tau/components/panel.linkedTestPlan.details/views/linkedTestPlan.details.manageRun.view'),
        TestPlanOpenedRunCountView =
            require('jsx!tau/components/testPlan.openedRunCount/views/testPlan.openedRunCount.view'),
        TestPlanRunResultsView = require('jsx!tau/components/testPlanRun.results/views/testPlanRun.results.view');

    return React.createClass({
        displayName: 'PanelTestPlan',
        render() {
            return (
                <div className="tau-container ui-collapsible">
                    <div className="tau-panel-testplan-details-header ui-collapsible-header expanded">
                        <div className="ui-children-container">
                            <span className="ui-label">{this.props.testPlanDetailsHeaderName}</span>
                            <span className="tau-icon tau-icon_name_newwindow"
                                title={`Open Related ${this.props.testPlanDetailsHeaderName}`}
                                onClick={this.props.store.openTestPlanTab.bind(this.props.store)}
                            />
                        </div>
                    </div>
                    <div className="tau-container">
                        <div className="tau-panel-testplan-details ui-collapsible__body">
                            <TestPlanManageRunView
                                onBtnClickHandler={this.props.store.onBtnClickHandler.bind(this.props.store)}
                                runAction={this.props.store.runAction}
                                isCreateRunAction={this.props.store.isCreateRunAction}/>
                            <TestPlanOpenedRunCountView
                                testPlanRunIconSmall={this.props.testPlanRunIconSmall}
                                openedRunCount={this.props.store.openedRunCount}
                                onOpenTestPlanRunsTab={this.props.store.openTestPlanRunsTab.bind(this.props.store)} />
                            <TestPlanRunResultsView label="Last Run Results" results={this.props.store.results} />
                        </div>
                    </div>
                </div>
            );
        }
    });
});
