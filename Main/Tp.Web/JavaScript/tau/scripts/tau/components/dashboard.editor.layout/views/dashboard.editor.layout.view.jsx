define(function(require) {
    var React = require('react');
    var _ = require('Underscore');

    var columnSpecificPostfixes = {
        1: 'one-column',
        2: 'two-column',
        3: 'three-column',
        4: 'four-column'
    };

    var LayoutItem = React.createClass({
        displayName: 'DashboardLayoutEditorItem',

        _buildClassName: function() {
            var itemClassName = {
                'tau-dashboard-page__settings-layout-list__item': true,
                'tau-active': this.props.model.getIsActive()
            };

            var postfix = columnSpecificPostfixes[this.props.model.columnCount];
            if (postfix) {
                itemClassName['tau-dashboard-page__settings-layout--' + postfix] = true;
            }

            return React.addons.classSet(itemClassName);
        },

        _onApply: function() {
            this.props.model.applyLayout();
        },

        render: function() {
            var columns = _.range(this.props.model.columnCount).map(function(i) {
                return <li key={i} className="tau-dashboard-page__settings-layout-skeleton__item"/>;
            });

            return (
                <li className={this._buildClassName()} onClick={this._onApply}>
                    <ul className="tau-dashboard-page__settings-layout-skeleton">
                        {columns}
                    </ul>
                    {this.props.model.name}
                </li>
            );
        }
    });

    return React.createClass({
        displayName: 'DashboardLayoutEditor',

        componentDidMount: function() {
            this.props.model.layoutChanged.add(function() {
                this.forceUpdate();
            }, this);
        },

        componentWillUnmount: function() {
            this.props.model.layoutChanged.remove(this);
        },

        render: function() {
            var layouts = _.map(this.props.model.layouts, function(layout) {
                return <LayoutItem key={layout.key} model={layout} />
            }, this);

            return <ul className="tau-dashboard-page__settings-layout-list">{layouts}</ul>;
        }
    });
});
