define(function(require) {
    var React = require('react');
    var $ = require('jQuery');
    var viewTypes = require('tau/services/boards/view.types');
    var customSharingMixin = require('jsx!./menu.item.access.view.customSharing.mixin');
    var NotificationView = require('jsx!./menu.item.access.send.notification');

    var dataArticles = {
        'dashboard': 'dashboard.editor.access',
        'customreport': 'customreport.editor.access',
        'view': 'board.editor.access'
    };

    function getDataArticleId(itemType) {
        return dataArticles[itemType] || dataArticles.view;
    }

    return React.createClass({
        displayName: 'menu.item.access.context',

        propTypes: {
            isEditable: React.PropTypes.bool.isRequired,
            isShared: React.PropTypes.bool.isRequired,
            customSharedData: React.PropTypes.shape({
                isActive: React.PropTypes.bool.isRequired
            }).isRequired,
            isAdmin: React.PropTypes.bool.isRequired,
            itemType: React.PropTypes.string.isRequired,
            isEmailNotificationsEnabled: React.PropTypes.bool.isRequired,
            enableAutoSubmit: React.PropTypes.bool.isRequired,
            createCustomSharingComponent: React.PropTypes.func.isRequired,
            onSendNotification: React.PropTypes.func.isRequired,

            onCancel: React.PropTypes.func.isRequired
        },

        mixins: [customSharingMixin],

        getInitialState() {
            return {
                uniqueOptionGroupName: _.uniqueId('share')
            };
        },


        _onApplyChanges: function() {
            if (this.refs.notificationView) {
                this.refs.notificationView.sendNotificationIfRequired();
            }

            this.props.accessModel.applyAccessChanges();
        },

        render() {
            var articleId = getDataArticleId(this.props.itemType);
            return (
                <div className="tau-access-settings">
                    <div className="tau-board-settings-group__help">
                        <span className="tau-help i-role-tooltipArticle" data-article-id={articleId}>
                            About Access
                        </span>
                    </div>

                    {this._renderButtons()}

                    <div className="tau-access-type">
                        {this._renderAccessOptions()}
                    </div>

                    <div
                        ref="customSharingPanel"
                        className="tau-notification-block">
                        {this._renderNotificationBlockContent()}
                    </div>

                </div>
            );
        },

        _renderAccessOptions() {
            var items = [];

            var itemTypeName = viewTypes.getViewTypeName(this.props.itemType);
            var itemTypeNamePlural = viewTypes.getViewTypeNamePlural(this.props.itemType);

            var accessModel = this.props.accessModel;

            var wrap = f => () => {
                f.call(accessModel);
                if (this.props.enableAutoSubmit) {
                    accessModel.applyAccessChanges();
                }
            };

            items.push(this._renderAccessOptionItem({
                isEditable: this.props.isEditable,
                isChecked: !this.props.isShared,
                accessValue: 'private',
                labelText: 'Private',
                descriptionText: 'Only you as the ' + itemTypeName + ' owner can see this ' + itemTypeName + '. ' +
                'That\'s the default option for new ' + itemTypeNamePlural + '.',
                onValueSet: wrap(accessModel.setPrivateAccess)
            }));

            // Display in read-only mode even if the user is not admin, so he can see the selected value for public view
            if (this.props.isAdmin || !this.props.isEditable) {
                items.push(this._renderAccessOptionItem({
                    isEditable: this.props.isAdmin && this.props.isEditable,
                    isChecked: this.props.isShared && !this.props.customSharedData.isActive,
                    accessValue: 'public',
                    labelText: 'Public',
                    descriptionText: 'Everyone can see this ' + itemTypeName + '. It will be shown in the left menu.',
                    onValueSet: wrap(accessModel.setPublicAccess)
                }))
            }

            items.push(this._renderAccessOptionItem({
                isEditable: this.props.isEditable,
                isChecked: this.props.isShared && this.props.customSharedData.isActive,
                accessValue: 'custom',
                labelText: 'Custom sharing',
                descriptionText: 'You as the ' + itemTypeName +
                ' owner or admin decide which team can see and change this ' + itemTypeName + '.',
                onValueSet: wrap(accessModel.setCustomAccess)
            }));

            return items;
        },

        _renderAccessOptionItem(config) {
            var labelClassName = 'tau-line';
            if (!config.isEditable) {
                labelClassName += ' tau-disabled';
            }

            return (
                <label key={config.accessValue} className={labelClassName}>
                    <div className="tau-field">
                        <input
                            role="action-share"
                            type="radio"
                            value={config.accessValue}
                            name={this.state.uniqueOptionGroupName}
                            disabled={!config.isEditable}
                            checked={config.isChecked}
                            onChange={config.onValueSet}/>
                    </div>

                    <div className="tau-txt">
                        <h3>{config.labelText}</h3>
                        <p>{config.descriptionText}</p>
                    </div>
                </label>
            );
        },

        _renderNotificationBlockContent() {
            if (!this.props.isEmailNotificationsEnabled) {
                return null;
            }

            return <NotificationView
                ref="notificationView"
                isEnabled={this.getIsCustomSharingEnabled() && this.props.accessModel.getCanNotify()}
                enableAutoSubmit={this.props.enableAutoSubmit}
                onSendNotification={this.props.onSendNotification}
                getHasNotificationDataChanged={this.props.getHasNotificationDataChanged}/>
        },

        _renderButtons() {
            if (this.props.enableAutoSubmit) {
                return null;
            }

            var confirmationButtonText = viewTypes.isMenuGroupType(this.props.itemType) ?
                'Save for Group and its Views' :
                'Save';

            var confirmationButton = this.props.isEditable ?
                <button
                    className="tau-btn tau-settings-close"
                    onClick={this._onApplyChanges}>
                    {confirmationButtonText}
                </button> :
                null;

            return (
                <div className="tau-access-settings-buttons-block">
                    {confirmationButton}
                    <button className="tau-btn" onClick={this.props.onCancel}>Cancel</button>
                </div>
            );
        }
    });
});