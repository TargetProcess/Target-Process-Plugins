define(function(require) {
    var React = require('react');
    var EditorContainer = require('jsx!./filter.builder.editor.container');
    var NewFilterView = require('jsx!./filter.builder.new.editor.view');
    var classNames = require('libs/classNames');

    return React.createClass({
        displayName: 'filter.builder.row.view',

        propTypes: {
            fieldModel: React.PropTypes.shape({
                fieldName: React.PropTypes.string.isRequired,
                getFilterModels: React.PropTypes.func.isRequired,
                removeFilter: React.PropTypes.func.isRequired,
                fieldModelId: React.PropTypes.string.isRequired
            }).isRequired
        },

        getInitialState() {
            return {
                addedCount: 0
            };
        },

        _applyEditorValue(filterValue) {
            this.props.fieldModel.addFilter(filterValue);

            // Force re-creation of "new filter" container.
            // Otherwise, it would always have the same key,
            // which would force editor class to reset its state to initial one when pressing Enter
            // to be properly reused.
            this.setState({addedCount: this.state.addedCount + 1});
        },

        render() {
            var fieldModel = this.props.fieldModel;
            var filterModels = fieldModel.getFilterModels();

            var filters = _.map(filterModels, filterModel =>
                <EditorContainer
                    key={filterModel.filterModelId}
                    filterModel={filterModel}
                    canRemoveFilter={true}
                    removeFilter={fieldModel.removeFilter.bind(fieldModel, filterModel)} />);

            if (fieldModel.getCanAddNewFilter()) {
                filters.push(<NewFilterView
                    key={"new-filter-view-" + this.state.addedCount}
                    fieldId={this.props.fieldModel.fieldModelId}
                    alreadyHasFilters={Boolean(filters.length)}
                    applyEditorValue={this._applyEditorValue}
                    getEditor={fieldModel.getEditor}
                    fieldName={fieldModel.fieldName}
                    getCurrentFilterValues={fieldModel.getCurrentFilterValues.bind(fieldModel)} />);
            }

            var containerClassName = classNames({
                'i-role-filter-builder__field': true,
                'filter-builder__row': true,
                'active': filterModels.length > 0
            });

            return (
                <div className={containerClassName}>
                    <div className="filter-builder__cell">
                        <div className="filter-builder__name">{fieldModel.fieldLabel}</div>
                    </div>
                    <div className="filter-builder__cell">
                        <div className="filter-builder__filter-list">
                            {filters}
                        </div>
                    </div>
                </div>);
        }
    });
});
