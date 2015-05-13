define(function(require) {
    var $ = require('jQuery');
    var React = require('react');
    var utils = require('./editorUtils');
    var autocompleteItemTemplate = require('tau/ui/templates/board.editor/ui.template.board.editor.autocomplete.item');

    return React.createClass({
        displayName: 'filter.builder.autocomplete.editor',

        propTypes: {
            editorContext: utils.editorContextPropTypes.isRequired,
            suggestionService: React.PropTypes.object.isRequired,
            placeholder: React.PropTypes.string
        },

        mixins: [React.addons.PureRenderMixin],

        getInitialState() {
            return {
                editValue: this.props.editorContext.getCurrentValue(),
                isNew: !this.props.editorContext.getCurrentValue()
            };
        },

        componentDidMount() {
            if (this.state.isNew) {
                this._initAutocomplete();
            }
        },

        _initAutocomplete() {
            var $input = $(this.getDOMNode()).find('input');

            $input.autocomplete({
                minLength: 0,
                position: {
                    my: 'left top',
                    at: 'left bottom+8',
                    collision: 'flip'
                },
                source: function(request, response) {
                    this._getSuggestions(request.term).done(function(result) {
                        var suggestions = result.Suggestions ? result.Suggestions.Items : [];
                        var filteredSuggestions = this._filterSuggestions(suggestions);
                        response(filteredSuggestions);
                    }.bind(this)).fail(function() {
                        response([]);
                    });
                }.bind(this),
                select: function(event, ui) {
                    this._setValue(ui.item.value);
                    event.stopPropagation();
                    return false;
                }.bind(this)
            });

            var autocomplete = $input.autocomplete('instance');

            autocomplete._renderItem = function(ul, item) {
                return autocompleteItemTemplate
                    .render(item)
                    .data('ui-autocomplete-item', {
                        value: item.Value
                    })
                    .appendTo(ul);
            };

            autocomplete._renderMenu = function(ul, items) {
                $(ul)
                    .addClass('tau-autocomplete__menu filter-builder-autocomplete-list drop-down-list')
                    .css({'maxHeight': $(window).height() * 0.4, 'zIndex': this.element.zIndex() + 10});

                _.each(items, function(item) {
                    this._renderItem(ul, item);
                }, this);
            };

            $input.on('focus', function() {
                autocomplete.search($input.val());
            });

            $input.on('focusout', function() {
                if (!this._valueIsSet) {
                    this._setValue($input.val(), true);
                }
            }.bind(this));
        },

        _getSuggestions(term) {
            return this.props.suggestionService.getSuggestions(this.props.editorContext.fieldName, term);
        },

        _filterSuggestions(suggestions) {
            var selectedValues = _.map(this.props.editorContext.getCurrentFilterValues(), _.property('value'));
            return _.filter(suggestions, function(suggestion) {
                return !_.contains(selectedValues, suggestion.Value);
            });
        },

        _onInputChange(e) {
            this.setState({editValue: e.target.value});
        },

        _onInputKeyPress(e) {
            if (e.charCode === $.ui.keyCode.ENTER) {
                this._setValue(e.target.value);
            }
        },

        _setValue(value, cleanIfNotMatch) {
            if (!value) {
                return;
            }

            this._getSuggestions(value.substring(0, value.length - 1)).done(function(result) {
                var suggestions = result.Suggestions ? result.Suggestions.Items : [];
                var filteredSuggestions = this._filterSuggestions(suggestions);
                var suggestion = _.find(filteredSuggestions, function(item) {
                    return item.Value.toLowerCase() === value.toLowerCase();
                });
                if (suggestion) {
                    this._valueIsSet = true;
                    this.props.editorContext.setNewValue({
                        value: suggestion.Value,
                        isObject: suggestion.SuggestionType === 'ObjectValue'
                    });
                } else if (cleanIfNotMatch) {
                    this.setState({editValue: ''});
                }
            }.bind(this));
        },

        render() {
            return this.state.isNew ? (
                <div>
                    <input
                        type="text"
                        className="filter-builder__filter__editable i-role-focus-target"
                        placeholder={this.props.placeholder}
                        onChange={this._onInputChange}
                        onKeyPress={this._onInputKeyPress}
                        value={this.state.editValue} />
                </div>
            ) : (
                <div>
                    {this.state.editValue}
                </div>
            );
        }
    });
});
