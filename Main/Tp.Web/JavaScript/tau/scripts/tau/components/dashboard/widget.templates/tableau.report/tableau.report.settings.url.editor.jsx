define(function(require) {
    var React = require('react');

    var ReportUrl = require('./tableau.report.url');

    return React.createClass({
        displayName: 'tableau.report.settings.url.editor',

        propTypes: {
            initialReportUrl: React.PropTypes.string,
            onChange: React.PropTypes.func,
            onEnterKey: React.PropTypes.func
        },

        getInitialState() {
            return {
                reportUrl: this.props.initialReportUrl
            };
        },

        getCurrentReportUrl() {
            return this.state.reportUrl;
        },

        _onUrlInputChange(e) {
            var newUrl = e.target.value;

            this.setState({reportUrl: newUrl});
            if (this.props.onChange) {
                this.props.onChange(newUrl);
            }
        },

        _onUrlInputKeyDown(e) {
            if (e.which === $.ui.keyCode.ENTER && this.props.onEnterKey) {
                this.props.onEnterKey();
            }
        },

        render() {
            return (
                <div>
                    <input
                        className="tau-tableau-report-widget-settings__url-editor tau-in-text tau-x-large"
                        type="url"
                        placeholder={'e.g. ' + ReportUrl.reportUrlExample}
                        value={this.state.reportUrl}
                        onChange={this._onUrlInputChange}
                        onKeyDown={this._onUrlInputKeyDown} />
                    <span className="tau-help i-role-tooltipArticle" data-article-id="tableau.report.widget.url"/>
                </div>
            );
        }
    });
});