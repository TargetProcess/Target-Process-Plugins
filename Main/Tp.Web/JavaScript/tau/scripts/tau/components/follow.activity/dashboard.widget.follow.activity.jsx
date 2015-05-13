define(function(require) {
    var React = require('react');
    var InfinityScrollList = require('jsx!tau/components/dashboard/infinity.scroll.list');
    var TemplateHelper = require('jsx!./template.helper');
    var StatusView = require('jsx!tau/components/dashboard/widget.templates/shared/status.view');

    return React.createClass({
        componentWillMount: function() {
            this._templateHelper = new TemplateHelper(this.props.configurator);
        },

        componentWillReceiveProps: function(newProps) {
            newProps.acid && this.refs.list.reset();
        },

        _loadMoreModifications: function(page, pageSize) {
            var data = {
                userId: this.props.configurator.getLoggedUser().id,
                to: this._getFirstLoadDate(),
                skip: (page - 1) * pageSize,
                take: pageSize
            };

            return this.props.service.loadModifications(this.props.acid, data);
        },

        _loadNewModifications: function() {
            var data = {
                userId: this.props.configurator.getLoggedUser().id,
                from: this._getFirstModificationDate()
            };

            return this.props.service.loadModifications(this.props.acid, data);
        },

        _getFirstLoadDate: function() {
            if (!this._firstLoadDate) {
                this._firstLoadDate = new Date();
            }

            return this._firstLoadDate;
        },

        _getFirstModificationDate: function() {
            var items = this.refs.list.state.items;
            return items.length > 0 ? items[0].timestamp : this._getFirstLoadDate();
        },

        _createModificationItem: function(modification, modificationIndex, modifications) {
            var hideHeader = modificationIndex > 0 && modification.general.id === modifications[modificationIndex - 1].general.id;

            return this._templateHelper.getTemplate(modification, hideHeader);
        },

        _getEmptyMessage: function() {
            return (
                <StatusView textClassName="tau-dashboard-widget-placeholder-text--follow">
                    <span>There is nothing to show.&nbsp;
                        <a href="https://guide.targetprocess.com/working-with-entities/follow-entity.html" target="_blank">Follow</a>
                    &nbsp;an entity to see changes!</span>
                </StatusView>
            );
        },

        render: function() {
            return (
                <InfinityScrollList
                    ref="list"
                    className="notification__body"
                    pageSize={100}
                    pageLimit={100}
                    loadMore={this._loadMoreModifications}
                    loadNew={this._loadNewModifications}
                    loadNewInterval={600}
                    renderItem={this._createModificationItem}
                    autoRefresh={true}
                    emptyMessage={this._getEmptyMessage()}
                />
            );
        }
    });
});
