define(function(require) {
    var React = require('libs/react/react-ex');
    var TeamIcon = require('jsx!./teamIcon.view');
    var classNames = require('libs/classNames');

    var StateSelectorHeader = React.createClass({
        displayName: 'StateSelectorHeader',
        propTypes: {
            onClick: React.PropTypes.func.isRequired,
            onMouseEnter: React.PropTypes.func,
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
                        <span className="i-role-name">{teamState.state.name}</span> for&nbsp;
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
            var triggerClasses = classNames({
                'multiple-team-state-select-trigger': true,
                'tau-icon-loading': this.props.isLoading
            });
            return (
                <div
                    className="multiple-team-state-select__item"
                    onClick={this.props.isLoading ? null : this.props.onClick}
                    onMouseEnter={this.props.isLoading ? null : this.props.onMouseEnter}
                >
                    <div className="multiple-team-state-select__item-content">
                        <div className="multiple-team-state-select-state">{this.props.parentState.name}</div>
                        {this._getTeamStateInfo(this.props.teamState)}
                        <div className={triggerClasses}></div>
                    </div>
                </div>
            );
        }
    });
    return StateSelectorHeader;
});
