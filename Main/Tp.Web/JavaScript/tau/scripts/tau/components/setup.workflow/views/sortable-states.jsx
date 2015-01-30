define(function(require) {
    var React = require('react'),
        _ = require('Underscore'),
        SortableStatesMixin = require('../mixins/sortable-states');

    var matchesParentEntityStateId = function(parentEntityStateId) {
        return function(teamState) {
            return teamState.parentEntityState.id === parentEntityStateId;
        };
    };

    return React.createClass({
        displayName: 'WorkflowSortableStates',

        getDefaultProps: function() {
            return {
                scrollableContainerRef: null,
                onReorderStart: _.identity,
                onReorderStop: _.identity,
                onReorderProgress: _.identity,
                onReorder: _.identity,
                onReorderCancel: _.identity,
                initialStates: null,
                currentStates: null,
                initialTeamWorkflows: null
            };
        },

        mixins: [SortableStatesMixin],

        getSortableOptions: function() {
            return {
                sortableHandleSelector: '.process-grid__state',
                onReorderStart: this.onReorderStart,
                onReorderEnd: this.onReorderEnd,
                onReorder: this.onReorder
            };
        },

        onReorderStart: function() {
            this.props.onReorderStart();
        },

        onReorder: function(key, overKey, placeAfter) {
            var currentStateId = parseInt(key, 10);
            var overStateId = parseInt(overKey, 10);

            var newStates = this._getNewStates(currentStateId, overStateId, placeAfter);
            var newTeamWorkflows = this._getNewTeamWorkflows(currentStateId, overStateId, newStates, placeAfter);
            this.props.onReorderProgress(newStates, newTeamWorkflows);
        },

        _getNewStates: function(currentStateId, overStateId, placeAfter) {
            var matchesCurrentId = _.matches({id: currentStateId});
            var matchesOverId = _.matches({id: overStateId});

            var states = this.props.initialStates;
            var currentState = _.find(states, matchesCurrentId);
            var statesWithoutCurrent = _.without(states, currentState);
            var overStateIndex = _.findIndex(statesWithoutCurrent, matchesOverId);

            var placeToMove = this._getInsertPosition(overStateIndex, 1, placeAfter);
            return _.insertAt(statesWithoutCurrent, currentState, placeToMove);
        },

        _getNewTeamWorkflows: function(currentStateId, overStateId, newStates, placeAfter) {
            var currentState = _.find(newStates, _.matches({id: currentStateId}));
            var currentStateIndex = _.indexOf(newStates, currentState);

            var siblings = {
                left: newStates[currentStateIndex - 1],
                right: newStates[currentStateIndex + 1]
            };

            var matches = {
                currentStateChild: matchesParentEntityStateId(currentStateId),
                overStateChild: matchesParentEntityStateId(overStateId),
                currentStateLeftSiblingChild: siblings.left ? matchesParentEntityStateId(siblings.left.id) : _.constant(false),
                currentStateRightSiblingChild: siblings.right ? matchesParentEntityStateId(siblings.right.id) : _.constant(false)
            };
            return _.map(this.props.initialTeamWorkflows, function(teamWorkflow) {
                return {
                    id: teamWorkflow.id,
                    name: teamWorkflow.name,
                    entityStates: this._getNewTeamStates(teamWorkflow.entityStates, currentState, matches, placeAfter)
                };
            }, this);
        },

        _getNewTeamStates: function(teamStates, currentState, matches, placeAfter) {
            if (_.all(teamStates, matches.currentStateChild)) {
                return teamStates;
            }
            var relatedTeamStates = _.filter(teamStates, matches.currentStateChild);
            if (relatedTeamStates.length) {
                var teamStatesWithoutCurrent = _.reject(teamStates, matches.currentStateChild);
                var overStateStartIndex = _.findIndex(teamStatesWithoutCurrent, matches.overStateChild);
                if (overStateStartIndex > -1) {
                    var overStatesCount = _.filter(teamStatesWithoutCurrent, matches.overStateChild).length;
                    var placeToMove = this._getInsertPosition(overStateStartIndex, overStatesCount, placeAfter);
                    teamStatesWithoutCurrent.splice(placeToMove, 0, relatedTeamStates);
                    return _.flatten(teamStatesWithoutCurrent);
                } else {
                    return teamStatesWithoutCurrent;
                }
            } else {
                var leftSiblings = _.filter(teamStates, matches.currentStateLeftSiblingChild);
                var rightSiblings = _.filter(teamStates, matches.currentStateRightSiblingChild);
                var shouldAddSyntheticTeamState = leftSiblings.length && rightSiblings.length;
                if (shouldAddSyntheticTeamState) {
                    var position = _.findIndex(teamStates, matches.currentStateLeftSiblingChild) + leftSiblings.length;
                    return this._addSyntheticTeamState(teamStates, currentState, position);
                }
            }
            return teamStates;
        },

        _addSyntheticTeamState: function(teamStates, currentState, placeToAdd) {
            var newTeamState = _.cloneDeep(currentState);
            newTeamState.parentEntityState = currentState;
            return _.insertAt(teamStates, newTeamState, placeToAdd);
        },

        _getInsertPosition: function(overStateIndex, overStatesCount, placeAfter) {
            return overStateIndex + (placeAfter ? overStatesCount : 0);
        },

        onReorderEnd: function(key, dropped) {
            this.props.onReorderStop();

            if (!dropped) {
                this.props.onReorderCancel();
                return;
            }

            if (!_.isEqual(this.props.initialStates, this.props.currentStates)) {
                var draggedState = _.findWhere(this.props.currentStates, {id: parseInt(key, 10)});
                var draggedStateIndex = _.indexOf(this.props.currentStates, draggedState);
                this.props.onReorder(draggedState, {
                    previous: this.props.currentStates[draggedStateIndex - 1],
                    next: this.props.currentStates[draggedStateIndex + 1]
                });
            }
        },

        EDGE_TRIGGER_DISTANCE: 100,
        SCROLL_DISTANCE: 10,

        scrollOnTheEdge: function(event) {
            var scrollable = this.props.scrollableContainerRef.getDOMNode(),
                offset = scrollable.getBoundingClientRect(),
                leftBorderDistance = event.pageX - offset.left,
                rightBorderDistance = scrollable.offsetWidth + offset.left - event.pageX;
            if (leftBorderDistance < this.EDGE_TRIGGER_DISTANCE) {
                scrollable.scrollLeft -= this.SCROLL_DISTANCE;
            } else if (rightBorderDistance < this.EDGE_TRIGGER_DISTANCE) {
                scrollable.scrollLeft += this.SCROLL_DISTANCE;
            }
            return event;
        },

        render: function() {
            return (<div className="process-grid"
                onDragStart={this.sortStart}
                onDragOver={_.compose(this.sortDragOver, this.scrollOnTheEdge)}
                onDragEnd={this.sortEnd}
                onDrop={this.sortDrop}
                onMouseDown={this.sortMouseDown}>
                    {this.props.children}
            </div>);
        }
    });
});