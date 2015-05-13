define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TimeLineMilestoneDetailsAddButtonsView',
        mixins: [React.addons.PureRenderMixin],

        propTypes: {
            onAdd: React.PropTypes.func.isRequired,
            onEditStateChanged: React.PropTypes.func.isRequired,
            onCancel: React.PropTypes.func.isRequired
        },

        _handleAdd() {
            this.props.onEditStateChanged({});
            this.props.onAdd();
        },

        render() {
            return (
                <div className="tau-timeline-milestone-popup__edit__assign__controls">
                    <button onClick={this._handleAdd} className="tau-btn tau-success tau-btn" type="button">Add Milestone</button>
                    <button onClick={this.props.onCancel} className="tau-btn tau-btn-gray tau-btn" type="button">Cancel</button>
                </div>
            );
        }
    });
});
