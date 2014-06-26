define(function(require) {
    var React = require('libs/react/react-ex');

    var ListItemView = require('jsx!./board.menu.list.item.view');

    return React.defineClass(
        ['boardMenuBus', 'boardContextMenuService'],
        function(bus, contextMenuService) {

            return {
                render: function() {
                    var boardItems = this.props.boards.map(function(board) {
                        return ListItemView({
                            board: board,
                            key: board.boardId,
                            isCurrentBoard: board.boardId === this.props.currentBoardId,
                            isFocusedBoard: board.boardId === this.props.focusedBoardId,
                            showContextMenu: this._showContextMenu
                        });
                     }.bind(this));

                    return <ul className="t3-views-list">{boardItems}</ul>;
                },

                _showContextMenu: function(board, $target) {
                    var templateConfig = {
                        board: board,
                        isCurrentBoard: board.boardId === this.props.currentBoardId
                    };

                    contextMenuService.showMenu($target, templateConfig);
                }
            }
        });
});
