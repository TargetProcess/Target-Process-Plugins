define(function(require) {
    var $ = require('jQuery');
    var React = require('libs/react/react-ex');

    return React.defineClass(['boardMenuBus'], function(bus) {
        return {
            getInitialState: function() {
                return {
                    isFocused: false
                };
            },

            render: function() {
                var className = [
                    't3-search',
                    this.state.isFocused ? 't3-active' : null
                ].join(' ');

                return (
                    <div className={className}>
                        <input
                            className='i-role-views-list-search'
                            type="text"
                            value={this.props.searchTerm}
                            onChange={this._onSearchTermChanged}
                            onKeyDown={this._onKeyDown}
                            onFocus={this._onFocus}
                            onBlur={this._onBlur}/>
                    </div>
                    );
            },

            _onSearchTermChanged: function(e) {
                var filter = e.target.value;
                bus.fire('board.menu.update.search.filter', filter);
            },

            _onKeyDown: function(e) {
                var handler = this._getOperationForKey(e.keyCode);
                if (handler) {
                    handler();
                    e.preventDefault();
                }
            },

            _getOperationForKey: function(keyCode) {
                switch (keyCode) {
                    case $.ui.keyCode.DOWN:
                        return this._tryFocusNextBoard;
                    case $.ui.keyCode.UP:
                        return this._tryFocusPrevBoard;
                    case $.ui.keyCode.ENTER:
                        return this._trySwitchToFocusedBoard;
                }
            },

            _tryFocusNextBoard: function() {
                this._tryFocusWith(function(focusedIndex, boardCount) {
                    return focusedIndex < boardCount - 1 ? focusedIndex + 1 : 0;
                });
            },

            _tryFocusPrevBoard: function() {
                this._tryFocusWith(function(focusedIndex, boardCount) {
                    return focusedIndex > 0 ? focusedIndex - 1 : boardCount - 1;
                });
            },

            _tryFocusWith: function(getNextIndex) {
                if (this.props.boards.length > 0) {
                    var focusedIndex = this._getFocusedBoardIndex();
                    var nextIndex = getNextIndex(focusedIndex, this.props.boards.length);
                    var nextBoardId = this.props.boards[nextIndex].boardId;
                    bus.fire('board.menu.focus.board', nextBoardId);
                }
            },

            _trySwitchToFocusedBoard: function() {
                var focusedId = this.props.focusedBoardId;

                if (focusedId) {
                    // TODO: check that the board exists?
                    bus.fire('board.menu.switch.board', focusedId);
                    window.location.href = this.props.model.getBoardLink(focusedId);
                }
            },

            _getFocusedBoardIndex: function() {
                var focusedBoardId = this.props.focusedBoardId;
                return _.findIndex(this.props.boards, function(board) {
                    return board.boardId === focusedBoardId;
                });
            },

            _onFocus: function() {
                this.setState({isFocused: true});
            },

            _onBlur: function() {
                this.setState({isFocused: false});
            }
        }
    });
});
