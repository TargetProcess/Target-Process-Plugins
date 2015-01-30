define(function(require) {
    var _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        t = React.PropTypes,
        TeamIcon = require('jsx!./teamIcon.view');

    var WorkflowDataType = t.shape({
        canSetup: t.bool.isRequired,
        url: t.string
    });

    var DisabledTransitionHelp = React.createClass({
        propTypes: {
            workflowData: WorkflowDataType.isRequired
        },
        render: function() {
            return (
                <span
                    className="tau-help i-role-tooltipArticle"
                    data-url={this.props.workflowData.url}
                    data-can-setup={this.props.workflowData.canSetup}
                    data-article-id="entityState.noTransition"
                />
            );
        }
    });

    var EntityStateGroups = React.createClass({
        displayName: 'EntityStateGroups',
        propTypes: {
            items: t.array.isRequired,
            currentStates: t.object.isRequired,
            workflowData: WorkflowDataType.isRequired,
            actions: t.object.isRequired
        },

        _renderStateWithoutSubStates: function(entityState) {
            var isActive = entityState.id === this.props.currentStates.parentState.id;
            var isClickable = !isActive && entityState.isTransitionAllowed;
            var classes = React.addons.classSet({
                'multiple-team-state-select-list__item': true,
                'tau-active': isActive,
                'tau-disabled': !isActive && !entityState.isTransitionAllowed
            });
            return (
                <li key={entityState.id}
                    className={classes}
                    onClick={isClickable ? this.props.actions.changeState.bind(null, entityState.id) : null}>
                    <div className="multiple-team-state-select-list__team-list__item-value">{entityState.name}</div>
                    {isClickable || isActive ? null : <DisabledTransitionHelp workflowData={this.props.workflowData}/>}
                </li>
            );
        },


        _renderStateWithSubStates: function(parentState, team, shouldShowTeamLabel) {
            var isParentActive = parentState.id === this.props.currentStates.parentState.id;
            var isParentDisabled = !isParentActive && !parentState.isTransitionAllowed;
            var classes = React.addons.classSet({
                'multiple-team-state-select-list__team-list__item': true,
                'tau-active': isParentActive,
                'tau-disabled': isParentDisabled
            });
            return (
                <li key={parentState.id} className={classes}>
                    <div className="multiple-team-state-select-list__header">
                        <div className="multiple-team-state-select-list__team-list__item-value">{parentState.name}</div>
                        {shouldShowTeamLabel ? this._getTeamLabel(team) : null}
                    </div>
                    <ul className="multiple-team-sub-state-list">
                    {_.map(parentState.subStates, function(subState) {
                        var teamState = this.props.currentStates.teamState;
                        var isActive = isParentActive && teamState && subState.id === teamState.state.id;
                        var isClickable = !isActive && !isParentDisabled;

                        var classes = React.addons.classSet({
                            'multiple-team-sub-state-list__item': true,
                            'tau-active': isActive,
                            'tau-disabled': isParentDisabled
                        });
                        return (
                            <li key={subState.id}
                                className={classes}
                                onClick={isClickable ? this.props.actions.changeTeamState.bind(null, team.id, subState.id) : null}>
                                <div className="multiple-team-state-select-list__team-list__item-value">{subState.name}</div>
                                {isClickable || isActive ? null : <DisabledTransitionHelp workflowData={this.props.workflowData}/>}
                            </li>
                        );
                    }.bind(this))}
                    </ul>
                </li>
            );
        },

        _getTeamLabel: function(team) {
            return (
                <span className="tau-checkbox-label">
                    <TeamIcon name={team.icon}/>
                    {team.name}
                </span>
            );
        },

        render: function() {
            return (
                <ul className={'multiple-team-state-select-list'}>
                { _.map(this.props.items, function(group) {
                    var key = _.pluck(group.entityStates, 'id').join('-');
                    if (group.hasSubStates) {
                        return (
                            <li key={key} className="multiple-team-state-select-list__item has-sub-states">
                                <ul className="multiple-team-state-select-list__team-list">
                                {_.map(group.entityStates, function(entityState, i) {
                                    var isFirst = i === 0;
                                    return this._renderStateWithSubStates(entityState, group.team, isFirst);
                                }.bind(this))}
                                </ul>
                            </li>
                        );
                    } else {
                        var items = _.map(group.entityStates, function(entityState) {
                            return this._renderStateWithoutSubStates(entityState);
                        }.bind(this));
                        return <div key={key}>{items}</div>;
                    }
                }.bind(this))}
                </ul>
            )
        }
    });
    return EntityStateGroups;
});
