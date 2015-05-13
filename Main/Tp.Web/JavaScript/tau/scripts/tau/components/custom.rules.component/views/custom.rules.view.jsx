define(function(require) {
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');
    var _ = require('Underscore');

    var dependencies = [
        'customRulesBus',
        'CustomRulesFeedbackView',
        'CustomRulesListView'
    ];

    return React.defineClass(dependencies, function(customRulesBus, CustomRulesFeedbackView, CustomRulesListView) {

        return {
            displayName: 'CustomRules',

            getInitialState: function() {
                return {
                    feedbackViewDisplayed: false
                };
            },

            _onFeedbackButtonClick: function() {
                this._toggleFeedbackView();
            },

            _toggleFeedbackView: function() {
                this.setState({ feedbackViewDisplayed: !this.state.feedbackViewDisplayed });
            },

            _onFeedbackSubmit: function(feedback) {
                this._toggleFeedbackView();
                customRulesBus.fire('customRules.sendFeedback', feedback);
            },

            render: function() {
                var feedbackView = React.createElement(CustomRulesFeedbackView, {
                    onSubmit: this._onFeedbackSubmit,
                    onCancel: this._toggleFeedbackView
                });
                var listView = React.createElement(CustomRulesListView, { customRulesList: this.props.data });
                var feedbackButtonClasses = classNames({
                    invisible: this.state.feedbackViewDisplayed,
                    'tau-btn': true,
                    'tau-btn-big': true,
                    'tau-primary': true,
                    'i-role-feedback-toggle': true
                });

                return (
                    <section className="custom-rules">
                        <div className="custom-rules__wrap">
                            <div className="custom-rules__header">
                                <div onClick={this._onFeedbackButtonClick} className={feedbackButtonClasses}>Share your wishes with us</div>
                                <div className="header-h1">Custom Rules</div>
                            </div>
                            { this.state.feedbackViewDisplayed ? feedbackView : null }
                            <div className="custom-rules__description">
                                Use these rules to change the default business logic and to automate some actions in the system.
                                Note that the rules are global, they are not limited to a specific project or process.
                                If there is a rule enabled that you feel wasn't applied, check the system log.
                                Don't worry if the rule you need isn't here just yet; we're already working on a language for creating your own custom rules.
                            </div>
                        </div>
                        {listView}
                    </section>
                );
            }
        }
    });
});
