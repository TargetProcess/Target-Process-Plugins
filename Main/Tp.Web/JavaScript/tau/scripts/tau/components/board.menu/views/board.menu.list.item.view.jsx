define(function(require) {
    var $ = require('jQuery');

    var React = require('react');

    return React.createClass({
        render: function() {
            var board = this.props.board;

            return (
                <li>
                    <a href={board.link}>
                        <div className={this._getClassName()}>
                            <div className="t3-name">{board.name}</div>
                            <div className="t3-actions-trigger" onClick={this._showContextMenu}></div>
                        </div>
                    </a>
                </li>
                );
        },

        _showContextMenu: function(event) {
            this.props.showContextMenu && this.props.showContextMenu(this.props.board, $(event.target));

            event.stopPropagation();
            event.preventDefault();
        },

        _getClassName: function() {
            var isVisible = this.props.board.menuIsVisible;
            var classes = {
                't3-view': true,
                't3-private': !this.props.board.isShared,
                't3-active': this.props.isCurrentBoard,
                't3-focused': this.props.isFocusedBoard,
                't3-hidden': isVisible === false
            };

            var viewModeName = (this.props.board.viewMode || '').toLowerCase();
            var viewModeClass = this._viewModeClassNames[viewModeName] || this._defaultViewModeClassName;
            classes[viewModeClass] = true;

            return React.addons.classSet(classes);
        },

        _viewModeClassNames: {
            'list': 't3-detailed-view',
            'newlist': 't3-list-view',
            'timeline': 't3-timeline-view'
        },

        _defaultViewModeClassName: 't3-grid-view'
    });
});
