define(function(require) {
    var React = require('libs/react/react-ex'),
        entityTypeNames = require('tau/const/entityType.names');

    return React.defineClass(
        ['termResolver', 'LinkedTestPlanCreateButton'],
        function(termResolver, LinkedTestPlanCreateButton) {
            return {
                displayName: 'LinkedTestPlanUserStoryMigrate',
                mixins: [React.addons.PureRenderMixin],
                userStoryNameTerm: termResolver.name(entityTypeNames.US),
                userStoryNamesTerm: termResolver.names(entityTypeNames.US),
                testCaseNamesTerm: termResolver.names(entityTypeNames.TEST_CASE),
                testPlanNameTerm: termResolver.name(entityTypeNames.TEST_PLAN),
                render() {
                    return (
                        <div className="tau-list-header">
                            <div className="tau-list-header__h1">
                                {`Include ${this.testCaseNamesTerm} listed below to the ${this.userStoryNameTerm} ${this.testPlanNameTerm}`}
                            </div>
                            <div className="tau-list-header__text">
                                {`Now it's possible to organize ${this.testCaseNamesTerm} linked to a ${this.userStoryNameTerm} to a related ${this.testPlanNameTerm}.`}
                                <br/>
                                {`That will allow to run it as many times as you need and re-use ${this.testCaseNamesTerm}`}
                                <br/>
                                {`to test different ${this.userStoryNamesTerm}. `}
                                <a href="http://guide.targetprocess.com/entity-test-plan.html" target="_blank"
                                    className="tau-list-header__more">Read more</a>
                            </div>
                            <LinkedTestPlanCreateButton onCreateTestPlan={this.props.onCreateTestPlan}/>
                        </div>);
                }
            };
        });
});