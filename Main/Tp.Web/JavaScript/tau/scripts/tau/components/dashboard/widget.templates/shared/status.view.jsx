define(function(require) {
    var React = require('react');

    /**
     * @class WidgetErrorView
     * @extends ReactComponent
     */
    var StatusView = React.createClass({
        statics: {
            renderToDOM(props, children) {
                var container = document.createElement('div');

                container.style.width = '100%';
                container.style.height = '100%';

                React.render(React.createElement(StatusView, props, children), container);

                return container;
            }
        },
        displayName: 'WidgetStatusView',

        getDefaultProps() {
            return {
                containerClassName: '',
                textClassName: ''
            };
        },

        render() {
            var containerClasses = 'tau-dashboard-widget-placeholder ' + this.props.containerClassName;
            var textClasses = 'tau-dashboard-widget-placeholder-text ' +  this.props.textClassName;

            return (
                <div className={containerClasses}>
                    <div className="tau-dashboard-widget-placeholder-wrapper tau-role-status-placeholder">
                        <div className={textClasses}>
                            {this.props.children}
                        </div>
                    </div>
                </div>
            );
        }
    });

    return StatusView;
});
