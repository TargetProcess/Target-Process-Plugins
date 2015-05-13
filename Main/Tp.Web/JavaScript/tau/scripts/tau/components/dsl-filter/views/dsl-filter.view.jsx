define(function(require) {
    var React = require('libs/react/react-ex'),
        ReactIgnore = require('jsx!tau/core/helpers/reactIgnore'),
        dslFilter = require('tau/components/component.dsl-filter');
    return React.createClass({
        displayName: 'DslFilter',
        componentWillUnmount() {
            if(this._dslFilterBus) {
                this._dslFilterBus.fire('destroy');
            }
        },
        componentDidMount() {
            var dslFilterBus = dslFilter.create({
                context: {
                    configurator: this.props.configurator
                },
                filter: this.props.filter
            });
            dslFilterBus.on('afterRender', (evt, {element})=> {
                element.appendTo(this.refs.filter.getDOMNode());
            });
            dslFilterBus.on('filterChanged', (evt, value) => {
                this.props.onChange({filter: value});
            });
            dslFilterBus.on('filterFocus', () => {
                this.props.onFocus();
            });
            dslFilterBus.on('filterBlur', () => {
                this.props.onBlur();
            });
            dslFilterBus.initialize({});
            dslFilterBus.fire('boardSettings.ready', {boardSettings: this.props.boardSettings});
            dslFilterBus.fire('setFilterValue', this.props.filter);
            if (!this.props.isEditable) {
                dslFilterBus.fire('disableFilter');
            }
            this._dslFilterBus = dslFilterBus;
        },
        render() {
            return (
                <ReactIgnore>
                    <span ref="filter" className="tau-board-settings__setup-encoding-filter"></span>
                </ReactIgnore>
            );
        }
    });
});