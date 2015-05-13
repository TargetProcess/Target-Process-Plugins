define(function(require) {
    var $ = require('jQuery');
    var React = require('react');
    var utils = require('./editorUtils');

    var InputAutoSizeMixin = {
        componentDidMount() {
            this._adjustSize();
        },

        componentDidUpdate(prevProps, prevState) {
            if (this.state.value !== prevState.value) {
                this._adjustSize();
            }
        },

        /**
         * @private
         */
        _adjustSize() {
            var autoSizeWidth = this.refs.autoSizeSource.getDOMNode().offsetWidth;
            this.refs.editor.getDOMNode().style.width = (autoSizeWidth + 1) + 'px';
        }
    };

    var EditableText = React.createClass({
        displayName: 'filter.builder.text.editable',

        propTypes: {
            placeholder: React.PropTypes.string,
            initialValue: React.PropTypes.any,
            applyValue: React.PropTypes.func,
            handleEmptyValue: React.PropTypes.func
        },

        mixins: [React.addons.PureRenderMixin, InputAutoSizeMixin],

        getDefaultProps: () => ({
            placeholder: null,
            initialValue: null,
            applyValue: _.constant(),
            handleEmptyValue: _.constant()
        }),

        getInitialState() {
            return {
                value: this.props.initialValue
            };
        },

        componentDidMount() {
            this._lastAppliedValue = this.props.initialValue;
        },

        _onKeyDown(e) {
            if (e.keyCode === $.ui.keyCode.ENTER) {
                this._tryApplyValue();
                var node = this.refs.editor.getDOMNode();
                if (node.blur) {
                    node.blur();
                }
            }
        },

        _onChange(e) {
            this.setState({value: e.target.value});
        },

        _onBlur() {
            var applicationResult = this._tryApplyValue();
            if (applicationResult.isEmpty) {
                this.props.handleEmptyValue();
            }
        },

        _tryApplyValue() {
            var newValue = this._normalizeInput(this.state.value);
            if (!newValue) {
                return {
                    isEmpty: true
                };
            }

            var lastValue = this._normalizeInput(this._lastAppliedValue);

            if (newValue === lastValue) {
                return {
                    isEmpty: false
                };
            }

            this._lastAppliedValue = newValue;
            this.props.applyValue(newValue);

            return {
                isEmpty: false
            };
        },

        _normalizeInput(x) {
            return _.trim(_.asString(x));
        },

        render() {
            return (
                <div>
                    <input
                        ref="editor"
                        type="text"
                        className="filter-builder__filter__editable i-role-focus-target"
                        placeholder={this.props.placeholder}
                        value={this.state.value}
                        onChange={this._onChange}
                        onKeyDown={this._onKeyDown}
                        onBlur={this._onBlur}/>
                    <span
                        ref="autoSizeSource"
                        className="filter-builder__filter__editable-width">
                        {this.state.value}
                    </span>
                </div>
            );
        }
    });

    return React.createClass({
        displayName: 'filter.builder.text.editor',

        propTypes: {
            editorContext: utils.editorContextPropTypes.isRequired,
            placeholder: React.PropTypes.string
        },

        mixins: [React.addons.PureRenderMixin],

        render() {
            var context = this.props.editorContext;

            return (
                <EditableText
                    placeholder={this.props.placeholder}
                    initialValue={context.getCurrentValue()}
                    applyValue={context.setNewValue.bind(context)}
                    handleEmptyValue={this.props.handleEmptyValue} />
            );
        }
    });
});