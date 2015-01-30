define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'LinkedViewLink',
        propTypes : {
            viewUrl: React.PropTypes.string
        },

        render: function() {
            if (!this.props.viewUrl) {
                return null;
            }

            return (
                <a
                    href={this.props.viewUrl}
                    target="_blank"
                    className="tau-widget-settings-list__link">
                Open in new tab
                </a>
            );
        }
    });
});