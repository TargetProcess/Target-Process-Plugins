define(function(require) {
    var React = require('libs/react/react-ex'),
        ChangeStateIfMounted = require('libs/react/mixins/change-state-if-mounted'),
        entityTypeNames = require('tau/const/entityType.names'),
        classNames = require('libs/classNames');

    return React.defineClass(
        ['termResolver'],
        function(termResolver) {
            return {
                displayName: 'LinkedTestPlanCreateButton',
                mixins: [React.addons.PureRenderMixin, ChangeStateIfMounted],
                testPlanNameTerm: termResolver.name(entityTypeNames.TEST_PLAN),
                _onCreateTestPlan() {
                    this.setState({isCreating: true});
                    this.props.onCreateTestPlan()
                        .always(() => this.setStateIfMounted({isCreating: false}));
                },
                getInitialState() {
                    return {
                        isCreating: false
                    };
                },
                render() {
                    var classSet = classNames({
                        'tau-linked-testplan-create': true,
                        'tau-btn': true,
                        'tau-btn-big': true,
                        'tau-primary': !this.state.isCreating,
                        'tau-btn-loader': this.state.isCreating,
                        'tau-btn-create-unit': this.state.isCreating
                    });
                    return (<button
                        className={classSet}
                        disabled={this.state.isCreating && 'disabled'}
                        type="button"
                        onClick={this._onCreateTestPlan}>{`Create ${this.testPlanNameTerm}`}</button>);
                }
            };
        });
});