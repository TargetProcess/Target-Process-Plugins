define(function(require) {
    var React = require('react'),
        classNames = require('libs/classNames'),
        ChangeStateIfMounted = require('libs/react/mixins/change-state-if-mounted');

    return React.createClass({
        mixins: [React.addons.PureRenderMixin, ChangeStateIfMounted],

        displayName: 'TestPlanManageRun',

        propTypes: {
            onBtnClickHandler: React.PropTypes.func.isRequired
        },

        getInitialState() {
            return {isCreating: false};
        },

        _handleOnClick() {
            if (this.props.isCreateRunAction) {
                this.setState({isCreating: true});
                this.props.onBtnClickHandler()
                    .always(() => this.setStateIfMounted({isCreating: false}));
            } else {
                this.props.onBtnClickHandler();
            }
        },

        render() {
            var classSet = {
                'tau-btn': true,
                'tau-btn-medium': true,
                'tau-success': this.props.isCreateRunAction && !this.state.isCreating,
                'tau-btn-create-unit': this.props.isCreateRunAction,
                'tau-btn-loader': this.props.isCreateRunAction && this.state.isCreating,
                'tau-btn-play': !this.props.isCreateRunAction
            };

            return (
                <button
                    className={classNames(classSet)}
                    type="button"
                    onClick={this._handleOnClick}
                    disabled={this.state.isCreating || !this.props.runAction ? "disabled" : null}>
                    {this.props.runAction}
                </button>
            );
        }
    });
});