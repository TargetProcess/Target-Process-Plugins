define(function(require) {
    'use strict';

    var React = require('libs/react/react-ex'),
        _ = require('Underscore');

    return React.createClass({
        displayName: 'Workflows entity type selector',
        render: function() {
            var workflows = _.chain(this.props.workflows)
                .sortBy(function(workflow) {
                    return workflow.root ? '' : workflow.name.toLowerCase();
                }, this)
                .map(function(workflow) {
                    return (<option value={workflow.id} key={workflow.id}>{workflow.name || 'Project workflow'}</option>);
                }, this)
                .value();

            var selectedWorkflowStates = _.map(this.props.selectedWorkflow.entityStates, function(state) {
                return (
                    <li className="tau-category-items-workflow-settings-state-list__item" key={'state' + state}>
                        {state}
                    </li>
                );
            }, this);

            return (
                <li className={'tau-category-items-workflow-settings__item'}
                    key={this.props.entityType}
                    data-entitytype={this.props.entityType}>
                    <em className={'tau-entity-icon tau-entity-icon--' + this.props.entityType}>
                        {this.props.terms.getTerms(this.props.entityType).iconSmall}
                    </em>
                &nbsp;
                    <select value={this.props.selectedWorkflow.id}
                        className={"tau-category-items-workflow-settings-select i-role-team-workflow-" + this.props.group}
                        onChange={this.props.selectWorkflow}>
                        {workflows}
                    </select>
                    <ul className="tau-category-items-workflow-settings-state-list" key={'states' + this.props.selectedWorkflow.id}>
                        {selectedWorkflowStates}
                    </ul>
                </li>);
        }
    });
});