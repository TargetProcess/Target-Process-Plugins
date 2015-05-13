define(function(require) {
    var React = require('libs/react/react-ex'),
        classNames = require('libs/classNames'),
        SettingsService = require('../services/state.item.process.settings.service'),
        SortableItem = require('tau/components/react/mixins/sortable.item');

    return React.createClass({
        getInitialState: function() {
            return {
                name: this.props.name,
                isRenaming: false,
                settingsService: this._getSettingsService()
            };
        },

        componentDidMount: function() {
            if (this.props.isNew) {
                this._toggleSettings();
            }
        },

        componentWillReceiveProps: function(nextProps) {
            if (!nextProps.isLoading) {
                this.setState({
                    name: nextProps.name
                });
            }
        },

        _getSettingsService: function() {
            var result = new SettingsService(
                function() {
                    return this.refs.settings.getDOMNode();
                }.bind(this),
                function() {
                    return this.refs.namePlaceholder.getDOMNode();
                }.bind(this),
                this.props.stateSettings);

            result.onToggleRename.add(this._toggleRename, this);
            result.onRename.add(this._rename, this);
            result.onRename.add(function(newName) {
                this.setState({name: newName});
            }, this);

            return result;
        },

        _toggleRename: function(isShown) {
            this.setState({isRenaming: isShown, containerWidth: this.refs.container.getDOMNode().offsetWidth});
        },

        _toggleSettings: function() {
            this.state.settingsService.toggleSettings(this.props.id);
        },

        _beginNameEdit: function() {
            this.state.settingsService.showNameEditor(false);
        },

        _rename: function(newName, options) {
            if (!options.isManaged) {
                this.props.renameStateAction({
                    id: this.props.id,
                    name: newName
                });
            }
        },

        _onMouseMove: function(e) {
            var stateElement = e.currentTarget;
            var elementRect = stateElement.getBoundingClientRect();
            var x = e.pageX - elementRect.left;
            if (x < elementRect.width / 2) {
                this.props.leftTriggered();
            } else {
                this.props.rightTriggered();
            }
        },

        _onMouseOut: function() {
            this.props.outTriggered();
        },

        componentWillUnmount: function() {
            this.state.settingsService.dispose();
        },

        render: function() {
            var gridItemClasses = classNames({
                'process-grid__item': true,
                'active': this.state.isRenaming
            });

            var gridItemStyle = this.state.isRenaming ? {
                'minWidth': this.state.containerWidth
            } : {'width': this.props.width};

            var isSortable = this.props.isSortable && !this.state.isRenaming;
            var isDraggable = isSortable && this.props.allowDrag;
            var isRenameable = this.props.isEnabled && !this.state.isRenaming;
            var gridStateClasses = classNames({
                'process-grid__state': true,
                'tau-draggable': isDraggable,
                'tau-clickable': !isDraggable && isRenameable,
                'edit': this.state.isRenaming
            });

            var actions = {
                toggleSettings: this.props.isEnabled ? this._toggleSettings : null,
                beginEdit: isRenameable ? this._beginNameEdit : null
            };

            var showSettings = this.props.isEnabled;
            var showLoader = this.props.isLoading;
            var settingsClasses = classNames({
                'tau-icon-general': showSettings || showLoader,
                'tau-icon-settings-small-dark': showSettings,
                'tau-icon-loading': showLoader
            });

            var title = isSortable ?
                (isDraggable ? '' : 'The order of the states mapped to the initial and final states of Team Workflows can\'t be changed.') :
                'The order of the initial and final states can\'t be changed.';

            return (
                <div ref="container" className={gridItemClasses} style={gridItemStyle}
                    {...SortableItem.attributes(this.props.id, SortableItem.PROCESS_ITEM_STATE, isDraggable)}
                    data-sortable={isSortable}>
                    <div className={gridStateClasses} title={title}
                        onMouseMove={this._onMouseMove} onMouseOut={this._onMouseOut}>
                        <div ref="namePlaceholder" className="tau-in-text" onDoubleClick={actions.beginEdit}>{this.state.name}</div>
                        <span ref="settings" title="Change state settings" className={settingsClasses} onClick={actions.toggleSettings} />
                    </div>
                </div>
            );
        }
    });
});
