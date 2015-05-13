define(function(require) {
    var $ = require('jQuery');
    var React = require('react');

    var SettingsListItem = require('jsx!../shared/settings.list.item.view');
    var SettingsContainer = require('jsx!../shared/settings.container.view');
    var UrlEditor = require('jsx!./tableau.report.settings.url.editor');

    var ReportUrl = require('./tableau.report.url');

    return React.createClass({
        displayName: 'tableau.report.selector',

        propTypes: {
            onApply: React.PropTypes.func.isRequired
        },

        getDefaultProps() {
            return {
                labelText: 'Please specify a link to the Tableau report you\'d like to display here'
            };
        },

        getInitialState() {
            return {
                reportUrl: ''
            };
        },

        _onUrlInputChange(reportUrl) {
            this.setState({reportUrl: reportUrl});
        },

        _applyUrl() {
            if (this._isValidUrl()) {
                this.props.onApply(this.state.reportUrl);
            }
        },

        _isValidUrl() {
            return ReportUrl.isValidReportUrl(this.state.reportUrl);
        },

        render() {
            return (
                <SettingsContainer>
                    <SettingsListItem
                        title="Please specify a link to the Tableau report you'd like to display here">
                        <UrlEditor
                            onChange={this._onUrlInputChange}
                            onEnterKey={this._applyUrl}/>
                    </SettingsListItem>

                    {this._renderApplyButton()}
                </SettingsContainer>
            );
        },

        _renderApplyButton() {
            return (
                <button
                    className="tau-btn tau-primary"
                    onClick={this._applyUrl}
                    disabled={!this._isValidUrl()}>
                    Show this report
                </button>
            );
        }
    });
});