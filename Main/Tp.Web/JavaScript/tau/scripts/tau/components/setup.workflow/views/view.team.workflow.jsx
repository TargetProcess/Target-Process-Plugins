define(function(require) {
    var $ = require('jQuery'),
        React = require('libs/react/react-ex'),
        ButtonView = require('jsx!./button');

    var TeamStateView = React.createClass({
        render: function() {
            var style = {
                'left': this.props.teamState.left,
                'width': this.props.teamState.width
            };
            return (
                <div className="process-grid__custom-process__item" style={style}>
                    <div className="process-grid__state">
                        <span className="tau-in-text">{this.props.teamState.name}</span>
                    </div>
                </div>
            );
        }
    });

    return React.createClass({

        componentDidMount: function() {
            var $target = $(this.refs.deleteButton.getDOMNode());
            $target.tauConfirm({
                content: '<h3>Do you really want to delete this Team workflow?</h3>',
                callbacks: {
                    success: function() {
                        this.props.deleteTeamWorkflowAction(this.props.teamWorkflow);
                    }.bind(this)
                },
                zIndex: 1000
            });
        },

        render: function() {

            var teamStates = _.map(this.props.teamWorkflow.entityStates, function(teamState) {
                return <TeamStateView key={teamState.id} teamState={teamState} />;
            });

            var customProcessClasses = [
                'process-grid__custom-process',
                'tau-entity-background--' + this.props.entityType,
                'tau-entity-color--' + this.props.entityType
            ].join(' ');

            return (
                <div className={customProcessClasses} style={this.props.teamWorkflow.position}>
                    <div className="process-grid__custom-process__title">
                        <div className="process-grid__custom-process__name">{this.props.teamWorkflow.name}</div>
                        <span ref="deleteButton" className="tau-remove-team-workflow i-role-remove-team-workflow" title="Delete Team Workflow" />
                    </div>
                    <div className="process-grid__custom-process__body">
                        {teamStates}
                    </div>
                </div>);
        }
    });
});