define(function(require) {
    var React = require('react'),
        TestPlanCreateRunView = require('jsx!tau/components/panel.testPlan.run/views/testPlan.createRun.view'),
        TestPlanOpenedRunCountView =
            require('jsx!tau/components/testPlan.openedRunCount/views/testPlan.openedRunCount.view'),
        TestPlanRunResultsView = require('jsx!tau/components/testPlanRun.results/views/testPlanRun.results.view');

    return React.createClass({
        displayName: 'PanelTestPlanRun',
        render() {
            return (
                <div className="tau-container ui-collapsible tau-panel-testplan-run">
                    <div className="ui-collapsible__body">
                        <TestPlanCreateRunView
                            onCreateRun={this.props.store.createRunAndNavigate.bind(this.props.store)} />
                        <TestPlanOpenedRunCountView
                            testPlanRunIconSmall={this.props.testPlanRunIconSmall}
                            openedRunCount={this.props.store.openedRunCount}
                            isOnTestPlanRunsTab={this.props.store.isOnTestPlanRunsTab}
                            onOpenTestPlanRunsTab = {this.props.store.openTestPlanRunsTab.bind(this.props.store)} />
                        <TestPlanRunResultsView label="Last Run Results" results={this.props.store.results} />
                    </div>
                </div>
            );
        }
    });
});
