define(function(require) {
    var React = require('libs/react/react-ex');

    return React.createClass({
        render: function() {
            var props = this.props;
            props.className = (props.className || '') + ' tau-btn';
            props.type = 'button';
            return React.createElement('button', props, props.children);
        }
    });
});
