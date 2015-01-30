define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore');

    return React.defineClass([], function() {
        return {
            displayName: 'ProcessOptionGroup',

            _activate: function() {
                this.props.setActivePage({
                    optionGroupId: this.props.optionGroup.getId()
                });
            },

            render: function() {
                var classes = React.addons.classSet({
                    't3-process__item': true,
                    't3-active': this.props.active
                });
                return (
                    <div className={classes} role="process-option-group">
                        <a href="#" onClick={this._activate} role="process-option-group-title" className="t3-process__item__title">
                            <div className="t3-name">{this.props.optionGroup.getTitle()}</div>
                        </a>
                    </div>
                );
            }
        };
    });
});
