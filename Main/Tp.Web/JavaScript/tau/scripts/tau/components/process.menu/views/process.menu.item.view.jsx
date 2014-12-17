define(function(require) {
    var $ = require('jQuery'),
        React = require('libs/react/react-ex'),
        optionGroupsCreator = require('tau/components/setup.process/process.optionGroups'),
        RenameService = require('tau/services/rename.service'),
        boardBubbleTemplate = require('template!tau/components/tauBubble/templates/board.bubble'),
        affectedProjectsTemplate = require('template!tau/components/process.menu/templates/affected.projects.template');

    var ProcessUsageSummary = React.createClass({
        _getProjectsCount: function() {
            return this.props.process.projects.length;
        },

        _getAffectedProjectsSummary: function() {
            var projectsCount = this._getProjectsCount();
            if (projectsCount > 0) {
                return 'Changes affect ' + projectsCount + ' project(s)';
            } else {
                var text = 'The process is not used.';
                if (!this.props.process.isDefault) {
                    text += ' You can delete it.';
                }
                return text;
            }
        },

        _toggleProjectsBubble: function() {
            if (this._getProjectsCount()) {
                $(this.getDOMNode()).tauBubble({
                    className: 'tau-bubble-tooltipArticle',
                    template: boardBubbleTemplate.render(),
                    onPositionConfig: function(config) {
                        config.collision = 'fit';
                        config.my = 'left';
                        config.at = 'right center';
                    },
                    hideOnScroll: true,
                    hideOnScrollContainer: '.t3-process__catalog',
                    content: affectedProjectsTemplate.render({ projects: this.props.process.projects }),
                    zIndex: 999,
                    onHide: function() {
                        this.destroy();
                    },
                    onResize: function($popup, data) {
                        var $content = $popup.find('[role=content]');
                        var margins = $popup.outerHeight() - $content.height();
                        $content.css({
                            maxHeight: window.document.body.offsetHeight - margins,
                            overflowY: 'auto'
                        });
                    }
                }).tauBubble('toggle');
            }
        },

        render: function() {
            var isProjectsLinkVisible = this.props.process.projects.length > 0;

            var classes = React.addons.classSet({
                't3-process__text': true,
                't3-process__link': isProjectsLinkVisible
            });

            return (
                <div className={classes} onClick={this._toggleProjectsBubble}>{this._getAffectedProjectsSummary()}</div>
            );
        }
    });

    return React.defineClass(['item.optionGroup.view', 'item.optionGroup.workflows.view'], function(OptionGroupView, WorkflowOptionGroupView) {
        return {
            _activate: function() {
                this.props.setActivePage({processId: this.props.process.id});
            },
            _selectProcess: function() {
                this._activate();
            },
            _showContextMenu: function(event) {
                var $target = $(this.refs.title.getDOMNode());

                this.props.processContextMenuService.showMenuForProcess($target, this.props.process);

                event.preventDefault();
                event.stopPropagation();
            },
            getInitialState: function() {
                return {
                    bodyHeight: 'auto'
                };
            },
            componentDidUpdate: function() {
                if (this.props.isBeingRenamed) {
                    var element = this.refs.header.getDOMNode();

                    RenameService
                        .startRenamingActivity(element)
                        .done(function(newName) {
                            this.props.renameProcessAction(this.props.process.id, newName);
                        }.bind(this))
                        .always(function() {
                            this.props.renameProcessFinish();
                        }.bind(this));
                }
            },
            componentWillReceiveProps: function(newProps) {
                var hasJustAttachedToDom = !this.props.isInDom && newProps.isInDom;
                if (hasJustAttachedToDom) {
                    this.setState({
                        bodyHeight: $(this.getDOMNode()).find('.t3-process__body').height()
                    });
                }
            },
            render: function() {
                var isMenuItemActive = this.props.active || !this.props.isInDom;

                var optionGroups = null;

                if (isMenuItemActive) {
                    optionGroups = _.map(optionGroupsCreator.groups, function(group) {
                        var optionGroup = optionGroupsCreator.create(this.props.urlBuilder, group, this.props.process.id);
                        var isActive = this.props.activePage.optionGroupId == optionGroup.getId();
                        var params = {
                            key: group,
                            optionGroup: optionGroup,
                            active: isActive,
                            activePage: this.props.activePage,
                            setActivePage: this.props.setActivePage
                        };
                        if (optionGroupsCreator.groups.Workflows == optionGroup.getId()) {
                            _.extend(params, { entityTypes: this.props.process.entityTypes });
                            return new WorkflowOptionGroupView(params);
                        } else {
                            return new OptionGroupView(params);
                        }
                    }, this);
                }

                var classes = React.addons.classSet({
                    't3-process__item t3-process__item--outer': true,
                    't3-active': isMenuItemActive
                });

                var titleClickHandler = this.props.isBeingRenamed ? null : this._selectProcess;
                return (
                    <div className={classes} role="process">
                        <div className="t3-process__item__title" role="title" ref="title" onClick={titleClickHandler}>
                            <div className="t3-actions-trigger" onClick={this._showContextMenu}></div>
                            <div className="t3-process__counter">{this.props.process.projects.length}</div>
                            <div className="t3-header" ref="header">{this.props.process.name}</div>
                            <ProcessUsageSummary process={this.props.process} />
                        </div>
                        <div className="t3-process__body" style={{'height': isMenuItemActive ? this.state.bodyHeight : 0}}>
                            {optionGroups}
                        </div>
                    </div>
                    );
            }
        };
    });
});
