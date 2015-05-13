define(function(require) {
    'use strict';

    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        WorkflowUtils = require('../utils/workflow.utils'),
        WorkflowSelector = require('jsx!./workflowSelector');

    return React.createClass({
        displayName: 'WorkflowSelectorWithControls',

        getInitialState: function() {
            return {
                customizedWorkflows: this.props.customizedWorkflows
            };
        },

        onWorkflowChanged: function(data) {
            this.state.customizedWorkflows = data.customizedWorkflows;
            this.props.onWorkflowChanged(this.state.customizedWorkflows);
        },

        saveWorkflow: function() {
            var customizedWorkflows = _.compact(_.map(this.props.entityTypes, function(entityType) {
                return this.props.customizedWorkflows[entityType] ||
                    WorkflowUtils.findDefaultWorkflow(this.props.workflows, entityType);
            }, this));
            this.props.onWorkflowSaved(customizedWorkflows);
        },

        render: function() {
            return (
                <div>
                    <WorkflowSelector
                        key={this.props.group.id}
                        group={this.props.group}
                        workflows={this.props.workflows}
                        entityTypes={this.props.entityTypes}
                        terms={this.props.terms}
                        workflowHelpData={this.props.workflowHelpData}
                        customizedWorkflows={this.state.customizedWorkflows}
                        onWorkflowChanged={this.props.onWorkflowChanged}
                    />
                    <div className="tau-settings-actions">
                        <button className="tau-btn tau-primary i-role-action-submit" onClick={this.saveWorkflow}>Save</button>
                    </div>
                </div>);
        }
    });
});