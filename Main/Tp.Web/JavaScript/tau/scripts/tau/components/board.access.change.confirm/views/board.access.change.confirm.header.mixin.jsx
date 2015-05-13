define(function(require) {
    var React = require('react');

    return {
        _getHeader(text, className) {
            var actualClass = 'ui-access-change-confirm__header ' + (className || '');
            return (
                <div className={actualClass}>{text}</div>
            );
        },
        _getPublicHeader() {
            return this._getHeader('This View will be visible to all users.', 'ui-access-change-confirm__header--public');
        },
        _getPrivateHeader() {
            return this._getHeader('This View will be visible to you only.', 'ui-access-change-confirm__header--private');
        },
        _getCustomHeader() {
            return this._getHeader('This View will be visible to some Teams and Projects.');
        },
        _getCustomToCustomHeader() {
            return this._getHeader('This View\'s visibility will be changed:');
        }
    };
});