define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        ChangeStateIfMounted = require('libs/react/mixins/change-state-if-mounted'),
        classNames = require('libs/classNames'),
        entityTypeNames = require('tau/const/entityType.names');

    return React.defineClass(
        ['termResolver', 'LinkedTestPlanCreateButton'],
        function(termResolver, LinkedTestPlanCreateButton) {
            return {
                displayName: 'LinkedTestPlanCreate',
                mixins: [React.addons.PureRenderMixin, ChangeStateIfMounted],
                testPlanNameTerm: termResolver.name(entityTypeNames.TEST_PLAN),
                testPlanNameIconSmallTerm: termResolver.iconSmall(entityTypeNames.TEST_PLAN),
                testPlanNamesTerm: termResolver.names(entityTypeNames.TEST_PLAN),
                _onCreateTypeChange(createType) {
                    this.setState({createType: createType});
                },
                getInitialState() {
                    return {
                        createType: this.props.CreateType.empty
                    };
                },
                render() {
                    var entityNameTerm = termResolver.name(this.props.entityTypeName);
                    var linkedTestPlanCounts = this.props.linkedTestPlanCounts;
                    var createTypes = _.reduce(this.props.CreateType, (createTypeMemo, createType) => {
                        if (this.props.canCreateOfType(createType)) {
                            var testPlanCount = linkedTestPlanCounts[createType];
                            if (testPlanCount >= 0) {
                                var linkedTestPlanCount =
                                    <div className="tau-list-header__test-plans-list__counter tau-board-unit" title="Test Plan">
                                        <span className="tau-entity-icon tau-entity-icon--testplan">
                                            {this.testPlanNameIconSmallTerm}
                                        </span>
                                        <span className="tau-board-unit__value">{testPlanCount}</span>
                                    </div>;
                            }
                            var className = classNames({'tau-list-header__test-plans-list__item': true});
                            createTypeMemo.push(
                                <li className={className} key={createType}>
                                    <label className="tau-list-header__test-plans-list__checkbox">
                                        <input type="radio" name="test-plans"
                                            checked={this.state.createType === createType}
                                            onChange={this._onCreateTypeChange.bind(this, createType)}
                                        />
                                        <span>{this.props.getCreateTypeDescription(createType)}</span>
                                    </label>
                                    {linkedTestPlanCount}
                                </li>);
                        }
                        return createTypeMemo;
                    }, []);

                    return (
                        <div className="tau-list-header">
                            <div className="tau-list-header__h1">
                                {`Create a related ${this.testPlanNameTerm}`}
                            </div>
                            <div className="tau-list-header__text">
                                {`Include the following ${this.testPlanNamesTerm} to the ${entityNameTerm} ${this.testPlanNameTerm}`}
                            </div>
                            <ul className="tau-list-header__test-plans-list">
                                {createTypes}
                            </ul>
                            <LinkedTestPlanCreateButton
                                onCreateTestPlan={this.props.onCreateTestPlan.bind(this, this.state.createType)}/>
                        </div>
                    );
                }
            };
        });
});