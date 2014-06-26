define(function(require) {
    var React = require('libs/react/react-ex');

    return React.defineClass(['boardMenuBus', 'settingsService'], function(bus, settingsService) {
        return {
            componentDidMount: function() {
                settingsService.getShowHiddenBoards().then(this._setShowHidden);
            },

            render: function() {
                var toggleHiddenButton = this.props.hasHiddenBoards ?
                    <div className={this._getToggleHiddenBoardsButtonClassName()} onClick={this._onToggleHiddenBoards}></div> :
                    null;

                return (
                    <div className="t3-controls">
                        <div className="t3-control t3-add-view-trigger" onClick={this._onCreateBoard}></div>
                        {toggleHiddenButton}
                    </div>
                    );
            },

            _getToggleHiddenBoardsButtonClassName: function() {
                var cx = React.addons.classSet;
                var classes = {
                    't3-control': true,
                    't3-show-hidden-views-trigger': true,
                    't3-active': this.props.showHidden
                };

                return cx(classes);
            },

            _onToggleHiddenBoards: function() {
                var newValue = !this.props.showHidden;
                this._setShowHidden(newValue);
                settingsService.setShowHiddenBoards(newValue);
            },

            _onCreateBoard: function() {
                this.props.createBoard && this.props.createBoard();
            },

            _setShowHidden: function(show) {
                bus.fire('board.menu.toggle.hidden', show);
            }
        }
    });
});
