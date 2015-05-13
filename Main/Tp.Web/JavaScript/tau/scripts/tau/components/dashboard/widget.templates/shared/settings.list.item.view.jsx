define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'widget.settings.list.item.view',

        propTypes: {
            labelClassName: React.PropTypes.string,
            title: React.PropTypes.string
        },

        render() {
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