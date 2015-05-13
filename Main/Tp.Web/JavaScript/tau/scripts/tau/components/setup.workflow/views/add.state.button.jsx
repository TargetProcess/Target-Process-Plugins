define(function(require) {
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');

    return React.createClass({
        render: function() {
            var buttonClasses = classNames({
                'tau-btn-add-state': true,
                'active': this.props.isActive
            });

            var placeholderClasses = classNames({
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