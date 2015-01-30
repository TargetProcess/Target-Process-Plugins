define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var ListSelector = require('./selector.view');
    var ViewLink = require('jsx!../../linked.view.base/view.link.view');
    var PageSizeSelector = require('jsx!../page.size.selector.view');
    var SettingsContainer = require('jsx!../../shared/settings.container.view');

    return React.createClass({
        displayName: 'LinkedListWidgetSettings',

        propTypes: {
            viewUrl: React.PropTypes.string,
            views: React.PropTypes.array.isRequired,
            initialSelectedViewId: React.PropTypes.string,
            initialPageSize: React.PropTypes.number
        },

        getSelectedSettings: function() {
            return {
                selectedViewId: this.refs.listSelector.getSelectedViewId(),
                pageSize: this.refs.pageSizeSelector.getSelectedPageSize()
            };
        },

        render: function() {
            return (
                <SettingsContainer>
                    <ListSelector
                        ref="listSelector"
                        initialSelectedViewId={this.props.initialSelectedViewId}
                        views={this.props.views}/>
                    <ViewLink viewUrl={this.props.viewUrl}/>
                    <PageSizeSelector
                        ref="pageSizeSelector"
                        initialPageSize={this.props.initialPageSize}/>
                </SettingsContainer>
            );
        }
    });
});