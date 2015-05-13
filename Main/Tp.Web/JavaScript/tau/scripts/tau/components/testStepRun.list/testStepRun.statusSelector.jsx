define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var $ = require('jQuery');

    return React.createClass({
        displayName: 'TestStepRun.StatusSelector',

        componentDidMount() {
            var $node = $(this.getDOMNode());
            $node.hide();
            $node.show(100);

            // required for blur to work
            $node.find('button').first().focus();
        },

        render() {
            var passed = <button className="tau-btn tau-btn-big tau-passed i-role-steprun-status-selector" key='p' onClick={_.partial(this.props.run.changeStatus, true)}>
                Passed
            </button>;
            var failed = <button className="tau-btn tau-btn-big tau-failed i-role-steprun-status-selector" key='f' onClick={_.partial(this.props.run.changeStatus, false)}>
                Failed
            </button>;
            var notRun = <button className="tau-btn tau-btn-big tau-notrun i-role-steprun-status-selector" key='n' onClick={_.partial(this.props.run.changeStatus, null)}>
                Not Run
            </button>;

            var buttons;
            if (this.props.run.runned) {
                buttons = this.props.run.passed ?
                    [passed, failed, notRun] :
                    [failed, passed, notRun];
            } else {
                buttons = [notRun, passed, failed];
            }

            return (
                <div className="test-case-description_switch" onBlur={this.onBlur}>
                    {buttons}
                </div>
            );
        },

        onBlur(e) {
            _.defer(function() {
                if (document.activeElement && $(document.activeElement).hasClass('i-role-steprun-status-selector')) {
                    return;
                }
                this.props.onBlur();
            }.bind(this));
        }
    });
});