define(function(require) {
    var React = require('libs/react/react-ex'),
        classNames = require('libs/classNames'),
        t = React.PropTypes,
        renameService = require('tau/services/rename.service'),
        ConfirmationService = require('../services/state.item.process.settings.confirmation.service');

    var TeamStateView = React.createClass({
        displayName: 'TeamStateView',

        propTypes: {
            teamState: t.shape({
                id: t.number.isRequired,
                name: t.string.isRequired,
                isInitial: t.bool,
                isFinal: t.bool,
                left: t.string.isRequired,
                width: t.string.isRequired
            }),
            parentSubStates: t.array.isRequired,
            entityTerms: t.object.isRequired,
            isEnabled: t.bool.isRequired,
            isLoading: t.bool.isRequired,
            renameStateAction: t.func.isRequired,
            stateSettings: t.object.isRequired
        },

        getInitialState: function() {
            return {
                name: this.props.teamState.name,
                isRenaming: false
            };
        },

        componentWillReceiveProps: function(nextProps) {
            this.setState({name: nextProps.teamState.name});
        },

        _beginNameEdit: function() {
            var nameEl = this.refs.namePlaceholder.getDOMNode();
            this._toggleRename(this, true);
            return renameService.startRenamingActivity(nameEl, {className: 'editableText'})
                .always(this._toggleRename.bind(this, false))
                .done(this._rename);
        },

        _rename: function(newName, oldName) {
            if (newName !== oldName) {
                this.props.renameStateAction({
                    id: this.props.teamState.id,
                    name: newName
                });
                this.setState({name: newName});
            }
        },

        _toggleRename: function(isShown) {
            this.setState({isRenaming: isShown});
        },

        _isInitialOrFinal: function() {
            return this.props.teamState.isInitial || this.props.teamState.isFinal;
        },

        _hasBrothers: function() {
            return this.props.parentSubStates && this.props.parentSubStates.length > 1;
        },

        _onDelete: function(e) {
            var deleteButton = e.currentTarget;
            var confirmationService = new ConfirmationService(this.props.stateSettings);
            var config = {
                states: this.props.parentSubStates,
                roles: [],
                terms: this.props.entityTerms
            };
            confirmationService
                .confirmDelete(config, this.props.teamState, deleteButton)
                .then(function(migrationState, affectedEntitiesCount) {
                    if (affectedEntitiesCount > 0) {
                        this.props.stateSettings.migrate(this.props.teamState, migrationState)
                            .then(this.props.stateSettings.performDelete.bind(this.props.stateSettings, this.props.teamState));
                    } else {
                        this.props.stateSettings.performDelete(this.props.teamState);
                    }
                }.bind(this));
        },

        _canBeDeleted: function() {
            return !this._isInitialOrFinal() && this._hasBrothers();
        },

        _deletedTitle: function() {
            return this._isInitialOrFinal() ?
                'The initial and final states can\'t be deleted.' :
                (this._hasBrothers() ? '' : 'The entity state could not be deleted because it makes a gap in workflow.');
        },

        render: function() {
            var style = {
                'left': this.props.teamState.left,
                'width': this.props.teamState.width
            };
            var isEditable = this.props.isEnabled && !this.state.isRenaming;
            var gridStateClasses = classNames({
                'process-grid__state': true,
                'tau-clickable': isEditable,
                'edit': this.state.isRenaming
            });
            var deleteStateClasses = classNames({
                'tau-icon-general tau-icon-trash': isEditable,
                'i-role-remove-team-state': true,
                'disabled': !this._canBeDeleted(),
                'tau-icon-general': this.props.isLoading,
                'tau-icon-loading': this.props.isLoading
            });

            var beginEdit = isEditable ? this._beginNameEdit : null;
            var deleteState = isEditable && this._canBeDeleted() ? this._onDelete : null;
            return (
                <div className="process-grid__custom-process__item" style={style}>
                    <div className={gridStateClasses}>
                        <div ref="namePlaceholder" className="tau-in-text" onDoubleClick={beginEdit}>{this.state.name}</div>
                        <span className={deleteStateClasses} disabled={!this._canBeDeleted()}
                            title={this._deletedTitle()} onClick={deleteState}></span>
                    </div>
                </div>
            );
        }
    });

    return TeamStateView;
});
