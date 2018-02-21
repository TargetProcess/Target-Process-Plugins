tau.mashups
    .addDependency('Underscore')
    .addDependency('jQuery')
    .addDependency('app.path')
    .addDependency('tau/utils/utils.date')
    .addDependency('tau-intl')
    .addDependency('tau/ui/renderTeamIconWithFallback')
    .addDependency('tau/utils/utils.htmlConverter')
    .addModule('tp/search/templates/template.search.fn',
        function (_, $, appPath, dateUtils, intl, renderTeamIconWithFallback, htmlConverter) {
            return {
                renderTeamIconWithFallback: renderTeamIconWithFallback,
                oldSearchUrl: function (searchString) {
                    return appPath.get() + '/Search/Search.aspx?SearchString=' + searchString;
                },

                generatePagingItems: function (current, total, radix) {
                    var r = [];
                    var lb = _.max([current - radix, 0]);
                    var rb = _.min([current + radix, total - 1]);

                    for (var i = 0; i < total; i++) {
                        if (i === 0 || i === (total - 1)) {
                            r.push({
                                index: i,
                                text: (i + 1)
                            });
                        } else if ((i >= lb) && (i <= rb)) {
                            r.push({
                                index: i,
                                text: (i + 1)
                            });
                        } else if (i === (lb - 1) || i === (rb + 1)) {
                            r.push({
                                index: i,
                                text: '...'
                            });
                        }
                    }

                    return r;
                },

                formatDate: function (createDate) {
                    var shortDateFormat = dateUtils.format.date['short'];
                    var dateStr = shortDateFormat(dateUtils.convertToTimezone(createDate));
                    var ageStr = intl.formatRelativeMessage(createDate);
                    return dateStr + ' (' + ageStr + ')';
                },

                nodeCutOffIterator: function (htmlNode, radix) {
                    if (htmlNode) {
                        var txt = htmlNode.textContent.replace(/\s+/g, ' ');
                        var len = txt.length;
                        var splitChars = _.toArray(' []{}()/\\,.:;?!&+-*');

                        if (len > 2 * radix) {
                            var startIndex = _.indexOfAnyChar(txt, splitChars, radix);
                            startIndex = (startIndex === -1) ? radix : startIndex;
                            var partStart = txt.substring(0, startIndex);

                            var rest = len - radix;
                            var endIndex = _.indexOfAnyChar(txt, splitChars, rest);
                            endIndex = (endIndex === -1) ? rest : endIndex;
                            var partEnd = txt.substring(endIndex, len);

                            htmlNode.textContent = partStart + ' ... ' + partEnd;
                        }
                    }

                    return htmlNode;
                },

                iterateTokenThruKeywords: function (keywords, token) {
                    var r = {
                        token: token,
                        matches: []
                    };

                    var ltoken = token.toLocaleLowerCase();
                    r.matches = _(keywords).reduce(function (memo, k) {
                        var lk = k.toLocaleLowerCase();
                        var index = 0;
                        do {
                            index = ltoken.indexOf(lk, index);
                            if (index !== -1) {
                                var lb = index;
                                var rb = lb + lk.length;
                                memo.push({ a: lb, z: rb });
                                index = rb;
                            }
                        } while (index !== -1);

                        return memo;
                    }, r.matches);

                    var matches = _(r.matches).sortBy(function (m) {
                        return m.a;
                    });

                    var reducedMatches = [];
                    while (matches.length) {
                        var v = matches.shift();
                        while (matches.length && (v.z >= matches[0].a)) {
                            if (matches[0].z > v.z) {
                                (v.z = matches[0].z);
                            }
                            matches.shift();
                        }
                        reducedMatches.push(v);
                    }

                    r.matches = reducedMatches;
                    return r;
                },

                /**
                 * Shorten text and highlight word occurrences.
                 *
                 * @param {String} originHtml
                 * @param {Array} keywords
                 * @param {Boolean} shorten
                 * @returns {*}
                 */
                format: function (originHtml, keywords, shorten) {
                    var originText = $('<div>').html(htmlConverter.fromSourceToHtml(originHtml)).text();
                    originText = $.trim(originText).replace(/</g, '&lt;').replace(/>/g, '&gt;');

                    if (!originText) {
                        return '';
                    }

                    var highlightedText = _
                        .chain(originText.split(' '))
                        .map(function (token) {
                            return this.iterateTokenThruKeywords(keywords, token);
                        }.bind(this))
                        .reduce(function (memo, item) {
                            var t = item.token;
                            var r = [];
                            var index = 0;
                            for (var i = 0; i < item.matches.length; i++) {
                                var m = item.matches[i];
                                r.push(
                                    t.substring(index, m.a),
                                    '<span class="tau-search-highlight">',
                                    t.substring(m.a, m.z),
                                    '</span>'
                                );
                                index = m.z;
                            }
                            r.push(t.substring(index));
                            memo.push(r.join(''));

                            return memo;
                        }, [])
                        .value()
                        .join(' ');

                    if (!shorten) {
                        return highlightedText;
                    }

                    var newTextNode = $('<div>' + highlightedText + '</div>');
                    var resultNode = $('<div></div>');
                    var textIterator = this.nodeCutOffIterator;
                    var hiddenMatches = 0;
                    var charsCounter = 0;

                    newTextNode
                        .contents()
                        .each(function (i, val) {
                            var isTextNode = (val.nodeType == Node.TEXT_NODE);

                            var node;
                            if (isTextNode) {
                                node = textIterator(val, 30);
                                charsCounter += node.textContent.length;
                            } else {
                                charsCounter += $(val).text().length;
                                node = val;
                            }

                            if (charsCounter > 315) {
                                if (!isTextNode) {
                                    ++hiddenMatches;
                                }
                            } else {
                                resultNode.append(node);
                            }
                        });

                    if (hiddenMatches > 0) {
                        var matchesText = intl.formatMessage(
                            '+ {hiddenMatches} {hiddenMatches, plural, one {more match} other {more matches}}',
                            { hiddenMatches: hiddenMatches });
                        resultNode.append('... <strong class="more-matches">' + matchesText + '.</strong>');
                    }

                    return resultNode.html();
                }
            };
        });
