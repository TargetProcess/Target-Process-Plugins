define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'ViewsMenuOwnersEditorContainer',

        propTypes: {
            onClick: React.PropTypes.func.isRequired
        },

        render() {
            return (
                <div onClick={this.props.onClick}>{this.props.children}</div>
            );
        }
    });
});