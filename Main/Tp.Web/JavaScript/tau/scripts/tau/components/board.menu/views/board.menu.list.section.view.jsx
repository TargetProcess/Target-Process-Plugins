define(function(require) {
    var React = require('libs/react/react-ex');
    var _ = require('Underscore');

    var transformations = require('./../services/board.menu.transformations.service');
    var constants = require('./../models/board.menu.constants');

    return React.defineClass(
        ['board.menu.list.group.view'],
        function(GroupView) {
            return {
                displayName: 'BoardMenuListSection',

                _getSectionClassName: function() {
                    return React.addons.classSet({
                        't3-views-section': true,
                        't3-favorite-views-section i-role-favorite-views-section': this.props.section.getIsFavoritesSection(),
                        't3-other-views-section i-role-other-views-section': this.props.section.getIsOthersSection()
                    });
                },

                render: function() {
                    var visibleData = this.props.section.getVisibleData(this.props.querySpec);
                    var visibleRegularGroups = _.filter(visibleData.groups, function(group) {
                        return group.getIsRegularGroup();
                    });

                    if (!visibleData.boards.length && !visibleRegularGroups.length) {
                        return null;
                    }

                    var groupViews = _.map(visibleData.groups, function(group) {
                        return <GroupView
                            groupModel={group}
                            id={group.groupId}
                            key={group.groupId}
                            querySpec={this.props.querySpec}
                            currentBoardId={this.props.currentBoardId}
                            focusedBoardId={this.props.focusedBoardId}
                            renamingId={this.props.renamingId}
                        />;
                    }, this);

                    var name = <div className="t3-name">{this.props.section.name}</div>;

                    return (
                        <div className={this._getSectionClassName()}>
                            {name}
                            {groupViews}
                        </div>
                    );
                }
            };
        }
    );
});
