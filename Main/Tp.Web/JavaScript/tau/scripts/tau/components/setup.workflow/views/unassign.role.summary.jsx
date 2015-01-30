define(function(require) {
    var React = require('libs/react/react-ex');

    return React.createClass({
        displayName: 'UnassignRoleSummary',
        propTypes: {
            roleAssignmentsCount: React.PropTypes.number.isRequired,
            role: React.PropTypes.shape({ name: React.PropTypes.string }).isRequired,
            terms: React.PropTypes.shape({ entityTypes: React.PropTypes.string }).isRequired
        },
        render: function() {
            var count = this.props.roleAssignmentsCount;
            return(
                <div>
                    <p>{this.props.role.name} role will be removed from the workflow as it is no longer in use.</p>
                    <p>People assigned as {this.props.role.name} will be unassigned from <span className="i-role-assignments-count">{count}</span> {this.props.terms.names}.</p>
                </div>
                );
        }
    });
});