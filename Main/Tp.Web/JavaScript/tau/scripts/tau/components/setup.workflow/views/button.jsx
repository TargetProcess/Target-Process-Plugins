define(function(require) {
    var React = require('libs/react/react-ex');

    return React.createClass({
        render: function() {
            return this.transferPropsTo(
                <button className="tau-btn" type="button">{this.props.children}</button>
            );
        }
    });
});