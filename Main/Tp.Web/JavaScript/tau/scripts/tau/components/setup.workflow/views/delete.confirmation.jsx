define(function(require) {
    var React = require('libs/react/react-ex'),
        ButtonView = require('jsx!./button'),
        UnassignRoleSummary = require('jsx!./unassign.role.summary'),
        _ = require('Underscore');

    return React.createClass({
        mixins: [React.addons.LinkedStateMixin],

        DEFAULT_SELECT_VALUE: '',

        getInitialState: function() {
            return {
                migrationStateId: this.DEFAULT_SELECT_VALUE
            };
        },

        _doesAffectEntities: function() {
            return this.props.affectedEntitiesCount > 0;
        },

        _isDeleteForbidden: function() {
            return this._doesAffectEntities() && this.state.migrationStateId === this.DEFAULT_SELECT_VALUE;
        },

        _onSubmit: function() {
            if (this._doesAffectEntities()) {
                var migrationStateId = parseInt(this.state.migrationStateId, 10);
                var migrationState = _.findWhere(this.props.states, { id: migrationStateId });
                this.props.onSubmit(migrationState);
            } else {
                this.props.onSubmit();
            }
        },

        _subStatesFormatMessage: function() {
            var count = this.props.subStatesCount;
            if (!count) {
                return null;
            } else if (count > 1) {
                return (<p>{count} sub workflow states, which are mapped to this state, will also be deleted.</p>);
            } else {
                return (<p>Sub workflow state, which is mapped to this state, will also be deleted.</p>);
            }
        },

        _formatMessage: function() {
            var count = this.props.affectedEntitiesCount;
            var isPlural = count > 1;
            return (
                <p>
                    <span className="i-role-affected-items-count">{count}</span>&nbsp;
                    {isPlural ? this.props.terms.names + ' are ' : this.props.terms.name + ' is '}
                    currently in this state. Select another state where {isPlural ? 'they' : 'it'} should be moved to.
                </p>
                );
        },

        render: function() {
            var content = this._doesAffectEntities() ?
                (
                    <div>
                        {this._formatMessage()}
                        <select valueLink={this.linkState('migrationStateId')} className="tau-select tau-state-settings__new-state">
                            <option value={this.DEFAULT_SELECT_VALUE}>Select state</option>
                            {_.map(this.props.states, function(state) {
                                return <option key={state.id} value={state.id}>{state.name}</option>
                            })}
                        </select>
                    </div>
                ) :
                <p>No {this.props.terms.names} are in this state, so it can be deleted.</p>;

            return (
                <div className="tau-state-settings__content">
                    {this.props.roleAssignmentsCount === 0 ? null :
                        <UnassignRoleSummary
                        roleAssignmentsCount={this.props.roleAssignmentsCount}
                        role={this.props.role}
                        terms={this.props.terms}/>}
                    {this._subStatesFormatMessage()}
                    {content}
                    <div className="tau-buttons">
                        <ButtonView onClick={this._onSubmit} className="tau-danger" disabled={this._isDeleteForbidden()}>Continue</ButtonView>
                        <ButtonView onClick={this.props.onCancel}>Cancel</ButtonView>
                    </div>
                </div>
                );
        }
    });
});