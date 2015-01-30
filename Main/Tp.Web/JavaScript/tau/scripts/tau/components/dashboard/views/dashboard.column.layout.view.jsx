define(function(require) {
    var _ = require('Underscore');
    var $ = require('jQuery');
    var React = require('react');
    var Sortable = require('tau/components/dashboard/sortable.widgets');
    var WidgetContainer = require('jsx!./dashboard.widget.container.view');

    var ColumnSelectionMixin = require('./dashboard.column.selection.mixin');

    return React.createClass({
        displayName: 'ColumnLayout',

        mixins: [Sortable, ColumnSelectionMixin],

        getSortableOptions() {
            return {
                sortableRootSelector: '.tau-dashboard-columns-layout__body',
                sortableItemSelector: '.i-role-dashboard-widget[draggable=true]',
                sortableHandleSelector: '.tau-dashboard-widget__title',
                sortItems: this.sortItems
            };
        },

        _columnSortableTargetPrefix: 'column-',

        _buildColumnSortableKey(columnModel) {
            return this._columnSortableTargetPrefix + columnModel.instanceId;
        },

        _tryGetColumnKeyFromSortableKey(sortableKey) {
            if (_.startsWith(sortableKey, this._columnSortableTargetPrefix)) {
                return sortableKey.substring(this._columnSortableTargetPrefix.length);
            }
        },

        sortItems(widgetKey, targetKey, placeAfter) {
            var targetColumnKey = this._tryGetColumnKeyFromSortableKey(targetKey);
            if (targetColumnKey) {
                this.props.layout.moveWidgetToColumn(widgetKey, targetColumnKey, placeAfter);
            } else {
                this.props.layout.moveWidget(widgetKey, targetKey, placeAfter);
            }
        },

        render() {
            var layout = this.props.layout;
            var hasAnyWidgets = false;
            var columns = _.map(layout.columns, function(columnDef) {
                var widgets = _.map(columnDef.widgets, widget => this._renderWidget(columnDef, widget));
                if (widgets.length) {
                    hasAnyWidgets = true;
                }

                return (<div key={columnDef.instanceId}
                        data-sortable-key={this._buildColumnSortableKey(columnDef)}
                        className="i-role-dashboard-column tau-dashboard-columns-layout__column">
                    <div className="tau-dashboard-widget__drop-placeholder"></div>
                    <div className="i-role-dashboard-column-widgets tau-dashboard-widgets"
                            ref={this.getColumnContentRefName(columnDef)}>
                        {widgets}
                    </div>
                </div>);
            }, this);

            var emptyMessage;
            if (!hasAnyWidgets) {
                emptyMessage = <div className="tau-dashboard-empty-message">
                    <h2 className="tau-dashboard-empty-message__title">Add your first widget to this dashboard</h2>
                    <h3 className="tau-dashboard-empty-message__title--small">
                        If you need more columns you can change the layout
                    </h3>
                </div>;
            }

            return (<div className="i-role-dashboard-layout tau-dashboard-columns-layout">
                <div className="tau-dashboard-columns-layout__body"
                        onMouseDown={this.sortMouseDown}
                        onDragStart={this.sortStart} onDragOver={this.sortDragOver}
                        onDragEnd={this.sortEnd} onDrop={this.sortDrop}>
                    <div className="tau-dashboard-columns-layout__columns">
                        {columns}
                    </div>
                </div>
                {emptyMessage}
            </div>);
        },

        _renderWidget(column, widget) {
            return <WidgetContainer id={widget.instanceId} key={widget.instanceId} column={column}
                    isEditable={this.props.layout.isEditable} widget={widget} />;
        }
    });
});
