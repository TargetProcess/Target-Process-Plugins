define(function(require) {
    var React = require('react'),
        ChangeStateIfMounted = require('libs/react/mixins/change-state-if-mounted'),
        classNames = require('libs/classNames');

    return React.createClass({
        mixins: [React.addons.PureRenderMixin, ChangeStateIfMounted],
        displayName: 'TestPlanCreateRun',
        getInitialState() {
            return {isCreating: false};
        },
        _handleOnClick() {
            this.setState({isCreating: true});
            this.props.onCreateRun()
                .always(() => this.setStateIfMounted({isCreating: false}));
        },
        render() {
            var classSet = classNames({
                'tau-btn': true,
                'tau-btn-medium': true,
                'tau-success': !this.state.isCreating,
                'tau-btn-create-unit': true,
                'tau-btn-loader': this.state.isCreating
            });
            return (
                <button
                    className={classSet}
                    type="button"
                    onClick={this._handleOnClick}
                    disabled={this.state.isCreating ? true : null}>
                    Create run
                </button>
            );
        }
    });
});