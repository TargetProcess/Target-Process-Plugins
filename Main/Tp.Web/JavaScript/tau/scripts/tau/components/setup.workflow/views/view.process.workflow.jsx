define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        StateItemPositioningService = require('../services/state.item.positioning.service'),
        StateItemView = require('jsx!./state.item.process.workflow'),
        SortableStates = require('jsx!./sortable-states'),
        AddStateButtonView = require('jsx!./add.state.button'),
        TeamView = require('jsx!./view.team.workflow'),
        TextUtils = require('tau/utils/utils.text');

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

        _mkStateView: function(state, defaultTeamWorkflowText) {
            return React.createElement(StateItemView, {
                key: state.id,
                id: state.id,
                name: state.name,
                isNew: state.isNew,
                isSortable: this.props.allowEditing && !state.isInitial && !state.isFinal,
                allowDrag: this.props.allowEditing && !state.disableDrag,
                isEnabled: this._isInteractionEnabled(),
                isLoading: this.props.updatingStateId === state.id,
                canAddTeamWorkflow: state.isInitial && this.props.canAddTeamWorkflow,
                stateSettings: this.props.stateSettings,
                renameStateAction: this.props.renameStateAction,
                leftTriggered: this._mkHoveredStateSetter(state.isInitial ? 'right' : 'left', state.id),
                rightTriggered: this._mkHoveredStateSetter(state.isFinal ? 'left' : 'right', state.id),
                outTriggered: this._mkHoveredStateSetter('none', null),
                addTeamWorkflowAction: this.props.addTeamWorkflowAction,
                defaultTeamWorkflowText: defaultTeamWorkflowText,
                width: state.width + '%'
            });
        },

        _generateDefaultTeamWorkflowText: function(states, teamWorkflows) {
            var teamWorkflowParentStateIndex = states.length > 2 ? parseInt((states.length - 1) / 2, 10) : -1;
            var dsl = _.map(states, function(state, index) {
                var teamWorkflow = index === teamWorkflowParentStateIndex ? ' Open, Done' : '';
                return state.name + ':' + teamWorkflow;
            }).join('\n');
            var title = TextUtils.getUniqueName('Sub workflow', _.pluck(teamWorkflows, 'name'));
            return {title: title, dsl: dsl};
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

            var defaultTeamWorkflowText = this._generateDefaultTeamWorkflowText(this.state.states, this.state.teamWorkflows);
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

                acc.states.push(this._mkStateView(state, defaultTeamWorkflowText));
                acc.previousState = state;

                return acc;
            }, seed, this).states;

            var teamWorkflows = _.map(this.state.teamWorkflows, function(teamWorkflow) {
                return React.createElement(TeamView, {
                    key: teamWorkflow.id,
                    teamWorkflow: teamWorkflow,
                    entityType: this.props.entityType,
                    deleteTeamWorkflowAction: this.props.deleteTeamWorkflowAction
                });
            }, this);

            var wrapperStyles = {
                minWidth: stateItemPositioningService.getMinWidth(),
                height: stateItemPositioningService.getHeight(),
                minHeight: '100%'
            };

            return (
                <div ref="scrollable" className="i-role-views-workflow-container tau-workflow-contents textSelectionDisabled">
                    <div className="tau-container__title header-h3">
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
                        </div>
                    </div>
                </div>
            );
        }
    });
});
