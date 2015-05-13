define(function(require) {
    var _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        classNames = require('libs/classNames'),
        t = React.PropTypes,
        TeamIcon = require('jsx!./teamIcon.view'),
        TextConverter = require('tau/utils/utils.textConverter'),
        TooltipArticle = require('jsx!tau/components/react/tooltipArticle');

    var WorkflowHelpDataType = t.shape({
        isTp3: t.bool.isRequired,
        canSetup: t.bool.isRequired,
        url: t.string
    });

    var DisabledTransitionHelp = React.createClass({
        propTypes: {
            workflowHelpData: WorkflowHelpDataType.isRequired
        },
        _getArticleId: function() {
            if (this.props.workflowHelpData.canSetup) {
                if (this.props.workflowHelpData.isTp3) {
                    return 'entityState.noTransition.admin';
                } else {
                    return 'entityState.noTransition.admin.with-no-link';
                }
            } else {
                return 'entityState.noTransition.user';
            }
        },
        render: function() {
            var dataAttributes = {
                'data-url': this.props.workflowHelpData.url
            };
            return (
                <TooltipArticle
                    articleId={this._getArticleId()}
                    dataAttributes={dataAttributes}
                />
            );
        }
    });

    var AddCommentView = React.createClass({
        propTypes: {
            onSave: t.func.isRequired,
            onCancel: t.func.isRequired,
            isActive: t.bool.isRequired
        },

        getInitialState: function() {
            return {
                saveDisabled: true,
                comment: ''
            };
        },

        componentWillReceiveProps: function(nextProps) {
            if (nextProps.isActive) {
                setTimeout(function() {
                    if (this.isMounted()) {
                        this.refs.comment.getDOMNode().focus();
                    }
                }.bind(this), 500);
            }
        },

        _handleChange: function(event) {
            var comment = event.target.value;
            this.setState({saveDisabled: _.isEmpty(comment.trim()), comment: comment});
        },

        _onSave: function() {
            var comment = new TextConverter().convert(this.state.comment, true);
            this.props.onSave(comment);
        },

        _onCancel: function(event) {
            this.setState({saveDisabled: true, comment: ''});
            this.props.onCancel(event);
        },

        render: function() {
            return (<div className="multiple-team-state-select__comment-container">
                <textarea ref="comment" onChange={this._handleChange} className="tau-in-text i-role-text" placeholder="Provide a required comment" value={this.state.comment}></textarea>
                <button className="tau-btn tau-primary i-role-save" type="button" disabled={this.state.saveDisabled ? "disabled" : ""} onClick={this._onSave}>Save</button>
                <button className="tau-btn tau-btn-gray i-role-cancel" type="button" onClick={this._onCancel}>Cancel</button>
            </div>)
        }
    });

    var EntityStateGroups = React.createClass({
        displayName: 'EntityStateGroups',
        propTypes: {
            items: t.array.isRequired,
            currentStates: t.object.isRequired,
            workflowHelpData: WorkflowHelpDataType.isRequired,
            actions: t.object.isRequired
        },

        getInitialState: function() {
            return {
                commentStateId: null
            }
        },

        _changeTeamState: function(teamId, teamState, parentState, isParentActive) {
            if (parentState.isCommentRequired && !isParentActive) {
                this.setState({commentStateId: teamState.id});
            } else {
                this.props.actions.changeTeamState(teamId, teamState.id);
            }
        },

        _changeState: function(entityState, event) {
            if (entityState.isCommentRequired) {
                this.setState({commentStateId: entityState.id});
            } else {
                this.props.actions.changeState(entityState.id);
            }
        },

        _cancelComment: function(event) {
            this.setState({commentStateId: null});

            event.preventDefault();
            event.stopPropagation();
        },

        _saveStateWithComment: function(entityState, comment) {
            this.props.actions.changeState(entityState.id, comment);
        },

        _saveTeamStateWithComment: function(teamId, teamState, comment) {
            this.props.actions.changeTeamState(teamId, teamState.id, comment);
        },

        _renderStateWithoutSubStates: function(entityState) {
            var isActive = entityState.id === this.props.currentStates.parentState.id;
            var isClickable = !isActive && entityState.isTransitionAllowed;
            var isCommentFormActive = entityState.id === this.state.commentStateId;
            var classes = classNames({
                'multiple-team-state-select-list__item': true,
                'tau-active': isActive,
                'tau-disabled': !isActive && !entityState.isTransitionAllowed,
                'tau-comment': isCommentFormActive
            });
            return (
                <li key={entityState.id}
                    className={classes}
                    onClick={isClickable ? this._changeState.bind(null, entityState) : null}>
                    <div className="multiple-team-state-select-list__team-list__item-value">{entityState.name}</div>
                    {isClickable || isActive ? null : <DisabledTransitionHelp workflowHelpData={this.props.workflowHelpData}/>}
                    {entityState.isCommentRequired ?
                        <AddCommentView isActive={isCommentFormActive} onSave={this._saveStateWithComment.bind(null, entityState)} onCancel={this._cancelComment} /> :
                        null}
                </li>
            );
        },


        _renderStateWithSubStates: function(parentState, team, shouldShowTeamLabel) {
            var isParentActive = parentState.id === this.props.currentStates.parentState.id;
            var isParentDisabled = !isParentActive && !parentState.isTransitionAllowed;
            var classes = classNames({
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
                        var isCommentFormActive = subState.id === this.state.commentStateId;

                        var classes = classNames({
                            'multiple-team-sub-state-list__item': true,
                            'tau-active': isActive,
                            'tau-disabled': isParentDisabled,
                            'tau-comment': isCommentFormActive
                        });
                        return (
                            <li key={subState.id}
                                className={classes}
                                onClick={isClickable ? this._changeTeamState.bind(null, team.id, subState, parentState, isParentActive) : null}>
                                <div className="multiple-team-state-select-list__team-list__item-value">{subState.name}</div>
                                {isClickable || isActive ? null : <DisabledTransitionHelp workflowHelpData={this.props.workflowHelpData}/>}
                                {parentState.isCommentRequired && !isParentActive ?
                                    <AddCommentView isActive={isCommentFormActive} onSave={this._saveTeamStateWithComment.bind(null, team.id, subState)} onCancel={this._cancelComment} /> :
                                    null}
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
