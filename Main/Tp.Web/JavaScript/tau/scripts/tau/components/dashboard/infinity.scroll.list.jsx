define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var InfinityScroll = require('libs/react/react-infinity-scroll');

    return React.createClass({
        propTypes: {
            pageSize: React.PropTypes.number.isRequired,
            pageLimit: React.PropTypes.number,
            loadMore: React.PropTypes.func.isRequired,
            loadNew: React.PropTypes.func,
            loadNewInterval: React.PropTypes.number,
            renderItem: React.PropTypes.func.isRequired,
            autoRefresh: React.PropTypes.bool
        },

        getInitialState: function() {
            return {
                hasMore: true,
                items: [],
                hasError: false
            };
        },

        componentDidMount: function() {
            if (this.props.autoRefresh) {
                this._autoRefreshIntervalId = setInterval(this.forceUpdate.bind(this), 1000 * 60);
            }

            this._loadNewIntervalId = setInterval(this.loadNew, 1000 * (this.props.loadNewInterval || 60));
        },

        componentWillUnmount: function() {
            clearInterval(this._autoRefreshIntervalId);
            clearInterval(this._loadNewIntervalId);
        },

        reset: function() {
            this.refs.scroll.reset();
            this.setState(this.getInitialState());
        },

        _getLoader: function() {
            return this.props.loader || <div className="tau-dashboard-widget-placeholder tau-loading--centered" />;
        },

        _getEmptyMessage: function() {
            return this.props.emptyMessage || <div className="empty-message">No items to show</div>;
        },

        _getErrorMessage: function() {
            return <div className="empty-message">Sorry, something went wrong :(</div>;
        },

        _getHasMore: function(items, page) {
            return items.length === this.props.pageSize && (!this.props.pageLimit || page < this.props.pageLimit);
        },

        loadMore: function(page) {
            this.isMounted() && this.props.loadMore(page, this.props.pageSize)
                .then(function(newItems) {
                    this.isMounted() && this.setState({
                        hasMore: this._getHasMore(newItems, page),
                        items: this.state.items.concat(newItems)
                    });
                }.bind(this))
                .fail(function(){
                    this.isMounted() && this.setState({
                        hasMore: false,
                        hasError: true
                    });
                }.bind(this));
        },

        loadNew: function() {
            this.props.loadNew && this.props.loadNew().then(function(newItems) {
                this.isMounted() && newItems.length && this.setState({
                    items: newItems.concat(this.state.items)
                });
            }.bind(this));
        },

        render: function() {
            var renderedItems = _.map(this.state.items, this.props.renderItem);

            return (
                <div style={{maxHeight: this.props.maxHeight || '500px', overflowY: 'auto'}} className={this.props.className || ''}>
                    <InfinityScroll
                    ref="scroll"
                    loader={this._getLoader()}
                    loadMore={this.loadMore}
                    hasMore={this.state.hasMore}>
                    {renderedItems}
                    {this.state.hasError ? this._getErrorMessage() : null}
                    {this.state.items.length === 0 && !this.state.hasMore && !this.state.hasError ? this._getEmptyMessage() : null}
                    </InfinityScroll>
                </div>
                );
        }
    });
});