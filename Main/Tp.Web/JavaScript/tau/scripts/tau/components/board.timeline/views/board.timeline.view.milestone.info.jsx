define(function(require) {
    var React = require('react'),
        FlagView = require('jsx!tau/components/board.timeline/views/board.timeline.view.milestone.details.flag');

    return React.createClass({
        displayName: 'TimeLineMilestoneDetailsView',

        _getProjectStyle(project) {
            return {
                backgroundColor: project.color || null
            };
        },

        _handleEditClick() {
            this.props.onToggleEditMode(true);
        },

        render() {
            var colorCssClass = `tau-timeline-milestone--${this.props.color}`;

            var projects = this.props.projects.map(project =>
                    <div key={project.id}  title={project.name} className="tau-board-unit__value_type_abbr" style={this._getProjectStyle(project)}>{project.abbreviation}</div>
            );

            return (
                <div className="tau-timeline-milestone-popup-wrapper">
                    <div className="tau-timeline-milestone-popup__info">
                        <div className={"tau-timeline-milestone-popup__info__title " + colorCssClass}>{this.props.name}</div>
                        <div className={"tau-timeline-milestone-popup__info__date " + colorCssClass}>{this.props.date}</div>
                        <div className="tau-timeline-milestone-popup__info__description">
                            {this.props.description}
                        </div>
                        {projects}
                        <button onClick={this._handleEditClick} className="tau-btn i-role-edit" type="button">Edit</button>
                    </div>
                    <FlagView hidden = {this.props.isNew} color = {this.props.color} onResetActiveMilestone = {this.props.onResetActiveMilestone} />
                </div>

            );
        }
    })
        ;
})
;
