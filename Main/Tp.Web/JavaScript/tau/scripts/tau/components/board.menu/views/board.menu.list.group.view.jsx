define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');

    var transformations = require('./../services/board.menu.transformations.service');
    var renamer = require('tau/components/board.menu/services/board.menu.item.rename.service');

    //noinspection JSUnusedLocalSymbols
    var Name = require('jsx!./../views/board.menu.list.item.name.view');
    var SortableItem = require('tau/components/react/mixins/sortable.item');

    var GroupModel = require('tau/components/board.menu/models/board.menu.board.group');

    return React.defineClass([
            'board.menu.list.item.view',
            'boardMenuViewsMenuService',
            'boardContextMenuService',
            'loggedUser',
            'boardMenuBus'
        ],
        function(ListItemView, viewsMenuService, contextMenuService, loggedUser, bus) {
            return {
                displayName: 'BoardMenuListGroup',

                propTypes: {
                    id: React.PropTypes.string.isRequired,
                    groupModel: React.PropTypes.instanceOf(GroupModel).isRequired,
                    querySpec: React.PropTypes.object.isRequired,
                    currentBoardId: React.PropTypes.string.isRequired,
                    focusedBoardId: React.PropTypes.string.isRequired,
                    renamingId: React.PropTypes.string
                },

                _isRenamingActivated: false,

                _getGroupClassName: function() {
                    var model = this.props.groupModel,
                        regularGroup = model.getIsRegularGroup();
                    return classNames({
                        't3-group': true,
                        't3-hidden': model.menuIsVisible === false,
                        't3-private': model.getIsPrivate(),
                        't3-custom': model.getIsCustomShared(),
                        't3-regular-group': regularGroup,
                        't3-other-group': !regularGroup,
                        't3-expanded': model.isExpanded || this.props.querySpec.filterText
                    });
                },

                _getGroupContainerClassName: function() {
                    var model = this.props.groupModel,
                        regularGroup = model.getIsRegularGroup();
                    return classNames({
                        't3-view-list-group': true,
                        't3-view-list-regular-group': regularGroup,
                        't3-view-list-other-group': !regularGroup,
                        'i-role-views-menu-favorites-group': model.getIsFavoriteUngroupedGroup()
                    });
                },

                _showViewContextMenu: function(board, $target) {
                    var isCurrentBoard = board.boardId === this.props.currentBoardId;
                    contextMenuService.showMenuForBoard($target, board, this.props.groupModel, isCurrentBoard);
                },

                _toggleGroupExpanded: function() {
                    var origin = {
                        isExpanded: this.props.groupModel.isExpanded
                    };

                    var update = {
                        isExpanded: !origin.isExpanded
                    };

                    viewsMenuService.groupsApi.updateViewMenuItem(this.props.groupModel.groupId, update, origin);
                },

                _showGroupContextMenu: function(event) {
                    var $target = $(event.target);
                    if ($target.hasClass('t3-name') && $target.hasClass('t3-edit')) {
                        return;
                    }

                    $target = $target.closest('.t3-header');

                    contextMenuService.showMenuForGroup($target, this.props.groupModel, this.props.currentBoardId);

                    event.preventDefault();
                    event.stopPropagation();
                },

                _getSortableKey: function(groupConfig) {
                    return groupConfig.groupId;
                    //return this.props.canDrag ? groupConfig.groupId : null;
                },

                _startRenamingIfNeeded: function() {
                    // in case of problems we could check for !$element.data('uiEditableText')
                    // instead of this._isRenamingActivated
                    if (!renamer.isBeingRenamed(this)) {
                        this._isRenamingActivated = false;
                    } else if (!this._isRenamingActivated) {
                        this._isRenamingActivated = true;
                        renamer
                            .startRenameAndSave({
                                apiService: viewsMenuService.groupsApi,
                                itemId: this.props.renamingId,
                                bus: bus,
                                $element: $(this.getDOMNode()).find('.i-role-group-name')
                            });
                    }
                },

                _isGroupContainingCurrentBoard: function() {
                    return this.props.groupModel.hasBoard(this.props.currentBoardId);
                },

                componentWillMount: function() {
                    this.props.groupModel.isExpanded = this.props.groupModel.isExpanded ||
                        this._isGroupContainingCurrentBoard();
                },

                componentDidMount: function() {
                    this._startRenamingIfNeeded();
                },

                componentDidUpdate: function() {
                    this._startRenamingIfNeeded();
                },

                render: function() {
                    var groupModel = this.props.groupModel,
                        boards = transformations.getVisibleBoards(groupModel.boards, this.props.querySpec),
                        isEmpty = !boards.length,
                        isRegular = groupModel.getIsRegularGroup();

                    if (this.props.querySpec.filterText && isEmpty && isRegular) {
                        return (<span></span>);
                    }

                    var listItems = _.map(boards, function(board) {
                        return <ListItemView
                            id={board.boardId}
                            key={board.boardId}
                            board={board}
                            isCurrentBoard={board.boardId === this.props.currentBoardId}
                            isFocusedBoard={board.boardId === this.props.focusedBoardId}
                            showContextMenu={this._showViewContextMenu}
                            renamingId={this.props.renamingId}
                        />;
                    }, this);

                    if (!isRegular) {
                        return (
                            <div className={this._getGroupContainerClassName()}
                                {...SortableItem.attributes(this._getSortableKey(groupModel), SortableItem.MENU_GROUP, false)}>
                                <div className={this._getGroupClassName()}>
                                    {listItems}
                                </div>
                                <div className="t3-views__drop-placeholder"></div>
                            </div>
                        );
                    }

                    // Fix [draggable] > [contenteditable] problem in IE11
                    // Disable [draggable] when renaming of group or one of its items is in progress
                    var groupIsDraggable = this.props.groupModel.getCanPrioritizeGroup(loggedUser) && !this.props.renamingId;

                    return (
                        <div className={this._getGroupContainerClassName()}
                            {...SortableItem.attributes(this._getSortableKey(groupModel), SortableItem.MENU_GROUP, groupIsDraggable)}>
                            <div className={this._getGroupClassName()} onContextMenu={this._showGroupContextMenu}>
                                <Name name={groupModel.name || groupModel.groupId} onClick={this._toggleGroupExpanded}
                                    onActionClick={this._showGroupContextMenu} isGroup={true} />
                                {listItems}
                            </div>
                            <div className="t3-views__drop-placeholder"></div>
                        </div>
                    );
                }
            };
        }
    );
});
