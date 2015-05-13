define(function(require) {
    var React = require('react');

    var SettingsListItem = require('jsx!../shared/settings.list.item.view');
    var SettingsContainer = require('jsx!../shared/settings.container.view');
    var UrlEditor = require('jsx!./tableau.report.settings.url.editor');

    return React.createClass({
        displayName: 'tableau.report.settings.view',

        propTypes: {
            initialReportUrl: React.PropTypes.string
        },

        getCurrentSettings() {
            return {
                reportUrl: this.refs.urlEditor.getCurrentReportUrl()
            };
        },

        render() {
            return (
                <SettingsContainer>
                    <SettingsListItem
                        title="Link to the Tableau report">
                        <UrlEditor
                            ref="urlEditor"
                            initialReportUrl={this.props.initialReportUrl} />
                    </SettingsListItem>
                </SettingsContainer>
            );
        }
    });
});