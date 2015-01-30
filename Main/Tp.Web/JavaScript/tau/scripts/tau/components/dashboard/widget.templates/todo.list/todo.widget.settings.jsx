define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var entityLayouts = require('tau/components/dashboard/widget.templates/todo.list/todo.widget.entity.layouts');
    var WidgetConstants = require('./todo.widget.constants');

    var HighlightSettingsItem = React.createClass({
        render: function() {
            return (
                <li className="tau-widget-settings-highlight-cards__item">
                    <label className="tau-checkbox">
                        <input type="checkbox" defaultChecked={this.props.defaultChecked}
                            onChange={this.props.onChange}/>
                        <i className="tau-checkbox__icon"></i>
                        {this.props.children}
                        <i style={{backgroundColor: WidgetConstants.HIGHLIGHT_COLOR}}
                            className="tau-icon tau-icon_type_color"></i>
                    </label>
                </li>
            );
        }
    });

    return React.createClass({
        getInitialState: function() {
            return {
                entities: this.props.initialEntities,
                highlight: this.props.initialHighlight
            };
        },

        componentDidMount: function() {
            $(this.getDOMNode()).find('.i-role-no-state-changes-days').editableText({
                mask: /^\d*$/,
                className: 'none',
                classNameActive: 'none',
                restoreText: false
            });
        },

        componentWillUnmount: function() {
            $(this.getDOMNode()).find('.i-role-no-state-changes-days').editableText('destroy');
        },

        _getEntityTerm: function(entityType) {
            return this.props.termProcessor.getTerms(entityType).names;
        },

        _getEntityItem: function(entityType) {
            var isChecked = _.contains(this.state.entities, entityType);
            return <li className="tau-widget-settings-show-cards__item" key={entityType}>
                <label className={'tau-checkbox tau-checkbox--entity tau-checkbox--entity--' + entityType}>
                    <input type="checkbox" value={entityType} defaultChecked={isChecked} onChange={this._onEntityCheckedChange}/>
                    <i className="tau-checkbox__icon"></i>
                    <span>{this._getEntityTerm(entityType)}</span>
                </label>
            </li>;
        },

        _onEntityCheckedChange: function(event) {
            var entities = this.state.entities;
            var entityName = event.target.getAttribute('value');
            if (event.target.checked) {
                entities.push(entityName)
            } else {
                entities = _.filter(entities, function(entity) {
                    return entity !== entityName;
                });
            }
            this.setState({entities: entities});
        },

        _onHighlightBlockersChange: function(event) {
            var highlight = this.state.highlight;
            highlight.blockers = event.target.checked;
            this.setState({highlight: highlight});
        },

        _onHighlightNoChangeStatesChange: function(event) {
            var highlight = this.state.highlight;
            highlight.noStateChanges = event.target.checked;
            this.setState({highlight: highlight});
        },

        _onHighlightNoChangeStatesDaysChange: function(event) {
            var highlight = this.state.highlight;
            highlight.noStateChangesDays = event.target.value;
            this.setState({highlight: highlight});
        },

        render: function() {
            var types = _.keys(entityLayouts);
            var entityItems = _.map(types, this._getEntityItem);
            return (
                <div className="tau-widget-settings__content-cards">
                    <ul className="tau-widget-settings-show-cards">
                        <li className="tau-widget-settings-show-cards__item tau-widget-settings-cards-title">Show cards</li>
                        {entityItems}
                    </ul>
                    <ul className="tau-widget-settings-highlight-cards">
                        <li className="tau-widget-settings-highlight-cards__item tau-widget-settings-cards-title">
                        Highlight cards
                        </li>
                        <HighlightSettingsItem defaultChecked={this.state.highlight.blockers}
                            onChange={this._onHighlightBlockersChange}>
                            <span>Top priority and blockers</span>
                        </HighlightSettingsItem>
                        <HighlightSettingsItem defaultChecked={this.state.highlight.noStateChanges}
                            onChange={this._onHighlightNoChangeStatesChange}>
                            <span>No state changes in
                                <input disabled={this.state.highlight.noStateChanges ? "" : "disabled"}
                                    value={this.state.highlight.noStateChangesDays}
                                    className="i-role-no-state-changes-days tau-in-text" type="text"
                                    onChange={this._onHighlightNoChangeStatesDaysChange}/>
                            days</span>
                        </HighlightSettingsItem>
                    </ul>
                </div>
            );
        }
    });
});
