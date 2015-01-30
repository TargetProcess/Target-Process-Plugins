define(function(require) {
    var React = require('libs/react/react-ex');

    return React.createClass({
        render: function() {
            return (
                <div class="tau-dashboard-error-layout">
                    <div class="tau-dashboard-error-layout__header">Error occurred</div>
                    <div class="tau-dashboard-error_layout__body">{this.props.layout.errorMessage || 'Unknown error'}</div>
                </div>
                );
        }
    });
});
