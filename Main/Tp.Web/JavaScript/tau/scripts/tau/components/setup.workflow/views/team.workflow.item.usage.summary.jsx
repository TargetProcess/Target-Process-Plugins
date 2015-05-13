define(function(require) {
    var $ = require('jQuery'),
        _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        boardBubbleTemplate = require('template!tau/components/tauBubble/templates/board.bubble'),
        affectedTeamsTemplate = require('template!tau/components/setup.workflow/templates/affected.teams.template');

    return React.createClass({
        _getAffectedTeamsCount: function() {
            return this.props.teamWorkflow.affectedTeams.length;
        },

        _getAffectedTeamsSummary: function() {
            var teamsCount = this._getAffectedTeamsCount();
            if (teamsCount > 0) {
                return 'Used by ' + teamsCount + ' team' +
                    (teamsCount > 1 ? 's' : '');
            } else {
                return '';
            }
        },

        _renderPopupContents: function() {
            var teams = this._getAffectedTeamsCount() > 0 ?
                this.props.teamWorkflow.affectedTeams :
                this.props.processTeams;
            var $content = affectedTeamsTemplate.render({
                teams: _.map(teams, function(team) {
                    return {
                        id: team.id,
                        name: team.name,
                        url: this.props.urlBuilder.getNewViewUrl(team.id, 'team')
                    };
                }, this),
                message: this._getAffectedTeamsCount() > 0 ? '' :
                    'The workflow is not in use. Open a team view from the list below and specify on which project they will use this workflow.'
            });
            $content.find('a').on('click', function(e) {
                var teamId = $(e.target).data('teamid');
                if (teamId) {
                    e.stopPropagation();
                    e.preventDefault();

                    this.props.openEntityViewAction($(e.target).data('teamid'), 'team');
                }
            }.bind(this));
            return $content;
        },

        _initBubble: function($toggle) {
            var content = this._renderPopupContents();
            var tauBubble = $toggle.tauBubble({
                content: content,
                className: 'tau-bubble-tooltipArticle',
                template: boardBubbleTemplate.render(),
                onPositionConfig: function(config) {
                    config.collision = 'fit';
                    config.my = 'center top';
                    config.at = 'center bottom';
                },
                hideOnScroll: true,
                hideOnScrollContainer: '.process-grid__wrap',
                zIndex: 999,
                onHide: function() {
                    this.destroy();
                },
                onResize: function($popup) {
                    var $content = $popup.find('[role=content]');
                    var margins = $popup.outerHeight() - $content.height();
                    $content.css({
                        maxHeight: window.document.body.offsetHeight - margins,
                        overflowY: 'auto'
                    });
                }
            }).tauBubble('instance');
            content.on('click', 'a', function() {
                tauBubble.hide();
            });
            return tauBubble;
        },

        _toggleTeamsBubble: function() {
            var $toggle = $(this.getDOMNode());
            var tauBubble = $toggle.tauBubble('instance') || this._initBubble($toggle);
            tauBubble.toggle();
        },

        render: function() {
            var affectedTeamsCount = this._getAffectedTeamsCount();
            var linkClasses = affectedTeamsCount > 0 ? 'process-grid__custom-process__used-in-teams-link' : 'tau-icon-general tau-icon-warning';
            return (
                <div className={linkClasses} onClick={this._toggleTeamsBubble}>{this._getAffectedTeamsSummary()}</div>
            );
        }
    });
});
