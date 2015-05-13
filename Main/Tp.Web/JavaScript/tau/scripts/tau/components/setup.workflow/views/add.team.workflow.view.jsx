define(function(require) {
    var _ = require('Underscore'),
        classNames = require('libs/classNames'),
        React = require('libs/react/react-ex'),
        t = React.PropTypes;

    return React.createClass({

        mixins: [React.addons.LinkedStateMixin],

        displayName: 'TeamWorkflowEditor',

        propTypes: {
            teamWorkflow: t.shape({
                title: t.string.isRequired,
                dsl: t.string.isRequired
            }),
            affectedTeams: t.array.isRequired,
            isNew: t.bool,
            saveAction: t.func.isRequired,
            deleteAction: t.func
        },

        getInitialState: function() {
            return {
                canEdit: true,
                underDelete: false,
                title: this.props.teamWorkflow.title,
                dsl: this.props.teamWorkflow.dsl
            };
        },

        componentDidMount: function() {
            var deleteButton = this.refs.deleteButton;
            if (deleteButton && this._isEditable()) {
                $(deleteButton.getDOMNode()).tauConfirm({
                    content: '<h3>Do you really want to delete this Team Workflow?</h3>',
                    callbacks: {
                        success: function() {
                            this.setState({underDelete: true});
                            this.props.deleteAction();
                        }.bind(this)
                    },
                    zIndex: 1000
                });
            }
        },

        _isEditable: function() {
            return this.state.canEdit && !this.state.underDelete;
        },

        _save: function() {
            this.setState({canEdit: false});
            this.props.saveAction(this.state.title, this.state.dsl,
                this.props.teamWorkflow.title, this.props.teamWorkflow.dsl)
                .fail(function() {
                    this.setState({canEdit: true});
                }.bind(this));
        },

        render: function() {
            var warningMessage = this.props.isNew ? null :
                ( <div className="add-team-workflow-mapping-format__warning">
                    This operation can take a few moments, as it requires an update to any item related to the Team Workflow. Batch updating a Team Workflow may affect the Team State of items related to it. To rename or delete Team States it is recommended to modify the state directly.
                </div>);
            var saveAction = this._isEditable() ? this._save : null;
            var editDisabled = this._isEditable() ? '' : 'disabled';
            var saveClasses = classNames({
                'tau-btn': true,
                'tau-primary': true,
                'tau-success': this.props.isNew,
                'i-role-actionok': true
            });
            var deleteClasses = classNames({
                'tau-attention': true,
                'tau-btn': true,
                'tau-btn-remove': true,
                'tau-btn-wait': this.state.underDelete,
                'i-role-remove-team-workflow': true
            });
            var affectedTeams = this.props.affectedTeams;
            var deleteDisabled = editDisabled || affectedTeams.length > 0;
            var deleteWarning = affectedTeams.length === 1 ?
                'Team Workflow can\'t be deleted as used by \'' + affectedTeams[0].name + '\' team' :
                'Team Workflow can\'t be deleted as used by ' + affectedTeams.length + ' teams';
            var deleteTitle = affectedTeams.length > 0 ? deleteWarning : 'Delete Team Workflow';
            var deleteButton = this.props.deleteAction ?
                <button className={deleteClasses} ref="deleteButton" title={deleteTitle} disabled={deleteDisabled} /> : '';
            var saveLoading = this.state.canEdit ? '' : <span className="tau-save-icon-loading tau-icon-general tau-icon-loading" />;
            var dslTextAreaClasses = "tau-in-text add-team-workflow__textarea";
            if (!this.props.isNew) {
                dslTextAreaClasses += " extended";
            }
            return (
                <div className="add-team-workflow">
                    <div className="add-team-workflow-content">
                        <div className="add-team-workflow-fields">
                            <input tabIndex="0" name="title" autoComplete="off" autoFocus="true" className="tau-in-text add-team-workflow__title" placeholder="Name" valueLink={this.linkState("title")} disabled={editDisabled}/>
                            <textarea name="dsl" className={dslTextAreaClasses} valueLink={this.linkState("dsl")} disabled={editDisabled}/>
                        </div>
                        <div className="add-team-workflow-mapping-format">
                            {warningMessage}
                            <div className="add-team-workflow-mapping-format__text">
                                Every entity state in Team Workflow should be mapped to a corresponding entity state in Project Workflow.
                            </div>
                            <div className="add-team-workflow-mapping-format__text">
                                Provide mapping in the format:
                            </div>
                            <div className="add-team-workflow__mapping-format__content">Project State: Team State 1, Team State 2</div>
                            <a className="add-team-workflow-mapping-format__link" href="//guide.targetprocess.com/settings/team-workflow.html" target="_blank">Read more about Team Workflow</a>
                        </div>
                    </div>
                    <div className="add-team-workflow-controls">
                        <button className={saveClasses} disabled={editDisabled} onClick={saveAction}>Save changes</button>
                        {saveLoading}
                        {deleteButton}
                    </div>
                </div>
            );
        }
    });
});
