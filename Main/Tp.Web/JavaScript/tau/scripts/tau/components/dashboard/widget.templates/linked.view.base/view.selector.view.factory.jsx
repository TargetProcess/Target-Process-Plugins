define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var SettingsListItem = require('jsx!../shared/settings.list.item.view');

    return {
        createSelectorViewClass: function(config) {
            var viewTypeName = config.viewTypeName;

            if (!_.isString(viewTypeName) || !viewTypeName.length) {
                throw new Error('viewTypeName should be defined and it should be a non-empty string');
            }

            return React.createClass({
                displayName: config.displayName,
                propTypes: {
                    views: React.PropTypes.arrayOf(React.PropTypes.shape({
                        key: React.PropTypes.string.isRequired,
                        name: React.PropTypes.string
                    })),
                    initialSelectedViewId: React.PropTypes.string,
                    onChange: React.PropTypes.func
                },

                getDefaultProps: _.constant({
                    views: [],
                    onChange: _.constant()
                }),

                getInitialState: function() {
                    return {
                        selectedViewId: this.props.initialSelectedViewId || ''
                    };
                },

                componentDidUpdate: function(prevProps, prevState) {
                    if (this.state.selectedViewId !== prevState.selectedViewId) {
                        this.props.onChange(this.state.selectedViewId);
                    }
                },

                getSelectedViewId: function() {
                    return this.state.selectedViewId;
                },

                _onViewSelected: function(e) {
                    var selectedViewId = e.target.value;
                    this.setState({selectedViewId : selectedViewId});
                },

                render: function() {
                    var selectedViewId = _.any(this.props.views, _.matches({
                        key: this.state.selectedViewId
                    })) ? this.state.selectedViewId : '';

                    var viewNodes = _.map(this.props.views, view =>
                        <option key={view.key} value={view.key}>{view.name}</option>
                    );

                    viewNodes.unshift(<option key='__none__' value='' disabled>Please choose a {viewTypeName}..</option>);

                    var title = 'Use '+ viewTypeName;

                    return (
                        <SettingsListItem
                            labelClassName="tau-select"
                            title={title}>
                            <select
                                className="tau-select tau-widget-settings__list i-role-widget__linked-view__view-selector"
                                value={selectedViewId}
                                onChange={this._onViewSelected}>
                            {viewNodes}
                            </select>
                        </SettingsListItem>
                    );
                }
            });
        }
    };
});