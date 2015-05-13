define(function(require) {
    var React = require('libs/react/react-ex');
    var classNames = require('libs/classNames');
    var _ = require('Underscore');

    var dependencies = [
        'customRulesBus',
        'CustomRuleTogglerView'
    ];

    return React.defineClass(dependencies, function(customRulesBus, CustomRuleTogglerView) {
        return {
            displayName: 'CustomRuleItem',

            _onCustomRuleStatusToggle: function() {
                customRulesBus.fire('customRule.toggleStatus', this.props);
            },

            render: function() {
                var customRuleTogglerView = React.createElement(CustomRuleTogglerView, {
                    isEnabled: this.props.isEnabled,
                    onStatusToggled: this._onCustomRuleStatusToggle
                });
                var classes = classNames({
                    'custom-rules__list__item': true,
                    active: this.props.isEnabled
                });

                return (
                    <div className={classes}>
                        <div className="custom-rules__list__item__title" dangerouslySetInnerHTML={{__html: this.props.name}}></div>
                        <div className="custom-rules__list__item__text" dangerouslySetInnerHTML={{__html: this.props.description || ''}}></div>
                        {customRuleTogglerView}
                    </div>
                );
            }
        }
    });
});
