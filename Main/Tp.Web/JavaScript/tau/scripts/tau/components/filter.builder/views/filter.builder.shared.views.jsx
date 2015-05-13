define(function(require) {
    var React = require('react');

    return {
        Container: React.createClass({
            render() {
                return (
                    <div className="filter-builder">
                        {this.props.children}
                    </div>
                );
            }
        }),

        StatusMessage: React.createClass({
            render() {
                return (
                    <div className="filter-builder__state-message">
                        {this.props.children}
                    </div>
                );
            }
        })
    };
});