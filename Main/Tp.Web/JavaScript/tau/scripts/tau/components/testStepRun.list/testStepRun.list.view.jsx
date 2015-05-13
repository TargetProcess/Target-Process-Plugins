define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var TestStepRun = require('jsx!tau/components/testStepRun.list/testStepRun.view');

    return React.createClass({
        displayName: 'TestStepRunList',

        render() {
            var activeRun = _.findWhere(this.props.runs, {runned: false});
            var runs = _.map(this.props.runs, function(run) {
                run = _.extend({}, run, {
                    changeStatus: _.partial(this.props.changeStatus, run)
                });
                var active = this.props.editable && activeRun && run.id === activeRun.id;
                var editable = this.props.editable && (run.runned || active);
                return (<TestStepRun run={run} active={active} editable={editable} disabled={this.props.disabled} key={run.id} />);
            }.bind(this));

            return (
                <div className="test-case_list i-role-list i-role-teststeprun-list">
                    <div className="test-case-run-description">
                        <div className="test-case-run-description_title ">
                            <div className="test-case-run-description_colom">Step</div>
                            <div className="test-case-run-description_colom">Expected result</div>
                            <div className="test-case-run-description_colom">Status</div>
                        </div>
                    </div>
                    {runs}
                </div>
            );
        }
    });
});