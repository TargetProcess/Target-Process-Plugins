define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var AccessDetails = require('jsx!./board.access.change.confirm.details.view');
    var accessLevel = require('../models/board.access.level.constants');
    var HeaderMixin = require('jsx!./board.access.change.confirm.header.mixin');

    return React.createClass({
        displayName: 'board.access.change.confirm.custom',
        mixins: [HeaderMixin],
        propTypes: {
            currentAccessDetails: React.PropTypes.object.isRequired,
            newAccessDetails: React.PropTypes.object.isRequired
        },
        getInitialState() {
            return {opened: false};
        },
        _getContentForSimpleSharing() {
            return (
                <div>
                    <div>Currently it is visible to:</div>
                    <div className="tau-access-change-confirm_details_container">
                        <AccessDetails model={this.props.currentAccessDetails} />
                    </div>
                </div>
            );
        },
        _getContentForCustomSharing() {
            return (
                <div className="tau-access-change-confirm-table">
                    <div className="tau-access-change-confirm__current">
                        <AccessDetails model={this.props.currentAccessDetails} showChanged={true} />
                    </div>
                    <div className="tau-access-change-confirm__new">
                        <AccessDetails model={this.props.newAccessDetails} showChanged={true} />
                    </div>
                </div>
            );
        },
        render() {
            var newAccessDetails = this.props.newAccessDetails;

            if (newAccessDetails.sharing === accessLevel.PUBLIC) {
                return (
                    <div>
                        {this._getPublicHeader()}
                        {this._getContentForSimpleSharing()}
                    </div>
                );
            }
            if (newAccessDetails.sharing === accessLevel.PRIVATE) {
                return (
                    <div>
                        {this._getPrivateHeader()}
                        {this._getContentForSimpleSharing()}
                    </div>
                );
            }
            if (newAccessDetails.sharing === accessLevel.CUSTOM) {
                return (
                    <div>
                        {this._getCustomToCustomHeader()}
                        {this._getContentForCustomSharing()}
                    </div>
                );
            }

            return null;
        }
    });
});