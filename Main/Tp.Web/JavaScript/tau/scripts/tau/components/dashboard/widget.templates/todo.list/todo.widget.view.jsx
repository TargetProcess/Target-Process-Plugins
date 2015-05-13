define(function(require) {
    var _ = require('Underscore');
    var React = require('react');
    var WidgetConstants = require('./todo.widget.constants');
    var OpenCardViewMixin = require('./todo.widget.open.card.view.mixin');
    var StatusView = require('jsx!../shared/status.view');
    var UnitInteractionMixin = require('jsx!./todo.widget.unit.interaction.mixin');

    return React.createClass({
        displayName: 'ToDo Widget',

        mixins: [OpenCardViewMixin, UnitInteractionMixin],

        getInitialState() {
            return {
                isLoaded: false,
                hasMore: false,
                cards: []
            };
        },

        componentWillMount() {
            this.props.cardsRetrieving.on(this._onCardsRetrieving, this);
            this.props.cardsRetrieved.on(this._onCardsRetrieved, this);
        },

        componentWillUnmount() {
            this.props.cardsRetrieving.remove(this);
            this.props.cardsRetrieved.remove(this);
        },

        _onCardsRetrieving() {
            if (this.isMounted()) {
                this.setState({
                    isLoaded: false
                });
            }
        },

        _onCardsRetrieved(data) {
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

        _hasMoreMessage() {
            return this.state.hasMore ?
                <div className="empty-message">Only the first {this.state.cards.length} cards are shown</div> :
                null;
        },

        _getStylesForItem(card) {
            return this.props.highlighter.shouldHighlightCard(card, this.state.highlight) ?
            {backgroundColor: WidgetConstants.HIGHLIGHT_COLOR} :
            {};
        },

        render() {
            if (!this.state.isLoaded) {
                return <div className="tau-dashboard-widget-placeholder tau-loading--centered" />;
            }

            var list = _.map(this.state.cards, card => {
                var markup = this.props.renderCardAsText(card, this.state.layouts[card.type.toLowerCase()]);
                return <div key={card.data.cardData.id} className="tau-dashboard-widget-todo-list__item"
                    dangerouslySetInnerHTML={{__html: markup}} style={this._getStylesForItem(card)}></div>;
            }, this);

            if (list.length) {
                return <div className="tau-dashboard-widget-todo-list">
                {list}
                {this._hasMoreMessage()}
                </div>;
            }

            return <StatusView
                containerClassName="tau-dashboard-widget-placeholder--success"
                textClassName="tau-dashboard-widget-placeholder-text--success">
                <span>You have no assigned work. Get some rest!</span>
            </StatusView>;
        }
    });
});
