define(function(require) {
    var React = require('libs/react/react-ex');

    return React.defineClass(['termsBus', 'termProcessor'], function(termsBus, termProcessor) {
        return {
            handlerChangeSingleTerm: function(evt) {
                var data = {};
                data[this.props.key] = {single: evt.target.value};
                termsBus.fire('changeTerm', data);
            },
            handlerChangePluralTerm: function(evt) {
                var data = {};
                data[this.props.key] = {plural: evt.target.value};
                termsBus.fire('changeTerm', data);
            },
            saveTerms: function(e) {
                if (e.keyCode === 13) {
                    termsBus.fire('saveTerms');
                }
            },
            render: function() {
                var shortAbbreviation = function(term) {
                    return termProcessor.getShortAbbreviation(term);
                };
                var longAbbreviation = function(term) {
                    return termProcessor.getLongAbbreviation(term);
                };
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
                var entityKind = this.props.term.entityKind.toLowerCase();

                var shortClasses = "tau-entity-icon tau-entity-icon--icon tau-entity-icon--" + entityKind;
                var longClasses = "tau-entity-icon tau-entity-icon-full tau-entity-icon--" + entityKind;

                return (
                    <tr className={rowClass} key={this.props.key}>
                        <th className='tau-table-caption'>
                            <label className={labelClasses}>{this.props.term.defaultSingle}</label>
                        </th>
                        <td>
                            <input disabled={disabledSingle} type="text" onKeyPress={this.saveTerms} onChange={this.handlerChangeSingleTerm} className={singleClasses} value={this.props.term.single}  placeholder={this.props.term.defaultSingle}/>
                        </td>
                        <td>
                            <input disabled={disabledPlural} type="text" onKeyPress={this.saveTerms} onChange={this.handlerChangePluralTerm} className={pluralClasses} value={this.props.term.plural} placeholder={this.props.term.defaultPlural}/>
                        </td>
                        <td>
                            <i className={shortClasses}>{shortAbbreviation(this.props.term.single || this.props.term.defaultSingle)}</i>
                        </td>
                        <td>
                            <i className={longClasses}>{longAbbreviation(this.props.term.single || this.props.term.defaultSingle)}</i>
                        </td>
                    </tr>
                    );
            }
        }
    });
});
