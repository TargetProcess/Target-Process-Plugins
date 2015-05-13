define(function(require) {
    var $ = require('jQuery'),
        _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        t = React.PropTypes,
        AddTeamWorkflow = require('jsx!./add.team.workflow.view');

    return React.createClass({

        displayName: 'AddTeamWorkflow',

        propTypes: {
            defaultTeamWorkflow: t.shape({
                title: t.string.isRequired,
                dsl: t.string.isRequired
            }),
            isEnabled: t.bool,
            addTeamWorkflowAction: t.func.isRequired
        },

        _showTeamWorkflowBubble: function() {
            var $button = $(this.getDOMNode());

            var viewContainer = document.createElement('div');
            React.renderClass(AddTeamWorkflow, {
                teamWorkflow: this.props.defaultTeamWorkflow,
                affectedTeams: [],
                isNew: true,
                saveAction: function(title, dsl) {
                    return this.props.addTeamWorkflowAction(title, dsl).then(function() {
                        $button.tauBubble('hide');
                    });
                }.bind(this)
            }, viewContainer);

            $button.tauBubble({
                content: viewContainer,
                zIndex: 999,
                showOnCreation: true,
                onPositionConfig: function(config) {
                    config.my = 'left-30 bottom';
                    config.at = 'left top';
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
            var showAction = this.props.isEnabled ? this._showTeamWorkflowBubble : null;
            var classes = "tau-btn tau-btn-add-small tau-add-team-workflow tau-board-customize-beta i-role-add-team-workflow";
            return (
                <button className={classes} title="Add Team Workflow" disabled={!this.props.isEnabled} onClick={showAction}>
                    <span>Add Team Workflow</span>
                </button>
            );
        }
    });
});
