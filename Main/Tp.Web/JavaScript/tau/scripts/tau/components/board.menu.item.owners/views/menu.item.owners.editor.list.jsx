define(function(require) {
    var $ = require('jQuery');
    var _ = require('Underscore');
    var React = require('react');

    require('tau/ui/extensions/user/jquery.tauUserAutocomplete');

    return React.createClass({
        displayName: 'menu.item.owners.editor.list',

        propTypes: {
            owners: React.PropTypes.arrayOf(React.PropTypes.shape({
                displayName: React.PropTypes.string.isRequired,
                avatarUri: React.PropTypes.string.isRequired,
                userId: React.PropTypes.number.isRequired,
                getCanDelete: React.PropTypes.func.isRequired
            })).isRequired,
            canEdit: React.PropTypes.bool.isRequired,
            store: React.PropTypes.object.isRequired,
            store2: React.PropTypes.object.isRequired,
            keyboardManager: React.PropTypes.object.isRequired,
            // Unfortunately required for tests because there is no other way to get React element
            // when it's created outside of the parent virtual DOM tree.
            notifyListEditorCreated: React.PropTypes.func
        },

        getDefaultProps() {
            return {
                notifyListEditorCreated: _.constant()
            };
        },

        componentDidMount() {
            if (this.refs.addNew) {
                $(this.refs.addNew.getDOMNode()).tauUserAutocomplete({
                    position: {
                        my: 'right top',
                        at: 'right bottom+5',
                        collision: 'none'
                    },
                    store2: this.props.store2,
                    rejectUserIdsGetter: function() {
                        return _.pluck(this.props.owners, 'userId');
                    }.bind(this),
                    open: function() {
                        this.props.keyboardManager.pushHandler({});
                    }.bind(this),
                    close: function() {
                        this.props.keyboardManager.popHandler();
                    }.bind(this),
                    select: function(event, ui) {
                        this.props.store.addOwner(ui.item.id);
                    }.bind(this)
                });
            }
        },

        componentWillUnmount() {
            if (this.refs.addNew) {
                var $addNew = $(this.refs.addNew.getDOMNode());
                if ($addNew.tauUserAutocomplete('instance')) {
                    $addNew.tauUserAutocomplete('destroy');
                }
            }
        },

        _handleDelete(userId) {
            this.props.store.deleteOwner(userId);
        },

        _renderDeleteButton(owner) {
            if (!owner.getCanDelete()) {
                return null;
            }

            return (
                <i
                    className="tau-delete i-role-delete"
                    onClick={this._handleDelete.bind(this, owner.userId)} />
            );
        },

        render() {
            var owners = _.map(this.props.owners, owner => {
                return (
                    <li
                        key={owner.userId}
                        className="tau-menu-item">
                        <img
                            className="tau-owners__owner-image"
                            src={owner.avatarUri}
                            title={owner.displayName}/>
                        <span>{owner.displayName}</span>
                        {this._renderDeleteButton(owner)}
                    </li>
                );
            });

            var addNew = this.props.canEdit ?
                <div className="tau-invite-wrap i-role-add-wrap">
                    <input ref="addNew" type="text" className="tau-in-text i-role-add" placeholder="Add owner"/>
                </div> :
                null;

            return (
                <div>
                    <ul className="tau-owners i-role-owners">
                        {owners}
                    </ul>
                    {addNew}
                </div>
            );
        }
    });
});