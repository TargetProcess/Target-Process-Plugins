define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        DslFilter = require('jsx!./../../dsl-filter/views/dsl-filter.view'),
        PickerIndicator = require('jsx!./visualEncoding.colorPicker.indicator.view'),
        SortableItem = require('tau/components/react/mixins/sortable.item');

    return React.createClass({
        displayName: 'VisualEncodingItem',
        _saveFilter(value) {
            this.props.model.bus.fire('saveRule', _.defaults({filter: value.filter}, this.props));
        },
        getInitialState() {
            return {draggable: true};
        },
        _getDslProps() {
            return {
                configurator: this.props.model.configurator,
                boardSettings: this.props.model.boardSettings,
                onChange: this._saveFilter,
                onFocus: this._onFilterFocus,
                onBlur: this._onFilterBlur,
                filter: this.props.filter,
                isEditable: this.props.model.isEditable
            };
        },
        _onFilterFocus() {
            this.setState({draggable: false});
        },
        _onFilterBlur() {
            this.setState({draggable: true});
        },
        _getColorPickerProps() {
            return {
                colors: this.props.model.configurator.getColorEncodingService().getColorMap(),
                onChange: this._saveValue,
                value: this.props.value,
                isEditable: this.props.model.isEditable
            };
        },
        _saveValue(value) {
            this.props.model.bus.fire('saveRule', _.defaults({value: value.value}, this.props));
        },
        _removeItem() {
            this.props.model.bus.fire('removeRule', this.props);
        },
        render() {
            var idEditable = this.props.model.isEditable;
            var sortableHelper = idEditable ?
                <span className="ui-sortable-helper">
                    <div className="tau-icon-general tau-icon-dragdrop-indicator"></div>
                </span> :
                null;
            var removeItem = idEditable ?
                <span onClick={this._removeItem} className="tau-icon-general tau-icon-close-red i-role-clear-filter"></span> :
                null;

            return (
                <li className="tau-color-encoding-list__item" {...SortableItem.attributes(this.props.UUID, SortableItem.VISUAL_ENCODING_ITEM, idEditable && this.state.draggable)}>
                    {sortableHelper}
                    <DslFilter {...this._getDslProps()}/>
                    <PickerIndicator {...this._getColorPickerProps()}/>
                    {removeItem}
                    <div className="tau-color-encoding__drop-placeholder"></div>
                </li>
            )
        }

    });
});
