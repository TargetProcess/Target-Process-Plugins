define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var classNames = require('libs/classNames');

    return React.createClass({
        getDefaultProps: _.constant({
            labelsOnTop: true
        }),

        render: function() {
            var className = classNames({
                'tau-widget-settings-list': true,
                'tau-widget-settings-list--col': this.props.labelsOnTop
            });

            return (
                <ul className={className}>
                    {this.props.children}
                </ul>
            );
        }
    });
});
