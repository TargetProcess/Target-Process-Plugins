define(function(require) {
    var _ = require('Underscore');

    return {
        componentDidMount() {
            this.props
                .createCustomSharingComponent(this.refs.customSharingPanel.getDOMNode())
                .then(token => {
                    this._customSharingComponentToken = token;
                    if (this.isMounted()) {
                        this._updateCustomSharingPanel();
                    }
                });
        },

        componentDidUpdate() {
            this._updateCustomSharingPanel();
        },

        /**
         * @private
         */
        _updateCustomSharingPanel() {
            if (this._customSharingComponentToken) {
                var updateData = _.deepClone(this.props.customSharedData);
                updateData.isActive = updateData.isActive && this.props.isEditable;
                this._customSharingComponentToken.updateCustomSharingComponent(updateData);
            }
        },

        /**
         * @return {Boolean}
         */
        getIsCustomSharingEnabled() {
            return this.props.isEditable && this.props.customSharedData.isActive;
        }
    };
});