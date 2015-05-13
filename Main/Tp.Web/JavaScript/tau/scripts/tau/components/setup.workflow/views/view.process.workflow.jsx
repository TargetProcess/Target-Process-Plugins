define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        StateItemPositioningService = require('../services/state.item.positioning.service'),
        StateItemView = require('jsx!./state.item.process.workflow'),
        SortableStates = require('jsx!./sortable-states'),
        AddStateButtonView = require('jsx!./add.state.button'),
        TeamView = require('jsx!./view.team.workflow'),
        AddTeamWorkflowButton = require('jsx!./add.team.workflow.button'),
        TextUtils = require('tau/utils/utils.text'),
        Feedback = require('jsx!tau/components/feedback/views/feedback.view');

    var ADD_STATE_FADE_OUT_TIME = 500;
    var addStateFadeOutTimeout = null;

    return React.createClass({
        displayName: 'ProcessWorkflow',
        getInitialState: function() {
            return {
                states: this.props.states,
                teamWorkflows: this.props.teamWorkflows,
                isReordering: false,
                hoveredState: {
                    side: 'none', id: null
                }
            };
        },

        componentWillReceiveProps: function(newProps) {
            if (this.state.isReordering) {
                return;
            }
            if (!_.isUndefined(newProps.states)) {
                this.setState({states: newProps.states});
            }
            if (!_.isUndefined(newProps.teamWorkflows)) {
                this.setState({teamWorkflows: newProps.teamWorkflows});
            }
        },

        _onReorderStart: function() {
            this.setState({ isReordering: true });
        },

        _onReorderStop: function() {
            this.setState({ isReordering: false });
        },

        _onReorderProgress: function(newStates, newTeamWorkflows) {
            this.setState({
                states: newStates,
                teamWorkflows: newTeamWorkflows
            });
        },

        _onReorderCancel: function() {
            this.setState({
                states: this.props.states,
                teamWorkflows: this.props.teamWorkflows
            });
        },

        _isInteractionEnabled: function() {
            return this.props.allowEditing && !this.state.isReordering;
        },

        _mkHoveredStateSetter: function(side, stateId) {
            var hoveredState = { side: side, id: stateId };

            var updateState = function(hoveredState) {
                if (this.isMounted()) {
                    this.setState({ hoveredState: hoveredState });
                }
            }.bind(this, hoveredState);

            return function(side, updateState) {
                clearTimeout(addStateFadeOutTimeout);

                if (side === 'none') {
                    addStateFadeOutTimeout = setTimeout(updateState, ADD_STATE_FADE_OUT_TIME);
                } else {
                    updateState();
                }
            }.bind(this, side, updateState);
        },

        _mkStateView: function(state) {
            return React.createElement(StateItemView, {
                key: state.id,
                id: state.id,
                name: state.name,
                isNew: state.isNew,
                isSortable: this.props.allowEditing && !state.isInitial && !state.isFinal,
                allowDrag: this.props.allowEditing && !state.disableDrag,
                isEnabled: this._isInteractionEnabled(),
                isLoading: this.props.updatingStateId === state.id,
                stateSettings: this.props.stateSettings,
                renameStateAction: this.props.renameStateAction,
                leftTriggered: this._mkHoveredStateSetter(state.isInitial ? 'right' : 'left', state.id),
                rightTriggered: this._mkHoveredStateSetter(state.isFinal ? 'left' : 'right', state.id),
                outTriggered: this._mkHoveredStateSetter('none', null),
                width: state.width + '%'
            });
        },

        _generateDefaultTeamWorkflow: function(states, teamWorkflows) {
            var teamWorkflowParentStateIndex = parseInt((states.length - 1) / 2, 10);
            var dsl = _.map(states, function(state, index) {
                var teamWorkflow = state.name + (index === teamWorkflowParentStateIndex ? ', Team State' : '');
                return state.name + ': ' + teamWorkflow;
            }).join('\n');
            var title = TextUtils.getUniqueName('Team Workflow', _.pluck(teamWorkflows, 'name'));
            return {title: title, dsl: dsl};
        },

        _getTeamWorkflowText: function(states, teamWorkflow) {
            var teamStates = _.groupBy(teamWorkflow.entityStates, _.complexProperty('parentEntityState.id'));
            return _.map(states, function(state) {
                var subStates = teamStates[state.id];
                var teamWorkflow = subStates ? _.pluck(subStates, 'name').join(', ') : '';
                return state.name + ': ' + teamWorkflow;
            }).join('\n');
        },

        _isAdderActive: function(previous, next) {
            var state = this.state.hoveredState;

            if (state.side === 'left') {
                return state.id === next.id;
            } else if (state.side === 'right') {
                return state.id === previous.id;
            }

            return false;
        },

        render: function() {
            if (!this.state.states) {
                return <div className="tau-loader"></div>;
            }

            var seed = {
                previousState: {id: null},
                states: []
            };

            var stateItemPositioningService = new StateItemPositioningService(this.state.states, this.state.teamWorkflows);
            stateItemPositioningService.positioningStates();

            _.each(this.state.teamWorkflows, function(teamWorkflow) {
                var uniqueParentStatesCount = _.chain(teamWorkflow.entityStates)
                    .map(_.complexProperty('parentEntityState.id'))
                    .unique()
                    .size()
                    .value();
                // if all sub workflow states are mapped to one parent we can drag this state
                if (uniqueParentStatesCount > 1) {
                    _.each(teamWorkflow.entityStates, function(teamState) {
                        if (teamState.isInitial || teamState.isFinal) {
                            var parentState = _.findWhere(this.state.states, {id: teamState.parentEntityState.id});
                            parentState.disableDrag = true;
                        }
                    }.bind(this));
                }
            }.bind(this));

            var defaultTeamWorkflow = this._generateDefaultTeamWorkflow(this.state.states, this.state.teamWorkflows);
            var states = _.reduce(this.state.states, function(acc, state) {
                if (!_.isEmpty(acc.states)) {
                    var addStateButton = React.createElement(AddStateButtonView, {
                        key: acc.previousState.id + ':' + state.id,
                        addStateAction: this.props.addStateAction.bind(this, acc.previousState, state),
                        isActive: this._isAdderActive(acc.previousState, state),
                        isEnabled: this._isInteractionEnabled()
                    });

                    acc.states.push(addStateButton);
                }

                acc.states.push(this._mkStateView(state));
                acc.previousState = state;

                return acc;
            }, seed, this).states;

            var teamWorkflows = _.map(this.state.teamWorkflows, function(teamWorkflow) {
                return React.createElement(TeamView, {
                    key: teamWorkflow.id,
                    teamWorkflow: teamWorkflow,
                    processTeams: this.props.processTeams,
                    entityType: this.props.entityType,
                    teamWorkflowText: this._getTeamWorkflowText(this.state.states, teamWorkflow),
                    editTeamWorkflowAction: this.props.editTeamWorkflowAction,
                    deleteTeamWorkflowAction: this.props.deleteTeamWorkflowAction,
                    urlBuilder: this.props.urlBuilder,
                    openEntityViewAction: this.props.openEntityViewAction,
                    renameStateAction: this.props.renameStateAction,
                    stateSettings: this.props.stateSettings,
                    isEnabled: this._isInteractionEnabled(),
                    updatingStateId: this.props.updatingStateId,
                    entityTerms: this.props.entityTerms
                });
            }, this);

            var wrapperStyles = {
                minWidth: stateItemPositioningService.getMinWidth(),
                height: stateItemPositioningService.getHeight(),
                minHeight: '100%'
            };

            var addTeamWorkflowButton = this.props.canAddTeamWorkflow ?
                <AddTeamWorkflowButton addTeamWorkflowAction={this.props.addTeamWorkflowAction}
                    isEnabled={this._isInteractionEnabled()}
                    defaultTeamWorkflow={defaultTeamWorkflow} /> :
                null;

            return (
                <div ref="scrollable" className="i-role-views-workflow-container tau-workflow-contents textSelectionDisabled">
                    <div className="tau-container__title header-h3">
                        <Feedback featureName="Workflow Setup"/>
                        {this.props.process.name}
                        <span className="tau-icon-general tau-icon-r-arrow-big"></span>
                        Workflow for <span className={'tau-entity-color tau-entity-color--' + this.props.entityType}>{this.props.entityTerms.name}</span>
                    </div>
                    <div className="tau-container__body">
                        <div className="process-grid__wrap" style={wrapperStyles}>
                            <SortableStates
                                scrollableContainerRef={this.refs.scrollable}
                                initialStates={this.props.states}
                                currentStates={this.state.states}
                                initialTeamWorkflows={this.props.teamWorkflows}
                                onReorderStart={this._onReorderStart}
                                onReorderStop={this._onReorderStop}
                                onReorderProgress={this._onReorderProgress}
                                onReorder={this.props.reorderStateAction}
                                onReorderCancel={this._onReorderCancel}
                            >
                                {states}
                            </SortableStates>
                            {teamWorkflows}
                            {addTeamWorkflowButton}
                        </div>
                    </div>
                </div>
            );
        }
    });
});
