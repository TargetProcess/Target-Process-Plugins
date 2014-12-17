define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore');

    return React.defineClass(['item.view', 'process.context.menu.service'], function(MenuItemView,
        ProcessContextMenuService) {
        return {
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
            render: function() {
                if (!this.state.processes) {
                    return (
                        <div className="t3-views-navigator t3-processes-navigator">
                            <div className="tau-loader"></div>
                        </div>
                        );
                }

                var processContextMenuService = new ProcessContextMenuService();
                processContextMenuService.renameProcessTriggered.add(this._renameProcessStart);
                processContextMenuService.cloneProcessTriggered.add(this.props.actions.cloneProcess);
                processContextMenuService.deleteProcessTriggered.add(this.props.actions.deleteProcess);

                var processes = _.map(this.state.processes, function(item) {
                    var isActive = item.id === this.props.activePage.processId;
                    var isBeingRenamed = item.id === this.state.renamingId;

                    return new MenuItemView({
                        key: item.id,
                        process: item,
                        active: isActive,
                        isInDom: this.state.isInDom,
                        activePage: this.props.activePage,
                        urlBuilder: this.props.urlBuilder,
                        showContextMenu: this._showContextMenu,
                        setActivePage: this.props.setActivePage,
                        isBeingRenamed: isBeingRenamed,
                        processContextMenuService: processContextMenuService,
                        renameProcessAction: this.props.actions.renameProcess,
                        cloneProcessAction: this.props.actions.cloneProcess,
                        renameProcessFinish: this._renameProcessFinish
                    });
                }, this);

                return (
                    <div className="t3-views-navigator t3-processes-navigator">
                        <div className="t3-process__title-navigation t3-processes__title">Processes
                            <button type="button" className="tau-btn tau-settings-close t3-processes__close i-role-close">Finish</button>
                        </div>
                        <div className="t3-process__catalog">
                            {processes}
                        </div>
                    </div>
                    );
            }
        }
    });
});
