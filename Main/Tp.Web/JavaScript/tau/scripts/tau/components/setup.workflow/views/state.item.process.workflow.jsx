define(function(require) {
    var React = require('libs/react/react-ex'),
        SettingsService = require('../services/state.item.process.settings.menu.service');

    return React.createClass({
        getInitialState: function() {
            return {
                name: this.props.name,
                edit: false
            };
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
                this.refs.settings.getDOMNode(),
                this.refs.namePlaceholder.getDOMNode(),
                this.props.stateSettings);
            result.onToggleRename.add(function(isShown) {
                this.setState({ edit: isShown, containerWidth: this.refs.container.getDOMNode().offsetWidth });
            }, this);
            result.onRename.add(this._rename, this);
            return result;
        },

        _toggleSettings: function() {
            this._getSettingsService().toggleSettings(this.props.id);
        },

        _beginNameEdit: function() {
            this._getSettingsService().showNameEditor(false);
        },

        _rename: function(newName) {
            var nameChanged = newName !== this.state.name;
            if (nameChanged) {
                this.setState({name: newName});

                this.props.renameStateAction({
                    id: this.props.id,
                    name: newName
                });
            }
        },

        render: function() {
            var containerClasses = React.addons.classSet({
                'process-grid__item': true,
                'active': this.state.edit
            });

            var containerStyle = this.state.edit ? {
                'minWidth': this.state.containerWidth
            } : {};

            var innerClasses = React.addons.classSet({
                'process-grid__state': true,
                'edit': this.state.edit
            });

            var actions = {
                toggleSettings: this.props.isDisabled ? null : this._toggleSettings,
                beginEdit: this.props.isDisabled ? null : this._beginNameEdit
            };

            var showSettings = !this.props.isDisabled;
            var showLoader = this.props.isLoading;
            var settingsClasses = React.addons.classSet({
                'tau-icon-general': showSettings || showLoader,
                'tau-icon-settings-small-dark': showSettings,
                'tau-icon-loading': showLoader
            });

            return (
                <div ref="container" className={containerClasses} style={containerStyle}>
                    <div className={innerClasses}>
                        <div ref="namePlaceholder" className="tau-in-text" onDoubleClick={actions.beginEdit}>{this.state.name}</div>
                        <span ref="settings" className={settingsClasses} onClick={actions.toggleSettings} />
                    </div>
                </div>);
        }
    });
});