define(function(require) {
    var React = require('libs/react/react-ex'),
        _ = require('Underscore'),
        StateItemView = require('jsx!./state.item.process.workflow');

    return React.createClass({

        render: function() {
            if (!this.props.states) {
                return <div className="tau-loader"></div>;
            }

            var States = _.map(this.props.states, function(state) {
                return StateItemView({
                    key: state.id,
                    id: state.id,
                    name: state.name,
                    isDisabled: this.props.updatingStateId !== null,
                    isLoading: this.props.updatingStateId === state.id,
                    stateSettings: this.props.stateSettings,
                    renameStateAction: this.props.renameStateAction
                });
            }, this);

            return (
                <div className="i-role-views-workflow-container">
                    <div className="tau-container__title">
                        <span className="header-h3">Workflow for</span>
                        <i className={'tau-entity-icon tau-entity-icon--' + this.props.entityType}>{this.props.entityTerms.name}</i>
                    </div>
                    <div className="tau-container__body">
                        <div className="process-grid__wrap">
                            <div className="process-grid">
                                {States}
                            </div>
                        </div>
                    </div>
                </div>
            );
        }
    });
});
