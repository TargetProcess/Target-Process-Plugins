define(function(require) {
    var React = require('react');
    var _ = require('Underscore');

    return React.createClass({
        displayName: 'DashboardEditorWidget',

        render: function() {
            return (
                <div key={this.props.id} className="tau-board-settings__template-list__item i-role-item">
                    <div className="tau-board-settings__template-list__preview">
                        <img className="tau-board-settings__template-list__preview-image" alt=""
                            src={this.props.previewSrc} />
                        <button type="button" className="i-role-add-widget tau-btn-add-template"
                            data-label="setup" onClick={this.props.selectItem}>
                            Add
                        </button>
                    </div>
                    <div className="tau-board-settings__template-list__name">{this.props.name}</div>
                    <div className="tau-board-settings__template-list__desc">{this.props.description}</div>
                </div>
            );
        }
    });
});
