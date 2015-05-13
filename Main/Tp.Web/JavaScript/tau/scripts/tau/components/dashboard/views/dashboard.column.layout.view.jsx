define(function(require) {
    var _ = require('Underscore');
    var $ = require('jQuery');
    var React = require('react');
    var Sortable = require('tau/components/dashboard/sortable.widgets');
    var SortableItem = require('tau/components/react/mixins/sortable.item');
    var sortableScroll = require('tau/components/react/mixins/sortable.scroll');
    var WidgetContainer = require('jsx!./dashboard.widget.container.view');
    var ColumnSelectionMixin = require('./dashboard.column.selection.mixin');
    var ListTemplate = require('tau/components/dashboard/widget.templates/new.list/linked/new.list.linked.widget.template');
    var ReportTemplate = require('tau/components/dashboard/widget.templates/custom.report/linked/custom.report.linked.template');

    return React.createClass({
        displayName: 'ColumnLayout',

        mixins: [Sortable, ColumnSelectionMixin],

        getSortableOptions() {
            return {
                sortableRootSelector: '.tau-dashboard-columns-layout__body',
                sortableItemSelector: '.i-role-dashboard-widget',
                sortableHandleSelector: '.tau-dashboard-widget__drag-handle',
                setDragImage(event, $handle) {
                    return {element: $handle[0], x: 20, y: 22};
                },
                sortItems: this.sortItems,
                acceptedTypes: [SortableItem.DASHBOARD_WIDGET,
                    SortableItem.MENU_ITEM_LIST, SortableItem.MENU_ITEM_REPORT]
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

        /**
         * @param {{type: String, key: String, name: String?}} droppedItem
         * @param {String} targetKey
         * @param {Boolean} placeAfter
         */
        sortItems(droppedItem, targetKey, placeAfter) {
            switch (droppedItem.type) {
                case SortableItem.DASHBOARD_WIDGET:
                    this._moveWidget(droppedItem, targetKey, placeAfter);
                    break;

                case SortableItem.MENU_ITEM_LIST:
                    this._createWidgetFromMenuItem(ListTemplate.TEMPLATE_ID, droppedItem, targetKey, placeAfter);
                    break;

                case SortableItem.MENU_ITEM_REPORT:
                    this._createWidgetFromMenuItem(ReportTemplate.TEMPLATE_ID, droppedItem, targetKey, placeAfter);
                    break;

                default:
                    console.warn('Unknown item type "' + droppedItem.type + '" was passed to sortItems()');
            }
        },

        _moveWidget(widget, targetKey, placeAfter) {
            var targetColumnKey = this._tryGetColumnKeyFromSortableKey(targetKey);
            if (targetColumnKey) {
                this.props.layout.moveWidgetToColumn(widget.key, targetColumnKey, placeAfter);
            } else {
                this.props.layout.moveWidget(widget.key, targetKey, placeAfter);
            }
        },

        _createWidgetFromMenuItem(widgetTemplateId, menuItem, targetKey, placeAfter) {
            var targetColumnKey = this._tryGetColumnKeyFromSortableKey(targetKey);
            var target = {widgetId: targetKey, columnId: targetColumnKey, placeAfter: placeAfter};
            this.props.layout.createWidgetFromView(widgetTemplateId, {id: menuItem.key, name: menuItem.name}, target);
        },

        render() {
            var layout = this.props.layout;
            var hasAnyWidgets = false;
            var columns = _.map(layout.columns, function(columnDef) {
                var widgets = _.map(columnDef.widgets, widget => this._renderWidget(columnDef, widget));
                if (widgets.length) {
                    hasAnyWidgets = true;
                }

                var sortableKey = this._buildColumnSortableKey(columnDef);

                var columnStyle = {};
                var desiredWidth = columnDef.getDesiredColumnWidth();
                if (desiredWidth) {
                    columnStyle.width = desiredWidth;
                }

                return (
                    <div key={columnDef.instanceId}
                        id={'dashboard-' + sortableKey} data-sortable-key={sortableKey}
                        className="i-role-dashboard-column tau-dashboard-columns-layout__column i-role-unit-editor-popup-append-target"
                        style={columnStyle}>
                        <div
                            className="i-role-dashboard-column-widgets tau-dashboard-widgets"
                            ref={this.getColumnContentRefName(columnDef)}>
                            <div className="tau-dashboard-widget__drop-placeholder"></div>
                            {widgets}
                        </div>
                    </div>
                );
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

            var sortableAttributes = layout.isEditable ?
                sortableScroll.verticalScroll(() => this.refs.scrollable, this) : {};

            return (
                <div className="i-role-dashboard-layout tau-dashboard-columns-layout">
                    <div ref="scrollable" className="tau-dashboard-columns-layout__body tau-custom-scrollbar" {...sortableAttributes}>
                        <div className="tau-dashboard-columns-layout__columns">
                            {columns}
                        </div>
                    </div>
                    {emptyMessage}
                </div>
            );
        },

        _renderWidget(column, widget) {
            return <WidgetContainer id={widget.instanceId} key={widget.instanceId} column={column}
                isEditable={this.props.layout.isEditable} widget={widget} />;
        }
    });
});
