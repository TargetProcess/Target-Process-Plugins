define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        displayName: 'filter.builder.editor.container',

        propTypes: {
            filterModel: React.PropTypes.shape({
                getEditor: React.PropTypes.func.isRequired,
                getEditorContext: React.PropTypes.func.isRequired
            }).isRequired,
            canRemoveFilter: React.PropTypes.bool.isRequired,
            removeFilter: React.PropTypes.func.isRequired
        },

        _removeFilter() {
            this.props.removeFilter();
        },

        render() {
            var editor = this.props.filterModel.getEditor()({
                editorContext: this.props.filterModel.getEditorContext(),
                handleEmptyValue: this._removeFilter
            });

            var removeControl = this.props.canRemoveFilter ?
                <span
                    className="tau-icon-general tau-icon-close-red i-role-delete-filter"
                    onMouseDown={this._onMouseDown}
                    onClick={this._removeFilter} /> : null;

            var className = classNames({
                'filter-builder__filter': true,
                'i-role-filter-builder__field__filter': true
            });

            return (
                <div className={className}>
                    {editor}
                    {removeControl}
                </div>
            );
        }
    });
});