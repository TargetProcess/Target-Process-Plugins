define(function(require) {
    var React = require('libs/react/react-ex'),
        ReactIgnore = require('jsx!tau/core/helpers/reactIgnore'),
        configurator = require('tau/configurator'),
        feedback = require('tau/components/component.feedback');
    return React.createClass({
        displayName: 'Feedback',
        componentDidMount() {
            var config = {
                context: {
                    configurator: configurator
                },
                feedbackConfig: {
                    featureName: this.props.featureName,
                    buttonClass: this.props.buttonClass || 'tau-btn tau-primary'
                }
            };
            var feedbackBus = feedback.create(config);
            feedbackBus.on('afterRender', (evt, {element})=> {
                element.appendTo(this.refs.container.getDOMNode());
            });

            feedbackBus.initialize(config);

            this._feedbackBus = feedbackBus;
        },
        componentWillUnmount() {
            this._feedbackBus.fire('destroy');
        },

        render() {
            return (
                <ReactIgnore>
                    <div className={this.props.feedbackClassName || ''} ref="container"></div>
                </ReactIgnore>
            )
        }
    });
});


