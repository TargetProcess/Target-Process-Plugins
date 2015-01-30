define(function(require) {
    var $ = require('jQuery');
    var React = require('libs/react/react-ex');
    var ActionsMenuItem = require('./board.menu.actions.menu.item');
    var IntegrationMixin = require('./board.menu.actions.integration.mixin');

    return React.defineClass(
        ['boardMenuBus', 'settingsService', 'boardsOperationsService', 'integrationService', 'featuresService'],
        function(bus, settingsService, boardsOperationsService, integrationService, featuresService) {
            return {
                displayName: 'BoardMenuActions',

                mixins: [IntegrationMixin],

                _getIntegrationService: _.constant(integrationService),

                getInitialState: function() {
                    return {
                        isNewItemMenuVisible: false
                    };
                },

                componentDidMount: function() {
                    settingsService.getShowHiddenBoards().then(this._setShowHidden);

                    $('body').on('click.viewMenuActions', function(event) {
                        if (!$(event.target).closest('.i-role-add-board-button').length) {
                            this.setState({isNewItemMenuVisible: false});
                        }
                    }.bind(this));
                },

                componentWillUnmount: function() {
                    $('body').off('.viewMenuActions');
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

                _resetMenuLook: function() {
                    bus.fire('board.menu.update.search.filter', '');
                },

                _onCreateView: function() {
                    this._resetMenuLook();
                    boardsOperationsService.createViewAndRedirect();
                },

                _onCreateDashboard: function() {
                    this._resetMenuLook();
                    boardsOperationsService.createDashboardAndRedirect();
                },

                _onCreateGroup: function() {
                    this._resetMenuLook();

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

                _onCreateReport: function() {
                    this._resetMenuLook();
                    boardsOperationsService.createReportAndRedirect();
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
                    var customItems = _.reduce(integrationService.getCustomActionsMenuItems(), (acc, item, index) => {
                        acc['customItem' + index] = item;
                        return acc;
                    }, {});

                    return (
                        <div className={this._getNewItemMenuClassName()}>
                            {this._getNewMenuItem('Group', 't3-group i-role-create-group-menu-item', this._onCreateGroup)}
                            {this._getNewDashboardItem()}
                            {this._getNewReportItem()}
                            {this._getNewMenuItem('View', 't3-view t3-grid i-role-create-view-menu-item', this._onCreateView)}
                            {customItems}
                        </div>
                    );
                },

                _getNewDashboardItem: function() {
                    if (!featuresService.isEnabled('dashboards')) {
                        return null;
                    }

                    return this._getNewMenuItem(
                        'Dashboard', 't3-dashboard i-role-create-dashboard-menu-item',
                        this._onCreateDashboard);
                },

                _getNewReportItem: function() {
                    if (!featuresService.isEnabled('customReports')) {
                        return null;
                    }

                    return this._getNewMenuItem(
                        'Report', 't3-customReport i-role-create-customReport-menu-item',
                        this._onCreateReport);
                },

                _getNewMenuItem: function(name, className, onActivate) {
                    return <ActionsMenuItem className={className} onActivate={onActivate} name={name} />;
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
