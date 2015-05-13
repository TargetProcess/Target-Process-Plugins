define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TimeLineMilestoneDetailsEditButtonsView',
        mixins: [React.addons.PureRenderMixin],

        propTypes: {
            onEdit: React.PropTypes.func.isRequired,
            onDelete: React.PropTypes.func.isRequired
        },

        componentDidMount() {
            $(this.refs.deleteButton.getDOMNode()).tauConfirm({
                content: '<h3>Do you really want to remove this Milestone?</h3>',
                callbacks: {
                    success: this.props.onDelete
                }
            });
        },

        componentWillUnmount() {
            var $deleteButton = $(this.refs.deleteButton.getDOMNode());
            if ($deleteButton.tauConfirm('instance')) {
                $deleteButton.tauConfirm('destroy');
            }
        },

        render() {
            return (
                <div className="tau-timeline-milestone-popup__edit__assign__controls">
                    <button onClick={this.props.onEdit} className="tau-btn tau-success tau-btn" type="button">Save Milestone</button>
                    <button ref='deleteButton' className="tau-btn tau-attention tau-btn" type="button">Delete</button>
                </div>
            );
        }
    });
});
