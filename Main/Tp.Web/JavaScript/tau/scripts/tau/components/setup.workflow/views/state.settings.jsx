define(function(require) {
    var _ = require('Underscore'),
        React = require('libs/react/react-ex'),
        ButtonView = require('jsx!./button');

    var TransitionsView = React.createClass({
        _getValidStateIds: function() {
            var currentState = _.findWhere(this.props.states, {isCurrent: true});
            return _.pluck(_.without(this.props.states, currentState), 'id');
        },

        _getStateContent: function(state) {
            if (state.isCurrent) {
                return <span className="tau-transitions-states__list__item--current">{state.name}</span>;
            } else {
                var isActive = _.contains(this.props.activeStateIds, state.id);

                return (
                    <label className="tau-checkbox tau-extension-board-tooltip">
                        <input type="checkbox" value={state.id} checked={isActive} onChange={this._onStateCheckboxChange}/>
                        <i className="tau-checkbox__icon"></i>
                        <span className="tau-checkbox-label">{state.name}</span>
                    </label>
                    );
            }
        },

        _onStateCheckboxChange: function(e) {
            var stateId = parseInt(e.currentTarget.value, 10);

            this.props.changeStatesAction({
                add: e.currentTarget.checked,
                values: [stateId]
            })
        },

        _toggleAllStates: function(e) {
            var enable = e.currentTarget.checked;
            this.props.changeStatesAction({
                add: enable,
                values: this._getValidStateIds()
            });
        },

        render: function() {
            var allStatesEnabled = _.difference(this._getValidStateIds(), this.props.activeStateIds).length === 0;

            return (
                <ul className="tau-transitions-states__list">
                    <li className="tau-transitions-states__list__item">
                        <label className="tau-checkbox tau-extension-board-tooltip">
                            <input type="checkbox" value="all" checked={allStatesEnabled} onChange={this._toggleAllStates} />
                            <i className="tau-checkbox__icon"></i>
                            <span className="tau-checkbox-label tau-transitions-category__title">{this.props.name}</span>
                        </label>
                    </li>
                    {_.map(this.props.states, function(state) {
                        return (
                            <li key={state.id} className="tau-transitions-states__list__item">
                                {this._getStateContent(state)}
                            </li>
                            );
                    }, this)}
                </ul>
            )
        }
    });

    var PlannedView = React.createClass({
        render: function() {
            return (
                <label className="tau-checkbox tau-extension-board-tooltip">
                    <input type="checkbox" checkedLink={this.props.isPlannedLink} />
                    <i className="tau-checkbox__icon"></i>
                    <span className="tau-checkbox-label">State is Planned</span>
                </label>
                );
        }
    });

    var RolesView = React.createClass({
        render: function() {
            return (
                <div>
                    <label className="tau-select">
                        <span className="tau-transitions-category__title tau-transitions-category__title--updater">Responsible role</span>
                        <select disabled={this.props.disabled} valueLink={this.props.activeRoleLink} className="tau-select tau-transitions-states-feature-list__role">
                            {_.map(this.props.roles, function(role) {
                                return <option key={role.id} value={role.id}>{role.name}</option>
                            })}
                        </select>
                    </label>
                    <p className="tau-transitions-updater-comment">People assigned to this role for a {this.props.terms.name} will be responsible for it in this state.</p>
                </div>
                );
        }
    });

    return React.createClass({
        mixins: [React.addons.LinkedStateMixin],

        getInitialState: function() {
            return this.props.config.settings;
        },

        _changePreviousStates: function(diff) {
            this._changeActiveStates('previousStateIds', diff);
        },

        _changeNextStates: function(diff) {
            this._changeActiveStates('nextStateIds', diff);
        },

        _changeActiveStates: function(key, diff) {
            var partialState = {};
            if (diff.add) {
                partialState[key] = _.union(this.state[key], diff.values);
            } else {
                partialState[key] = _.difference(this.state[key], diff.values);
            }
            this.setState(partialState);
        },

        _onSave: function() {
            this.props.saveStateAction(this.state);
        },

        _onDelete: function(e) {
            this.props.deleteStateAction(this.state, e.currentTarget);
        },

        _isInitialOrFinal: function() {
            return this.state.isInitial || this.state.isFinal;
        },

        _canBePlanned: function() {
            return !this._isInitialOrFinal();
        },

        _canBeDeleted: function() {
            return !this._isInitialOrFinal();
        },

        render: function() {
            var plannedView = this.state.isInitial || this.state.isFinal ? null :
                <PlannedView isPlannedLink={this.linkState('isPlanned')} />;

            return (
                <div className="tau-transitions-setup">
                    <section className="tau-transitions-category tau-previous-states">
                        <TransitionsView
                        name="Previous states"
                        states={this.props.config.states}
                        activeStateIds={this.state.previousStateIds}
                        changeStatesAction={this._changePreviousStates}/>
                    </section>
                    <section className="tau-transitions-category tau-transitions-updater">
                        <RolesView disabled={this.state.isFinal} activeRoleLink={this.linkState('activeRoleId')} roles={this.props.config.roles} terms={this.props.config.terms} />
                        {this._canBePlanned() ? <PlannedView isPlannedLink={this.linkState('isPlanned')} /> : null}
                        <section className="tau-transitions-controls">
                            <ButtonView className="tau-btn tau-primary" onClick={this._onSave}>Save</ButtonView>
                            {this._canBeDeleted() ? <ButtonView className="tau-attention tau-remove-state" onClick={this._onDelete}>Delete State</ButtonView> : null}
                        </section>
                    </section>
                    <section className="tau-transitions-category tau-next-states">
                        <TransitionsView
                        name="Next states"
                        states={this.props.config.states}
                        activeStateIds={this.state.nextStateIds}
                        changeStatesAction={this._changeNextStates}/>
                    </section>
                </div>
                );
        }
    });
});