define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var classNames = require('libs/classNames');

    return React.createClass({
        _onSelected: function() {
            if (_.isFunction(this.props.onSelected)) {
                this.props.onSelected(this.props.name);
            }
        },

        render: function() {
            var className = classNames({
                'i-role-tag': true,
                'tau-board-settings__tag-list__item': true,
                'tau-active': this.props.isSelected
            });
            return (
                <li className={className} onClick={this._onSelected}>
                    <span>{this.props.name}</span>
                </li>
            );
        }
    });
});
