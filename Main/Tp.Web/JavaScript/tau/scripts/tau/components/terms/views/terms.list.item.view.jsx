define(function(require) {
    var React = require('libs/react/react-ex');

    return React.defineClass(['termsBus', 'termProcessor'], function(termsBus, termProcessor) {
        return {
            displayName: 'TermsListItem',

            handlerChangeSingleTerm: function(evt) {
                var data = {};
                data[this.props.term.entityKind] = {single: evt.target.value};
                termsBus.fire('changeTerm', data);
            },
            handlerChangePluralTerm: function(evt) {
                var data = {};
                data[this.props.term.entityKind] = {plural: evt.target.value};
                termsBus.fire('changeTerm', data);
            },
            saveTerms: function(e) {
                if (e.keyCode === 13) {
                    termsBus.fire('saveTerms');
                }
            },

            getShortAbbreviation: function() {
                var term = this.props.term.single || this.props.term.defaultSingle;
                return termProcessor.getShortAbbreviation(term);
            },
            getLongAbbreviation: function() {
                var term = this.props.term.single || this.props.term.defaultSingle;
                return termProcessor.getLongAbbreviation(term);
            },

            render: function() {
                var cx = React.addons.classSet;
                var labelClasses = cx({
                    'tau-in-text__label': true,
                    'rename-text': !this.props.term.isDefault
                });

                var rowClass = cx({
                    spacer: this.props.term.isSpacer
                });

                var pluralClasses = cx({
                    'tau-in-text': true,
                    'tau-error': this.props.term.isInvalidPlural
                });
                var singleClasses = cx({
                    'tau-in-text': true,
                    'tau-error': this.props.term.isInvalidSingle
                });
                var disabledSingle = !this.props.isValidCollection && !this.props.term.isInvalidSingle;
                var disabledPlural = !this.props.isValidCollection && !this.props.term.isInvalidPlural;

                var entityKindClassSuffix = this.props.term.entityKind.toLowerCase();
                var shortClasses = 'tau-entity-icon tau-entity-icon--icon tau-entity-icon--' + entityKindClassSuffix;
                var longClasses = 'tau-entity-icon tau-entity-icon-full tau-entity-icon--' + entityKindClassSuffix;

                return (
                    <tr className={rowClass} key={this.props.term.entityKind}>
                        <th className='tau-table-caption'>
                            <label className={labelClasses}>{this.props.term.defaultSingle}</label>
                        </th>
                        <td>
                            <input disabled={disabledSingle} type="text" onKeyPress={this.saveTerms}
                                onChange={this.handlerChangeSingleTerm} className={singleClasses}
                                value={this.props.term.single} placeholder={this.props.term.defaultSingle}/>
                        </td>
                        <td>
                            <input disabled={disabledPlural} type="text" onKeyPress={this.saveTerms}
                                onChange={this.handlerChangePluralTerm} className={pluralClasses}
                                value={this.props.term.plural} placeholder={this.props.term.defaultPlural}/>
                        </td>
                        <td>
                            <i className={shortClasses}>{this.getShortAbbreviation()}</i>
                        </td>
                        <td>
                            <i className={longClasses}>{this.getLongAbbreviation()}</i>
                        </td>
                    </tr>
                );
            }
        }
    });
});
