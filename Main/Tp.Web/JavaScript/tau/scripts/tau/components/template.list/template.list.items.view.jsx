define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var React = require('react');
    var Tag = require('jsx!./template.list.tag.view');
    var ScrollMixin = require('./template.list.scroll.mixin');

    return React.createClass({
        propTypes: {
            selectListItem: React.PropTypes.func.isRequired,
            emptyMessage: React.PropTypes.string,
            searchPlaceholder: React.PropTypes.string,
            ItemComponent: React.PropTypes.func.isRequired,
            items: React.PropTypes.array.isRequired,
            templateListClass: React.PropTypes.string
        },

        displayName: 'TemplateListItems',

        mixins: [ScrollMixin],

        getDefaultProps: function() {
            return {
                emptyMessage: 'No items found',
                searchPlaceholder: 'Search'
            };
        },

        getInitialState: function() {
            return {
                filterText: '',
                selectedTag: 'All'
            };
        },

        _onTagSelected: function(tagName) {
            this.setState({selectedTag: tagName});
        },

        _onFilterChanged: function(e) {
            this.setState({filterText: e.target.value});
        },

        _onFilterKeyDown: function(e) {
            if (e.keyCode === $.ui.keyCode.ESCAPE) {
                this.setState({filterText: ''});
            }
        },

        _filterItems: function() {
            var filterText = this.state.filterText.toLowerCase();
            var tag = this.state.selectedTag.toLowerCase();

            return _.filter(this.props.items, function(item) {
                return this._matchesTag(tag, item) && this._matchesFilter(filterText, item);
            }, this);
        },

        _matchesTag: function(tag, item) {
            if (!tag || tag === 'all') {
                return true;
            }

            var itemTags = _.map(item.tags, function(t) {
                return t.toLowerCase();
            });

            return itemTags.indexOf(tag) > -1;
        },

        _matchesFilter: function(filter, item) {
            if (!filter) {
                return true;
            }

            var itemName = (item.name || '').toLowerCase();
            var itemDescription = (item.description || '').toLowerCase();

            return itemName.indexOf(filter) > -1 || itemDescription.indexOf(filter) > -1;
        },

        _getEmptyMessage: function() {
            return <div className="tau-board-settings__template-list_empty-message">{this.props.emptyMessage}</div>;
        },

        _renderTags: function() {
            var selectedTag = this.state.selectedTag.toLowerCase();

            var tags = this.props.tags || this._collectTagsFromTemplates(this.props.items);

            return _.map(tags, function(tagName) {
                return <Tag
                    key={tagName}
                    name={tagName}
                    isSelected={tagName.toLowerCase() === selectedTag}
                    onSelected={this._onTagSelected}
                />;
            }, this);
        },

        _collectTagsFromTemplates: function(templates) {
            var tags = _
                .chain(templates)
                .flatMap(_.property('tags'))
                .compact()
                .unique(_.titleize)
                .value();

            tags.unshift('All');

            return tags;
        },

        _renderBody: function() {
            var items = _.map(this._filterItems(), function(item) {
                var selectListItem = this.props.selectListItem;

                var childProps = _.extend({
                    key: item.id,
                    selectItem: selectListItem.bind(null, item)
                }, item);

                return React.createElement(this.props.ItemComponent, childProps)
            }, this);

            return items.length ? items : this._getEmptyMessage();
        },

        render: function() {
            var templateListClass = [
                'tau-board-settings__template-list',
                this.props.templateListClass
            ].join(' ');

            return (
                <div className="tau-board-settings__container">
                    <div className="tau-board-settings__container__wrap">
                        <div className="tau-inline-group tau-search">
                            <input
                                type="text"
                                name="name"
                                placeholder={this.props.searchPlaceholder}
                                className="tau-in-text"
                                value={this.state.filterText}
                                onChange={this._onFilterChanged}
                                onKeyDown={this._onFilterKeyDown} />
                        </div>
                        <div className="tau-board-settings__tags-container">
                            <ul className="i-role-tagslist tau-board-settings__tag-list textSelectionDisabled">
                                {this._renderTags()}
                            </ul>
                        </div>
                        <div className="tau-board-settings__template-list__wrap">
                            <span className="tau-nav-right"></span>
                            <span className="tau-nav-left"></span>
                            <div className={templateListClass}>
                                {this._renderBody()}
                            </div>
                        </div>
                    </div>
                </div>
            );
        }
    });
});
