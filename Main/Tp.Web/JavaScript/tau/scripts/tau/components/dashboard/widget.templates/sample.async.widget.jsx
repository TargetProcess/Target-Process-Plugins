/* global console */

define(function(require) {
    var $ = require('jQuery');
    var React = require('libs/react/react-ex');
    var DOM = React.DOM;

    var DEFAULT_DELAY = 3000;
    var DEFAULT_SETTINGS_DELAY = 1000;

    var Widget = React.createClass({
        render: function() {
            var content = 'I am finally loaded after ' + this.props.delay + 'ms delay';
            return DOM.div({}, content);
        }
    });

    var WidgetSettings = React.createClass({
        mixins: [React.addons.LinkedStateMixin],

        getInitialState: function() {
            return {
                delay: this.props.initialDelay,
                settingsDelay: this.props.initialSettingsDelay
            };
        },

        render: function() {
            return (
                <ul className="tau-widget-settings-list">
                    <li className="tau-widget-settings-list__item">
                        <label>
                            <span className="tau-widget-settings-list__title">Render delay</span>
                            <input
                                className="i-role-dashboard-sample-widget-settings-content-input"
                                type="text"
                                valueLink={this.linkState("delay")}/>
                        </label>
                    </li>
                    <li className="tau-widget-settings-list__item">
                        <label>
                            <span className="tau-widget-settings-list__title">Settings delay</span>
                            <input
                                className="i-role-dashboard-sample-widget-settings-content-input"
                                type="text"
                                valueLink={this.linkState("settingsDelay")}/>
                        </label>
                    </li>
                </ul>
            );
        }
    });

    return function() {
        return {
            id: 'sample_async_widget',
            name: 'Sample async',
            description: 'Sample asynchronously loaded widget',
            tags: ['~debug'],

            defaultSettings: {
                delay: DEFAULT_DELAY,
                settingsDelay: DEFAULT_SETTINGS_DELAY
            },

            insert: function(placeholder, settings) {
                return $.when
                    .timeout(settings.delay)
                    .then(function() {
                        React.renderClass(Widget, settings, placeholder);
                        return {
                            // No `update` callback to force async widget reloading on "apply settings".

                            destroy: function() {
                                React.unmountComponentAtNode(placeholder);
                                console.log('Async widget was destroyed');
                            }
                        };
                    });
            },

            insertSettings: function(placeholder, settings) {
                return $.when
                    .timeout(settings.settingsDelay)
                    .then(function() {
                        var view = React.renderClass(WidgetSettings, {
                            initialDelay: settings.delay,
                            initialSettingsDelay: settings.settingsDelay
                        }, placeholder);

                        return function() {
                            return _.pick(view.state, 'delay', 'settingsDelay');
                        };
                    });
            }
        };
    };
});