define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var React = require('react');
    var {Container, StatusMessage} = require('jsx!./filter.builder.shared.views');
    var FieldSelector = require('jsx!./filter.builder.field.selector');
    var RowView = require('jsx!./filter.builder.row.view');

    var AutoFocusMixin = require('jsx!./filter.builder.new.field.auto.focus.mixin');

    return React.createClass({
        displayName: 'filter.builder.view',

        propTypes: {
            builderModel: React.PropTypes.shape({
                addField: React.PropTypes.func.isRequired,
                getFieldModels: React.PropTypes.func.isRequired,
                clearAllFields: React.PropTypes.func.isRequired,
                getAvailableNewFields: React.PropTypes.func.isRequired
            }).isRequired
        },

        mixins: [AutoFocusMixin],

        _onFieldSelected(evt) {
            var selectedValue = $(evt.target).val();
            if (selectedValue) {
                var addedFieldId = this.props.builderModel.addField(selectedValue).fieldModelId;
                this.autoFocusOn(addedFieldId);
            }
        },

        _onClearFilters() {
            this.props.builderModel.clearAllFields();
        },

        render() {
            var fieldModels = this.props.builderModel.getFieldModels();
            var rows = _.map(fieldModels, fieldModel =>
                <RowView key={fieldModel.fieldModelId} fieldModel={fieldModel} builderModel={this.props.builderModel} />);

            var fieldSelector = this._renderFieldSelector();

            if (!rows.length && !fieldSelector) {
                return (
                    <Container>
                        <StatusMessage>
                            Visual filters are not supported for current configuration
                        </StatusMessage>
                    </Container>
                );
            }

            var clearButton = this._renderClearButton(fieldModels);
            var controls = clearButton || fieldSelector ?
                <div className="filter-builder__controls">
                    {fieldSelector}
                    {clearButton}
                </div> : null;

            return (
                <Container>
                    <div className="filter-builder__body">
                        <div className="filter-builder__table">
                            {rows}
                        </div>
                    </div>
                    {controls}
                </Container>
            );
        },

        _renderFieldSelector() {
            var options = _.map(this.props.builderModel.getAvailableNewFields(), fieldDefinition => ({
                value: fieldDefinition.name,
                label: fieldDefinition.label
            }));

            return options.length ? <FieldSelector
                name="field-name"
                defaultLabel="+ Add rule"
                options={options}
                onFieldSelected={this._onFieldSelected}
                /> : null;
        },

        _renderClearButton(fieldModels) {
            if (!_.any(fieldModels, f => f.hasFilters())) {
                return null;
            }

            return (
                <button
                    className="tau-btn tau-attention i-role-clear-filters"
                    onClick={this._onClearFilters}>
                    Clear filter
                </button>
            );
        }
    });
});
