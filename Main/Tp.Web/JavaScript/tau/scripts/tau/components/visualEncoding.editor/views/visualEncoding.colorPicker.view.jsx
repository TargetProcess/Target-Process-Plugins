define(function(require) {
    var React = require('libs/react/react-ex'),
        PickerItem = require('jsx!./visualEncoding.colorPicker.item.view'),
        ColorSelector = require('jsx!./visualEncoding.colorSelector.view'),

        _ = require('Underscore');

    return React.createClass({
        displayName:'ColorPicker',
        render() {
            var items = _.map(this.props.colors, (item) => {
                var isSelect = item.className === this.props.selected;
                return <PickerItem key={item.className} selected={isSelect} onSelect={this.props.onSelect} value={item.className} />
            });

            return (
                <div className='tau-color-picker'>
                    <div className='tau-color-picker__list'>{items}</div>
                    <ColorSelector selectedColor={this.props.selected} onSelect={this.props.onSelect} />
                </div>
            );
        }
    });
});