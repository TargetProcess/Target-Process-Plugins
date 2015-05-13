define(function(require) {
    var React = require('react'),
        MilestoneMixin = require('tau/components/board.timeline/views/board.timeline.view.active.milestone.mixin');


    return React.createClass({
        displayName: 'TimeLineMilestoneRulerView',
        mixins: [React.addons.PureRenderMixin, MilestoneMixin],
        FLAG_POSITION_MARGIN: 29,

        componentDidUpdate() {
            if (this.props.isActive) {
                this.props.onUpdatePosition(this._getFlagPosition(this.refs.flag.getDOMNode()));
            }
        },

        _handleClick() {
            this.props.onSetActiveMilestone(this.props.id, this._getFlagPosition(this.refs.flag.getDOMNode()));
        },

        _getFlagPosition(flagDOMNode) {
            var detailsMarginLeft = window.getComputedStyle(flagDOMNode.parentNode, null).getPropertyValue('margin-left').replace('px', '');
            var left = flagDOMNode.getBoundingClientRect().left - detailsMarginLeft - 1;
            var top = flagDOMNode.getBoundingClientRect().top - this.FLAG_POSITION_MARGIN + 'px';
            return {left, top};
        },

        render() {
            var classSet = `tau-timeline-milestone-marker ${this.props.cssClassName} ${this.props.isHighlighted ? 'tau-hover' : ''}`;

            return (
                <div onClick = {this._handleClick} onMouseOver = {this._handleMouseOver} onMouseOut = {this._handleMouseOut} className={classSet}
                    style={{
                        left: this.props.left,
                        opacity: this.props.isHidden ? 0 : 1
                    }}>
                    <div ref="flag" className="tau-timeline-milestone-flag"></div>
                    <div className="tau-timeline-milestone-title">{this.props.name}</div>
                </div>
            );
        }
    });
});
