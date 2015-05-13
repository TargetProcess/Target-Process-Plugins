define(function(require) {
    'use strict';

    var React = require('libs/react/react-ex');

    return React.createClass({
        getInitialState: function() {
            return {isCompleted: false};
        },

        _generatedAndSend: function() {
            this.props.actions
                .generateAndSendPassword()
                .then(this._complete);
        },

        _complete: function() {
            this.setState({isCompleted: true});
        },

        render: function() {
            var content;

            if (this.state.isCompleted) {
                content = (
                    <div className="tau-account-settings__generate-password__content" onClick={this.props.actions.close}>
                        <p className="tau-sent-password">A new password has been sent.</p>
                    </div>
                );
            } else {
                content = (
                    <div className="tau-account-settings__generate-password__content">
                        <p>A new password will be generated and sent to the user's email:</p>
                        <p><strong title={this.props.email}>{this.props.email}</strong></p>

                        <div className="tau-buttons">
                            <button className="tau-btn tau-btn-grey" onClick={this._generatedAndSend}>Generate and Send</button>
                        </div>
                    </div>
                );
            }

            return content;
        }
    });
});