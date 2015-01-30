define(function(require) {
    var React = require('libs/react/react-ex');

    return React.createClass({
        render: function() {
            var buttonClasses = React.addons.classSet({
                'tau-btn-add-state': true,
                'active': this.props.isActive
            });

            var placeholderClasses =  React.addons.classSet({
                'tau-add-state-placeholder': true,
                'enabled': this.props.isEnabled
            });

            return (
                <div className={placeholderClasses}>
                    <div className="tau-add-state-hover-zone"/>
                    <button className={buttonClasses} title="Add state" onClick={this.props.addStateAction}>Add state</button>
                </div>
                );
        }
    });
});