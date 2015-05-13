define(function(require) {
    var React = require('libs/react/react-ex');
    var $ = require('jQuery');

    var TooltipArticle = React.createClass({
        displayName: 'TooltipArticle',
        propTypes: {
            articleId: React.PropTypes.string.isRequired,
            dataAttributes: React.PropTypes.object
        },

        componentDidUpdate: function() {
            var $node = $(this.getDOMNode());
            var tauBubbleArticle = $node.data('ui-tauBubbleArticle');
            if (tauBubbleArticle) {
                tauBubbleArticle.destroy();
                // We need to clear jquery data cache if data attributes have changed
                $node.removeData();
            }
        },

        render: function() {
            return (
                <span
                    className="tau-help i-role-tooltipArticle"
                    data-article-id={this.props.articleId}
                    {...this.props.dataAttributes}
                />
            );
        }
    });

    return TooltipArticle;
});