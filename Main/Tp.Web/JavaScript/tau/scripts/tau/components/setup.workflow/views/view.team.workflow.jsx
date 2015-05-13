define(function(require) {
    var $ = require('jQuery'),
        _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        ButtonView = require('jsx!./button'),
        TeamWorkflowUsageSummary = require('jsx!./team.workflow.item.usage.summary'),
        TeamStateView = require('jsx!./view.team.workflow.state'),
        AddTeamWorkflow = require('jsx!./add.team.workflow.view');

    return React.createClass({

        _toggleSettings: function(e) {
            var $settingsButton = $(e.currentTarget);

            var viewContainer = document.createElement('div');
            React.renderClass(AddTeamWorkflow, {
                teamWorkflow: {
                    title: this.props.teamWorkflow.name,
                    dsl: this.props.teamWorkflowText
                },
                affectedTeams: this.props.teamWorkflow.affectedTeams,
                saveAction: function(newTitle, newDsl, oldTitle, oldDsl) {
                    return this.props.editTeamWorkflowAction(this.props.teamWorkflow.id, newTitle, newDsl, oldTitle, oldDsl).then(function() {
                        $settingsButton.tauBubble('hide');
                    });
                }.bind(this),
                deleteAction: function() {
                    this.props.deleteTeamWorkflowAction(this.props.teamWorkflow).always(function() {
                        $settingsButton.tauBubble('hide');
                    });
                }.bind(this)
            }, viewContainer);

            $settingsButton.tauBubble({
                content: viewContainer,
                zIndex: 999,
                showOnCreation: true,
                onPositionConfig: function(config) {
                    config.collision = 'fit flip';
                    config.within = $('.i-role-views-workflow-container');
                    config.my = 'center top';
                    config.at = 'right bottom';
                },
                onShow: function() {
                    $(viewContainer).find('[name=title]').focus();
                },
                onHide: function() {
                    this.destroy();
                }
            });
        },

        render: function() {
            var teamStates = this.props.teamWorkflow.entityStates;
            var parentStateGroups = _.groupBy(teamStates, _.complexProperty('parentEntityState.id'));
            var teamStateViews = _.map(teamStates, function(teamState) {
                var parentSubStates = parentStateGroups[teamState.parentEntityState.id];
                var isEnabled = this.props.isEnabled || false;
                var isLoading = this.props.updatingStateId === teamState.id;
                return React.createElement(TeamStateView, {
                    key: teamState.id,
                    teamState: teamState,
                    parentSubStates: parentSubStates,
                    entityTerms: this.props.entityTerms,
                    isEnabled: isEnabled,
                    isLoading: isLoading,
                    renameStateAction: this.props.renameStateAction,
                    stateSettings: this.props.stateSettings
                });
            }.bind(this));

            var customProcessClasses = [
                'process-grid__custom-process',
                'tau-entity-background--' + this.props.entityType,
                'tau-entity-color--' + this.props.entityType
            ].join(' ');

            var toggleSettings = this.props.isEnabled ? this._toggleSettings : null;
            var settingsClasses = 'tau-icon-general tau-icon-settings-small-dark i-role-team-workflow-settings';
            if (!this.props.isEnabled) {
                settingsClasses += ' disabled';
            }

            return (
                <div className={customProcessClasses} style={this.props.teamWorkflow.position}>
                    <div className="process-grid__custom-process__title">
                        <div className="process-grid__custom-process__name">{this.props.teamWorkflow.name}</div>
                        <span ref="settings" title="Change Team Workflow" className={settingsClasses} onClick={toggleSettings} />
                        <TeamWorkflowUsageSummary teamWorkflow={this.props.teamWorkflow} processTeams={this.props.processTeams}
                            urlBuilder={this.props.urlBuilder} openEntityViewAction={this.props.openEntityViewAction} />
                    </div>
                    <div className="process-grid__custom-process__body">
                        {teamStateViews}
                    </div>
                </div>
            );
        }
    });
});
