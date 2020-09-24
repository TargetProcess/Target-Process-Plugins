Ext.ns('Tp.renders');

Tp.renders.Render = Ext.extend(Object, {
    entityLinkRender: function(entityKind) {
        return function(val) {
            return "<a onclick='Tp.renders.getInstance().dblClickHandler(this)' href='javascript:JumpTo(" + val + ", \"" + entityKind + "\");'>" + val + "</a>";
        };
    },

    dblClickHandler: function(element) {
        var timerId = window.setTimeout(runClick, 1000);
        event.returnValue = false;

        element.ondblclick = function() {
            window.clearTimeout(timerId);
        };

        function runClick() {
            element.onclick = function() {
                return true;
            };
            element.click();
        }
    }
});

Tp.renders.type = "Tp.renders.Render";

Tp.renders.getInstance = function() {
    if (Tp.renders.instance == null) {
        Tp.renders.instance = eval("new " + Tp.renders.type);
    }
    return Tp.renders.instance;
};

/**
 * Creates renderer for string values.
 */
function stringRenderer() {
    return function(value) {
        if (value) {
            return Ext.util.Format.htmlEncode(value); // Prevent XSS attacks
        }
        return "";
    };
}

/**
 * Creates renderer for boolean values.
 */
function boolRenderer() {
    return function (value) {
        if (value === true) {
            return "<img src='" + Application.baseUrl + "/img/yes.gif' />";
        }
        if (value === false) {
            return "<img src='" + Application.baseUrl + "/img/no.gif' />";
        }
        return "&nbsp;";
    };
}

/**
 * Creates localized renderer for float values.
 */
function floatRenderer() {
    return function (value) {
        return isNaN(value) ? '' : String(value).replace(".", Sys.CultureInfo.CurrentCulture.numberFormat.CurrencyDecimalSeparator);
    };
}

/**
 * Creates renderer for description and other rich text fields.
 */
function richTextRenderer() {
    return function(value) {
        if (value) {
            return Ext.util.Format.stripTags(value);
        }
        return "";
    };
}

/**
 * Creates renderer for URL custom fields.
 */
function urlRenderer() {
    return function(value) {
        if (value) {
            var url = "";
            var label = "";
            var s = value.split("\n");
            if (s.length == 2) {
                url = Ext.util.Format.htmlEncode(s[1]);
                label = Ext.util.Format.htmlEncode(s[0]) || url;
            }
            else {
                url = Ext.util.Format.htmlEncode(value);
                label = Ext.util.Format.htmlEncode(value);
            }
            return "<a href=\"" + url + "\" target=\"_blank\" rel=\"noopener noreferrer\">" + label + "</a>";
        }
        return "";
    };
}

/**
 * Creates renderer for Entity custom fields.
 */
function entityRenderer() {
    return function(value) {
        if (value) {
            var url = "";
            var entityId = "";
            var entityType = "";
            var label = "";
            var icon = "";
            var s = value.split("\n");
            if (s.length == 3) {
                entityId = s[1];
                entityType = s[2];
                url = Application.getViewUrl(entityId, entityType);
                label = Ext.util.Format.htmlEncode(s[0]);
                icon = "<img class='icon' src='" + Application.baseUrl + "/img/" + entityType + ".gif'>&nbsp;";
            }
            else {
                return value;
            }
            return icon + "<a href=\"" + url + "\" target=\"_blank\" rel=\"noopener noreferrer\">" + label + "</a>";
        }
        return "";
    };
}

function enumRenderer(dictionary) {
    function noValue(value) {
        return value === '' || value === null || typeof (value) === 'undefined';
    }

    return function (value) {
        for (n in dictionary) {
            if (dictionary[n].Key === value) {
                const enumDesc = dictionary[n];
                return !enumDesc.IsNone ? enumDesc.Value : '';
            }
        }
        if (noValue(value)) {
            return '';
        }
        return "<span style=\"color: red;\">Undefined</span>";
    };
}

/*
 * Creates rendenrer for a link to entity view form.
 */
function viewLinkRenderer(entityKind) {
    return function(value) {
        if (value != null) {
            var normalizedEntityKind = entityKind.toLowerCase().replace('squad', 'team');
            var url = entityKind === 'Assignable' || entityKind === 'General'
                ? Application.getShortViewUrl(value)
                : Application.getViewUrl(value, normalizedEntityKind);

            return '<a class="i-role-tp3-link"' +
                ' data-entity-id="' + value + '"' +
                ' data-entity-type="' + normalizedEntityKind + '"' +
                ' href="' + url + '">' + value + '</a>';
        }
        return '';
    };
}
