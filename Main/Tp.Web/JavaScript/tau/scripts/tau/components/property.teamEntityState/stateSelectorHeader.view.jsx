define(function(require) {
    var React = require('libs/react/react-ex');
    var TeamIcon = require('jsx!./teamIcon.view');

    var StateSelectorHeader = React.createClass({
        displayName: 'StateSelectorHeader',
        propTypes: {
            onClick: React.PropTypes.func.isRequired,
            isLoading: React.PropTypes.bool
        },
        _getTeamStateInfo: function() {
            var teamState = this.props.teamState;
            var parentState = this.props.parentState;
            if (!(teamState && teamState.state) || teamState.state.id === parentState.id) {
                return null;
            }
            return (
                <div className="multiple-team-state-select-team-state-info">
                    <div className="multiple-team-state-select-sub-state">
                        <span className="i-role-name">{teamState.state.name}</span> for
                    </div>
                    <div className="multiple-team-state-select-team">
                        <span className="tau-checkbox-label">
                            <TeamIcon name={teamState.team.icon}/>
                            {teamState.team.name}
                        </span>
                    </div>
                </div>
            );
        },
        render: function() {
            return (
                <div className="multiple-team-state-select__item" onClick={this.props.isLoading ? null : this.props.onClick}>
                    <div className="multiple-team-state-select__item-content">
                        <div className="multiple-team-state-select-state">{this.props.parentState.name}</div>
                        {this._getTeamStateInfo(this.props.teamState)}
                        <div className="multiple-team-state-select-trigger"></div>
                    </div>
                </div>
            );
        }
    });
    return StateSelectorHeader;
});
