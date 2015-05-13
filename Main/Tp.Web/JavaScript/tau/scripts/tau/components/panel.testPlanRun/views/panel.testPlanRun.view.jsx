define(function(require) {
    var React = require('react'),
        TestPlanRunResultsView = require('jsx!tau/components/testPlanRun.results/views/testPlanRun.results.view'),
        TestPlanRunStartRun = require('jsx!tau/components/panel.testPlanRun/views/testPlanRun.startRun.view'),
        BugsCountView = require('jsx!tau/components/testPlanRun.bugCount/views/testPlanRun.bugCount.view');

    return React.createClass({
        displayName: 'PanelTestPlanRunRun',
        render() {
            return (
                <div className="tau-container ui-collapsible tau-panel-testplanrun-run">
                    <div className="ui-collapsible__body">
                        <TestPlanRunStartRun isRunnable = {this.props.store.isRunnable} runAction = {this.props.store.runAction} onStartRun = {this.props.store.startRun.bind(this.props.store)}/>
                        <TestPlanRunResultsView label="Run Results" results={this.props.store.results}>
                            <BugsCountView label="Bugs Count" bugIconSmall={this.props.bugIconSmall} openBugsCount={this.props.store.openBugsCount}
                                totalBugsCount = {this.props.store.totalBugsCount} clickHandler = {this.props.store.clickHandler}
                                isOnBugsTab = {this.props.store.isOnBugsTab}/>
                        </TestPlanRunResultsView>
                    </div>
                </div>
            );
        }
    });
});
