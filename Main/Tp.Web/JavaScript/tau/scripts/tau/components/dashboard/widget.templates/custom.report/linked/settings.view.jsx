define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var ViewSelector = require('./selector.view');
    var ViewLink = require('jsx!../../linked.view.base/view.link.view');
    var SettingsContainer = require('jsx!../../shared/settings.container.view');

    return React.createClass({
        displayName: 'LinkedReportWidgetSettings',
        propTypes: {
            initialSelectedViewId: React.PropTypes.string,
            views: React.PropTypes.array.isRequired,
            viewUrl: React.PropTypes.string
        },

        getSelectedSettings: function() {
            return {
                selectedViewId: this.refs.viewSelector.getSelectedViewId()
            };
        },

        render: function() {
            return (
                <SettingsContainer>
                    <ViewSelector
                        ref="viewSelector"
                        initialSelectedViewId={this.props.initialSelectedViewId}
                        views={this.props.views}/>
                    <ViewLink viewUrl={this.props.viewUrl}/>
                </SettingsContainer>
            );
        }
    });
});