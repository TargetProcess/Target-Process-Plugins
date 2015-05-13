define(function(require) {
    var React = require('react');

    /**
     * @class WidgetErrorView
     * @extends ReactComponent
     */
    return React.createClass({
        displayName: 'WidgetErrorView',

        propTypes: {
            header: React.PropTypes.string,
            message: React.PropTypes.string,
            technicalDetails: React.PropTypes.string
        },

        getDefaultProps() {
            return {
                header: 'Unable to display a widget content here',
                message: 'Some error occurred and we\'re not sure why',
                technicalDetails: null
            };
        },

        getInitialState() {
            return {
                showTechnicalDetails: false
            };
        },

        _showDetails() {
            this.setState({showTechnicalDetails: true});
        },

        render() {
            return (
                <div className="tau-dashboard-widget-placeholder">
                    <div className="tau-dashboard-widget-placeholder-wrapper">
                        <div className="tau-dashboard-widget-placeholder-text">
                            <div>{this.props.header}</div>
                            <div className="i-role-widget-error-view__message">{this.props.message}</div>
                            {this._renderDetailsBlock()}
                        </div>
                    </div>
                </div>
            );
        },

        _renderDetailsBlock() {
            if (!this.props.technicalDetails) {
                return null;
            }

            var content = this.state.showTechnicalDetails ?
                <div>{this.props.technicalDetails}</div> :
                <button
                    className="tau-btn"
                    onClick={this._showDetails}>
                    Show error details
                </button>;

            return (
                <div className="tau-dashboard-widget-placeholder__details">{content}</div>
            );
        }
    });
});
