define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TimeLineMilestoneColors',
        POSSIBLE_COLORS: ['red',
            'orange',
            'yellow',
            'olive',
            'green',
            'turquoise',
            'blue',
            'orchid',
            'violet',
            'magenta'],

        mixins: [React.addons.PureRenderMixin],

        _handleColorSelect(color) {
            this.props.onColorSelect(color);
        },

        _cssClassNameForMilestoneFlag() {
            return `tau-timeline-milestone-marker tau-timeline-milestone--${this.props.color}`;
        },

        _getClassNameForColor(color) {
            return `tau-timeline-milestone-popup__edit_choose-color__item tau-timeline-milestone--${color}${this.props.color == color ? ' tau-active' : ''}`;
        },

        render() {
            var colors = this.POSSIBLE_COLORS.map(
                    color => <li onClick={this._handleColorSelect.bind(this, color)} key={color} className={this._getClassNameForColor(color)}>
                    <div className="tau-timeline-milestone-flag"></div>
                </li>);

            return (
                <ul className="tau-timeline-milestone-popup__edit_choose-color">
                    {colors}
                </ul>
            );
        }
    });
});
