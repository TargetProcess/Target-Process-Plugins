define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'board.access.change.confirm.details.projects.view',
        propTypes: {
            showChanged: React.PropTypes.bool,
            projects: React.PropTypes.arrayOf(React.PropTypes.shape({
                id: React.PropTypes.number.isRequired,
                name: React.PropTypes.string.isRequired,
                color: React.PropTypes.string,
                added: React.PropTypes.bool,
                removed: React.PropTypes.bool
            })).isRequired
        },
        render() {
            if (!this.props.projects.length) {
                return null;
            }

            var showChanged = this.props.showChanged;
            var projects = this.props.projects.map(project => {
                var colorStyle = {backgroundColor: project.color};
                var className = 'ui-access-change-confirm__list_item';

                if (showChanged) {
                    if (project.added) {
                        className += ' tau-access-change-confirm_added';
                    }
                    if (project.removed) {
                        className += ' tau-access-change-confirm_removed';
                    }
                }

                return (
                    <li key={project.id} className={className}>
                        <i className="tau-icon tau-icon_type_color"
                            style={colorStyle}></i>
                        <span className="tau-access-change-confirm__name">{project.name}</span>
                    </li>
                );
            });

            return (
                <div className="tau-access-change-confirm__custom">
                    <div className="ui-access-change-confirm__title">Projects</div>
                    <ul className="ui-access-change-confirm__list">
                    {projects}
                    </ul>
                </div>
            );
        }
    })
});