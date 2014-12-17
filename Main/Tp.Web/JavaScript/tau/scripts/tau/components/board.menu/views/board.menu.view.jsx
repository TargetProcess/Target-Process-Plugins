define(function(require) {
    var _ = require('Underscore');
    var $ = require('jQuery');
    var React = require('libs/react/react-ex');
    var transformations = require('./../services/board.menu.transformations.service');

    var dependencies = [
        'boardMenuListView',
        'boardMenuActionsView',
        'boardMenuSearchView',
        'boardSettingsService'
    ];

    return React.defineClass(dependencies,
        function(ListView, ActionsView, SearchView, boardSettingsService) {

            return {
                getDefaultProps: function() {
                    return {
                        filterText: '',
                        viewMenuSections: null
                    };
                },

                getInitialState: function() {
                    return {
                        currentBoardId: '',
                        showHidden: false,
                        focusedBoardId: '',
                        isLoading: true
                    };
                },

                componentWillMount: function() {
                    this.setState({currentBoardId: boardSettingsService.getCurrentSettings().id});
                },

                componentDidMount: function() {
                    this.setState({isLoading: true});

                    this.props.model.loadBoards()
                        .always(function() {
                            this.setState({
                                isLoading: false
                            });
                        }.bind(this));
                },

                componentWillReceiveProps: function(newProps) {
                    if (newProps.filterText !== this.props.filterText) {
                        var querySpec = this._createBoardQuerySpec();
                        querySpec.filterText = newProps.filterText;
                        var focusedId = transformations.getDefaultFocusedBoardId(
                            newProps.viewMenuSections.getAllViewGroups(), querySpec);
                        this.setState({focusedBoardId: focusedId});
                    }
                },

                _getVisibleBoards: function() {
                    // TODO: optimize
                    var boards = _.flatMap(this.props.viewMenuSections.getAllViewGroups(), _.property('boards'));
                    return transformations.getVisibleBoards(boards, this._createBoardQuerySpec());
                },

                /**
                 * @return {BoardQuerySpec}
                 */
                _createBoardQuerySpec: function() {
                    return {
                        filterText: this.props.filterText,
                        showHidden: this.state.showHidden,
                        currentBoardId: this.state.currentBoardId
                    };
                },

                render: function() {
                    if (this.state.isLoading) {
                        return <div className="t3-views-navigator i-role-board-menu"></div>;
                    }

                    //TODO: move current query spec to model

                    var querySpec = this._createBoardQuerySpec();

                    return (
                        <div className="t3-views-navigator i-role-board-menu">
                            <SearchView
                            viewMenuSections={this.props.viewMenuSections}
                            querySpec={querySpec}
                            focusedBoardId={this.state.focusedBoardId}/>
                            <ListView
                            ref="listView"
                            viewMenuSections={this.props.viewMenuSections}
                            currentBoardId={this.state.currentBoardId}
                            focusedBoardId={this.state.focusedBoardId}
                            model={this.props.model}
                            querySpec={querySpec} />
                            <ActionsView
                            hasHiddenBoards={this.props.viewMenuSections.hasHiddenItems()}
                            showHidden={this.state.showHidden} />
                        </div>
                        );
                }
            }
        });
});
