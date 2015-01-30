define(function(require) {
    var $ = require('jQuery');
    var React = require('react');
    var SettingsListItem = require('jsx!../shared/settings.list.item.view');
    var filterRenderer = require('./list.filter.component.renderer');

    return React.createClass({
        componentDidMount: function() {
            filterRenderer.insertFilterComponent(this.refs.filterContainer.getDOMNode(),
                this.props.configurator, this.props.listDefinition);
        },

        getFilter: function() {
            return $(this.getDOMNode()).find('.i-role-filter-input').val();
        },

        render: function() {
            return (
                <SettingsListItem labelClassName="tau-select-inline" title="Filter">
                    <div className="tau-inline-group" ref="filterContainer"></div>
                </SettingsListItem>
            );
        }
    });
});
