define(function(require) {
    var React = require('libs/react/react-ex');
    var t = React.PropTypes;

    var TeamIcon = React.createClass({
        displayName: 'TeamIcon',
        propTypes: {
            name: t.string
        },
        render: function() {
            if (!this.props.name) {
                return null;
            }
            return (<i className={'tau-icon tau-icon_type_svg tau-icon_name_' + this.props.name}></i>);
        }
    });
    return TeamIcon;
});
