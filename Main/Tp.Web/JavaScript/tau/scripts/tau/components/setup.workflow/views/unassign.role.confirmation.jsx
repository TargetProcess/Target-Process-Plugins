define(function(require) {
    var React = require('libs/react/react-ex'),
        UnassignRoleSummary = require('jsx!./unassign.role.summary'),
        ButtonView = require('jsx!./button');

    return React.createClass({
        displayName: 'UnassignRoleConfirmation',
        propTypes: {
            onSubmit: React.PropTypes.func.isRequired,
            onCancel: React.PropTypes.func.isRequired
        },
        render: function() {
            return (
                <div className="tau-state-settings__content">
                    <UnassignRoleSummary
                        roleAssignmentsCount={this.props.roleAssignmentsCount}
                        role={this.props.role}
                        terms={this.props.terms}/>
                    <div className="tau-buttons">
                        <ButtonView onClick={this.props.onSubmit} className="tau-danger">Continue</ButtonView>
                        <ButtonView onClick={this.props.onCancel}>Cancel</ButtonView>
                    </div>
                </div>
                );
        }
    });
});