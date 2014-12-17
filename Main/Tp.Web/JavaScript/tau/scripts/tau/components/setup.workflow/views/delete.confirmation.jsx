define(function(require) {
    var React = require('libs/react/react-ex'),
        $ = require('jQuery'),
        ButtonView = require('jsx!./button'),
        _ = require('Underscore');

    return React.createClass({
        mixins: [React.addons.LinkedStateMixin],

        DEFAULT_SELECT_VALUE: '',

        getInitialState: function() {
            return {
                migrationStateId: this.DEFAULT_SELECT_VALUE
            };
        },

        _isDeleteForbidden: function() {
            return this.state.migrationStateId === this.DEFAULT_SELECT_VALUE;
        },

        _onSubmit: function() {
            var migrationStateId = parseInt(this.state.migrationStateId, 10);
            var migrationState = _.findWhere(this.props.states, { id: migrationStateId });
            this.props.onSubmit(migrationState);
        },

        render: function() {
            return (
                <div className="tau-transitions-setup__content">
                    <p>Entities currently in this state will be moved to the selected one:</p>
                    <select valueLink={this.linkState('migrationStateId')} className="tau-select tau-transitions-setup__new-state">
                        <option value={this.DEFAULT_SELECT_VALUE}>Select state</option>
                        {_.map(this.props.states, function(state) {
                            return <option key={state.id} value={state.id}>{state.name}</option>
                        })}
                    </select>
                    <div className="tau-buttons">
                        <ButtonView onClick={this._onSubmit} className="tau-danger" disabled={this._isDeleteForbidden()}>Delete</ButtonView>
                        <ButtonView onClick={this.props.onCancel}>Cancel</ButtonView>
                    </div>
                </div>
                );
        }
    });
});