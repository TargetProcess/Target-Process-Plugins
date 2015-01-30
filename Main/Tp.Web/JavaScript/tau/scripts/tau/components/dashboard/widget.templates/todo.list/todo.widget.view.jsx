define(function(require) {
    var React = require('react');
    var WidgetConstants = require('./todo.widget.constants');
    var OpenCardViewMixin = require('./todo.widget.open.card.view.mixin');

    return React.createClass({
        displayName: 'ToDo Widget',

        mixins: [OpenCardViewMixin],

        getInitialState: function() {
            return {
                isLoaded: false,
                hasMore: false,
                cards: []
            };
        },

        componentWillMount: function() {
            this.props.cardsRetrieved.on(this._onCardsRetrieved, this);
        },

        componentWillUnmount: function() {
            this.props.cardsRetrieved.remove(this);
        },

        _onCardsRetrieved: function(data) {
            if (this.isMounted()) {
                this.setState({
                    isLoaded: true,
                    cards: data.cards,
                    hasMore: data.hasMore,
                    layouts: data.layouts,
                    highlight: data.highlight
                });
            }
        },

        _emptyMessage: function() {
            return <div className="empty-message">
            You have no assigned work
                <br />
            Get some rest
            </div>;
        },

        _hasMoreMessage: function() {
            return this.state.hasMore ?
                <div className="empty-message">Only the first {this.state.cards.length} cards are shown</div> :
                null;
        },

        _getStylesForItem: function(card) {
            return this.props.highlighter.shouldHighlightCard(card, this.state.highlight) ?
            {backgroundColor: WidgetConstants.HIGHLIGHT_COLOR} :
            {};
        },

        render: function() {
            if (!this.state.isLoaded) {
                return <div className="empty-message">Loading data...</div>;
            }

            var list = _.map(this.state.cards, function(card) {
                var markup = this.props.renderCardAsText(card, this.state.layouts[card.type.toLowerCase()]);
                return <div key={card.data.cardData.id} className="tau-dashboard-widget-todo-list__item"
                    dangerouslySetInnerHTML={{__html: markup}} style={this._getStylesForItem(card)}></div>;
            }, this);

            return <div className="tau-dashboard-widget-todo-list">
                {list.length ? list : this._emptyMessage()}
                {this._hasMoreMessage()}
            </div>;
        }
    });
});
