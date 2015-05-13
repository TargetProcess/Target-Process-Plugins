define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var classNames = require('libs/classNames');
    var guideUrls = {
        general: 'https://guide.targetprocess.com/settings/single-sign-on-in-targetprocess.html',
        okta: 'https://guide.targetprocess.com/settings/how-to-set-up-single-sign-on-with-okta.html',
        onelogin: 'https://guide.targetprocess.com/settings/how-to-set-up-single-sign-on-with-onelogin.html',
        adfs: 'https://guide.targetprocess.com/settings/how-to-set-up-single-sign-on-with-adfs.html',
        otherProviders: 'https://guide.targetprocess.com/settings/how-to-set-up-single-sign-on-with-other-providers.html'
    };

    return React.createClass({
        displayName: 'SingleSignon',

        getInitialState: function() {
            return {
                isEnabled: false,
                signonUrl: null,
                certificate: null,
                isUserProvisioningEnabled: true,
                isFormAuthenticationDisabled: false,
                exceptionUsers: []
            };
        },

        componentDidMount: function() {
            this._initializeExceptionsList();
        },

        _initializeExceptionsList: function() {
            var $textarea = $(this.refs.exceptionUsers.getDOMNode());

            $textarea.tauInviter({
                configurator: this.props.configurator,
                allowEmails: true,
                allowOnlyExistingUsers: true,
                change: this._onExceptionUsersChange
            });
        },

        componentWillReceiveProps: function(nextProps) {
            var nextInitialSettings = nextProps.initialSettings;

            if (nextInitialSettings && !_.isEqual(this.props.initialSettings, nextInitialSettings)) {
                this.setState({
                    isEnabled: nextInitialSettings.isEnabled,
                    signonUrl: nextInitialSettings.signonUrl,
                    certificate: nextInitialSettings.certificate,
                    isUserProvisioningEnabled: nextInitialSettings.isUserProvisioningEnabled,
                    isFormAuthenticationDisabled: nextInitialSettings.isFormAuthenticationDisabled
                });

                this._setExceptionUsers(nextInitialSettings.exceptionUsers);
            }
        },

        componentDidUpdate: function() {
            if (this.state.hasDataValidationFailed && this._isDataValid()) {
                this.setState({ hasDataValidationFailed: false });
            }

            if ((this._triedToSaveOnce() && !this.props.saveInProgress) && !this.state.hasDataChangedAfterSave && this._isDataChanged()) {
                this.setState({ hasDataChangedAfterSave: true });
            }
        },

        _triedToSaveOnce: function() {
            return _.isBoolean(this.props.saveSuccessful);
        },

        _onStatusChange: function(e) {
            this.setState({ isEnabled: e.target.checked });

            if (e.target.checked) {
                this.refs.signonUrl.getDOMNode().focus();
            }
        },

        _onSignonUrlChange: function(e) {
            this.setState({ signonUrl: e.target.value });
        },

        _onCertificateChange: function(e) {
            this.setState({ certificate: e.target.value });
        },

         _onUserProvisioningChange: function(e) {
            this.setState({ isUserProvisioningEnabled: e.target.checked });
        },

        _onFormAuthChange: function(e) {
            var isChecked = e.target.checked;

            this.setState({ isFormAuthenticationDisabled: isChecked });

            if (isChecked) {
                var tauInviter = this._getTauInviterInstance();

                this._ensureExceptionUsersPresent();
                tauInviter.focus();
            }
        },

        _ensureExceptionUsersPresent: function() {
            var exceptionUsers = this._getExceptionUsersList();

            if (_.isEmpty(exceptionUsers)) {
                var currentUser = this.props.configurator.getLoggedUser();

                this._setExceptionUsers([
                    {
                        id: currentUser.id,
                        name: currentUser.name,
                        avatarUri: currentUser.avatarUri
                    }
                ]);
            }
        },

        _setExceptionUsers: function(users) {
            var tauInviter = this._getTauInviterInstance();

            tauInviter.setValue(users);
            this._onExceptionUsersChange();
        },

        _getTauInviterInstance: function() {
            var $textarea = $(this.refs.exceptionUsers.getDOMNode());

            return $textarea.tauInviter('instance');
        },

        _getExceptionUsersList: function() {
            var tauInviter = this._getTauInviterInstance();
            var exceptionUsers = tauInviter.getValue();
            exceptionUsers = _.reject(exceptionUsers, function(exceptionUser) {
                return exceptionUser.type == 'invited';
            });

            return exceptionUsers;
        },

        _onExceptionUsersChange: function() {
            var exceptionUsers = this._getExceptionUsersList();

            this.setState({ exceptionUsers: exceptionUsers });
        },

        _onSave: function(e) {
            var isDataValid = this._isDataValid();

            if (isDataValid) {
                var formData = this._getFormData();

                this.setState({ hasDataChangedAfterSave: false });
                this.props.bus.fire('sso.settings.save', formData);
            } else {
                this.setState({ hasDataValidationFailed: true });
            }

            e.preventDefault();
        },

        _getFormData: function() {
            var formData = {
                isEnabled: this.state.isEnabled,
                signonUrl: this.state.signonUrl,
                certificate: this.state.certificate,
                isUserProvisioningEnabled: this.state.isUserProvisioningEnabled,
                isFormAuthenticationDisabled: this.state.isFormAuthenticationDisabled,
                exceptionUsers: this.state.exceptionUsers
            };

            return formData;
        },

        _isDataValid: function() {
            var result = true;

            if (!this._isSignonUrlValid()) {
                result = false;
            }

            return result;
        },

        _isSignonUrlValid: function() {
            return !this.state.isEnabled || Boolean(this.state.signonUrl);
        },

        _isDataChanged: function() {
            var formData = this._getFormData();

            return !_.isEqual(this.props.initialSettings, formData);
        },


        _shouldDisableSave: function() {
            var isInProgress = this.props.saveInProgress;
            var isDataSame = !this._isDataChanged();
            var hasLastSaveFailed = this._triedToSaveOnce() && !this.props.saveSuccessful;
            var hasDataValidationFailed = this.state.hasDataValidationFailed;

            return hasDataValidationFailed || isInProgress || (isDataSame && !hasLastSaveFailed);
        },

        _getOperationStatusIndicator: function() {
            var isDataChanged = this._isDataChanged();
            var triedToSave = this._triedToSaveOnce();
            var shouldHideStatusIndicator = this.state.hasDataChangedAfterSave ||
                this.props.saveInProgress ||
                !triedToSave ||
                (isDataChanged && this.props.saveSuccessful);
            var classes = classNames({
                'tau-btn-state': true,
                'hidden': shouldHideStatusIndicator,
                'tau-btn-state--saved': this.props.saveSuccessful,
                'tau-btn-state--saving-failed': !this.props.saveSuccessful
            });
            var label = (this.props.saveSuccessful) ? 'Saved' : 'Saving failed';

            return (
                <div className={classes}>{label}</div>
            );
        },

        _getLoginBackdoorUrl: function() {
            return this.props.applicationPath + '/login.aspx?login=form';
        },

        render: function() {
            var saveButtonClasses = classNames({
                'tau-btn tau-btn-big tau-primary i-role-save': true,
                'tau-btn-wait': this.props.saveInProgress,
                'tau-btn-saved': this.props.saveSuccessful
            });

            var shouldHighlightSignonUrlInError = this.state.hasDataValidationFailed && !this._isSignonUrlValid();
            var signonInputClasses = classNames({
                'tau-in-text i-role-url': true,
                'tau-error': shouldHighlightSignonUrlInError
            });

            var operationStatusIndicator = this._getOperationStatusIndicator();
            var loginBackdoorURL = this._getLoginBackdoorUrl();

            return (
<div className="single-sign-on">
    <div className="single-sign-on__header">
        <div className="header-h1">Single Sign-on</div>
    </div>
    <div className="single-sign-on__description">
        <p className="single-sign-on__description__item">Set up Single sign-on to log in to Targetprocess via SAML-using identity providers. See <a href={guideUrls.general} target="_blank">our general article on SSO</a> and guides for integration with specific providers: <a href={guideUrls.okta} target="_blank">Okta</a>, <a href={guideUrls.onelogin} target="_blank">Onelogin</a> and <a href={guideUrls.adfs} target="_blank">Active Directory Federation Services</a> (you can use <a href={guideUrls.otherProviders} target="_blank">other providers</a> as well).</p>
    </div>
    <div className="single-sign-on__information">
        <div className="single-sign-on__information__header">Targetprocess Information</div>
        <div className="single-sign-on__information__description">
        Use these values to configure a connector in your identity provider settings:
        </div>
        <ul className="single-sign-on__tp-information">
            <li className="single-sign-on__tp-information__item">
                <div className="single-sign-on__tp-information-name">Assertion Consumer URL:</div>
                <div className="single-sign-on__tp-information-wrap">
                    <input className="single-sign-on__tp-information-value" readOnly type="text" value={this.props.applicationPath + '/api/sso/saml2'}></input>
                </div>
            </li>
        </ul>
    </div>
    <ul className="single-sign-on__settings">
        <li className="single-sign-on__settings__item">
            <label className="tau-checkbox">
                <input type="checkbox" className='i-role-status' checked={this.state.isEnabled} onChange={this._onStatusChange} />
                <i className="tau-checkbox__icon"></i>
                <span className="tau-checkbox-label">Enable Single Sign-on</span>
            </label>
            <ul className="single-sign-on__settings__parameters">
                <li className="single-sign-on__settings__parameters__item">
                    <label className="single-sign-on__settings__parameters__label" htmlFor="sign-on-url">Sign-on URL:</label>
                    <input ref="signonUrl" value={this.state.signonUrl} onChange={this._onSignonUrlChange} className={signonInputClasses} type="text" placeholder="Identity Provider Single Sign-on URL, SAML 2.0 Endpoint (https://example.com/sso/saml)"/>
                </li>
                <li className="single-sign-on__settings__parameters__item">
                    <label className="single-sign-on__settings__parameters__label" htmlFor="sign-on-certificate">Certificate:</label>
                    <textarea value={this.state.certificate} onChange={this._onCertificateChange} name="certificate" className="tau-in-text i-role-certificate" placeholder="Copy your whole X.509 Certificate content here"></textarea>
                </li>
            </ul>
        </li>
        <li className="single-sign-on__settings__item">
            <label className="tau-checkbox">
                <input className='i-role-user-provisioning' checked={this.state.isUserProvisioningEnabled} onChange={this._onUserProvisioningChange} type="checkbox" />
                <i className="tau-checkbox__icon"></i>
                <span className="tau-checkbox-label">Enable JIT Provisioning</span>
            </label>
            <span className="tau-help i-role-tooltipArticle" data-article-id="sso.options.user-provisioning"></span>
        </li>
        <li className="single-sign-on__settings__item">
            <label className="tau-checkbox">
                <input type="checkbox" checked={this.state.isFormAuthenticationDisabled} onChange={this._onFormAuthChange} />
                <i className="tau-checkbox__icon"></i>
                <span className="tau-checkbox-label">Disable login form</span>
            </label>
            <span className="tau-help i-role-tooltipArticle" data-article-id="sso.options.disable-form-authentication"></span>
            <div className="single-sign-on__settings__standard-access">
                <div className="single-sign-on__settings__standard-access__title">
                    Exceptions list — allow these users to log in with their logins and passwords (the form is available at&nbsp;<a href={loginBackdoorURL}>{loginBackdoorURL}</a>):
                </div>
                <div className="single-sign-on__settings__standard-access__account-list tau-invite-widget">
                    <textarea ref="exceptionUsers" className="tau-in-text" placeholder="Start typing name(s) or email(s) to add users"></textarea>
                </div>
            </div>
        </li>
    </ul>
    <button className={saveButtonClasses} disabled={this._shouldDisableSave()} onClick={this._onSave}>Save</button>
    {operationStatusIndicator}
</div>
            );
        }
    });
});
