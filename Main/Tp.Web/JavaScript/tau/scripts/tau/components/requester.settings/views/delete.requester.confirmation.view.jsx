define(function(require) {
    'use strict';

    var React = require('libs/react/react-ex');

    return React.createClass({
        render: function() {
            return (<div className="tau-account-settings__delete-requester__content">
                <p><span>Are you sure?</span></p>
                <div className="tau-buttons">
                    <button className="tau-primary tau-btn" type="button" onClick={this.props.actions.delete}>Yes, I'm sure</button>
                    <button className="tau-btn" type="button" onClick={this.props.actions.close}>No</button>
                </div>
            </div>);
        }
    });
});