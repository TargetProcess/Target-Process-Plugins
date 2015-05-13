define(function(require) {
    var React = require('react'),
        MilestoneMixin = require('tau/components/board.timeline/views/board.timeline.view.active.milestone.mixin');

    return React.createClass({
        displayName: 'TimeLineMilestoneView',
        mixins: [React.addons.PureRenderMixin, MilestoneMixin],

        render(){
            var className = `tau-timeline-milestone-marker ${this.props.cssClassName} ${this.props.isHighlighted ? 'tau-hover' : ''}`;
            return (
                <div onMouseOver = {this._handleMouseOver} onMouseOut = {this._handleMouseOut} className={className} style={{left: this.props.left}}>
                </div>
            );
        }
    });
});
