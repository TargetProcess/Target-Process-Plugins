define(function(require) {
    var React = require('react');
    var classNames = require('libs/classNames');

    return React.createClass({
        displayName: 'menu.item.owners.container',

        propTypes: {
            isListEditorVisible: React.PropTypes.bool.isRequired,
            onClick: React.PropTypes.func.isRequired
        },

        render() {
            var buttonClasses = classNames({
                'tau-btn': true,
                'tau-checked': this.props.isListEditorVisible
            });

            return (
                <button className={buttonClasses} onClick={this.props.onClick}>
                    {this.props.children}
                </button>
            );
        }
    });
});