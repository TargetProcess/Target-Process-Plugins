define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var PrivateAccessDetails = require('jsx!./board.access.change.confirm.details.private.view');
    var PublicAccessDetails = require('jsx!./board.access.change.confirm.details.public.view');
    var CustomAccessDetails = require('jsx!./board.access.change.confirm.details.custom.view');
    var accessLevel = require('../models/board.access.level.constants');

    return React.createClass({
        displayName: 'board.access.change.confirm.view',
        propTypes: {
            onApply: React.PropTypes.func.isRequired,
            onCancel: React.PropTypes.func.isRequired,
            currentAccessDetails: React.PropTypes.object.isRequired,
            newAccessDetails: React.PropTypes.object.isRequired
        },
        getDefaultProps: function() {
            return {
                onApply: _.constant(),
                onCancel: _.constant()
            };
        },
        _getContent() {
            var currentAccess = this.props.currentAccessDetails;
            var newAccess = this.props.newAccessDetails;

            if (currentAccess.sharing === accessLevel.PRIVATE) {
                return (<PrivateAccessDetails newAccessDetails={newAccess} />);
            }
            if (currentAccess.sharing === accessLevel.PUBLIC) {
                return (<PublicAccessDetails newAccessDetails={newAccess} />);
            }
            if (currentAccess.sharing === accessLevel.CUSTOM) {
               return (<CustomAccessDetails newAccessDetails={newAccess} currentAccessDetails={currentAccess} />);
            }

            return null;
        },
        render() {
            return (
                <div>
                    <div className="ui-access-change-confirm__container">
                        {this._getContent()}
                    </div>
                    <div>
                        <button className="tau-btn tau-primary tau-btn-ok tau-role-ok" onClick={this.props.onApply} autoFocus="true">Move view</button>
                        <button className="tau-btn tau-btn-cancel tau-role-cancel" onClick={this.props.onCancel}>Cancel</button>
                    </div>
                </div>
            );
        }
    });
});