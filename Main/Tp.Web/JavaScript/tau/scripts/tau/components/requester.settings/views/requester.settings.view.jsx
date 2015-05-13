define(function(require) {
    var _ = require('Underscore'),
        classNames = require('libs/classNames'),
        BubbleService = require('tau/services/bubble.service'),
        GeneratedAndSendPasswordView = require('jsx!./generated.and.send.password.view'),
        DeleteRequesterConfirmationView = require('jsx!./delete.requester.confirmation.view'),
        React = require('libs/react/react-ex');

    return React.createClass({
        getInitialState: function() {
            return {
                modifiedFields: {},
                originalEmail: this.props.requester.email,
                isUpdateInProgress: false,
                validationContext: {}
            };
        },

        _updateSettings: function() {
            this.setState({isUpdateInProgress: true});
            var requester = _.extend(this.props.requester, this.state.modifiedFields);

            this.props.actions
                .updateSettings(requester)
                .then(this._refreshOriginalEmail)
                .fail(this._setValidationContext)
                .always(_.delay.bind(_, function() {
                    if (this.isMounted()) {
                        this.setState({isUpdateInProgress: false});
                    }
                }.bind(this), 100));
        },

        _refreshOriginalEmail: function(requester) {
            this.setState({originalEmail: requester.email});
        },

        _setValidationContext: function(errorData) {
            this.setState(_.pick(errorData || {}, 'validationContext'));
        },

        _showGenerateAndSendPasswordBubble: function(e) {
            var bubbleService = new BubbleService(e.target);

            bubbleService.createAndShowBubble(GeneratedAndSendPasswordView, {
                email: this.state.originalEmail,
                actions: {
                    generateAndSendPassword: this.props.actions.generateAndSendPassword,
                    close: bubbleService.closeBubble.bind(bubbleService)
                }
            });
        },

        _showDeleteConfirmation: function(e) {
            var bubbleService = new BubbleService(e.target);

            bubbleService.createAndShowBubble(DeleteRequesterConfirmationView, {
                actions: {
                    delete: this.props.actions.delete,
                    close: bubbleService.closeBubble.bind(bubbleService)
                }
            });
        },

        _buildOptions: function(source, idKey, labelKey) {
            return _.map(source, function(item) {
                return (<option key={item[idKey]} value={item[idKey]}>{item[labelKey]}</option>);
            });
        },

        _getSetter: function(key) {
            return function(event) {
                this.state.validationContext = {};
                this.state.modifiedFields[key] = event.target.value;

                this.setState(this.state);
            }.bind(this);
        },

        _getClassName: function(key, className) {
            className = className || 'tau-in-text';

            var classes = {
                'tau-large': true,
                'tau-error': !_.isUndefined(this.state.validationContext[key])
            };

            classes[className] = true;

            return classNames(classes);
        },

        _spreadAttributes: function(key, className) {
            return {
                className: this._getClassName(key, className),
                defaultValue: this.props.requester[key],
                onChange: this._getSetter(key),
                title: this.state.validationContext[key]
            };
        },

        render: function() {
            var updateButtonClasses = classNames({
                'tau-btn tau-primary tau-update': true,
                'tau-btn-wait': this.state.isUpdateInProgress
            });

            var generateAndSendPasswordLink = null;

            if (this.props.isGenerateAndSendPasswordAvailable) {
                generateAndSendPasswordLink = (<a href="#" className="tau-account-settings__generate-password-link" onClick={this._showGenerateAndSendPasswordBubble}>Generate and Send Password</a>);
            }

            return (
                <div className="tau-account-settings-container">
                    <div className="tau-account-settings">
                        <table className="tau-account-settings-data">
                            <tbody>
                                <tr>
                                    <td><label>Email<input {...this._spreadAttributes('email')} type="text" tabIndex="1" /></label></td>
                                    <td>
                                        <label>Password</label> {generateAndSendPasswordLink}
                                        <label>
                                            <input {...this._spreadAttributes('password')} type="password" tabIndex="4"/>
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td><label>Phone<input {...this._spreadAttributes('phone')} type="text" tabIndex="2" /></label></td>
                                    <td><label>Confirm Password<input {...this._spreadAttributes('confirmPassword')} type="password" tabIndex="5"/></label></td>
                                </tr>
                                <tr>
                                    <td><label>Company<select {...this._spreadAttributes('companyId', 'tau-select')} tabIndex="3">
                                        {this._buildOptions(this.props.companies, 'id', 'name')}
                                    </select></label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colSpan="2">
                                        <label>Notes<textarea {...this._spreadAttributes('notes', 'tau-account-settings-data-notes')} tabIndex="7" /></label>
                                    </td>
                                </tr>
                            </tbody>
                        </table>

                        <button tabIndex="8" className={updateButtonClasses} type="button" onClick={this._updateSettings}>Update Settings</button>
                        <button tabIndex="10" className="tau-btn tau-danger tau-delete" type="button" onClick={this._showDeleteConfirmation}>Delete Requester</button>
                    </div>
                </div>
            );
        }
    });
});
