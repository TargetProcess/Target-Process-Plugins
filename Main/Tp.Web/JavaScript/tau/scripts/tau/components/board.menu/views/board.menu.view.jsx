define(function(require) {
    var _ = require('Underscore');

    var React = require('libs/react/react-ex');

    var listModel = require('./../models/board.menu.list.model');

    var dependencies = [
        'boardMenuListView',
        'boardMenuActionsView',
        'boardMenuSearchView',
        'boardSettingsService'
    ];

    return React.defineClass(dependencies, function(ListView, ActionsView, SearchView, boardSettingsService) {

        return {
            getInitialState: function() {
                return {
                    allBoards: [],
                    filterText: '',
                    currentBoardId: '',
                    showHidden: false,
                    focusedBoardId: '',
                    isLoading: false
                };
            },

            componentWillMount: function() {
                this.setState({currentBoardId: boardSettingsService.getCurrentSettings().id});

                this.props.model.acidUpdated.add(this._onAcidUpdated, this);
                boardSettingsService.settingsUpdated.add(this._onBoardSettingsUpdated, this);
            },

            componentWillUnmount: function() {
                this.props.model.acidUpdated.remove(this);
                boardSettingsService.settingsUpdated.remove(this);
            },

            componentDidMount: function() {
                this.setState({isLoading: true});

                this.props.model.loadBoards()
                    .done(this._whenBoardsLoaded)
                    .fail(this._whenBoardLoadingFailed);
            },

            _whenBoardsLoaded: function(data) {
                var boards = _.map(data.items, this._createBoardItem);
                this.setState({
                    isLoading: false,
                    allBoards: boards
                });
            },

            _whenBoardLoadingFailed: function() {
                this.setState({
                    isLoading: false,
                    allBoards: []
                });
            },

            _createBoardItem: function(boardData) {
                // TODO: perhaps create and return React component here?
                // TODO: or maybe a board view model wrapper?
                var boardId = boardData.key || boardData.id;

                return {
                    boardId: boardId,
                    link: this.props.model.getBoardLink(boardId),
                    name: boardData.name || boardId,
                    viewMode: boardData.viewMode,
                    isShared: boardData.isShared,
                    menuIsVisible: boardData.menuIsVisible,
                    edit: !!boardData.edit
                };
            },

            _onAcidUpdated: function() {
                _.each(this.state.allBoards, function(board) {
                    board.link = this.props.model.getBoardLink(board.boardId);
                }, this);

                this.setState({allBoards: this.state.allBoards});
            },

            _onBoardSettingsUpdated: function(newSettings) {
                var newBoardId = boardSettingsService.getCurrentSettings().id;
                this.setState({currentBoardId: newBoardId});

                var currentBoard = _.find(this.state.allBoards, function(board) {
                    return board.boardId === newBoardId;
                });

                if (currentBoard) {
                    // FIXME: looks ugly
                    _.each(newSettings, function(v, k) {
                        if (_.has(currentBoard, k)) {
                            currentBoard[k] = v;
                        }
                    });

                    this.setState({allBoards: this.state.allBoards});
                }
            },

            _getVisibleBoards: function() {
                var filteredBoards = listModel.applyBoardFilter(this.state.allBoards, this.state.filterText);
                if (this.state.showHidden) {
                    return filteredBoards;
                }

                return listModel.filterHiddenBoards(filteredBoards, this.state.currentBoardId);
            },

            _hasHiddenBoards: function() {
                return _.some(this.state.allBoards, function(board) {
                    return board.menuIsVisible === false; // board is visible if true or not present
                });
            },

            _createBoard: function() {
                this.props.model.createBoard().done(function(boardDefinition) {
                    var boardItem = this._createBoardItem(boardDefinition);
                    this.state.allBoards.push(boardItem);
                    this.setState({allBoards: this.state.allBoards});

                    this.props.model.redirectToBoard(boardDefinition.id);
                }.bind(this));
            },

            render: function() {
                if (this.state.isLoading) {
                    return <div className="t3-views-navigator"></div>;
                }

                var visibleBoards = this._getVisibleBoards();

                return (
                    <div className="t3-views-navigator">
                        <SearchView
                            boards={visibleBoards}
                            searchTerm={this.state.filterText}
                            currentBoardId={this.state.currentBoardId}
                            focusedBoardId={this.state.focusedBoardId}
                            model={this.props.model}/>
                        <ListView
                            boards={visibleBoards}
                            currentBoardId={this.state.currentBoardId}
                            focusedBoardId={this.state.focusedBoardId}/>
                        <ActionsView
                            hasHiddenBoards={this._hasHiddenBoards()}
                            showHidden={this.state.showHidden}
                            createBoard={this._createBoard} />
                    </div>
                    );
            }
        }
    });
});
