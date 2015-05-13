define(function(require) {
    var React = require('libs/react/react-ex');

    return React.createClass({
        displayName:'ColorPickerItem',
        _onClick() {
            this.props.onSelect(this.props.value);
        },
        render() {
            var style = {backgroundColor: '#' + this.props.value};
            var pickColor = 'tau-color-picker__item';
            if(this.props.selected) {
                pickColor += ' tau-color-picker__item--selected';
            }
            return <div onClick={this._onClick} className={pickColor} style={style}></div>
        }
    });
});