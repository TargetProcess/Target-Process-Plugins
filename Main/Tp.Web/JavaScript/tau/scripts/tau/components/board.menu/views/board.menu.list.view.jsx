define(function(require) {
    var _ = require('Underscore');
    var React = require('libs/react/react-ex');
    var SortableGroups = require('tau/components/board.menu/sortable-groups');
    var Tooltips = require('tau/components/board.menu/tooltips');
    var Scrollable = require('tau/components/board.menu/views/board.menu.list.view.scrollable.mixin');

    return React.defineClass(
        ['board.menu.list.section.view', 'boardSettingsService', 'boardMenuScrollService'],

        function(SectionView, boardSettingsService, scrollService) {
            return {
                mixins: [SortableGroups, Tooltips, Scrollable],

                componentDidMount: function() {
                    boardSettingsService.currentBoardChanged.add(this._scrollToCurrentView, this);

                    var $list = $(this.getDOMNode());
                    var intervalId = setInterval(function() {
                        if ($list.closest('[role=application]').length > 0) {
                            this._scrollToCurrentView();
                            clearInterval(intervalId);
                        }
                    }.bind(this), 100);
                },

                componentWillUnmount: function() {
                    boardSettingsService.currentBoardChanged.remove(this);
                },

                _scrollToCurrentView: function() {
                    var model = this.props.model;
                    var currentBoard = model.viewMenuSections.findBoardById(this.props.currentBoardId);
                    if (currentBoard && currentBoard.menuGroupKey) {
                        var group = model.viewMenuSections.findGroupById(currentBoard.menuGroupKey);
                        if (group && group.hasBoard(currentBoard.boardId)) {
                            model.expandGroup(currentBoard.menuGroupKey);
                        }
                    }

                    scrollService.scrollToCurrentMenuElementIfNeeded();
                },

                sortItems: function(key, overKey, groupKey, placeAfter) {
                    this.props.model.prioritizeBoard(key, overKey, groupKey, placeAfter);
                },

                sortGroups: function(groupKey, targetGroupKey, placeAfter) {
                    this.props.model.prioritizeGroup(groupKey, targetGroupKey, placeAfter);
                },

                getInitialState: function() {
                    return {
                        renamingId: null
                    };
                },

                render: function() {
                    var sectionNodes = _.map(this.props.viewMenuSections.items, function(section) {
                        return new SectionView({
                            key: section.sectionType,
                            section: section,
                            querySpec: this.props.querySpec,
                            currentBoardId: this.props.currentBoardId,
                            focusedBoardId: this.props.focusedBoardId,
                            renamingId: this.state.renamingId
                        });
                    }, this);

                    return (
                        <div className="t3-views-catalog"
                        onDragStart={this.sortStart} onDragOver={this.sortDragOver}
                        onDragEnd={this.sortEnd} onDrop={this.sortDrop}>
                        {sectionNodes}
                        </div>
                        );

                }
            }
        });
});
