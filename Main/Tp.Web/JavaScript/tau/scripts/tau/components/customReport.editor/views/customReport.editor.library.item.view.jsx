define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var previewTemplate = require('text!../templates/chart-previews.svg');

    return React.createClass({
        displayName: 'CustomReportLibraryItemView',

        render: function() {
            var classNames = 'tau-board-settings__template-list__preview tau-chart-template ' + (this.props.classNames || []).join(' ');

            return (
                <div key={this.props.id} className="tau-board-settings__template-list__item i-role-item" onClick={this.props.selectItem}>
                    <div className={classNames} dangerouslySetInnerHTML={{__html: previewTemplate}}>
                    </div>
                    <div className="tau-board-settings__template-list__name">{this.props.name}</div>
                    <div className="tau-board-settings__template-list__desc">{this.props.description}</div>
                </div>
            );
        }
    });
});
