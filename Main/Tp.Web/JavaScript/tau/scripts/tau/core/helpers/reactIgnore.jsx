define(function(require) {

    var React = require('libs/react/react-ex');

    var ReactIgnore = React.createClass({
        displayName: 'ReactIgnore',
        shouldComponentUpdate: function() {
            return false;
        },

        render: function() {
            return React.Children.only(this.props.children);
        }
    });
    return ReactIgnore;
});


