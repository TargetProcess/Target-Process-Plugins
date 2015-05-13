define(function(require) {
    var React = require('react');
    var _ = require('Underscore');
    var ProjectsDetails = require('jsx!./board.access.change.confirm.details.projects.view');
    var TeamsDetails = require('jsx!./board.access.change.confirm.details.teams.view');

    return React.createClass({
        displayName: 'board.access.change.confirm.details.view',
        propTypes: {
            showChanged: React.PropTypes.bool,
            model: React.PropTypes.shape({
                projects: React.PropTypes.arrayOf(React.PropTypes.object).isRequired,
                teams: React.PropTypes.arrayOf(React.PropTypes.object).isRequired
            })
        },
        getDefaultProps() {
            return {
                showChanged: false
            };
        },
        render() {
            var isEmpty = !this.props.model.projects.length && !this.props.model.teams.length;

            if (isEmpty) {
                return (
                    <div>There are no Teams nor Projects.</div>
                );
            }

            return (
                <div>
                    <ProjectsDetails projects={this.props.model.projects} showChanged={this.props.showChanged} />
                    <TeamsDetails teams={this.props.model.teams} showChanged={this.props.showChanged} />
                </div>
            );
        }
    })
});