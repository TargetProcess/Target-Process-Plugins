define(function(require) {
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');

    return React.createClass({
        render: function() {
            var isGroup = this.props.isGroup;

            var nameClasses = classNames({
                't3-name': true,
                'i-role-group-name': isGroup,
                'i-role-item-name': !isGroup
            });

            var triggerClasses = classNames({
                't3-actions-trigger': true,
                'i-role-group-actions-trigger': isGroup
            });

            return (
                <div className="t3-header i-role-view-menu-header" onClick={this.props.onClick}>
                    <span className="tau-icon-general"></span>
                    <div
                        className={nameClasses}
                        title={this.props.name}>
                        {this.props.name}
                    </div>
                    <div className={triggerClasses} onClick={this.props.onActionClick}></div>
                </div>
            );
        }
    });
});
