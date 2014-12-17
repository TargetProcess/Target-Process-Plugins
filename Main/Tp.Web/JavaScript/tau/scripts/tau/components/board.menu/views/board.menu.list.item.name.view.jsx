define(function(require) {
    var $ = require('jQuery');
    var React = require('libs/react/react-ex');

    return React.defineClass([], function() {
        return {
            render: function() {
                var isGroup = this.props.isGroup;

                var nameClasses = React.addons.classSet({
                    't3-name': true,
                    'i-role-group-name': isGroup,
                    'i-role-item-name': !isGroup
                });

                var triggerClasses = React.addons.classSet({
                    't3-actions-trigger': true,
                    'i-role-group-actions-trigger': isGroup
                });

                return (
                    <div className="t3-header i-role-view-menu-header" onClick={this.props.onClick}>
                        <span className="tau-icon-general"></span>
                        <div className={nameClasses}>{this.props.name}</div>
                        <div className={triggerClasses} onClick={this.props.onActionClick}></div>
                    </div>
                    );
            }
        }
    });
});
