define(function(require) {
    var React = require('libs/react/react-ex'),
        t = React.PropTypes,
        StateSelectorHeader = require('jsx!./stateSelectorHeader.view'),
        EntityStateGroups = require('jsx!./entityStateGroups.view');

    return React.createClass({
        displayName: 'TeamEntityStatePropertyView',
        propTypes: {
            currentStates: t.shape({
                parentState: t.shape({id: t.number, name: t.string}).isRequired,
                teamState: t.shape({
                    team: t.shape({id: t.number, name: t.name}).isRequired,
                    state: t.shape({id: t.number, name: t.name}).isRequired
                })
            }).isRequired,
            actions: t.shape({
                openDropDown: t.func.isRequired,
                closeDropDown: t.func.isRequired,
                changeState: t.func.isRequired,
                changeTeamState: t.func.isRequired
            }).isRequired,
            isLoading: t.bool.isRequired,
            workflowData: t.object,
            entityStateGroups: t.array
        },
        render: function() {
            if (this.props.entityStateGroups) {
                return (
                    <div className="multiple-team-state-select tau-active">
                        <StateSelectorHeader {...this.props.currentStates} onClick={this.props.actions.closeDropDown}/>
                        <EntityStateGroups
                            items={this.props.entityStateGroups}
                            currentStates={this.props.currentStates}
                            workflowData={this.props.workflowData}
                            actions={this.props.actions}/>
                    </div>
                );
            } else {
                return (
                    <div className="multiple-team-state-select">
                        <StateSelectorHeader {...this.props.currentStates}
                            isLoading={this.props.isLoading}
                            onClick={this.props.actions.openDropDown}/>
                    </div>
                );
            }
        }
    });
});
