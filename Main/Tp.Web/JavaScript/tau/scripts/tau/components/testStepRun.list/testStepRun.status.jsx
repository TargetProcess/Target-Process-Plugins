define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');
    var StatusSelector = require('jsx!tau/components/testStepRun.list/testStepRun.statusSelector');
    var _ = require('Underscore');

    return React.createClass({
        displayName: 'TestStepRun.Status',

        getInitialState() {
            return {showStatusSelector: false};
        },

        componentWillReceiveProps() {
            this.setState(this.getInitialState());
        },

        render() {
            var run = this.props.run;
            var statusClass = 'tau-';
            var title;
            if (run.runned) {
                statusClass += run.passed ? 'passed': 'failed';
                title = run.passed ? 'Passed': 'Failed';
            } else {
                statusClass += 'notrun';
                title = 'Not Run';
            }

            var classes = {
                'tau-btn': true,
                'tau-btn-big': true
            };
            classes[statusClass] = true;

            var statusSelector = this.state.showStatusSelector &&
                (<StatusSelector key="statusSelector" run={run} onBlur={_.partial(this.toggleStatusSelector, false)}/>);

            return (
                <div>
                    <button
                        className={classNames(classes)}
                        onClick={_.partial(this.toggleStatusSelector, true)}
                        disabled={this.props.disabled}
                    >
                        {title}
                    </button>
                    {statusSelector}
                </div>
            );
        },

        toggleStatusSelector: function(show, e) {
            if (this.props.editable) {
                this.setState({showStatusSelector: show});
            }

            if (e) {
                // to prevent form submit in tp2
                e.stopPropagation();
                e.preventDefault();
            }
        }
    });
});