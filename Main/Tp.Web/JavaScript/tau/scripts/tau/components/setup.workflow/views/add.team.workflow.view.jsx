define(function(require) {
    var $ = require('jQuery'),
        _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        addTeamWorkflowTemplate = require('template!../templates/add.team.workflow.template');

    return React.createClass({

        _showTeamWorkflowBubble: function() {
            var $content = addTeamWorkflowTemplate.render({
                title: this.props.defaultTeamWorkflowText.title,
                dsl: this.props.defaultTeamWorkflowText.dsl
            }).on('click', '.i-role-actionok', function(event) {
                $(event.target).prop('disabled', true);
                var teamWorkflowTitle = $content.find('[name=title]').val();
                var teamWorkflowDsl = $content.find('[name=dsl]').val();
                this.props.addTeamWorkflowAction(teamWorkflowTitle, teamWorkflowDsl).then(function() {
                    $button.tauBubble('hide');
                }, function() {
                    $(event.target).prop('disabled', false);
                });
            }.bind(this));

            var $button = $(this.getDOMNode());
            $button.tauBubble({
                content: $content,
                zIndex: 999,
                showOnCreation: true,
                onPositionConfig: function(config) {
                    config.my = 'left-30 bottom';
                    config.at = 'left top';
                },
                onShow: function() {
                    $content.find('[name=title]').focus();
                },
                onHide: function() {
                    this.destroy();
                }
            });
        },

        render: function() {
            return (
                <button className="tau-btn tau-add tau-btn-big i-role-add-team-workflow" title="Add Sub Workflow" onClick={this._showTeamWorkflowBubble}></button>
                );
        }
    });
});
