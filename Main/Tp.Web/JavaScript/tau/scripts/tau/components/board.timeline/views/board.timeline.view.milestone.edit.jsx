define(function(require) {
    var React = require('react'),
        _ = require('Underscore'),
        FlagView = require('jsx!tau/components/board.timeline/views/board.timeline.view.milestone.details.flag'),
        ColorEditorView = require('jsx!tau/components/board.timeline/views/board.timeline.view.milestone.colors'),
        ProjectEditorView = require('jsx!tau/components/board.timeline/views/board.timeline.view.milestone.projects.editor'),
        dateUtils = require('tau/utils/utils.date'),
        EditButtonsView = require('jsx!tau/components/board.timeline/views/board.timeline.view.milestone.edit.buttons'),
        AddButtonsView = require('jsx!tau/components/board.timeline/views/board.timeline.view.milestone.add.buttons');

    return React.createClass({
        displayName: 'TimeLineMilestoneDetailsView',

        propTypes: {
            name: React.PropTypes.string.isRequired,
            color: React.PropTypes.string.isRequired,
            date: React.PropTypes.string.isRequired,
            projects: React.PropTypes.array,
            availableProjects: React.PropTypes.array,
            onResetActiveMilestone: React.PropTypes.func.isRequired
        },

        getInitialState() {
            var projectIds = _.pluck(this.props.projects, 'id');
            return _.defaults(this.props.editState, {
                id: this.props.id,
                date: this.props.date,
                description: this.props.description,
                name: this.props.name,
                selectedProjectIds: _.filter(_.pluck(this.props.availableProjects, 'id'),
                        id =>
                    projectIds.indexOf(id) >= 0
                ),
                color: this.props.color,
                animateValidationClassName: '',
                validated: false,
                datepicker: false
            });
        },

        componentDidMount() {
            $.datepicker.parseDate = function(format, value) {
                return dateUtils.parse(value);
            };

            $.datepicker.formatDate = function(format, date) {
                return dateUtils.formatAs(date, format);
            };

            var $datepickerDOMNode = $(this.refs.datepicker.getDOMNode());
            $datepickerDOMNode.datepicker({
                    onSelect: dateText => {
                        this._handleChangeDate(dateText);
                        this.setState({datepicker: false})
                    },
                    dateFormat: "d MMM yyyy",
                    defaultDate: this.state.date
                }
            );
        },

        _handleChangeDate(dateText) {
            $(this.refs.datepicker.getDOMNode()).datepicker('option', 'defaultDate', dateText);
            this.setState({date: dateText});
        },

        _handleChangeDateText(e) {
            this.setState({date: e.target.value});
        },

        componentWillUpdate: function(nextProps, nextState) {
            this.props.onEditStateChanged(nextState);
        },

        _handleNameChange(e) {
            this.setState({name: e.target.value});
        },

        _handleDescriptionChange(e) {
            this.setState({description: e.target.value});
        },

        _handleSave(action) {
            if (this.isValid()) {
                action(this._getMilestoneData());
            }
            else {
                this.setState({validated: true})
                this._animateValidation();
            }
        },

        _getMilestoneData() {
            return {
                name: this.state.name,
                description: this.state.description,
                color: this.state.color,
                date: this.refs.date.getDOMNode().value,
                projects: this.state.selectedProjectIds
            };
        },

        _handleColorSelect(color) {
            this.setState({color: color});
        },

        _handleProjectSelect(projectId, selected) {
            if (selected) {
                this.setState({selectedProjectIds: this.state.selectedProjectIds.concat([projectId])});
            }
            else {
                this.setState({selectedProjectIds: _.without(this.state.selectedProjectIds, projectId)});
            }
        },

        _handleSelectAll(e) {
            if (e.target.checked) {
                this.setState({selectedProjectIds: _.pluck(this.props.availableProjects, 'id')});
            }
            else {
                this.setState({selectedProjectIds: []});
            }
        },

        _animateValidation() {
            var animationClass = this.state.animateValidationClassName.indexOf('even') > 0 ? 'tau-checkbox-assign-milestone-shake-odd' : 'tau-checkbox-assign-milestone-shake-even';
            this.setState({animateValidationClassName: animationClass});
        },

        _isDateValid() {
            return this.refs.date ? !!dateUtils.parse(this.refs.date.getDOMNode().value) : true;
        },

        isValid() {
            return this._isNameValid() && this._isProjectsValid() && this._isDateValid();
        },

        _isNameValid() {
            return this.state.name.length > 0;
        },

        _isProjectsValid() {
            return this.state.selectedProjectIds.length > 0;
        },

        _toggleDatepicker() {
            this.setState({datepicker: !this.state.datepicker});
        },

        _getProjectStyle(project) {
            return {
                backgroundColor: project.color || null
            };
        },

        _buildClassNameFromArray(classNames) {
            return classNames.join(' ');
        },

        _getValidationClassNameFromElement(condition, errorCssClassName) {
            if (!this.state.validated) {
                return [];
            }
            return condition() ? [] : [errorCssClassName,
                this.state.animateValidationClassName];
        },

        renderButtons() {
            if (this.props.isNew) {
                return <AddButtonsView onEditStateChanged = {this.props.onEditStateChanged} onAdd = {this._handleSave.bind(this, this.props.onAddMilestone)} onCancel = {this.props.onResetActiveMilestone} />
            }
            else {
                return <EditButtonsView onEdit = {this._handleSave.bind(this, this.props.onSaveMilestone)} onDelete = {this.props.onDeleteMilestone} />;
            }
        },

        renderArrow() {
            if (this.props.arrow) {
                return (<div style={{
                    top: this.props.arrow.top + 'px',
                    left: this.props.arrow.left + 'px'
                }} data-orientation={this.props.arrow.orientation} role="arrow" className="tau-bubble__arrow"></div>)
            }
        },

        render() {
            return (
                <div>
                {this.renderArrow()}
                    <div style={{'maxHeight': this.props.maxHeight}} className="tau-timeline-milestone-popup-wrapper tau-custom-scrollbar">
                        <div className="tau-timeline-milestone-popup__edit">
                            <div className="tau-timeline-milestone-popup__edit__title">{`${this.props.isNew ? 'Add' : 'Edit'} milestone`}</div>
                            <span className="tau-inline-group">
                                <input ref="date" type="text" onChange={this._handleChangeDateText} className={this._buildClassNameFromArray(this._getValidationClassNameFromElement(this._isDateValid, 'tau-error').concat(['ui-dateeditor-source',
                                    'tau-in-text']))}  value={this.state.date}/>
                                <button onClick={this._toggleDatepicker}  className="tau-btn tau-date-range-options" type="button"></button>

                            </span>
                            <div className={this.state.datepicker ? '' : 'hidden'} ref ="datepicker"></div>
                            <div ref="name" placeholder="Milestone name" className={this._buildClassNameFromArray(this._getValidationClassNameFromElement(this._isNameValid, 'tau-error').concat(['tau-timeline-input-group']))}>
                                <input type="text" onChange={this._handleNameChange} className="tau-in-text i-role-milestone-title" placeholder="Milestone title" value={this.state.name}/>
                                <textarea onChange={this._handleDescriptionChange} className="tau-in-text tau-custom-scrollbar" value={this.state.description} placeholder="Milestone description"/>
                            </div>
                            <ColorEditorView color={this.state.color} onColorSelect = {this._handleColorSelect} />
                            <div className="tau-timeline-milestone-popup__edit__assign">
                                <div className="tau-timeline-milestone-popup__edit__assign__title">
                                    <label className="tau-checkbox">
                                        <input onClick={this._handleSelectAll} type="checkbox"/>
                                        <i className="tau-checkbox__icon"></i>
                                        <span ref='projects' className={this._buildClassNameFromArray(this._getValidationClassNameFromElement(this._isProjectsValid, 'tau-checkbox-assign-milestone-active').concat(['tau-checkbox-label']))}>Assign to projects</span>
                                    </label>
                                </div>
                                <ProjectEditorView availableProjects = {this.props.availableProjects} selectedProjectIds = {this.state.selectedProjectIds} onProjectSelect = {this._handleProjectSelect} />
                            </div>
                        </div>
                    {this.renderButtons()}
                        <FlagView hidden = {this.props.isNew} color = {this.props.color} onResetActiveMilestone = {this.props.onResetActiveMilestone} />
                    </div>
                </div>
            );
        }
    });
});
