define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        propTypes: {
            applyEditorValue: React.PropTypes.func.isRequired,
            getEditor: React.PropTypes.func.isRequired,
            getCurrentFilterValues: React.PropTypes.func.isRequired,
            alreadyHasFilters: React.PropTypes.bool.isRequired,
            fieldId: React.PropTypes.string.isRequired,
            fieldName: React.PropTypes.string.isRequired
        },

        _buildContainerClassName() {
            return classNames('filter-builder__filter filter-builder__filter-new placeholder',
                'i-role-filter-builder__field__new-filter',
                'i-role-filter-builder__field-' + this.props.fieldId);
        },

        render() {
            var editorContext = {
                fieldName: this.props.fieldName,
                getCurrentValue: _.constant(null),
                setNewValue: function(filterValue) {
                    this.props.applyEditorValue(filterValue);
                }.bind(this),
                getCurrentFilterValues: this.props.getCurrentFilterValues
            };

            var editor = this.props.getEditor({
                placeholder: this.props.alreadyHasFilters ? '+ Add' : 'Any',
                editorContext: editorContext
            });

            return (
                <div className={this._buildContainerClassName()}>
                    {editor}
                </div>
            );
        }
    });
});
