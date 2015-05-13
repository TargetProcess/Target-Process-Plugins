define(function(require) {
    var $ = require('jQuery');
    var React = require('react');
    var {Container, StatusMessage} = require('jsx!./filter.builder.shared.views');
    var View = require('jsx!./filter.builder.view');

    return React.createClass({
        displayName: 'filter.builder.loading.wrapper',

        propTypes: {
            builderModelPromise: React.PropTypes.object.isRequired
        },

        getInitialState() {
            var builderModel = this.props.builderModelPromise.state() === 'resolved' ?
                $.Deferred.extractValueOrFail(this.props.builderModelPromise) :
                null;

            return {
                isLoaded: Boolean(builderModel),
                builderModel: builderModel
            };
        },

        componentDidMount() {
            if (!this.state.isLoaded) {
                this.props.builderModelPromise.then(builderModel => {
                    if (this.isMounted()) {
                        this.setState({
                            builderModel: builderModel,
                            isLoaded: true
                        });
                    }
                });
            }
        },

        render() {
            if (!this.state.isLoaded) {
                return (
                    <Container>
                        <StatusMessage>
                            Initializing filters...
                        </StatusMessage>
                    </Container>
                )
            }

            return <View builderModel={this.state.builderModel}/>;
        }
    });
});