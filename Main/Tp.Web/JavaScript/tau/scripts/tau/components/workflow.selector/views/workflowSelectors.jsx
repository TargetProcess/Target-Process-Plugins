define(function(require) {
    'use strict';

    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        WorkflowSelector = require('jsx!tau/components/workflow.selector/views/workflowSelector'),
        TermProcessor = require('tau/core/termProcessor');

    return React.createClass({
        displayName: 'WorkflowSelectors',

        getInitialState: function() {
            return {
                customizedWorkflows: {}
            };
        },

        componentWillUpdate: function(nextProps) {
            if (this.props.resetOnProcessChange && nextProps.process.id != this.props.process.id) {
                this.setState({customizedWorkflows: {}});
            }
        },

        componentDidUpdate: function(prevProps) {
            if (prevProps.process.id != this.props.process.id) {
                var data = _.map(this.props.groups, function(group) {
                    return {
                        group: group,
                        customizedWorkflows: {}
                    };
                });
                this.props.onWorkflowChanged(data);
            }
        },

        onWorkflowChanged: function(data) {
            this.state.customizedWorkflows[data.group] = data.customizedWorkflows;
            this.setState({customizedWorkflows: this.state.customizedWorkflows});
            var eventData = {
                group: data.group,
                customizedWorkflows: _.pluck(_.values(data.customizedWorkflows), 'id')
            };
            this.props.onWorkflowChanged([eventData]);
        },

        render: function() {
            var termProcessor = new TermProcessor(this.props.process.terms);
            if (!this.props.selectedGroupId) {
                return <div />;
            }

            return (
                <WorkflowSelector
                    key={this.props.selectedGroupId}
                    group={this.props.selectedGroupId}
                    workflows={this.props.process.workflows}
                    entityTypes={this.props.entityTypes}
                    terms={termProcessor}
                    workflowHelpData={this.props.workflowHelpData}
                    customizedWorkflows={this.state.customizedWorkflows[this.props.selectedGroupId] || {}}
                    onWorkflowChanged={this.onWorkflowChanged}
                />
            );
        }
    });
});