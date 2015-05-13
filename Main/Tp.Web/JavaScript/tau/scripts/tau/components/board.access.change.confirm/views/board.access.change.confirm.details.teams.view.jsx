define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'board.access.change.confirm.details.teams.view',
        propTypes: {
            showChanged: React.PropTypes.bool,
            teams: React.PropTypes.arrayOf(React.PropTypes.shape({
                id: React.PropTypes.number.isRequired,
                name: React.PropTypes.string.isRequired,
                icon: React.PropTypes.string,
                added: React.PropTypes.bool,
                removed: React.PropTypes.bool
            })).isRequired
        },
        render() {
            if (!this.props.teams.length) {
                return null;
            }

            var showChanged = this.props.showChanged;
            var teams = this.props.teams.map(team => {
                var className = 'tau-icon tau-icon_type_svg tau-icon_name_' + team.icon;
                var teamClassName = 'ui-access-change-confirm__list_item';
                if (showChanged) {
                    if (team.added) {
                        teamClassName += ' tau-access-change-confirm_added';
                    }
                    if (team.removed) {
                        teamClassName += ' tau-access-change-confirm_removed';
                    }
                }

                return (
                    <li key={team.id} className={teamClassName}>
                        <i className={className}></i>
                        <span className="tau-access-change-confirm__name">{team.name}</span>
                    </li>
                );
            });

            return (
                <div className="tau-access-change-confirm__custom">
                    <div className="ui-access-change-confirm__title">Teams</div>
                    <ul className="ui-access-change-confirm__list">
                    {teams}
                    </ul>
                </div>
            );
        }
    })
});