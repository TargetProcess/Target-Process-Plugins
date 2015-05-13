define(function(require) {
    var React = require('libs/react/react-ex'),
        t = React.PropTypes,
        StateSelectorHeader = require('jsx!./stateSelectorHeader.view'),
        EntityStateGroups = require('jsx!./entityStateGroups.view'),
        TooltipArticle = require('jsx!tau/components/react/tooltipArticle');

    var TeamEntityStateProperty = React.createClass({
        displayName: 'TeamEntityStateProperty',
        propTypes: {
            currentStates: t.shape({
                parentState: t.shape({id: t.number, name: t.string}).isRequired,
                teamState: t.shape({
                    team: t.shape({id: t.number, name: t.name}).isRequired,
                    state: t.shape({id: t.number, name: t.name}).isRequired
                })
            }).isRequired,
            actions: t.shape({
                loadWorkflow: t.func.isRequired,
                openDropDown: t.func.isRequired,
                closeDropDown: t.func.isRequired,
                changeState: t.func.isRequired,
                changeTeamState: t.func.isRequired
            }).isRequired,
            isLoading: t.bool.isRequired,
            workflowHelpData: t.object.isRequired,
            entityStateGroups: t.array
        },
        render: function() {
            if (this.props.entityStateGroups && !this.props.isLoading) {
                return (
                    <div className="multiple-team-state-select tau-active">
                        <StateSelectorHeader {...this.props.currentStates}
                            isLoading={this.props.isLoading}
                            onClick={this.props.actions.closeDropDown}
                        />
                        <EntityStateGroups
                            items={this.props.entityStateGroups}
                            currentStates={this.props.currentStates}
                            workflowHelpData={this.props.workflowHelpData}
                            actions={this.props.actions}
                        />
                    </div>
                );
            } else {
                return (
                    <div className="multiple-team-state-select">
                        <StateSelectorHeader {...this.props.currentStates}
                            isLoading={this.props.isLoading}
                            onClick={this.props.actions.openDropDown}
                            onMouseEnter={this.props.actions.loadWorkflow}
                        />
                    </div>
                );
            }
        }
    });

    return React.createClass({
        displayName: 'TeamEntityStatePropertyWrapper',
        propTypes: {
            workflowHelpData: t.object.isRequired
        },
        render: function() {
            var articleId = this.props.workflowHelpData.isTp3 ?
                'workflow.is.customizable' :
                'workflow.is.customizable.with-no-link';
            return (
                <div>
                    <TeamEntityStateProperty {...this.props} />
                    {this.props.workflowHelpData.canSetup ?
                        <TooltipArticle
                            articleId={articleId}
                            dataAttributes={{'data-url': this.props.workflowHelpData.url}}
                        /> : null}
                </div>
            )
        }
    });
});
