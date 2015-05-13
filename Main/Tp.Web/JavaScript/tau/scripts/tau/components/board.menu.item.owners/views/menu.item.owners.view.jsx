define(function(require) {
    var React = require('react');
    var _ = require('Underscore');

    var DefaultContainerTemplate = require('jsx!./menu.item.owners.container');
    var EditorMixin = require('./menu.item.owners.editor.mixin');

    var MAX_OWNER_COUNT = 3;

    return React.createClass({
        displayName: 'menu.item.owner.view',

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
            ContainerTemplate: React.PropTypes.func,
            labelText: React.PropTypes.string
        },

        getDefaultProps() {
            return {
                labelText: 'View owners',
                ContainerTemplate: DefaultContainerTemplate
            };
        },

        mixins: [EditorMixin],

        render() {
            var owners = _.map(_.take(this.props.owners, MAX_OWNER_COUNT), owner => (
                <img
                    className="tau-owners__owner-image"
                    key={owner.id}
                    src={owner.avatarUri}
                    title={owner.displayName} />
            ));

            if (!owners.length) {
                return null;
            }

            // Don't move to `getDefaultProps` because `ContainerTemplate` may be `undefined`
            var ContainerTemplate = this.props.ContainerTemplate;

            return (
                <ContainerTemplate isListEditorVisible={this.state.isListEditorVisible} onClick={this.toggleListEditor}>
                    <span className="tau-label">{this.props.labelText}</span>
                    <span className="tau-owners-wrap i-role-owners-wrap">
                        <span className="i-role-owners">
                            {owners}
                        </span>
                    </span>
                </ContainerTemplate>
            );
        }
    });
});