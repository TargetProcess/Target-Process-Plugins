define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'menu.item.access.send.notification',

        propTypes: {
            isEnabled: React.PropTypes.bool.isRequired,
            enableAutoSubmit: React.PropTypes.bool.isRequired,
            onSendNotification: React.PropTypes.func.isRequired,
            getHasNotificationDataChanged: React.PropTypes.func.isRequired
        },

        getInitialState: () => ({
            sendNotificationWhenApplied: false,
            isSendingNotification: false
        }),

        _onSendNotification() {
            if (this.state.isSendingNotification) {
                return;
            }

            this.setState({isSendingNotification: true});
            this.props
                .onSendNotification()
                .always(() => {
                    if (this.isMounted()) {
                        this.setState({isSendingNotification: false});
                    }
                });
        },

        sendNotificationIfRequired() {
            if (this.state.sendNotificationWhenApplied) {
                this._onSendNotification();
            }
        },

        _onSendNotificationChanged: function(e) {
            this.setState({sendNotificationWhenApplied: e.target.checked});
        },

        render() {
            var disabled = !this.props.isEnabled ||
                this.state.isSendingNotification ||
                !this.props.getHasNotificationDataChanged();

            var notificationControl = this.props.enableAutoSubmit ?
                <button
                    className="tau-btn"
                    type="button"
                    disabled={disabled}
                    onClick={this._onSendNotification}>
                    Send notification
                </button> :

                <label
                    className="tau-checkbox tau-board-option-access-notify">
                    <input
                        type="checkbox"
                        disabled={disabled}
                        checked={this.state.sendNotificationWhenApplied}
                        onChange={this._onSendNotificationChanged}/>
                    <i className="tau-checkbox__icon"/>
                    <span>Send notification</span>
                </label>;

            return (
                <section className="tau-board-send-notification">
                    {notificationControl}
                </section>
            );
        }
    })
});