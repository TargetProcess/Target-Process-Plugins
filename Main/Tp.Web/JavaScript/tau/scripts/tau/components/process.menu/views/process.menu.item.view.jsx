define(function(require) {
    var $ = require('jQuery'),
        _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        optionGroupsCreator = require('tau/components/setup.process/process.optionGroups'),
        RenameService = require('tau/services/rename.service'),
        ProcessUsageSummary = require('jsx!./process.menu.item.usage.summary');

    return React.defineClass([
        'item.optionGroup.view', 'item.optionGroup.workflows.view'
    ], function(OptionGroupView, WorkflowOptionGroupView) {
        return {
            displayName: 'ProcessMenuItem',

            _activateProcess: function() {
                this.props.setActivePage({processId: this.props.process.id});
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
                if (hasJustAttachedToDom || this.props.isNew) {
                    this.setState({
                        bodyHeight: $(this.refs.processBody.getDOMNode()).height()
                    });
                }
                var becameActive = (hasJustAttachedToDom || !this.props.active) && newProps.active;
                if (becameActive) {
                    this._scrollIntoView();
                    if (!hasJustAttachedToDom) {
                        $(this.getDOMNode()).one('transitionend webkitTransitionEnd', this._scrollIntoViewIfActive);
                    }
                }
            },
            _scrollIntoViewIfActive: function() {
                if (this.props.active) {
                    this._scrollIntoView();
                }
            },
            _scrollIntoView: function() {
                var scrollableRect = this.props.getScrollableRect();
                var elementRect = this.getDOMNode().getBoundingClientRect();
                var isAboveView = elementRect.top < scrollableRect.top;
                var isBelowView = elementRect.top + elementRect.height > scrollableRect.top + scrollableRect.height;
                if (isAboveView || isBelowView) {
                    var alignWithTop = isAboveView;
                    this.getDOMNode().scrollIntoView(alignWithTop);
                }
            },
            render: function() {
                var isMenuItemActive = this.props.active || !this.props.isInDom;

                var optionGroups = _.map(optionGroupsCreator.groups, function(group) {
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
                        params.entityTypes = this.props.process.entityTypes;
                        //return <WorkflowOptionGroupView {...params} />;
                        return React.createElement(WorkflowOptionGroupView, params);
                    } else {
                        //return <OptionGroupView {...params} />;
                        return React.createElement(OptionGroupView, params);
                    }
                }, this);

                var classes = React.addons.classSet({
                    't3-process__item t3-process__item--outer': true,
                    't3-active': isMenuItemActive
                });

                var titleClickHandler = this.props.isBeingRenamed ? null : this._activateProcess;
                return (
                    <div className={classes} role="process">
                        <div className="t3-process__item__title" role="title" ref="title" onClick={titleClickHandler}>
                            <div className="t3-actions-trigger" onClick={this._showContextMenu}></div>
                            <div className="t3-process__counter">{this.props.process.projects.length}</div>
                            <div className="t3-header" ref="header">{this.props.process.name}</div>
                            <ProcessUsageSummary process={this.props.process} urlBuilder={this.props.urlBuilder}
                                openEntityViewAction={this.props.openEntityViewAction} hidden={!this.props.user.isAdministrator} />
                        </div>
                        <div
                            ref="processBody"
                            className="t3-process__body"
                            style={{'height': isMenuItemActive ? this.state.bodyHeight : 0}}>
                            {optionGroups}
                        </div>
                    </div>
                );
            }
        };
    });
});
