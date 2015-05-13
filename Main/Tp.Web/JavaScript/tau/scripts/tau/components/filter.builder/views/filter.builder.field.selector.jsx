define(function(require) {
    var _ = require('Underscore');
    var React = require('react');

    return React.createClass({
        displayName: 'filter.builder.field.selector',

        propTypes: {
            options: React.PropTypes.arrayOf(React.PropTypes.shape({
                value: React.PropTypes.string.isRequired,
                label: React.PropTypes.string.isRequired
            })).isRequired,
            selectedValue: React.PropTypes.string,
            onFieldSelected: React.PropTypes.func.isRequired,
            defaultLabel: React.PropTypes.string
        },

        mixins: [React.addons.PureRenderMixin],

        getDefaultProps() {
            return {
                selectedValue: '',
                defaultLabel: 'Select...'
            };
        },

        render() {
            var options = _.map(this.props.options, o => <option key={o.value} value={o.value}>{o.label}</option>);

            return (
                <select value={this.props.selectedValue} onChange={this.props.onFieldSelected}
                    className="i-role-field-selector tau-select">
                    <option key="-1" value="">{this.props.defaultLabel}</option>
                    {options}
                </select>
            );
        }
    });
});
