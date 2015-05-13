define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TimeLineMilestoneDetailsFlag',

        mixins: [React.addons.PureRenderMixin],

        _handleClick() {
            this.props.onResetActiveMilestone(null, null);
        },

        _cssClassNameForMilestoneFlag() {
            return `tau-timeline-milestone-marker tau-timeline-milestone--${this.props.color}`;
        },

        render() {
            var viewMarkup = null;
            if (!this.props.hidden) {
                viewMarkup = (
                    <div className={this._cssClassNameForMilestoneFlag()}>
                        <div onClick={this._handleClick} className="tau-timeline-milestone-flag"></div>
                    </div>
                );
            }
            return viewMarkup;
        }
    });
});
