define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore');

    return React.defineClass([], function() {
        return {
            _onSelectEntityType: function(entityType) {
                this.props.setActivePage({
                    entityType: entityType,
                    optionGroupId: this.props.optionGroup.getId()
                });
            },

            render: function() {
                var entityTypes = _.map(this.props.entityTypes, function(item) {
                    var entityType = item.entityType;
                    var isActive = entityType === this.props.activePage.entityType && this.props.active;
                    var classes = React.addons.classSet({
                        't3-active': isActive,
                        't3-process__list__item': true
                    });
                    return (
                        <li className={classes} key={entityType} onClick={this._onSelectEntityType.bind(this, entityType)}>
                            <i className={'tau-entity-icon tau-entity-icon--' + entityType}>{item.terms.iconSmall}</i>
                            {item.terms.name}
                        </li>);
                }, this);

                return (
                    <div className={'t3-process__item t3-process__item-submenu'} role="process-option-group">
                        <div role="process-option-group-title" className="t3-process__item__title">
                            <div className="t3-name"><strong>{this.props.optionGroup.getTitle()}</strong></div>
                        </div>
                        <ul className="t3-process__list">
                            {entityTypes}
                        </ul>
                    </div>
                    );
            }
        };
    });
});