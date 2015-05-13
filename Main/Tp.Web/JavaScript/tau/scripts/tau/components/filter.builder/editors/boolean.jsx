define(function(require) {
    var React = require('react');
    var utils = require('./editorUtils');

    return React.createClass({
        displayName: 'filter.builder.boolean.editor',

        propTypes: {
            editorContext: utils.editorContextPropTypes.isRequired
        },

        mixins: [React.addons.PureRenderMixin],

        getInitialState() {
            return {
                editValue: this.props.editorContext.getCurrentValue()
            };
        },

        _onInputChange(e) {
            var isChecked = e.target.checked;
            this.setState({editValue: isChecked});
            this.props.editorContext.setNewValue(isChecked);
        },

        render() {
            return (
                <label className="tau-checkbox">
                    <input
                        type="checkbox"
                        onChange={this._onInputChange}
                        defaultChecked={Boolean(this.state.editValue)}/>
                    <i className="tau-checkbox__icon" />
                    <span>{this.state.editValue ? 'True' : 'False'}</span>
                </label>
            );
        }
    });
});