define(function(require) {
    var React = require('react');
    var ContentView = require('jsx!./menu.item.access.view.content');

    return React.createClass({
        displayName: 'menu.item.access',

        propTypes: {
            isInitializing: React.PropTypes.bool.isRequired
        },

        getDefaultProps() {
            return {
                isInitializing: true
            };
        },

        getInitialState() {
            return {
                isLoaded: !this.props.isInitializing
            };
        },

        componentDidUpdate(prevProps) {
            var isLoaded = this.state.isLoaded;

            if (!isLoaded && prevProps.isInitializing && !this.props.isInitializing) {
                this.setState({isLoaded: true});
            }
        },

        render() {
            if (!this.state.isLoaded) {
                return (
                    <div className="tau-access-settings">
                        Loading...
                    </div>
                );
            }

            return <ContentView {...this.props} />;
        }
    });
});