define(function(require) {
    var React = require('libs/react/react-ex');
    var _ = require('Underscore');

    var dependencies = [
        'customRulesBus',
        'CustomRuleItemView'
    ];

    return React.defineClass(dependencies, function(customRulesBus, CustomRuleItemView) {
        return {
            render: function() {
                var items = {};

                _.each(this.props.customRulesList, function(customRule) {
                    items['key-' + customRule.id] = React.createElement(CustomRuleItemView, customRule);
                });

                return (
                    <div className="custom-rules__list">
                        {items}
                    </div>
                );
            }
        }
    });

});
