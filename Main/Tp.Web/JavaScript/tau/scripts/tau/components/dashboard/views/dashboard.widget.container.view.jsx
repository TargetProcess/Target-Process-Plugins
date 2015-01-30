define(function(require) {
    var React = require('libs/react/react-ex');
    var $ = require('jQuery');
    var Name = require('jsx!./dashboard.widget.name.view');
    var SettingsTrigger = require('jsx!./dashboard.widget.settings.trigger');

    var settingsUtils = require('./dashboard.widget.settings.view.utils');

    /**
     * @class DashboardWidgetContainerView
     * @extends ReactComponent
     */
    var DashboardWidgetContainerView = React.createClass({
        displayName: 'WidgetContainer',
        statics: {
            loadingTemplateMessage: 'Loading widget template...',
            loadingWidgetMessage: 'Loading widget...'
        },

        getInitialState: function() {
            return {
                isBeingRenamed: false,
                isLoaded: false,
                statusText: DashboardWidgetContainerView.loadingTemplateMessage
            };
        },

        componentDidMount: function() {
            this.props.widget.inserting.add(this._widgetInserting, this);
            this.props.widget.inserted.add(this._widgetInserted, this);

            if (!this.props.widget.getIsWidgetLoaded()) {
                this.props.widget.templateLoaded.once(this._templateLoaded, this);
                return;
            }

            this._insertWidgetContent();
        },

        componentWillUnmount: function() {
            this.props.widget.templateLoaded.remove(this);
            this.props.widget.inserting.remove(this);
            this.props.widget.inserted.remove(this);
            this.props.widget.remove();
        },

        _templateLoaded: function() {
            this._insertWidgetContent();
        },

        _insertWidgetContent: function() {
            if (!this.isMounted()) {
                return;
            }

            this.props.widget.insertIntoNewContainer();
        },

        _widgetInserting: function(args) {
            if (!this.isMounted()) {
                return;
            }

            var $container = $(this._findPlaceholder());
            $container.empty();
            $container.append(args.placeholder);

            this.setState({
                statusText: DashboardWidgetContainerView.loadingWidgetMessage,
                isLoaded: false
            });
        },

        _widgetInserted: function(args) {
            if (!this.isMounted()) {
                return;
            }

            this.setState({
                statusText: '',
                isLoaded: true
            });
        },

        _findPlaceholder: function() {
            return this.refs.contentPlaceholder.getDOMNode();
        },

        _removeWidget: function() {
            this.props.column.removeWidget(this.props.widget);
        },

        _onDeleteClicked: function(e) {
            var $deleteBtn = $(this.getDOMNode()).find('.i-role-dashboard-delete-widget');
            if (!$deleteBtn.tauConfirm('instance')) {
                var $container = $deleteBtn.closest('.i-role-dashboard-layout');
                $deleteBtn.tauConfirm({
                    appendTo: $container,
                    hideOnScrollContainer: $deleteBtn.scrollParent(),
                    content: '<h3>Do you really want to delete this widget?</h3>',
                    callbacks: {
                        success: this._removeWidget
                    },
                    className: 'tau-delete-widget-bubble',
                    zIndex: 130,
                    onPositionConfig: function(config) {
                        config.my = 'center top';
                        config.at = 'center bottom';
                    },
                    onShow: function() {
                        $deleteBtn.addClass('active');
                    },
                    onHide: function() {
                        $deleteBtn.removeClass('active');
                        $deleteBtn.tauConfirm('destroy');
                    }
                });
            }

            $deleteBtn.tauConfirm('show');
        },

        _onInsertSettings: function($trigger, toggleActive) {
            return settingsUtils
                .toggleWidgetSettings(this.props.widget, $trigger, this.props.isEditable, toggleActive)
                .then(function(settingsDescriptor) {
                    this.settingsDescriptor = settingsDescriptor;
                }.bind(this));
        },

        _onNameEditStart: function() {
            this.setState({isBeingRenamed: true});
        },

        _onNameEditDone: function(name) {
            this.props.widget.setName(name);
            this.setState({isBeingRenamed: false});
        },

        _onNameEditCancel: function() {
            this.setState({isBeingRenamed: false});
        },

        _constructContentWrapperStyle: function(sizes) {
            var outerStyle = {
                minHeight: sizes.minHeight,
                maxHeight: sizes.maxHeight
            };

            if (sizes.aspectRatio) {
                outerStyle.paddingTop = (100 / sizes.aspectRatio) + '%';
            } else {
                // Either AR or fixed height technique should be used, not both.
                outerStyle.height = sizes.height;
            }

            if (outerStyle.minHeight || outerStyle.maxHeight || outerStyle.height) {
                outerStyle.overflowY = 'auto';
            }

            return outerStyle;
        },

        _constructContentWrapperClassName: function(sizes) {
            return React.addons.classSet({
                'tau-dashboard-widget-content': true,
                'tau-dashboard-widget-content--ratio': !!sizes.aspectRatio, // stretch content when widget has fixed AR
                'tau-dashboard-widget-content--fill': !!sizes.height // stretch content when widget has fixed height
            });
        },

        render: function() {
            var isDraggable = this.state.isLoaded && this.props.isEditable && !this.state.isBeingRenamed;

            return (
                <div className="i-role-dashboard-widget tau-dashboard-widget"
                    draggable={isDraggable}
                    data-sortable-key={this.props.id}>
                    {this._renderHeader()}
                    {this._renderContent()}
                    <div className="tau-dashboard-widget__drop-placeholder"></div>
                </div>
            );
        },

        _renderHeader: function() {
            var settingsButton, deleteButton;

            if (this.state.isLoaded && this.props.widget.getHasSettings()) {
                settingsButton = <SettingsTrigger insertSettings={this._onInsertSettings} />;
            }

            if (this.props.isEditable) {
                deleteButton =
                    <button className="i-role-dashboard-delete-widget tau-dashboard-widget__delete"
                        onClick={this._onDeleteClicked}></button>;
            }

            var canRename = this.state.isLoaded && this.props.isEditable;

            return (
                <div className="tau-dashboard-widget-header textSelectionDisabled">
                    <Name name={this.props.widget.getName()}
                        isEditable={canRename} contentEditable={this.state.isBeingRenamed}
                        onEditStart={this._onNameEditStart} onEditDone={this._onNameEditDone}
                        onEditCancel={this._onNameEditCancel} />
                    <div className="tau-dashboard-widget__buttons">
                        {settingsButton}
                        {deleteButton}
                    </div>
                </div>
            );
        },

        _renderContent: function() {
            var sizes = this.props.widget.getSizes();

            var status = this.state.isLoaded ?
                null :
                <div className="tau-dashboard-widget-status">{this.state.statusText}</div>;

            var contentWrapperClassName = React.addons.classSet({
                'tau-dashboard-widget-content-wrapper': true,
                'i-role-dashboard-widget-content': true,
                'tau-dashboard-widget-content-wrapper--loading': !this.state.isLoaded
            });

            return (
                <div className={this._constructContentWrapperClassName(sizes)}
                    style={this._constructContentWrapperStyle(sizes)}>
                    {status}
                    <div
                        ref="contentPlaceholder"
                        className={contentWrapperClassName}/>
                </div>
            );
        }
    });

    return DashboardWidgetContainerView;
});
