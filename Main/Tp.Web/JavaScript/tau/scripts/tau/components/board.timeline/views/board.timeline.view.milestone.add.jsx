define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TimeLineMilestoneButtonAddView',

        _handleClick() {
            this.props.onAddButtonClick(this._getButtonPosition(this.refs.add.getDOMNode()));

        },

        _getButtonPosition(buttonDOMNode) {
            var left = buttonDOMNode.getBoundingClientRect().left + 20;
            var top = buttonDOMNode.getBoundingClientRect().top + 25 + 'px';
            return {left, top};
        },

        render() {
            var className = `tau-btn-add-milestone i-role-action-add-milestone${this.props.clicked ? ' tau-btn-add-milestone-visible' : ''}`;
            return (
                <button title="Add milestone" ref="add" onClick={this._handleClick} className={className} type="button"></button>
            );
        }
    });
});
