define(function(require) {
    var React = require('libs/react/react-ex');
    var _ = require('Underscore');
    var $ = require('jQuery');
    var feedbackTemplate = require('text!tau/components/custom.rules.component/templates/custom.rules.feedback.template.html');

    var dependencies = [
        'customRulesBus'
    ];

    return React.defineClass(dependencies, function(customRulesBus) {
        return {

            getInitialState: function() {
                return {
                    feedback: feedbackTemplate
                };
            },

            _handleSubmit: function() {
                this.props.onSubmit(this.state.feedback);
                this.setState({
                    feedback: feedbackTemplate
                });
            },

            _handleChange: function() {
                this.setState({ feedback: event.target.value });
            },

            render: function() {

                return (
                    <div className="custom-rules__forms">
                        <div className="custom-rules__forms__description">
                        Based on our your feedback, we came up with a set of most useful rules that you can apply from the list below. Meanwhile we're developing a flexible system for creating your own rules. We will be happy if you share your experience and ideas with us.
                        </div>
                        <textarea className="tau-in-text" onChange={this._handleChange} value={this.state.feedback}></textarea>
                        <button className="tau-btn tau-btn-big tau-primary i-role-feedback-submit" onClick={this._handleSubmit} type="button">Send</button>
                        <button className="tau-btn tau-btn-big i-role-feedback-cancel" onClick={this.props.onCancel} type="button">Cancel</button>
                    </div>
                );
            }
        }
    });

});
