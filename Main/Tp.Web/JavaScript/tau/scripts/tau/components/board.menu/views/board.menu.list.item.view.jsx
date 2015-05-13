define(function(require) {
    var $ = require('jQuery');
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');
    var renamer = require('tau/components/board.menu/services/board.menu.item.rename.service');
    var viewTypes = require('tau/services/boards/view.types');
    var Name = require('jsx!./../views/board.menu.list.item.name.view');
    var SortableItem = require('tau/components/react/mixins/sortable.item');

    var BoardModel = require('tau/components/board.menu/models/board.menu.board');

    return React.defineClass(
        ['boardMenuViewsMenuService', 'boardMenuBus'],
        function(viewsMenuService, bus) {
            return {
                displayName: 'BoardMenuListItem',

                propTypes: {
                    id: React.PropTypes.string.isRequired,
                    board: React.PropTypes.instanceOf(BoardModel).isRequired,
                    isCurrentBoard: React.PropTypes.bool.isRequired,
                    isFocusedBoard: React.PropTypes.bool.isRequired,
                    showContextMenu: React.PropTypes.func.isRequired,
                    renamingId: React.PropTypes.string
                },

                _viewModeClassNames: {
                    'list': 't3-detailed',
                    'newlist': 't3-list',
                    'timeline': 't3-timeline'
                },

                _reportTypeNames: {
                    'scatterplot': 't3-customReport--scatterplot',
                    'line': 't3-customReport--line',
                    'bar': 't3-customReport--bar',
                    'horizontalbar': 't3-customReport--bar'
                },

                _defaultViewModeClassName: 't3-grid',

                _showContextMenu: function(event) {
                    var $target = $(event.target);
                    if ($target.hasClass('t3-name') && $target.hasClass('t3-edit')) {
                        return;
                    }

                    if (this.props.showContextMenu) {
                        this.props.showContextMenu(this.props.board,
                            $target.closest('.t3-view').find('.t3-header'));
                    }

                    event.preventDefault();
                    event.stopPropagation();
                },

                _getSortableItemType: function() {
                    switch (this.props.board.itemType) {
                        case viewTypes.REPORT:
                            return SortableItem.MENU_ITEM_REPORT;
                        case viewTypes.BOARD:
                            var viewModeName = this._getViewModeName();
                            return (viewModeName === 'newlist') ?
                                SortableItem.MENU_ITEM_LIST :
                                SortableItem.MENU_ITEM_OTHER;
                        default:
                            return SortableItem.MENU_ITEM_OTHER;
                    }
                },

                _getClassName: function() {
                    var classes = {
                        't3-view': true,
                        't3-dashboard': this.props.board.itemType === viewTypes.DASHBOARD,
                        't3-customReport': this.props.board.itemType === viewTypes.REPORT,
                        'i-role-board-menu-item': true,
                        't3-private': this.props.board.getIsPrivate(),
                        't3-custom': this.props.board.getIsCustomShared(),
                        't3-active': this.props.isCurrentBoard,
                        'i-role-board-menu-item-active': this.props.isCurrentBoard,
                        't3-focused': this.props.isFocusedBoard,
                        't3-hidden': this.props.board.menuIsVisible === false
                    };

                    this._addBoardClass(classes);
                    this._addReportClass(classes);

                    return classNames(classes);
                },

                _getViewModeName: function() {
                    return (this.props.board.viewMode || '').toLowerCase();
                },

                _addBoardClass: function(classes) {
                    if (this.props.board.itemType === viewTypes.BOARD) {
                        var viewModeName = this._getViewModeName();
                        var viewModeClass = this._viewModeClassNames[viewModeName] || this._defaultViewModeClassName;
                        classes[viewModeClass] = true;
                    }
                },

                _addReportClass: function(classes) {
                    if (this.props.board.itemType === viewTypes.REPORT) {
                        var viewData = this.props.board.storedViewData;
                        if (viewData &&
                            viewData.reportSettings &&
                            viewData.reportSettings.report &&
                            viewData.reportSettings.report.type) {

                            var reportType = viewData.reportSettings.report.type.toLowerCase();
                            var reportTypeClass = this._reportTypeNames[reportType];
                            if (reportTypeClass) {
                                classes[reportTypeClass] = true;
                            }
                        }
                    }
                },

                _onLinkClick: function(evt) {
                    if (evt.button === 0) {
                        // Naturally, the menu model listens to the global board settings changes
                        // and updates internal state to render currently selected board.
                        // However we can introduce a minor perceptive performance improvement here
                        // if we optimistically update the state and render a board as selected
                        // immediately after user clicks a link without waiting for the change notification.
                        bus.fire('board.menu.switch.view', this.props.board.boardId);
                    }
                },

                _canDrag: function() {
                    return !renamer.isBeingRenamed(this);
                },

                // Firefox and IE allow <a> dragging even if their parent nodes have draggable="false".
                // Returning "false" from onDragStart event handler overrides this behavior.
                _preventAnchorDrag: function() {
                    if (!this._canDrag()) {
                        return false;
                    }
                },

                componentDidUpdate: function() {
                    if (renamer.isBeingRenamed(this)) {
                        renamer
                            .startRenameAndSave({
                                apiService: viewsMenuService.viewsApi,
                                itemId: this.props.renamingId,
                                bus: bus,
                                $element: $(this.getDOMNode()).find('.i-role-item-name')
                            });
                    }
                },

                render: function() {
                    var itemName = this._renderItemName();

                    return (
                        <div className={this._getClassName()} onContextMenu={this._showContextMenu}
                            {...SortableItem.attributes(this.props.id, this._getSortableItemType(), this._canDrag())}>
                            {itemName}
                            <div className="t3-views__drop-placeholder"></div>
                        </div>
                    );
                },

                _renderItemName: function() {
                    var isBeingRenamed = renamer.isBeingRenamed(this);

                    var name = (<Name name={this.props.board.name} onActionClick={this._showContextMenu} />);

                    if (isBeingRenamed) {
                        return (<div>{name}</div>);
                    }

                    return (
                        <a
                            className="t3-view-link"
                            href={this.props.board.link}
                            onClick={this._onLinkClick}
                            onDragStart={this._preventAnchorDrag}>
                        {name}
                        </a>
                    );
                }
            }
        });
});
