define(function(require) {
    var React = require('react');
    var $ = require('jQuery');

    var FeaturesService = require('tau/services/service.features');

    var featuresService = new FeaturesService();

    /**
     * @class DashboardWidgetSettingsContainerView
     * @extends ReactComponent
     */
    return React.createClass({
        displayName: 'WidgetSettingsContainer',

        mixins: [React.addons.LinkedStateMixin],

        getInitialState: function() {
            var sizes = this.props.initialSizes;
            var aspectRatio = sizes.aspectRatio;
            if (aspectRatio === '') {
                aspectRatio = 1;
            }
            return {
                widgetAspectRatio: aspectRatio,
                widgetHeight: sizes.height
            };
        },

        componentDidMount: function() {
            this.refs.contentPlaceholder.getDOMNode().appendChild(this.props.settingsContent);
        },

        _onApplyClicked: function() {
            this.props.applySettings(this);
        },

        _onKeyDown: function(e) {
            if (e.keyCode === $.ui.keyCode.ENTER) {
                this.props.isEditable && this._onApplyClicked();
                e.preventDefault();
            }
        },

        render: function() {
            return (
                <div className="i-role-widget-settings tau-widget-settings">
                    <section
                    className="i-role-widget-settings-content tau-widget-settings__content"
                    onKeyDown={this._onKeyDown}
                    ref="contentPlaceholder">
                    </section>
                    {this._renderSizeControls()}
                    {this._renderGeneralControls()}
                </div>
                );
        },

        _renderGeneralControls: function() {
            if (!this.props.isEditable) {
                return <div className="disable-cover"></div>;
            }

            return (
                <section className="tau-widget-settings__controls">
                    <button
                    className="i-role-widget-apply-changes tau-btn tau-primary"
                    onClick={this._onApplyClicked}>
                    Save
                    </button>
                </section>
                );
        },

        /**
         * Intended for internal debug and testing usage only, should be hidden or removed for public release.
         */
        _renderSizeControls: function() {
            if (!this.props.isEditable || !featuresService.isEnabled('dashboard.widget.size.controls')) {
                return null;
            }

            var control = this.state.widgetAspectRatio ?
                <label>
                    <span className="tau-widget-settings-list__title">Aspect ratio</span>
                    <input
                        type="number"
                        min="0.1"
                        step="0.1"
                        valueLink={this.linkState("widgetAspectRatio")}/>
                </label> :
                <label>
                    <span className="tau-widget-settings-list__title">Height</span>
                    <input
                        className="i-role-dashboard-sample-widget-settings-content-color"
                        type="number"
                        min="10"
                        step="10"
                        valueLink={this.linkState("widgetHeight")}/>
                </label>;

            return (
                <section
                    onKeyDown={this._onKeyDown}>
                    <ul className="tau-widget-settings-list">
                        <li className="tau-widget-settings-list__item">
                        {control}
                        </li>
                    </ul>
                </section>
                );
        }
    });
});
