define(function(require) {
    var $ = require('jQuery'),
        _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        classNames = require('libs/classNames'),
        boardBubbleTemplate = require('template!tau/components/tauBubble/templates/board.bubble'),
        affectedProjectsTemplate = require('template!tau/components/process.menu/templates/affected.projects.template');

    return React.createClass({
        _getProjectsCount: function() {
            return this.props.process.projects.length;
        },

        _getAffectedProjectsSummary: function() {
            var projectsCount = this._getProjectsCount();
            if (projectsCount > 0) {
                return 'Changes affect ' + projectsCount + ' project(s)';
            } else {
                return 'The process is not in use';
            }
        },

        _renderPopupContents: function() {
            var $content = affectedProjectsTemplate.render({
                projects: _.map(this.props.process.projects, function(project) {
                    return {
                        id: project.id,
                        name: project.name,
                        url: this.props.urlBuilder.getNewViewUrl(project.id, 'project')
                    };
                }, this)
            });
            $content.find('a').on('click', function(e) {
                e.stopPropagation();
                e.preventDefault();

                this.props.openEntityViewAction($(e.target).data('projectid'), 'project');
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
                    config.my = 'left';
                    config.at = 'right center';
                },
                hideOnScroll: true,
                hideOnScrollContainer: '.t3-process__catalog',
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

        _toggleProjectsBubble: function() {
            if (this._getProjectsCount()) {
                var $toggle = $(this.getDOMNode());
                var tauBubble = $toggle.tauBubble('instance') || this._initBubble($toggle);
                tauBubble.toggle();
            }
        },

        render: function() {
            var isProjectsLinkVisible = this.props.process.projects.length > 0;

            var classes = classNames({
                't3-process__text': true,
                't3-process__link': isProjectsLinkVisible
            });

            return (
                <div hidden={this.props.hidden} className={classes} onClick={this._toggleProjectsBubble}>{this._getAffectedProjectsSummary()}</div>
                );
        }
    });
});
