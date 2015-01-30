define(function(require) {
    var React = require('react');

    return React.createClass({
        render: function() {
            return (
                <li className="tau-widget-settings-list__item">
                    <label className={this.props.labelClassName}>
                        <span className="tau-widget-settings-list__title">{this.props.title}</span>
                        {this.props.children}
                    </label>
                </li>
            );
        }
    });
});