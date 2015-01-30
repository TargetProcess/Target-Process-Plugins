define(function(require) {
    var React = require('react');
    var WidgetConstants = require('./dashboard.widget.latest.comments.constants');

    return React.createClass({
        getInitialState: function() {
            return {
                source: this.props.initialSource
            };
        },

        render: function() {
            var source = this.state.source.toLowerCase();
            return (
                <ul className="tau-widget-settings-list">
                    <li className="tau-widget-settings-list__item">
                        <label className="tau-switch i-role-show-comments">
                            <span className="tau-widget-settings-list__title">Show comments</span>
                            <div className="tau-inline-group">
                                <button
                                className={'tau-btn tau-btn-board-view' + (source === WidgetConstants.SOURCE_ALL ? ' tau-checked' : '')}
                                type="button"
                                data-value={WidgetConstants.SOURCE_ALL}
                                onClick={this._onSourceButtonClick}>All</button>
                                <button
                                className={'tau-btn tau-btn-board-view tau-extension-board-tooltip' + (source === WidgetConstants.SOURCE_RELATED_TO_ME ? ' tau-checked' : '')}
                                type="button"
                                data-value={WidgetConstants.SOURCE_RELATED_TO_ME}
                                data-title="Comments from entities you follow, set as an owner or assigned to"
                                onClick={this._onSourceButtonClick}>Related to me</button>
                            </div>
                        </label>
                    </li>
                </ul>
                );
        },

        _onSourceButtonClick: function(event) {
            event.preventDefault();
            event.stopPropagation();
            this.setState({source: event.target.getAttribute('data-value')});
        }
    });
});