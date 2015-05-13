define(function(require) {
    var React = require('libs/react/react-ex'),
        $ = require('jQuery'),
        colorUtils = require('tau/utils/utils.color');

    var errorClass = 'tau-error';
    var convertPropsToState = function(props) {
        var color = '#' + props.selectedColor.toUpperCase();
        return {
            value: color,
            style: {backgroundColor: color},
            additionalClass: colorUtils.isValidHexColor(color) ? '' : errorClass
        };
    };

    return React.createClass({
        displayName: 'ColorSelector',
        _onChange(e) {
            var value = e.target.value;
            var background = value[0] === '#' ? value : '#' + value;
            this.setState({value: value, style: {background: background}});
        },
        _onKeyDown(e) {
            if (e.keyCode === $.ui.keyCode.ENTER) {
                var color = this.state.value;
                if (colorUtils.isValidHexColor(color)) {
                    this.props.onSelect(color.replace('#', ''));
                }
                else {
                    this.setState({additionalClass: errorClass});
                }
            }
        },
        getInitialState() {
            return convertPropsToState(this.props);
        },
        componentWillReceiveProps(nextProps) {
            this.setState(convertPropsToState(nextProps));
        },
        render() {
            var value = this.state.value;
            var style = this.state.style;
            var className = 'tau-in-text ' + this.state.additionalClass;
            return <div className='tau-color-picker__input-color'>
                <div className='tau-color-picker__item' style={style}></div>
                <input
                    className ={className}
                    type='text'
                    title='Enter color in hex format'
                    value={value}
                    onKeyDown={this._onKeyDown}
                    onChange={this._onChange}
                />
            </div>
        }
    });
});