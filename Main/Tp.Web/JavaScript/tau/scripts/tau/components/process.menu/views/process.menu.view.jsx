define(function(require) {
    var React = require('libs/react/react-ex'),
        classNames = require('libs/classNames'),
        _ = require('Underscore');

    return React.defineClass(['item.view', 'process.context.menu.service'], function(MenuItemView,
        ProcessContextMenuService) {
        return {
            displayName: 'ProcessMenu',
            _createProcess: function() {
                this.props.actions.createProcess();
            },
            _renameProcessStart: function(process) {
                this.setState({renamingId: process.id});
            },
            _renameProcessFinish: function() {
                this.setState({renamingId: null});
            },
            getDefaultProps: function() {
                return {
                    actions: {
                        renameProcessStart: _.identity,
                        renameProcess: _.identity,
                        renameProcessFinish: _.identity,
                        cloneProcess: _.identity,
                        deleteProcess: _.identity
                    }
                };
            },
            getInitialState: function() {
                return {
                    processes: null
                };
            },
            componentWillReceiveProps: function(newProps) {
                if (_.isBoolean(newProps.isUpdating)) {
                    this.setState({isUpdating: newProps.isUpdating});
                }
            },
            _getScrollableRect: function() {
                return this.refs.scrollable.getDOMNode().getBoundingClientRect();
            },
            render: function() {
                var loader = (
                    <div className="t3-views-navigator t3-processes-navigator">
                        <div className="tau-loader"></div>
                    </div>
                );
                if (!this.state.processes) {
                    return loader;
                }

                var contextMenuService = new ProcessContextMenuService(this.props.user);
                contextMenuService.renameProcessTriggered.add(this._renameProcessStart);
                contextMenuService.cloneProcessTriggered.add(this.props.actions.cloneProcess);
                contextMenuService.deleteProcessTriggered.add(this.props.actions.deleteProcess);

                var processes = _.map(this.state.processes, function(item) {
                    if (!item.isVisible) {
                        return;
                    }
                    var isActive = item.id === this.props.activePage.processId;
                    var isBeingRenamed = item.id === this.state.renamingId || (isActive && this.props.activePage.isNew);

                    return <MenuItemView
                        key={item.id}
                        process={item}
                        active={isActive}
                        isInDom={this.state.isInDom}
                        isNew={isActive && this.props.activePage.isNew}
                        activePage={this.props.activePage}
                        urlBuilder={this.props.urlBuilder}
                        openEntityViewAction={this.props.openEntityViewAction}
                        showContextMenu={this._showContextMenu}
                        setActivePage={this.props.setActivePage}
                        isBeingRenamed={isBeingRenamed}
                        processContextMenuService={contextMenuService}
                        getScrollableRect={this._getScrollableRect}
                        renameProcessAction={this.props.actions.renameProcess}
                        cloneProcessAction={this.props.actions.cloneProcess}
                        renameProcessFinish={this._renameProcessFinish}
                        user={this.props.user}
                    />;
                }, this);

                var navigatorClasses = classNames({
                    't3-views-navigator': true,
                    't3-processes-navigator': true,
                    't3-processes-update-overlay': this.state.isUpdating
                });

                return (
                    <div className={navigatorClasses}>
                        {this.state.isUpdating ? loader : null}
                        <div className="t3-process__title-navigation t3-processes__title">Processes
                            <button type="button" className="tau-btn tau-settings-close t3-processes__close i-role-close">Finish</button>
                        </div>
                        <div ref="scrollable" className="t3-process__catalog">
                            {processes}
                        </div>
                        <div className="t3-controls" hidden ={!this.props.user.isAdministrator}>
                            <div className="t3-add-view-trigger i-role-add-board-button" onClick={this._createProcess}>
                            Create process
                            </div>
                        </div>
                    </div>
                );
            }
        }
    });
});
