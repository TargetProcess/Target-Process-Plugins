define(function(require) {
    var React = require('libs/react/react-ex'),
        $ = require('jQuery'),
        classNames = require('libs/classNames'),
        Picker = require('jsx!./visualEncoding.colorPicker.view');

    return React.createClass({
        displayName: 'ColorPickerIndicator',
        componentWillUnmount() {
            var tauBubble = $(this.refs.picker.getDOMNode()).tauBubble('instance');
            if (tauBubble) {
                tauBubble.destroy();
            }
        },
        _pickColor(value) {
            $(this.refs.picker.getDOMNode()).tauBubble('hide');
            this.props.onChange({value: value});
        },
        _getPickProps() {
            return {
                onSelect: this._pickColor,
                selected: this.props.value,
                colors: this.props.colors
            };
        },
        componentDidMount() {
            if (this.props.isEditable) {
                var getProps = function() {
                    return {
                        onSelect: this._pickColor,
                        selected: this.props.value,
                        colors: this.props.colors
                    };
                }.bind(this);

                var content;
                $(this.refs.picker.getDOMNode()).tauBubble({
                    zIndex: 42,
                    onShow: popup => {
                        var pickerProps = getProps();
                        var content = popup.find('[role=content]').get(0);
                        React.render(<Picker {...pickerProps}/>, content);
                    },
                    destroy: () => {
                        if (content) {
                            React.unmountComponentAtNode(content);
                        }

                    }
                })
            }
        },
        _getClasess() {
            var classes = {
                'tau-color-picker__item': true,
                'tau-color-picker__item--indicator': true,
                'tau-color-picker__item--disabled': !this.props.isEditable
            };
            return classNames(classes);
        },
        render() {
            var style = {backgroundColor: '#' + this.props.value};
            return <div ref='picker' className={this._getClasess()} style={style}></div>
        }
    });
});
