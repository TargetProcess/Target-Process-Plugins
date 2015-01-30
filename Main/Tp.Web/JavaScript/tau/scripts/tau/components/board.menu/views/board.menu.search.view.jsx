define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var React = require('libs/react/react-ex');
    var transformations = require('./../services/board.menu.transformations.service');

    return React.defineClass(
        ['boardMenuBus', 'settingsService'],
        function(bus, settingsService) {

            return {
                getInitialState: function() {
                    return {
                        isFocused: false
                    };
                },

                _onSearchTermChanged: function(e) {
                    this._setFilter(e.target.value);
                },

                _setFilter: function(filter) {
                    bus.fire('board.menu.update.search.filter', filter);
                },

                _cancelFiltering: function() {
                    if (this.props.querySpec.filterText) {
                        this._setFilter('');
                    } else {
                        this._getInputNode().blur();
                    }
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
                        case $.ui.keyCode.ESCAPE:
                            return this._cancelFiltering;
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
                    var boards = this._getBoards();
                    if (boards.length > 0) {
                        var focusedIndex = this._getFocusedBoardIndex(boards);
                        var nextIndex = getNextIndex(focusedIndex, boards.length);
                        var nextBoardId = boards[nextIndex].boardId;
                        this._changeFocusedBoard(nextBoardId);
                    }
                },

                _trySwitchToFocusedBoard: function() {
                    var boards = this._getBoards();
                    var focusedBoard = this._getFocusedBoard(boards);
                    if (focusedBoard) {
                        bus.fire('board.menu.switch.view', focusedBoard.boardId);
                        this._setFilter('');
                        this._changeFocusedBoard('');
                        window.location.href = focusedBoard.link;
                    }
                },

                _getBoards: function() {
                    return _.flatMap(this.props.viewMenuSections.getAllViewGroups(), function(group) {
                        return transformations.getVisibleBoards(group.boards, this.props.querySpec);
                    }, this);
                },

                _getFocusedBoard: function(boards) {
                    var focusedBoardId = this.props.focusedBoardId;
                    return _.find(boards, _.matches({boardId: focusedBoardId}));
                },

                _getFocusedBoardIndex: function(boards) {
                    var focusedBoardId = this.props.focusedBoardId;
                    return _.findIndex(boards, _.matches({boardId: focusedBoardId}));
                },

                _onFocus: function() {
                    this.setState({isFocused: true});
                },

                _onBlur: function() {
                    this.setState({isFocused: false});
                    if (this.props.focusedBoardId) {
                        this._changeFocusedBoard('');
                    }
                },

                _changeFocusedBoard: function(id) {
                    bus.fire('board.menu.focus.board', id);
                },

                _expandPanelIfRequired: function() {
                    var self = this;

                    settingsService.expandBoardMenuIfCollapsed()
                        .then(function(expanded) {
                            if (expanded) {
                                self._getInputNode().focus(1);
                            }
                        });
                },

                _getInputNode: function() {
                    return $(this.getDOMNode()).find('.i-role-views-list-search');
                },

                _onClearButtonClick: function() {
                    this._setFilter('');
                },

                render: function() {
                    var className = [
                        't3-search',
                        this.state.isFocused ? 't3-active' : null
                    ].join(' ');

                    var clearButton = this.props.querySpec.filterText ?
                        <div className='tau-icon-general tau-icon-close-gray' onClick={this._onClearButtonClick}/> :
                        null;

                    return (
                        <div
                        className={className}
                        onClick={this._expandPanelIfRequired}>
                            <input
                            className='i-role-views-list-search'
                            type="text"
                            value={this.props.querySpec.filterText}
                            onChange={this._onSearchTermChanged}
                            onKeyDown={this._onKeyDown}
                            onFocus={this._onFocus}
                            onBlur={this._onBlur}/>
                        {clearButton}
                        </div>
                        );
                }
            }
        });
});
