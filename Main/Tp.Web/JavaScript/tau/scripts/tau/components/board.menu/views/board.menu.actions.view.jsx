define(function(require) {
    var React = require('libs/react/react-ex');

    return React.defineClass(
        ['boardMenuBus', 'settingsService', 'boardsOperationsService', 'integrationService'],
        function(bus, settingsService, boardsOperationsService, integrationService) {
            return {
                getInitialState: function() {
                    return {
                        isNewItemMenuVisible: false
                    };
                },

                componentDidMount: function() {
                    settingsService.getShowHiddenBoards().then(this._setShowHidden);
                    integrationService.actionsMenuItemsUpdated.add(this.forceUpdate, this);
                },

                componentWillUnmount: function() {
                    integrationService.actionsMenuItemsUpdated.remove(this);
                },

                _getAddViewTriggerClassName: function() {
                    return React.addons.classSet({
                        't3-add-view-trigger': true,
                        'i-role-add-board-button': true,
                        't3-pressed': this.state.isNewItemMenuVisible
                    });
                },

                _getToggleHiddenBoardsButtonClassName: function() {
                    return React.addons.classSet({
                        't3-show-hidden-views-trigger': true,
                        't3-pressed': this.props.showHidden
                    });
                },

                _getNewItemMenuClassName: function() {
                    return React.addons.classSet({
                        't3-add-view-options-list': true,
                        't3-active': this.state.isNewItemMenuVisible
                    });
                },

                _onToggleHiddenBoards: function() {
                    var newValue = !this.props.showHidden;
                    this._setShowHidden(newValue);
                    settingsService.setShowHiddenBoards(newValue);
                },

                _onShowOptionsClick: function() {
                    this.setState({isNewItemMenuVisible: !this.state.isNewItemMenuVisible});
                },

                _onCreateBoard: function() {
                    this.setState({isNewItemMenuVisible: false});
                    bus.fire('board.menu.update.search.filter', '');
                    boardsOperationsService.createBoardAndRedirect();
                },

                _onCreateGroup: function() {
                    this.setState({isNewItemMenuVisible: false});
                    bus.fire('board.menu.update.search.filter', '');

                    boardsOperationsService
                        .createViewGroup()
                        .then(function(createdGroup) {
                            settingsService
                                .getIsBoardMenuExpanded()
                                .then(function(isExpanded) {
                                    if (isExpanded) {
                                        bus.fire('board.menu.rename.start', createdGroup.data.key);
                                    }
                                });
                        });
                },

                _setShowHidden: function(show) {
                    bus.fire('board.menu.toggle.hidden', show);
                },

                render: function() {
                    return (
                        <div className="t3-controls">
                            {this._getNewItemMenu()}
                            <div
                            className={this._getAddViewTriggerClassName()}
                            onClick={this._onShowOptionsClick}
                            title="Create new Groups and Views">
                                <span>Create</span>
                            </div>
                            {this._getToggleHiddenButton()}
                        </div>
                        );
                },

                _getNewItemMenu: function() {
                    var customItems = integrationService.getCustomActionsMenuItems();

                    return (
                        <div className={this._getNewItemMenuClassName()}>
                            <div className="t3-group i-role-create-group-menu-item" onClick={this._onCreateGroup}>
                                <div className="t3-header">
                                    <span className="tau-icon-general"></span>
                                    <div className="t3-name">New Group</div>
                                </div>
                            </div>
                            <div className="t3-view t3-grid i-role-create-view-menu-item" onClick={this._onCreateBoard}>
                                <div className="t3-header">
                                    <span className="tau-icon-general"></span>
                                    <div className="t3-name">New View</div>
                                </div>
                            </div>
                            {customItems}
                        </div>
                        );
                },

                _getToggleHiddenButton: function() {
                    if (!this.props.hasHiddenBoards) {
                        return null;
                    }

                    var title = this.props.showHidden ?
                        'Hide Groups and Views from your menu' :
                        'Show hidden Groups and Views in your menu';

                    return (
                        <div
                        className={this._getToggleHiddenBoardsButtonClassName()}
                        onClick={this._onToggleHiddenBoards}
                        title={title}/>
                        );
                }
            }
        });
});
