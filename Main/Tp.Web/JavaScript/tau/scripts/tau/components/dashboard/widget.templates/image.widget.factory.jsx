define(function(require) {
    var React = require('libs/react/react-ex');
    var _ = require('Underscore');

    var ImageWidget = React.createClass({
        render: function() {
            return (
                <div className="i-role-dashboard-sample-widget">
                    <img src={this.props.imageSource} />
                </div>
            );
        }
    });

    return {
        create: function(config) {
            var template = _.extend({
                defaultSettings: {
                    name: config.name,
                    imageSource: config.previewSrc
                },
                insert: function(placeholder, settings) {
                    React.renderClass(ImageWidget, settings, placeholder);
                }
            }, config);

            if (!template.tags) {
                template.tags = [];
            }
            template.tags.push('~debug');

            return template;
        }
    };
});
