define(function(require) {
    var React = require('react');

    return React.createClass({
        getDefaultProps: _.constant({
            labelsOnTop: true
        }),

        render: function() {
            var className = React.addons.classSet({
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