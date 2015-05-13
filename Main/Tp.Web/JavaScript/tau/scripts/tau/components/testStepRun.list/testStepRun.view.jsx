define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');
    var _ = require('Underscore');
    var ActiveStatusSelector = require('jsx!tau/components/testStepRun.list/testStepRun.activeStatusSelector');
    var Status = require('jsx!tau/components/testStepRun.list/testStepRun.status');
    var sanitize = require('tau/core/templates-factory').jqote2customFunctions.sanitize;

    return React.createClass({
        displayName: 'TestStepRun',

        render() {
            return (
                <div className={classNames({
                    'test-case-run-description_rows': true,
                    'tau-teststep': true,
                    'active': this.props.active
                })}>
                    <div className="test-case-run-description_colom">
                        <div className="tau-in-text i-role-editable" dangerouslySetInnerHTML={{__html: sanitize(this.props.run.description)}} />
                    </div>
                    <div className="test-case-run-description_colom">
                        <div className="tau-in-text i-role-editable" dangerouslySetInnerHTML={{__html: sanitize(this.props.run.result)}} />
                    </div>
                    <div className="test-case-run-description_colom">
                        {this.renderStateColumn()}
                    </div>
                </div>
            );
        },

        renderStateColumn() {
            if (this.props.active) {
                return (<ActiveStatusSelector run={this.props.run}/>)
            } else {
                return (<Status run={this.props.run} editable={this.props.editable} disabled={this.props.disabled}/>)
            }
        }
    });
});