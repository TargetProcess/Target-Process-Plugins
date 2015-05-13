define(function(require) {
    var React = require('react');

    return React.createClass({
        displayName: 'TimeLineMilestoneProjectsEditor',

        _handleProjectSelect(e) {
            var projectId = parseInt(e.target.value, 10);
            this.props.onProjectSelect(projectId, e.target.checked);
        },

        _getProjectsList() {
            return this.props.availableProjects.map(project =>
                    <li key={project.id} className="i-role-item i-role-project i-role-team tau-category-settings__item">
                        <label data-title="" className="tau-checkbox tau-extension-board-tooltip ">
                            <input onChange={this._handleProjectSelect} type="checkbox" data-skip-disabled-check="undefined" value={project.id}  name="project"
                                checked={this.props.selectedProjectIds.indexOf(project.id) >= 0}/>
                            <i className="tau-checkbox__icon"></i>
                            <span className="tau-checkbox-label">
                                <i className="tau-icon" style={{backgroundColor: project.color || null}}></i>
                                <span>{project.name}</span>
                            </span>
                        </label>
                    </li>
            );
        },

        render() {
            var projects = this._getProjectsList();

            return (
                <div className="tau-projects">
                    <div className="tau-category-settings-container">
                        <section className="tau-category-settings">
                            <ul className="tau-category-items-selector i-role-list tau-custom-scrollbar">
                                {projects}
                            </ul>
                        </section>
                    </div>
                </div>
            );
        }
    });
});
