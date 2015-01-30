define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var SettingsListItem = require('jsx!../shared/settings.list.item.view');

    return React.createClass({
        displayName: 'ListWidgetPageSizeSelector',
        propTypes: {
            initialPageSize: React.PropTypes.number,
            availablePageSizes: React.PropTypes.arrayOf(React.PropTypes.number)
        },

        mixins: [React.addons.LinkedStateMixin],

        getDefaultProps: _.constant({
            availablePageSizes: [5, 10, 15, 20, 30],
            initialPageSize: 10
        }),

        getInitialState: function() {
            return {
                pageSize: this.props.initialPageSize
            };
        },

        getSelectedPageSize: function() {
            return this.state.pageSize;
        },

        render: function() {
            var options = _.map(this.props.availablePageSizes, function(pageSize) {
                return <option key={pageSize} value={pageSize}>{pageSize}</option>;
            });

            return (
                <SettingsListItem
                    labelClassName="tau-select-inline"
                    title="Show top">

                    <select
                        className="tau-select-inline tau-widget-settings__list"
                        valueLink={this.linkState("pageSize")}>
                        {options}
                    </select>
                    <span className="tau-widget-settings-list__title">elements</span>
                </SettingsListItem>
            );
        }
    });
});