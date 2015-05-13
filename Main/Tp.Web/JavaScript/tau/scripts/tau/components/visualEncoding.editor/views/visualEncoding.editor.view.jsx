define(function(require) {
    var React = require('libs/react/react-ex'),
        Item = require('jsx!./visualEncoding.editor.item.view'),
        Sortable = require('tau/components/react/mixins/sortable.axis.y'),
        SortableItem = require('tau/components/react/mixins/sortable.item'),
        Feedback = require('jsx!tau/components/feedback/views/feedback.view');
    return React.createClass({
        displayName: 'VisualEncoding',
        mixins: [Sortable],
        _renderItems() {
            return this.props.items.map(function(item) {
                return <Item key={item.UUID} {...item} model={this.props.model}/>;
            }, this);
        },
        getSortableOptions() {
            return {
                acceptedTypes: [SortableItem.VISUAL_ENCODING_ITEM],
                sortItems: (key, overKey, placeAfter)=> {
                    this.props.model.bus.fire('prioritizeRuleStop', {key, overKey, placeAfter});
                },
                sortableItemSelector: '.tau-color-encoding-list__item',
                sortableRootSelector: '.tau-color-encoding-list',
                sortableHandleSelector: '.ui-sortable-helper'
            };
        },

        _addNewRule() {
            this.props.model.bus.fire('addNewRule');
        },
        render() {
            var items = this._renderItems();
            var addNewRule = this.props.model.isEditable ?
                <div className="add-link" onClick={this._addNewRule}>
                    <span className="tau-icon-general tau-icon-green-plus-big"></span>
                    Add rule
                </div> :
                '';
            return (
                <div className="tau-board-settings tau-boardsettings tau-board-settings-visualEncoding" role="form-settings">
                    <Feedback feedbackClassName="tau-feedback_visual_encoding" featureName="Visual Encoding" buttonClass="tau-btn"/>
                    <h4>Highlight cards with different colors by custom rules</h4>
                    <span className="tau-help i-role-tooltipArticle" data-article-id="board.editor.visualEncoding"></span>
                    <div className="tau-section">
                        <ul className="tau-color-encoding-list" {...this.sortableTarget()}>
                            {items}
                        </ul>
                        {addNewRule}
                    </div>
                </div>
            )
        }
    });
});


