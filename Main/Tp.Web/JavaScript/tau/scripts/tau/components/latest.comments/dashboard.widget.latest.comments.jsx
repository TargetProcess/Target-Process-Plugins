define(function(require) {
    var $ = require('jQuery');
    var React = require('react');
    var InfinityScrollList = require('jsx!tau/components/dashboard/infinity.scroll.list');

    return React.createClass({
        componentWillReceiveProps: function() {
            this.refs.list.reset();
            this._firstLoadCommentId = null;
        },

        _loadMore: function(page, pageSize) {
            return this.props.service.loadComments(this.props.acid, this.props.source, page, pageSize, null, this._getFirstLoadCommentId())
                .then(function(items) {
                    if (!this._firstLoadCommentId && items.length) {
                        this._firstLoadCommentId = items[0].id;
                    }

                    return items;
                }.bind(this));
        },

        _loadNew: function() {
            return this.props.service.loadComments(this.props.acid, this.props.source, 1, 999, this._getLatestCommentId());
        },

        _getFirstLoadCommentId: function() {
            return this._firstLoadCommentId || null;
        },

        _getLatestCommentId: function() {
            var items = this.refs.list.state.items;
            return items.length ? items[0].id : null;
        },

        _renderCommentItem: function(comment) {
            var entityTypeName = comment.general.entityType.name.toLowerCase();
            var entityIconClass = 'tau-text tau-text_type_' + entityTypeName;

            return (
                <div className="tau-dashboard-widget-comment" key={comment.id}>
                    <div className="tau-dashboard-widget-comment__header">
                        <a href={comment.owner.userUrl} target="_blank" onClick={this._openEntityView.bind(this, comment.owner.id, 'user')}>
                            <img className="tau-dashboard-widget-person" src={comment.owner.avatarUrl} title={comment.owner.fullName} />
                        </a>
                        <a className="tau-dashboard-widget-name" href={comment.owner.userUrl}
                            onClick={this._openEntityView.bind(this, comment.owner.id, 'user')}>{comment.owner.fullName}</a>
                        <div className="tau-dashboard-widget-date-time"
                            title={comment.date.full}>{comment.date.userFriendly}</div>
                        {comment.parentId ? <span className="tau-icon tau-icon_name_back-arrow"></span> : null}
                    </div>
                    <div className="tau-dashboard-widget-comment__body">{comment.description}</div>
                    <div className="tau-dashboard-widget-comment__footer">
                        <a href={comment.general.url} className={entityIconClass}
                            onClick={this._openEntityView.bind(this, comment.general.id, entityTypeName)}>
                            {comment.general.entityType.abbreviation + ' ' + comment.general.id}
                        </a>
                        <a href={comment.general.url} className={entityIconClass}
                            onClick={this._openEntityView.bind(this, comment.general.id, entityTypeName)}>
                            {comment.general.name}
                        </a>
                    </div>
                </div>
            );
        },

        _openEntityView: function(entityId, entityType, event) {
            event.preventDefault();
            event.stopPropagation();
            this.props.service.openEntityView(entityId, entityType);
        },

        render: function() {
            return (
                <InfinityScrollList
                    ref="list"
                    pageSize={10}
                    loadMore={this._loadMore}
                    loadNew={this._loadNew}
                    loadNewInterval={600}
                    renderItem={this._renderCommentItem}
                    autoRefresh={true}
                />
            );
        }
    });
});
