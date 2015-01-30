define(function(require) {
    var React = require('libs/react/react-ex');
    var SettingsContainer = require('jsx!./shared/settings.container.view');
    var SettingsItem = require('jsx!./shared/settings.list.item.view');

    function loremIpsum() {
        var text = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.' +
            ' Donec a diam lectus. Sed sit amet ipsum mauris.' +
            ' Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit.' +
            ' Donec et mollis dolor. Praesent et diam eget libero egestas mattis sit amet vitae augue.' +
            ' Nam tincidunt congue enim, ut porta lorem lacinia consectetur.' +
            ' Donec ut libero sed arcu vehicula ultricies a non tortor.' +
            ' Lorem ipsum dolor sit amet, consectetur adipiscing elit.' +
            ' Aenean ut gravida lorem. Ut turpis felis, pulvinar a semper sed, adipiscing id dolor.' +
            ' Pellentesque auctor nisi id magna consequat sagittis.' +
            ' Curabitur dapibus enim sit amet elit pharetra tincidunt feugiat nisl imperdiet.' +
            ' Ut convallis libero in urna ultrices accumsan. Donec sed odio eros.' +
            ' Donec viverra mi quis quam pulvinar at malesuada arcu rhoncus.' +
            ' Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.' +
            ' In rutrum accumsan ultricies. Mauris vitae nisi at sem facilisis semper ac in est.';

        var split = text.split(' ');
        return _.take(split, _.random(5, split.length - 1)).join(' ');
    }

    var Widget = React.createClass({
        render: function() {
            return (
                <div className="i-role-dashboard-sample-widget" style={{
                    backgroundColor: this.props.color,
                    padding: "20px",
                    height: '100%'
                }}>
                    {this.props.content}
                </div>
            );
        }
    });

    var WidgetSettings = React.createClass({
        mixins: [React.addons.LinkedStateMixin],
        getInitialState: function() {
            return {
                content: this.props.content,
                color: this.props.color
            }
        },
        render: function() {
            return (
                <SettingsContainer labelsOnTop={false}>
                    <SettingsItem title="Content">
                        <input
                            className="i-role-dashboard-sample-widget-settings-content-input"
                            type="text"
                            valueLink={this.linkState("content")} />
                    </SettingsItem>
                    <SettingsItem title="Color">
                        <input
                            className="i-role-dashboard-sample-widget-settings-content-color"
                            type="text"
                            valueLink={this.linkState("color")} />
                    </SettingsItem>
                </SettingsContainer>
            );
        }
    });

    // Sample widget template that can be used to test the early functionality.
    // It should be moved to the test codebase and removed from production code later on.

    /**
     * Having a factory function is helpful when sample widget is used in a test environment as a stub with callbacks.
     * @param {Object} [config]
     * @return {DashboardWidgetTemplate}
     */
    return function(config) {
        config = config || {};

        var template = {
            id: config.id || 'sample_widget',
            name: config.name || 'Sample widget',
            description: config.description || 'Widget placeholder',
            tags: ['~debug', 'scrum', 'agile'],

            layout: {
                minHeight: config.minHeight,
                maxHeight: config.maxHeight,
                aspectRatio: config.aspectRatio
            },

            insert: function(placeholder, widgetSettings) {
                var view = React.renderClass(Widget, widgetSettings, placeholder);
                if (config.widgetInserted) {
                    config.widgetInserted(view);
                }
                return {update: view.setProps.bind(view)};
            },

            insertSettings: function(placeholder, widgetSettings) {
                var view = React.renderClass(WidgetSettings, widgetSettings, placeholder);
                if (config.settingsInserted) {
                    config.settingsInserted(view);
                }
                return function() {
                    return view.state;
                };
            }
        };

        // Simulate a dynamic, randomized "defaultSettings" property, unlikely to be used in real widget templates.

        Object.defineProperty(template, 'defaultSettings', {
            get: function() {
                var colors = [
                    '#69D2E7',
                    '#A7DBD8',
                    '#6495ED',
                    '#E0E4CC',
                    '#F38630',
                    '#FA6900',
                    '#BFCAAA',
                    '#D0A28A',
                    '#C58B80',
                    '#CF4459',
                    '#791241'
                ];

                return {
                    color: colors[_.random(colors.length - 1)],
                    content: loremIpsum()
                };
            },
            enumerable: true
        });

        return template;

    };
});
