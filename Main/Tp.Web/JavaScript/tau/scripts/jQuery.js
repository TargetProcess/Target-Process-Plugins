
define('libs/jquery/jquery.isSupport',["libs/jquery/jquery"],function(jQuery){var supports=function(){var div=document.createElement("div"),vendors="Khtml Ms O Moz Webkit".split(" "),len=vendors.length;return function(prop){if(prop in div.style)return!0;prop=prop.replace(/^[a-z]/,function(val){return val.toUpperCase()});while(len--)if(vendors[len]+prop in div.style)return!0;return!1}}();return function($){$.isSupport=function(propertyName){return supports(propertyName)}}(jQuery),jQuery});
/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.core.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.widget.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.mouse.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.position.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

//(function(a,b){a.ui=a.ui||{};var c=/left|center|right/,d=/top|center|bottom/,e="center",f={},g=a.fn.position,h=a.fn.offset;a.fn.position=function(b){if(!b||!b.of)return g.apply(this,arguments);b=a.extend({},b);var h=a(b.of),i=h[0],j=(b.collision||"flip").split(" "),k=b.offset?b.offset.split(" "):[0,0],l,m,n;return i.nodeType===9?(l=h.width(),m=h.height(),n={top:0,left:0}):i.setTimeout?(l=h.width(),m=h.height(),n={top:h.scrollTop(),left:h.scrollLeft()}):i.preventDefault?(b.at="left top",l=m=0,n={top:b.of.pageY,left:b.of.pageX}):(l=h.outerWidth(),m=h.outerHeight(),n=h.offset()),a.each(["my","at"],function(){var a=(b[this]||"").split(" ");a.length===1&&(a=c.test(a[0])?a.concat([e]):d.test(a[0])?[e].concat(a):[e,e]),a[0]=c.test(a[0])?a[0]:e,a[1]=d.test(a[1])?a[1]:e,b[this]=a}),j.length===1&&(j[1]=j[0]),k[0]=parseInt(k[0],10)||0,k.length===1&&(k[1]=k[0]),k[1]=parseInt(k[1],10)||0,b.at[0]==="right"?n.left+=l:b.at[0]===e&&(n.left+=l/2),b.at[1]==="bottom"?n.top+=m:b.at[1]===e&&(n.top+=m/2),n.left+=k[0],n.top+=k[1],this.each(function(){var c=a(this),d=c.outerWidth(),g=c.outerHeight(),h=parseInt(a.curCSS(this,"marginLeft",!0))||0,i=parseInt(a.curCSS(this,"marginTop",!0))||0,o=d+h+(parseInt(a.curCSS(this,"marginRight",!0))||0),p=g+i+(parseInt(a.curCSS(this,"marginBottom",!0))||0),q=a.extend({},n),r;b.my[0]==="right"?q.left-=d:b.my[0]===e&&(q.left-=d/2),b.my[1]==="bottom"?q.top-=g:b.my[1]===e&&(q.top-=g/2),f.fractions||(q.left=Math.round(q.left),q.top=Math.round(q.top)),r={left:q.left-h,top:q.top-i},a.each(["left","top"],function(c,e){a.ui.position[j[c]]&&a.ui.position[j[c]][e](q,{targetWidth:l,targetHeight:m,elemWidth:d,elemHeight:g,collisionPosition:r,collisionWidth:o,collisionHeight:p,offset:k,my:b.my,at:b.at})}),a.fn.bgiframe&&c.bgiframe(),c.offset(a.extend(q,{using:b.using}))})},a.ui.position={fit:{left:function(b,c){var d=a(window),e=c.collisionPosition.left+c.collisionWidth-d.width()-d.scrollLeft();b.left=e>0?b.left-e:Math.max(b.left-c.collisionPosition.left,b.left)},top:function(b,c){var d=a(window),e=c.collisionPosition.top+c.collisionHeight-d.height()-d.scrollTop();b.top=e>0?b.top-e:Math.max(b.top-c.collisionPosition.top,b.top)}},flip:{left:function(b,c){if(c.at[0]===e)return;var d=a(window),f=c.collisionPosition.left+c.collisionWidth-d.width()-d.scrollLeft(),g=c.my[0]==="left"?-c.elemWidth:c.my[0]==="right"?c.elemWidth:0,h=c.at[0]==="left"?c.targetWidth:-c.targetWidth,i=-2*c.offset[0];b.left+=c.collisionPosition.left<0?g+h+i:f>0?g+h+i:0},top:function(b,c){if(c.at[1]===e)return;var d=a(window),f=c.collisionPosition.top+c.collisionHeight-d.height()-d.scrollTop(),g=c.my[1]==="top"?-c.elemHeight:c.my[1]==="bottom"?c.elemHeight:0,h=c.at[1]==="top"?c.targetHeight:-c.targetHeight,i=-2*c.offset[1];b.top+=c.collisionPosition.top<0?g+h+i:f>0?g+h+i:0}}},a.offset.setOffset||(a.offset.setOffset=function(b,c){/static/.test(a.curCSS(b,"position"))&&(b.style.position="relative");var d=a(b),e=d.offset(),f=parseInt(a.curCSS(b,"top",!0),10)||0,g=parseInt(a.curCSS(b,"left",!0),10)||0,h={top:c.top-e.top+f,left:c.left-e.left+g};"using"in c?c.using.call(b,h):d.css(h)},a.fn.offset=function(b){var c=this[0];return!c||!c.ownerDocument?null:b?this.each(function(){a.offset.setOffset(this,b)}):h.call(this)}),function(){var b=document.getElementsByTagName("body")[0],c=document.createElement("div"),d,e,g,h,i;d=document.createElement(b?"div":"body"),g={visibility:"hidden",width:0,height:0,border:0,margin:0,background:"none"},b&&a.extend(g,{position:"absolute",left:"-1000px",top:"-1000px"});for(var j in g)d.style[j]=g[j];d.appendChild(c),e=b||document.documentElement,e.insertBefore(d,e.firstChild),c.style.cssText="position: absolute; left: 10.7432222px; top: 10.432325px; height: 30px; width: 201px;",h=a(c).offset(function(a,b){return b}).offset(),d.innerHTML="",e.removeChild(d),i=h.top+h.left+(b?2e3:0),f.fractions=i>21&&i<22}()})(jQuery);

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.draggable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.droppable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.resizable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.selectable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.sortable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.accordion.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.autocomplete.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.button.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.dialog.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.slider.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.tabs.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.datepicker.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.progressbar.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.core.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.blind.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.bounce.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.clip.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.drop.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.explode.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.fade.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.fold.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.highlight.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.pulsate.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.scale.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.shake.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.slide.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

/*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.transfer.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */

define('libs/jquery/jquery.ui',['libs/jquery/jquery'], function (jq) {
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.core.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        function c(b, c) {
            var e = b.nodeName.toLowerCase();
            if ("area" === e) {
                var f = b.parentNode, g = f.name, h;
                return!b.href || !g || f.nodeName.toLowerCase() !== "map" ? !1 : (h = a("img[usemap=#" + g + "]")[0], !!h && d(h))
            }
            return(/input|select|textarea|button|object/.test(e) ? !b.disabled : "a" == e ? b.href || c : c) && d(b)
        }

        function d(b) {
            return!a(b).parents().andSelf().filter(function () {
                return a.curCSS(this, "visibility") === "hidden" || a.expr.filters.hidden(this)
            }).length
        }

        a.ui = a.ui || {};
        if (a.ui.version)return;
        a.extend(a.ui, {version:"1.8.19", keyCode:{ALT:18, BACKSPACE:8, CAPS_LOCK:20, COMMA:188, COMMAND:91, COMMAND_LEFT:91, COMMAND_RIGHT:93, CONTROL:17, DELETE:46, DOWN:40, END:35, ENTER:13, ESCAPE:27, HOME:36, INSERT:45, LEFT:37, MENU:93, NUMPAD_ADD:107, NUMPAD_DECIMAL:110, NUMPAD_DIVIDE:111, NUMPAD_ENTER:108, NUMPAD_MULTIPLY:106, NUMPAD_SUBTRACT:109, PAGE_DOWN:34, PAGE_UP:33, PERIOD:190, RIGHT:39, SHIFT:16, SPACE:32, TAB:9, UP:38, WINDOWS:91}}), a.fn.extend({propAttr:a.fn.prop || a.fn.attr, _focus:a.fn.focus, focus:function (b, c) {
            return typeof b == "number" ? this.each(function () {
                var d = this;
                setTimeout(function () {
                    a(d).focus(), c && c.call(d)
                }, b)
            }) : this._focus.apply(this, arguments)
        }, scrollParent:function () {
            var b;
            return a.browser.msie && /(static|relative)/.test(this.css("position")) || /absolute/.test(this.css("position")) ? b = this.parents().filter(function () {
                return/(relative|absolute|fixed)/.test(a.curCSS(this, "position", 1)) && /(auto|scroll)/.test(a.curCSS(this, "overflow", 1) + a.curCSS(this, "overflow-y", 1) + a.curCSS(this, "overflow-x", 1))
            }).eq(0) : b = this.parents().filter(function () {
                return/(auto|scroll)/.test(a.curCSS(this, "overflow", 1) + a.curCSS(this, "overflow-y", 1) + a.curCSS(this, "overflow-x", 1))
            }).eq(0), /fixed/.test(this.css("position")) || !b.length ? a(document) : b
        }, zIndex:function (c) {
            if (c !== b)return this.css("zIndex", c);
            if (this.length) {
                var d = a(this[0]), e, f;
                while (d.length && d[0] !== document) {
                    e = d.css("position");
                    if (e === "absolute" || e === "relative" || e === "fixed") {
                        f = parseInt(d.css("zIndex"), 10);
                        if (!isNaN(f) && f !== 0)return f
                    }
                    d = d.parent()
                }
            }
            return 0
        }, disableSelection:function () {
            return this.bind((a.support.selectstart ? "selectstart" : "mousedown") + ".ui-disableSelection", function (a) {
                a.preventDefault()
            })
        }, enableSelection:function () {
            return this.unbind(".ui-disableSelection")
        }}), a.each(["Width", "Height"], function (c, d) {
            function h(b, c, d, f) {
                return a.each(e, function () {
                    c -= parseFloat(a.curCSS(b, "padding" + this, !0)) || 0, d && (c -= parseFloat(a.curCSS(b, "border" + this + "Width", !0)) || 0), f && (c -= parseFloat(a.curCSS(b, "margin" + this, !0)) || 0)
                }), c
            }

            var e = d === "Width" ? ["Left", "Right"] : ["Top", "Bottom"], f = d.toLowerCase(), g = {innerWidth:a.fn.innerWidth, innerHeight:a.fn.innerHeight, outerWidth:a.fn.outerWidth, outerHeight:a.fn.outerHeight};
            a.fn["inner" + d] = function (c) {
                return c === b ? g["inner" + d].call(this) : this.each(function () {
                    a(this).css(f, h(this, c) + "px")
                })
            }, a.fn["outer" + d] = function (b, c) {
                return typeof b != "number" ? g["outer" + d].call(this, b) : this.each(function () {
                    a(this).css(f, h(this, b, !0, c) + "px")
                })
            }
        }), a.extend(a.expr[":"], {data:function (b, c, d) {
            return!!a.data(b, d[3])
        }, focusable:function (b) {
            return c(b, !isNaN(a.attr(b, "tabindex")))
        }, tabbable:function (b) {
            var d = a.attr(b, "tabindex"), e = isNaN(d);
            return(e || d >= 0) && c(b, !e)
        }}), a(function () {
            var b = document.body, c = b.appendChild(c = document.createElement("div"));
            c.offsetHeight, a.extend(c.style, {minHeight:"100px", height:"auto", padding:0, borderWidth:0}), a.support.minHeight = c.offsetHeight === 100, a.support.selectstart = "onselectstart"in c, b.removeChild(c).style.display = "none"
        }), a.extend(a.ui, {plugin:{add:function (b, c, d) {
            var e = a.ui[b].prototype;
            for (var f in d)e.plugins[f] = e.plugins[f] || [], e.plugins[f].push([c, d[f]])
        }, call:function (a, b, c) {
            var d = a.plugins[b];
            if (!d || !a.element[0].parentNode)return;
            for (var e = 0; e < d.length; e++)a.options[d[e][0]] && d[e][1].apply(a.element, c)
        }}, contains:function (a, b) {
            return document.compareDocumentPosition ? a.compareDocumentPosition(b) & 16 : a !== b && a.contains(b)
        }, hasScroll:function (b, c) {
            if (a(b).css("overflow") === "hidden")return!1;
            var d = c && c === "left" ? "scrollLeft" : "scrollTop", e = !1;
            return b[d] > 0 ? !0 : (b[d] = 1, e = b[d] > 0, b[d] = 0, e)
        }, isOverAxis:function (a, b, c) {
            return a > b && a < b + c
        }, isOver:function (b, c, d, e, f, g) {
            return a.ui.isOverAxis(b, d, f) && a.ui.isOverAxis(c, e, g)
        }})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.widget.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        if (a.cleanData) {
            var c = a.cleanData;
            a.cleanData = function (b) {
                for (var d = 0, e; (e = b[d]) != null; d++)try {
                    a(e).triggerHandler("remove")
                } catch (f) {
                }
                c(b)
            }
        } else {
            var d = a.fn.remove;
            a.fn.remove = function (b, c) {
                return this.each(function () {
                    return c || (!b || a.filter(b, [this]).length) && a("*", this).add([this]).each(function () {
                        try {
                            a(this).triggerHandler("remove")
                        } catch (b) {
                        }
                    }), d.call(a(this), b, c)
                })
            }
        }
        a.widget = function (b, c, d) {
            var e = b.split(".")[0], f;
            b = b.split(".")[1], f = e + "-" + b, d || (d = c, c = a.Widget), a.expr[":"][f] = function (c) {
                return!!a.data(c, b)
            }, a[e] = a[e] || {}, a[e][b] = function (a, b) {
                arguments.length && this._createWidget(a, b)
            };
            var g = new c;
            g.options = a.extend(!0, {}, g.options), a[e][b].prototype = a.extend(!0, g, {namespace:e, widgetName:b, widgetEventPrefix:a[e][b].prototype.widgetEventPrefix || b, widgetBaseClass:f}, d), a.widget.bridge(b, a[e][b])
        }, a.widget.bridge = function (c, d) {
            a.fn[c] = function (e) {
                var f = typeof e == "string", g = Array.prototype.slice.call(arguments, 1), h = this;
                return e = !f && g.length ? a.extend.apply(null, [!0, e].concat(g)) : e, f && e.charAt(0) === "_" ? h : (f ? this.each(function () {
                    var d = a.data(this, c), f = d && a.isFunction(d[e]) ? d[e].apply(d, g) : d;
                    if (f !== d && f !== b)return h = f, !1
                }) : this.each(function () {
                    var b = a.data(this, c);
                    b ? b.option(e || {})._init() : a.data(this, c, new d(e, this))
                }), h)
            }
        }, a.Widget = function (a, b) {
            arguments.length && this._createWidget(a, b)
        }, a.Widget.prototype = {widgetName:"widget", widgetEventPrefix:"", options:{disabled:!1}, _createWidget:function (b, c) {
            a.data(c, this.widgetName, this), this.element = a(c), this.options = a.extend(!0, {}, this.options, this._getCreateOptions(), b);
            var d = this;
            this.element.bind("remove." + this.widgetName, function () {
                d.destroy()
            }), this._create(), this._trigger("create"), this._init()
        }, _getCreateOptions:function () {
            return a.metadata && a.metadata.get(this.element[0])[this.widgetName]
        }, _create:function () {
        }, _init:function () {
        }, destroy:function () {
            this.element.unbind("." + this.widgetName).removeData(this.widgetName), this.widget().unbind("." + this.widgetName).removeAttr("aria-disabled").removeClass(this.widgetBaseClass + "-disabled " + "ui-state-disabled")
        }, widget:function () {
            return this.element
        }, option:function (c, d) {
            var e = c;
            if (arguments.length === 0)return a.extend({}, this.options);
            if (typeof c == "string") {
                if (d === b)return this.options[c];
                e = {}, e[c] = d
            }
            return this._setOptions(e), this
        }, _setOptions:function (b) {
            var c = this;
            return a.each(b, function (a, b) {
                c._setOption(a, b)
            }), this
        }, _setOption:function (a, b) {
            return this.options[a] = b, a === "disabled" && this.widget()[b ? "addClass" : "removeClass"](this.widgetBaseClass + "-disabled" + " " + "ui-state-disabled").attr("aria-disabled", b), this
        }, enable:function () {
            return this._setOption("disabled", !1)
        }, disable:function () {
            return this._setOption("disabled", !0)
        }, _trigger:function (b, c, d) {
            var e, f, g = this.options[b];
            d = d || {}, c = a.Event(c), c.type = (b === this.widgetEventPrefix ? b : this.widgetEventPrefix + b).toLowerCase(), c.target = this.element[0], f = c.originalEvent;
            if (f)for (e in f)e in c || (c[e] = f[e]);
            return this.element.trigger(c, d), !(a.isFunction(g) && g.call(this.element[0], c, d) === !1 || c.isDefaultPrevented())
        }}
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.mouse.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        var c = !1;
        a(document).mouseup(function (a) {
            c = !1
        }), a.widget("ui.mouse", {options:{cancel:":input,option", distance:1, delay:0}, _mouseInit:function () {
            var b = this;
            this.element.bind("mousedown." + this.widgetName,function (a) {
                return b._mouseDown(a)
            }).bind("click." + this.widgetName, function (c) {
                if (!0 === a.data(c.target, b.widgetName + ".preventClickEvent"))return a.removeData(c.target, b.widgetName + ".preventClickEvent"), c.stopImmediatePropagation(), !1
            }), this.started = !1
        }, _mouseDestroy:function () {
            this.element.unbind("." + this.widgetName), a(document).unbind("mousemove." + this.widgetName, this._mouseMoveDelegate).unbind("mouseup." + this.widgetName, this._mouseUpDelegate)
        }, _mouseDown:function (b) {
            if (c)return;
            this._mouseStarted && this._mouseUp(b), this._mouseDownEvent = b;
            var d = this, e = b.which == 1, f = typeof this.options.cancel == "string" && b.target.nodeName ? a(b.target).closest(this.options.cancel).length : !1;
            if (!e || f || !this._mouseCapture(b))return!0;
            this.mouseDelayMet = !this.options.delay, this.mouseDelayMet || (this._mouseDelayTimer = setTimeout(function () {
                d.mouseDelayMet = !0
            }, this.options.delay));
            if (this._mouseDistanceMet(b) && this._mouseDelayMet(b)) {
                this._mouseStarted = this._mouseStart(b) !== !1;
                if (!this._mouseStarted)return b.preventDefault(), !0
            }
            return!0 === a.data(b.target, this.widgetName + ".preventClickEvent") && a.removeData(b.target, this.widgetName + ".preventClickEvent"), this._mouseMoveDelegate = function (a) {
                return d._mouseMove(a)
            }, this._mouseUpDelegate = function (a) {
                return d._mouseUp(a)
            }, a(document).bind("mousemove." + this.widgetName, this._mouseMoveDelegate).bind("mouseup." + this.widgetName, this._mouseUpDelegate), b.preventDefault(), c = !0, !0
        }, _mouseMove:function (b) {
            return!a.browser.msie || document.documentMode >= 9 || !!b.button ? this._mouseStarted ? (this._mouseDrag(b), b.preventDefault()) : (this._mouseDistanceMet(b) && this._mouseDelayMet(b) && (this._mouseStarted = this._mouseStart(this._mouseDownEvent, b) !== !1, this._mouseStarted ? this._mouseDrag(b) : this._mouseUp(b)), !this._mouseStarted) : this._mouseUp(b)
        }, _mouseUp:function (b) {
            return a(document).unbind("mousemove." + this.widgetName, this._mouseMoveDelegate).unbind("mouseup." + this.widgetName, this._mouseUpDelegate), this._mouseStarted && (this._mouseStarted = !1, b.target == this._mouseDownEvent.target && a.data(b.target, this.widgetName + ".preventClickEvent", !0), this._mouseStop(b)), !1
        }, _mouseDistanceMet:function (a) {
            return Math.max(Math.abs(this._mouseDownEvent.pageX - a.pageX), Math.abs(this._mouseDownEvent.pageY - a.pageY)) >= this.options.distance
        }, _mouseDelayMet:function (a) {
            return this.mouseDelayMet
        }, _mouseStart:function (a) {
        }, _mouseDrag:function (a) {
        }, _mouseStop:function (a) {
        }, _mouseCapture:function (a) {
            return!0
        }})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.position.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    //(function(a,b){a.ui=a.ui||{};var c=/left|center|right/,d=/top|center|bottom/,e="center",f={},g=a.fn.position,h=a.fn.offset;a.fn.position=function(b){if(!b||!b.of)return g.apply(this,arguments);b=a.extend({},b);var h=a(b.of),i=h[0],j=(b.collision||"flip").split(" "),k=b.offset?b.offset.split(" "):[0,0],l,m,n;return i.nodeType===9?(l=h.width(),m=h.height(),n={top:0,left:0}):i.setTimeout?(l=h.width(),m=h.height(),n={top:h.scrollTop(),left:h.scrollLeft()}):i.preventDefault?(b.at="left top",l=m=0,n={top:b.of.pageY,left:b.of.pageX}):(l=h.outerWidth(),m=h.outerHeight(),n=h.offset()),a.each(["my","at"],function(){var a=(b[this]||"").split(" ");a.length===1&&(a=c.test(a[0])?a.concat([e]):d.test(a[0])?[e].concat(a):[e,e]),a[0]=c.test(a[0])?a[0]:e,a[1]=d.test(a[1])?a[1]:e,b[this]=a}),j.length===1&&(j[1]=j[0]),k[0]=parseInt(k[0],10)||0,k.length===1&&(k[1]=k[0]),k[1]=parseInt(k[1],10)||0,b.at[0]==="right"?n.left+=l:b.at[0]===e&&(n.left+=l/2),b.at[1]==="bottom"?n.top+=m:b.at[1]===e&&(n.top+=m/2),n.left+=k[0],n.top+=k[1],this.each(function(){var c=a(this),d=c.outerWidth(),g=c.outerHeight(),h=parseInt(a.curCSS(this,"marginLeft",!0))||0,i=parseInt(a.curCSS(this,"marginTop",!0))||0,o=d+h+(parseInt(a.curCSS(this,"marginRight",!0))||0),p=g+i+(parseInt(a.curCSS(this,"marginBottom",!0))||0),q=a.extend({},n),r;b.my[0]==="right"?q.left-=d:b.my[0]===e&&(q.left-=d/2),b.my[1]==="bottom"?q.top-=g:b.my[1]===e&&(q.top-=g/2),f.fractions||(q.left=Math.round(q.left),q.top=Math.round(q.top)),r={left:q.left-h,top:q.top-i},a.each(["left","top"],function(c,e){a.ui.position[j[c]]&&a.ui.position[j[c]][e](q,{targetWidth:l,targetHeight:m,elemWidth:d,elemHeight:g,collisionPosition:r,collisionWidth:o,collisionHeight:p,offset:k,my:b.my,at:b.at})}),a.fn.bgiframe&&c.bgiframe(),c.offset(a.extend(q,{using:b.using}))})},a.ui.position={fit:{left:function(b,c){var d=a(window),e=c.collisionPosition.left+c.collisionWidth-d.width()-d.scrollLeft();b.left=e>0?b.left-e:Math.max(b.left-c.collisionPosition.left,b.left)},top:function(b,c){var d=a(window),e=c.collisionPosition.top+c.collisionHeight-d.height()-d.scrollTop();b.top=e>0?b.top-e:Math.max(b.top-c.collisionPosition.top,b.top)}},flip:{left:function(b,c){if(c.at[0]===e)return;var d=a(window),f=c.collisionPosition.left+c.collisionWidth-d.width()-d.scrollLeft(),g=c.my[0]==="left"?-c.elemWidth:c.my[0]==="right"?c.elemWidth:0,h=c.at[0]==="left"?c.targetWidth:-c.targetWidth,i=-2*c.offset[0];b.left+=c.collisionPosition.left<0?g+h+i:f>0?g+h+i:0},top:function(b,c){if(c.at[1]===e)return;var d=a(window),f=c.collisionPosition.top+c.collisionHeight-d.height()-d.scrollTop(),g=c.my[1]==="top"?-c.elemHeight:c.my[1]==="bottom"?c.elemHeight:0,h=c.at[1]==="top"?c.targetHeight:-c.targetHeight,i=-2*c.offset[1];b.top+=c.collisionPosition.top<0?g+h+i:f>0?g+h+i:0}}},a.offset.setOffset||(a.offset.setOffset=function(b,c){/static/.test(a.curCSS(b,"position"))&&(b.style.position="relative");var d=a(b),e=d.offset(),f=parseInt(a.curCSS(b,"top",!0),10)||0,g=parseInt(a.curCSS(b,"left",!0),10)||0,h={top:c.top-e.top+f,left:c.left-e.left+g};"using"in c?c.using.call(b,h):d.css(h)},a.fn.offset=function(b){var c=this[0];return!c||!c.ownerDocument?null:b?this.each(function(){a.offset.setOffset(this,b)}):h.call(this)}),function(){var b=document.getElementsByTagName("body")[0],c=document.createElement("div"),d,e,g,h,i;d=document.createElement(b?"div":"body"),g={visibility:"hidden",width:0,height:0,border:0,margin:0,background:"none"},b&&a.extend(g,{position:"absolute",left:"-1000px",top:"-1000px"});for(var j in g)d.style[j]=g[j];d.appendChild(c),e=b||document.documentElement,e.insertBefore(d,e.firstChild),c.style.cssText="position: absolute; left: 10.7432222px; top: 10.432325px; height: 30px; width: 201px;",h=a(c).offset(function(a,b){return b}).offset(),d.innerHTML="",e.removeChild(d),i=h.top+h.left+(b?2e3:0),f.fractions=i>21&&i<22}()})(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.draggable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.draggable", a.ui.mouse, {widgetEventPrefix:"drag", options:{addClasses:!0, appendTo:"parent", axis:!1, connectToSortable:!1, containment:!1, cursor:"auto", cursorAt:!1, grid:!1, handle:!1, helper:"original", iframeFix:!1, opacity:!1, refreshPositions:!1, revert:!1, revertDuration:500, scope:"default", scroll:!0, scrollSensitivity:20, scrollSpeed:20, snap:!1, snapMode:"both", snapTolerance:20, stack:!1, zIndex:!1}, _create:function () {
            this.options.helper == "original" && !/^(?:r|a|f)/.test(this.element.css("position")) && (this.element[0].style.position = "relative"), this.options.addClasses && this.element.addClass("ui-draggable"), this.options.disabled && this.element.addClass("ui-draggable-disabled"), this._mouseInit()
        }, destroy:function () {
            if (!this.element.data("draggable"))return;
            return this.element.removeData("draggable").unbind(".draggable").removeClass("ui-draggable ui-draggable-dragging ui-draggable-disabled"), this._mouseDestroy(), this
        }, _mouseCapture:function (b) {
            var c = this.options;
            return this.helper || c.disabled || a(b.target).is(".ui-resizable-handle") ? !1 : (this.handle = this._getHandle(b), this.handle ? (c.iframeFix && a(c.iframeFix === !0 ? "iframe" : c.iframeFix).each(function () {
                a('<div class="ui-draggable-iframeFix" style="background: #fff;"></div>').css({width:this.offsetWidth + "px", height:this.offsetHeight + "px", position:"absolute", opacity:"0.001", zIndex:1e3}).css(a(this).offset()).appendTo("body")
            }), !0) : !1)
        }, _mouseStart:function (b) {
            var c = this.options;
            return this.helper = this._createHelper(b), this._cacheHelperProportions(), a.ui.ddmanager && (a.ui.ddmanager.current = this), this._cacheMargins(), this.cssPosition = this.helper.css("position"), this.scrollParent = this.helper.scrollParent(), this.offset = this.positionAbs = this.element.offset(), this.offset = {top:this.offset.top - this.margins.top, left:this.offset.left - this.margins.left}, a.extend(this.offset, {click:{left:b.pageX - this.offset.left, top:b.pageY - this.offset.top}, parent:this._getParentOffset(), relative:this._getRelativeOffset()}), this.originalPosition = this.position = this._generatePosition(b), this.originalPageX = b.pageX, this.originalPageY = b.pageY, c.cursorAt && this._adjustOffsetFromHelper(c.cursorAt), c.containment && this._setContainment(), this._trigger("start", b) === !1 ? (this._clear(), !1) : (this._cacheHelperProportions(), a.ui.ddmanager && !c.dropBehaviour && a.ui.ddmanager.prepareOffsets(this, b), this.helper.addClass("ui-draggable-dragging"), this._mouseDrag(b, !0), a.ui.ddmanager && a.ui.ddmanager.dragStart(this, b), !0)
        }, _mouseDrag:function (b, c) {
            this.position = this._generatePosition(b), this.positionAbs = this._convertPositionTo("absolute");
            if (!c) {
                var d = this._uiHash();
                if (this._trigger("drag", b, d) === !1)return this._mouseUp({}), !1;
                this.position = d.position
            }
            if (!this.options.axis || this.options.axis != "y")this.helper[0].style.left = this.position.left + "px";
            if (!this.options.axis || this.options.axis != "x")this.helper[0].style.top = this.position.top + "px";
            return a.ui.ddmanager && a.ui.ddmanager.drag(this, b), !1
        }, _mouseStop:function (b) {
            var c = !1;
            a.ui.ddmanager && !this.options.dropBehaviour && (c = a.ui.ddmanager.drop(this, b)), this.dropped && (c = this.dropped, this.dropped = !1);
            if ((!this.element[0] || !this.element[0].parentNode) && this.options.helper == "original")return!1;
            if (this.options.revert == "invalid" && !c || this.options.revert == "valid" && c || this.options.revert === !0 || a.isFunction(this.options.revert) && this.options.revert.call(this.element, c)) {
                var d = this;
                a(this.helper).animate(this.originalPosition, parseInt(this.options.revertDuration, 10), function () {
                    d._trigger("stop", b) !== !1 && d._clear()
                })
            } else this._trigger("stop", b) !== !1 && this._clear();
            return!1
        }, _mouseUp:function (b) {
            return this.options.iframeFix === !0 && a("div.ui-draggable-iframeFix").each(function () {
                this.parentNode.removeChild(this)
            }), a.ui.ddmanager && a.ui.ddmanager.dragStop(this, b), a.ui.mouse.prototype._mouseUp.call(this, b)
        }, cancel:function () {
            return this.helper.is(".ui-draggable-dragging") ? this._mouseUp({}) : this._clear(), this
        }, _getHandle:function (b) {
            var c = !this.options.handle || !a(this.options.handle, this.element).length ? !0 : !1;
            return a(this.options.handle, this.element).find("*").andSelf().each(function () {
                this == b.target && (c = !0)
            }), c
        }, _createHelper:function (b) {
            var c = this.options, d = a.isFunction(c.helper) ? a(c.helper.apply(this.element[0], [b])) : c.helper == "clone" ? this.element.clone().removeAttr("id") : this.element;
            return d.parents("body").length || d.appendTo(c.appendTo == "parent" ? this.element[0].parentNode : c.appendTo), d[0] != this.element[0] && !/(fixed|absolute)/.test(d.css("position")) && d.css("position", "absolute"), d
        }, _adjustOffsetFromHelper:function (b) {
            typeof b == "string" && (b = b.split(" ")), a.isArray(b) && (b = {left:+b[0], top:+b[1] || 0}), "left"in b && (this.offset.click.left = b.left + this.margins.left), "right"in b && (this.offset.click.left = this.helperProportions.width - b.right + this.margins.left), "top"in b && (this.offset.click.top = b.top + this.margins.top), "bottom"in b && (this.offset.click.top = this.helperProportions.height - b.bottom + this.margins.top)
        }, _getParentOffset:function () {
            this.offsetParent = this.helper.offsetParent();
            var b = this.offsetParent.offset();
            this.cssPosition == "absolute" && this.scrollParent[0] != document && a.ui.contains(this.scrollParent[0], this.offsetParent[0]) && (b.left += this.scrollParent.scrollLeft(), b.top += this.scrollParent.scrollTop());
            if (this.offsetParent[0] == document.body || this.offsetParent[0].tagName && this.offsetParent[0].tagName.toLowerCase() == "html" && a.browser.msie)b = {top:0, left:0};
            return{top:b.top + (parseInt(this.offsetParent.css("borderTopWidth"), 10) || 0), left:b.left + (parseInt(this.offsetParent.css("borderLeftWidth"), 10) || 0)}
        }, _getRelativeOffset:function () {
            if (this.cssPosition == "relative") {
                var a = this.element.position();
                return{top:a.top - (parseInt(this.helper.css("top"), 10) || 0) + this.scrollParent.scrollTop(), left:a.left - (parseInt(this.helper.css("left"), 10) || 0) + this.scrollParent.scrollLeft()}
            }
            return{top:0, left:0}
        }, _cacheMargins:function () {
            this.margins = {left:parseInt(this.element.css("marginLeft"), 10) || 0, top:parseInt(this.element.css("marginTop"), 10) || 0, right:parseInt(this.element.css("marginRight"), 10) || 0, bottom:parseInt(this.element.css("marginBottom"), 10) || 0}
        }, _cacheHelperProportions:function () {
            this.helperProportions = {width:this.helper.outerWidth(), height:this.helper.outerHeight()}
        }, _setContainment:function () {
            var b = this.options;
            b.containment == "parent" && (b.containment = this.helper[0].parentNode);
            if (b.containment == "document" || b.containment == "window")this.containment = [b.containment == "document" ? 0 : a(window).scrollLeft() - this.offset.relative.left - this.offset.parent.left, b.containment == "document" ? 0 : a(window).scrollTop() - this.offset.relative.top - this.offset.parent.top, (b.containment == "document" ? 0 : a(window).scrollLeft()) + a(b.containment == "document" ? document : window).width() - this.helperProportions.width - this.margins.left, (b.containment == "document" ? 0 : a(window).scrollTop()) + (a(b.containment == "document" ? document : window).height() || document.body.parentNode.scrollHeight) - this.helperProportions.height - this.margins.top];
            if (!/^(document|window|parent)$/.test(b.containment) && b.containment.constructor != Array) {
                var c = a(b.containment), d = c[0];
                if (!d)return;
                var e = c.offset(), f = a(d).css("overflow") != "hidden";
                this.containment = [(parseInt(a(d).css("borderLeftWidth"), 10) || 0) + (parseInt(a(d).css("paddingLeft"), 10) || 0), (parseInt(a(d).css("borderTopWidth"), 10) || 0) + (parseInt(a(d).css("paddingTop"), 10) || 0), (f ? Math.max(d.scrollWidth, d.offsetWidth) : d.offsetWidth) - (parseInt(a(d).css("borderLeftWidth"), 10) || 0) - (parseInt(a(d).css("paddingRight"), 10) || 0) - this.helperProportions.width - this.margins.left - this.margins.right, (f ? Math.max(d.scrollHeight, d.offsetHeight) : d.offsetHeight) - (parseInt(a(d).css("borderTopWidth"), 10) || 0) - (parseInt(a(d).css("paddingBottom"), 10) || 0) - this.helperProportions.height - this.margins.top - this.margins.bottom], this.relative_container = c
            } else b.containment.constructor == Array && (this.containment = b.containment)
        }, _convertPositionTo:function (b, c) {
            c || (c = this.position);
            var d = b == "absolute" ? 1 : -1, e = this.options, f = this.cssPosition == "absolute" && (this.scrollParent[0] == document || !a.ui.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent, g = /(html|body)/i.test(f[0].tagName);
            return{top:c.top + this.offset.relative.top * d + this.offset.parent.top * d - (a.browser.safari && a.browser.version < 526 && this.cssPosition == "fixed" ? 0 : (this.cssPosition == "fixed" ? -this.scrollParent.scrollTop() : g ? 0 : f.scrollTop()) * d), left:c.left + this.offset.relative.left * d + this.offset.parent.left * d - (a.browser.safari && a.browser.version < 526 && this.cssPosition == "fixed" ? 0 : (this.cssPosition == "fixed" ? -this.scrollParent.scrollLeft() : g ? 0 : f.scrollLeft()) * d)}
        }, _generatePosition:function (b) {
            var c = this.options, d = this.cssPosition == "absolute" && (this.scrollParent[0] == document || !a.ui.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent, e = /(html|body)/i.test(d[0].tagName), f = b.pageX, g = b.pageY;
            if (this.originalPosition) {
                var h;
                if (this.containment) {
                    if (this.relative_container) {
                        var i = this.relative_container.offset();
                        h = [this.containment[0] + i.left, this.containment[1] + i.top, this.containment[2] + i.left, this.containment[3] + i.top]
                    } else h = this.containment;
                    b.pageX - this.offset.click.left < h[0] && (f = h[0] + this.offset.click.left), b.pageY - this.offset.click.top < h[1] && (g = h[1] + this.offset.click.top), b.pageX - this.offset.click.left > h[2] && (f = h[2] + this.offset.click.left), b.pageY - this.offset.click.top > h[3] && (g = h[3] + this.offset.click.top)
                }
                if (c.grid) {
                    var j = c.grid[1] ? this.originalPageY + Math.round((g - this.originalPageY) / c.grid[1]) * c.grid[1] : this.originalPageY;
                    g = h ? j - this.offset.click.top < h[1] || j - this.offset.click.top > h[3] ? j - this.offset.click.top < h[1] ? j + c.grid[1] : j - c.grid[1] : j : j;
                    var k = c.grid[0] ? this.originalPageX + Math.round((f - this.originalPageX) / c.grid[0]) * c.grid[0] : this.originalPageX;
                    f = h ? k - this.offset.click.left < h[0] || k - this.offset.click.left > h[2] ? k - this.offset.click.left < h[0] ? k + c.grid[0] : k - c.grid[0] : k : k
                }
            }
            return{top:g - this.offset.click.top - this.offset.relative.top - this.offset.parent.top + (a.browser.safari && a.browser.version < 526 && this.cssPosition == "fixed" ? 0 : this.cssPosition == "fixed" ? -this.scrollParent.scrollTop() : e ? 0 : d.scrollTop()), left:f - this.offset.click.left - this.offset.relative.left - this.offset.parent.left + (a.browser.safari && a.browser.version < 526 && this.cssPosition == "fixed" ? 0 : this.cssPosition == "fixed" ? -this.scrollParent.scrollLeft() : e ? 0 : d.scrollLeft())}
        }, _clear:function () {
            this.helper.removeClass("ui-draggable-dragging"), this.helper[0] != this.element[0] && !this.cancelHelperRemoval && this.helper.remove(), this.helper = null, this.cancelHelperRemoval = !1
        }, _trigger:function (b, c, d) {
            return d = d || this._uiHash(), a.ui.plugin.call(this, b, [c, d]), b == "drag" && (this.positionAbs = this._convertPositionTo("absolute")), a.Widget.prototype._trigger.call(this, b, c, d)
        }, plugins:{}, _uiHash:function (a) {
            return{helper:this.helper, position:this.position, originalPosition:this.originalPosition, offset:this.positionAbs}
        }}), a.extend(a.ui.draggable, {version:"1.8.19"}), a.ui.plugin.add("draggable", "connectToSortable", {start:function (b, c) {
            var d = a(this).data("draggable"), e = d.options, f = a.extend({}, c, {item:d.element});
            d.sortables = [], a(e.connectToSortable).each(function () {
                var c = a.data(this, "sortable");
                c && !c.options.disabled && (d.sortables.push({instance:c, shouldRevert:c.options.revert}), c.refreshPositions(), c._trigger("activate", b, f))
            })
        }, stop:function (b, c) {
            var d = a(this).data("draggable"), e = a.extend({}, c, {item:d.element});
            a.each(d.sortables, function () {
                this.instance.isOver ? (this.instance.isOver = 0, d.cancelHelperRemoval = !0, this.instance.cancelHelperRemoval = !1, this.shouldRevert && (this.instance.options.revert = !0), this.instance._mouseStop(b), this.instance.options.helper = this.instance.options._helper, d.options.helper == "original" && this.instance.currentItem.css({top:"auto", left:"auto"})) : (this.instance.cancelHelperRemoval = !1, this.instance._trigger("deactivate", b, e))
            })
        }, drag:function (b, c) {
            var d = a(this).data("draggable"), e = this, f = function (b) {
                var c = this.offset.click.top, d = this.offset.click.left, e = this.positionAbs.top, f = this.positionAbs.left, g = b.height, h = b.width, i = b.top, j = b.left;
                return a.ui.isOver(e + c, f + d, i, j, g, h)
            };
            a.each(d.sortables, function (f) {
                this.instance.positionAbs = d.positionAbs, this.instance.helperProportions = d.helperProportions, this.instance.offset.click = d.offset.click, this.instance._intersectsWith(this.instance.containerCache) ? (this.instance.isOver || (this.instance.isOver = 1, this.instance.currentItem = a(e).clone().removeAttr("id").appendTo(this.instance.element).data("sortable-item", !0), this.instance.options._helper = this.instance.options.helper, this.instance.options.helper = function () {
                    return c.helper[0]
                }, b.target = this.instance.currentItem[0], this.instance._mouseCapture(b, !0), this.instance._mouseStart(b, !0, !0), this.instance.offset.click.top = d.offset.click.top, this.instance.offset.click.left = d.offset.click.left, this.instance.offset.parent.left -= d.offset.parent.left - this.instance.offset.parent.left, this.instance.offset.parent.top -= d.offset.parent.top - this.instance.offset.parent.top, d._trigger("toSortable", b), d.dropped = this.instance.element, d.currentItem = d.element, this.instance.fromOutside = d), this.instance.currentItem && this.instance._mouseDrag(b)) : this.instance.isOver && (this.instance.isOver = 0, this.instance.cancelHelperRemoval = !0, this.instance.options.revert = !1, this.instance._trigger("out", b, this.instance._uiHash(this.instance)), this.instance._mouseStop(b, !0), this.instance.options.helper = this.instance.options._helper, this.instance.currentItem.remove(), this.instance.placeholder && this.instance.placeholder.remove(), d._trigger("fromSortable", b), d.dropped = !1)
            })
        }}), a.ui.plugin.add("draggable", "cursor", {start:function (b, c) {
            var d = a("body"), e = a(this).data("draggable").options;
            d.css("cursor") && (e._cursor = d.css("cursor")), d.css("cursor", e.cursor)
        }, stop:function (b, c) {
            var d = a(this).data("draggable").options;
            d._cursor && a("body").css("cursor", d._cursor)
        }}), a.ui.plugin.add("draggable", "opacity", {start:function (b, c) {
            var d = a(c.helper), e = a(this).data("draggable").options;
            d.css("opacity") && (e._opacity = d.css("opacity")), d.css("opacity", e.opacity)
        }, stop:function (b, c) {
            var d = a(this).data("draggable").options;
            d._opacity && a(c.helper).css("opacity", d._opacity)
        }}), a.ui.plugin.add("draggable", "scroll", {start:function (b, c) {
            var d = a(this).data("draggable");
            d.scrollParent[0] != document && d.scrollParent[0].tagName != "HTML" && (d.overflowOffset = d.scrollParent.offset())
        }, drag:function (b, c) {
            var d = a(this).data("draggable"), e = d.options, f = !1;
            if (d.scrollParent[0] != document && d.scrollParent[0].tagName != "HTML") {
                if (!e.axis || e.axis != "x")d.overflowOffset.top + d.scrollParent[0].offsetHeight - b.pageY < e.scrollSensitivity ? d.scrollParent[0].scrollTop = f = d.scrollParent[0].scrollTop + e.scrollSpeed : b.pageY - d.overflowOffset.top < e.scrollSensitivity && (d.scrollParent[0].scrollTop = f = d.scrollParent[0].scrollTop - e.scrollSpeed);
                if (!e.axis || e.axis != "y")d.overflowOffset.left + d.scrollParent[0].offsetWidth - b.pageX < e.scrollSensitivity ? d.scrollParent[0].scrollLeft = f = d.scrollParent[0].scrollLeft + e.scrollSpeed : b.pageX - d.overflowOffset.left < e.scrollSensitivity && (d.scrollParent[0].scrollLeft = f = d.scrollParent[0].scrollLeft - e.scrollSpeed)
            } else {
                if (!e.axis || e.axis != "x")b.pageY - a(document).scrollTop() < e.scrollSensitivity ? f = a(document).scrollTop(a(document).scrollTop() - e.scrollSpeed) : a(window).height() - (b.pageY - a(document).scrollTop()) < e.scrollSensitivity && (f = a(document).scrollTop(a(document).scrollTop() + e.scrollSpeed));
                if (!e.axis || e.axis != "y")b.pageX - a(document).scrollLeft() < e.scrollSensitivity ? f = a(document).scrollLeft(a(document).scrollLeft() - e.scrollSpeed) : a(window).width() - (b.pageX - a(document).scrollLeft()) < e.scrollSensitivity && (f = a(document).scrollLeft(a(document).scrollLeft() + e.scrollSpeed))
            }
            f !== !1 && a.ui.ddmanager && !e.dropBehaviour && a.ui.ddmanager.prepareOffsets(d, b)
        }}), a.ui.plugin.add("draggable", "snap", {start:function (b, c) {
            var d = a(this).data("draggable"), e = d.options;
            d.snapElements = [], a(e.snap.constructor != String ? e.snap.items || ":data(draggable)" : e.snap).each(function () {
                var b = a(this), c = b.offset();
                this != d.element[0] && d.snapElements.push({item:this, width:b.outerWidth(), height:b.outerHeight(), top:c.top, left:c.left})
            })
        }, drag:function (b, c) {
            var d = a(this).data("draggable"), e = d.options, f = e.snapTolerance, g = c.offset.left, h = g + d.helperProportions.width, i = c.offset.top, j = i + d.helperProportions.height;
            for (var k = d.snapElements.length - 1; k >= 0; k--) {
                var l = d.snapElements[k].left, m = l + d.snapElements[k].width, n = d.snapElements[k].top, o = n + d.snapElements[k].height;
                if (!(l - f < g && g < m + f && n - f < i && i < o + f || l - f < g && g < m + f && n - f < j && j < o + f || l - f < h && h < m + f && n - f < i && i < o + f || l - f < h && h < m + f && n - f < j && j < o + f)) {
                    d.snapElements[k].snapping && d.options.snap.release && d.options.snap.release.call(d.element, b, a.extend(d._uiHash(), {snapItem:d.snapElements[k].item})), d.snapElements[k].snapping = !1;
                    continue
                }
                if (e.snapMode != "inner") {
                    var p = Math.abs(n - j) <= f, q = Math.abs(o - i) <= f, r = Math.abs(l - h) <= f, s = Math.abs(m - g) <= f;
                    p && (c.position.top = d._convertPositionTo("relative", {top:n - d.helperProportions.height, left:0}).top - d.margins.top), q && (c.position.top = d._convertPositionTo("relative", {top:o, left:0}).top - d.margins.top), r && (c.position.left = d._convertPositionTo("relative", {top:0, left:l - d.helperProportions.width}).left - d.margins.left), s && (c.position.left = d._convertPositionTo("relative", {top:0, left:m}).left - d.margins.left)
                }
                var t = p || q || r || s;
                if (e.snapMode != "outer") {
                    var p = Math.abs(n - i) <= f, q = Math.abs(o - j) <= f, r = Math.abs(l - g) <= f, s = Math.abs(m - h) <= f;
                    p && (c.position.top = d._convertPositionTo("relative", {top:n, left:0}).top - d.margins.top), q && (c.position.top = d._convertPositionTo("relative", {top:o - d.helperProportions.height, left:0}).top - d.margins.top), r && (c.position.left = d._convertPositionTo("relative", {top:0, left:l}).left - d.margins.left), s && (c.position.left = d._convertPositionTo("relative", {top:0, left:m - d.helperProportions.width}).left - d.margins.left)
                }
                !d.snapElements[k].snapping && (p || q || r || s || t) && d.options.snap.snap && d.options.snap.snap.call(d.element, b, a.extend(d._uiHash(), {snapItem:d.snapElements[k].item})), d.snapElements[k].snapping = p || q || r || s || t
            }
        }}), a.ui.plugin.add("draggable", "stack", {start:function (b, c) {
            var d = a(this).data("draggable").options, e = a.makeArray(a(d.stack)).sort(function (b, c) {
                return(parseInt(a(b).css("zIndex"), 10) || 0) - (parseInt(a(c).css("zIndex"), 10) || 0)
            });
            if (!e.length)return;
            var f = parseInt(e[0].style.zIndex) || 0;
            a(e).each(function (a) {
                this.style.zIndex = f + a
            }), this[0].style.zIndex = f + e.length
        }}), a.ui.plugin.add("draggable", "zIndex", {start:function (b, c) {
            var d = a(c.helper), e = a(this).data("draggable").options;
            d.css("zIndex") && (e._zIndex = d.css("zIndex")), d.css("zIndex", e.zIndex)
        }, stop:function (b, c) {
            var d = a(this).data("draggable").options;
            d._zIndex && a(c.helper).css("zIndex", d._zIndex)
        }})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.droppable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.droppable", {widgetEventPrefix:"drop", options:{accept:"*", activeClass:!1, addClasses:!0, greedy:!1, hoverClass:!1, scope:"default", tolerance:"intersect"}, _create:function () {
            var b = this.options, c = b.accept;
            this.isover = 0, this.isout = 1, this.accept = a.isFunction(c) ? c : function (a) {
                return a.is(c)
            }, this.proportions = {width:this.element[0].offsetWidth, height:this.element[0].offsetHeight}, a.ui.ddmanager.droppables[b.scope] = a.ui.ddmanager.droppables[b.scope] || [], a.ui.ddmanager.droppables[b.scope].push(this), b.addClasses && this.element.addClass("ui-droppable")
        }, destroy:function () {
            var b = a.ui.ddmanager.droppables[this.options.scope];
            for (var c = 0; c < b.length; c++)b[c] == this && b.splice(c, 1);
            return this.element.removeClass("ui-droppable ui-droppable-disabled").removeData("droppable").unbind(".droppable"), this
        }, _setOption:function (b, c) {
            b == "accept" && (this.accept = a.isFunction(c) ? c : function (a) {
                return a.is(c)
            }), a.Widget.prototype._setOption.apply(this, arguments)
        }, _activate:function (b) {
            var c = a.ui.ddmanager.current;
            this.options.activeClass && this.element.addClass(this.options.activeClass), c && this._trigger("activate", b, this.ui(c))
        }, _deactivate:function (b) {
            var c = a.ui.ddmanager.current;
            this.options.activeClass && this.element.removeClass(this.options.activeClass), c && this._trigger("deactivate", b, this.ui(c))
        }, _over:function (b) {
            var c = a.ui.ddmanager.current;
            if (!c || (c.currentItem || c.element)[0] == this.element[0])return;
            this.accept.call(this.element[0], c.currentItem || c.element) && (this.options.hoverClass && this.element.addClass(this.options.hoverClass), this._trigger("over", b, this.ui(c)))
        }, _out:function (b) {
            var c = a.ui.ddmanager.current;
            if (!c || (c.currentItem || c.element)[0] == this.element[0])return;
            this.accept.call(this.element[0], c.currentItem || c.element) && (this.options.hoverClass && this.element.removeClass(this.options.hoverClass), this._trigger("out", b, this.ui(c)))
        }, _drop:function (b, c) {
            var d = c || a.ui.ddmanager.current;
            if (!d || (d.currentItem || d.element)[0] == this.element[0])return!1;
            var e = !1;
            return this.element.find(":data(droppable)").not(".ui-draggable-dragging").each(function () {
                var b = a.data(this, "droppable");
                if (b.options.greedy && !b.options.disabled && b.options.scope == d.options.scope && b.accept.call(b.element[0], d.currentItem || d.element) && a.ui.intersect(d, a.extend(b, {offset:b.element.offset()}), b.options.tolerance))return e = !0, !1
            }), e ? !1 : this.accept.call(this.element[0], d.currentItem || d.element) ? (this.options.activeClass && this.element.removeClass(this.options.activeClass), this.options.hoverClass && this.element.removeClass(this.options.hoverClass), this._trigger("drop", b, this.ui(d)), this.element) : !1
        }, ui:function (a) {
            return{draggable:a.currentItem || a.element, helper:a.helper, position:a.position, offset:a.positionAbs}
        }}), a.extend(a.ui.droppable, {version:"1.8.19"}), a.ui.intersect = function (b, c, d) {
            if (!c.offset)return!1;
            var e = (b.positionAbs || b.position.absolute).left, f = e + b.helperProportions.width, g = (b.positionAbs || b.position.absolute).top, h = g + b.helperProportions.height, i = c.offset.left, j = i + c.proportions.width, k = c.offset.top, l = k + c.proportions.height;
            switch (d) {
                case"fit":
                    return i <= e && f <= j && k <= g && h <= l;
                case"intersect":
                    return i < e + b.helperProportions.width / 2 && f - b.helperProportions.width / 2 < j && k < g + b.helperProportions.height / 2 && h - b.helperProportions.height / 2 < l;
                case"pointer":
                    var m = (b.positionAbs || b.position.absolute).left + (b.clickOffset || b.offset.click).left, n = (b.positionAbs || b.position.absolute).top + (b.clickOffset || b.offset.click).top, o = a.ui.isOver(n, m, k, i, c.proportions.height, c.proportions.width);
                    return o;
                case"touch":
                    return(g >= k && g <= l || h >= k && h <= l || g < k && h > l) && (e >= i && e <= j || f >= i && f <= j || e < i && f > j);
                default:
                    return!1
            }
        }, a.ui.ddmanager = {current:null, droppables:{"default":[]}, prepareOffsets:function (b, c) {
            var d = a.ui.ddmanager.droppables[b.options.scope] || [], e = c ? c.type : null, f = (b.currentItem || b.element).find(":data(droppable)").andSelf();
            g:for (var h = 0; h < d.length; h++) {
                if (d[h].options.disabled || b && !d[h].accept.call(d[h].element[0], b.currentItem || b.element))continue;
                for (var i = 0; i < f.length; i++)if (f[i] == d[h].element[0]) {
                    d[h].proportions.height = 0;
                    continue g
                }
                d[h].visible = d[h].element.css("display") != "none";
                if (!d[h].visible)continue;
                e == "mousedown" && d[h]._activate.call(d[h], c), d[h].offset = d[h].element.offset(), d[h].proportions = {width:d[h].element[0].offsetWidth, height:d[h].element[0].offsetHeight}
            }
        }, drop:function (b, c) {
            var d = !1;
            return a.each(a.ui.ddmanager.droppables[b.options.scope] || [], function () {
                if (!this.options)return;
                !this.options.disabled && this.visible && a.ui.intersect(b, this, this.options.tolerance) && (d = this._drop.call(this, c) || d), !this.options.disabled && this.visible && this.accept.call(this.element[0], b.currentItem || b.element) && (this.isout = 1, this.isover = 0, this._deactivate.call(this, c))
            }), d
        }, dragStart:function (b, c) {
            b.element.parents(":not(body,html)").bind("scroll.droppable", function () {
                b.options.refreshPositions || a.ui.ddmanager.prepareOffsets(b, c)
            })
        }, drag:function (b, c) {
            b.options.refreshPositions && a.ui.ddmanager.prepareOffsets(b, c), a.each(a.ui.ddmanager.droppables[b.options.scope] || [], function () {
                if (this.options.disabled || this.greedyChild || !this.visible)return;
                var d = a.ui.intersect(b, this, this.options.tolerance), e = !d && this.isover == 1 ? "isout" : d && this.isover == 0 ? "isover" : null;
                if (!e)return;
                var f;
                if (this.options.greedy) {
                    var g = this.element.parents(":data(droppable):eq(0)");
                    g.length && (f = a.data(g[0], "droppable"), f.greedyChild = e == "isover" ? 1 : 0)
                }
                f && e == "isover" && (f.isover = 0, f.isout = 1, f._out.call(f, c)), this[e] = 1, this[e == "isout" ? "isover" : "isout"] = 0, this[e == "isover" ? "_over" : "_out"].call(this, c), f && e == "isout" && (f.isout = 0, f.isover = 1, f._over.call(f, c))
            })
        }, dragStop:function (b, c) {
            b.element.parents(":not(body,html)").unbind("scroll.droppable"), b.options.refreshPositions || a.ui.ddmanager.prepareOffsets(b, c)
        }}
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.resizable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.resizable", a.ui.mouse, {widgetEventPrefix:"resize", options:{alsoResize:!1, animate:!1, animateDuration:"slow", animateEasing:"swing", aspectRatio:!1, autoHide:!1, containment:!1, ghost:!1, grid:!1, handles:"e,s,se", helper:!1, maxHeight:null, maxWidth:null, minHeight:10, minWidth:10, zIndex:1e3}, _create:function () {
            var b = this, c = this.options;
            this.element.addClass("ui-resizable"), a.extend(this, {_aspectRatio:!!c.aspectRatio, aspectRatio:c.aspectRatio, originalElement:this.element, _proportionallyResizeElements:[], _helper:c.helper || c.ghost || c.animate ? c.helper || "ui-resizable-helper" : null}), this.element[0].nodeName.match(/canvas|textarea|input|select|button|img/i) && (this.element.wrap(a('<div class="ui-wrapper" style="overflow: hidden;"></div>').css({position:this.element.css("position"), width:this.element.outerWidth(), height:this.element.outerHeight(), top:this.element.css("top"), left:this.element.css("left")})), this.element = this.element.parent().data("resizable", this.element.data("resizable")), this.elementIsWrapper = !0, this.element.css({marginLeft:this.originalElement.css("marginLeft"), marginTop:this.originalElement.css("marginTop"), marginRight:this.originalElement.css("marginRight"), marginBottom:this.originalElement.css("marginBottom")}), this.originalElement.css({marginLeft:0, marginTop:0, marginRight:0, marginBottom:0}), this.originalResizeStyle = this.originalElement.css("resize"), this.originalElement.css("resize", "none"), this._proportionallyResizeElements.push(this.originalElement.css({position:"static", zoom:1, display:"block"})), this.originalElement.css({margin:this.originalElement.css("margin")}), this._proportionallyResize()), this.handles = c.handles || (a(".ui-resizable-handle", this.element).length ? {n:".ui-resizable-n", e:".ui-resizable-e", s:".ui-resizable-s", w:".ui-resizable-w", se:".ui-resizable-se", sw:".ui-resizable-sw", ne:".ui-resizable-ne", nw:".ui-resizable-nw"} : "e,s,se");
            if (this.handles.constructor == String) {
                this.handles == "all" && (this.handles = "n,e,s,w,se,sw,ne,nw");
                var d = this.handles.split(",");
                this.handles = {};
                for (var e = 0; e < d.length; e++) {
                    var f = a.trim(d[e]), g = "ui-resizable-" + f, h = a('<div class="ui-resizable-handle ' + g + '"></div>');
                    /sw|se|ne|nw/.test(f) && h.css({zIndex:++c.zIndex}), "se" == f && h.addClass("ui-icon ui-icon-gripsmall-diagonal-se"), this.handles[f] = ".ui-resizable-" + f, this.element.append(h)
                }
            }
            this._renderAxis = function (b) {
                b = b || this.element;
                for (var c in this.handles) {
                    this.handles[c].constructor == String && (this.handles[c] = a(this.handles[c], this.element).show());
                    if (this.elementIsWrapper && this.originalElement[0].nodeName.match(/textarea|input|select|button/i)) {
                        var d = a(this.handles[c], this.element), e = 0;
                        e = /sw|ne|nw|se|n|s/.test(c) ? d.outerHeight() : d.outerWidth();
                        var f = ["padding", /ne|nw|n/.test(c) ? "Top" : /se|sw|s/.test(c) ? "Bottom" : /^e$/.test(c) ? "Right" : "Left"].join("");
                        b.css(f, e), this._proportionallyResize()
                    }
                    if (!a(this.handles[c]).length)continue
                }
            }, this._renderAxis(this.element), this._handles = a(".ui-resizable-handle", this.element).disableSelection(), this._handles.mouseover(function () {
                if (!b.resizing) {
                    if (this.className)var a = this.className.match(/ui-resizable-(se|sw|ne|nw|n|e|s|w)/i);
                    b.axis = a && a[1] ? a[1] : "se"
                }
            }), c.autoHide && (this._handles.hide(), a(this.element).addClass("ui-resizable-autohide").hover(function () {
                if (c.disabled)return;
                a(this).removeClass("ui-resizable-autohide"), b._handles.show()
            }, function () {
                if (c.disabled)return;
                b.resizing || (a(this).addClass("ui-resizable-autohide"), b._handles.hide())
            })), this._mouseInit()
        }, destroy:function () {
            this._mouseDestroy();
            var b = function (b) {
                a(b).removeClass("ui-resizable ui-resizable-disabled ui-resizable-resizing").removeData("resizable").unbind(".resizable").find(".ui-resizable-handle").remove()
            };
            if (this.elementIsWrapper) {
                b(this.element);
                var c = this.element;
                c.after(this.originalElement.css({position:c.css("position"), width:c.outerWidth(), height:c.outerHeight(), top:c.css("top"), left:c.css("left")})).remove()
            }
            return this.originalElement.css("resize", this.originalResizeStyle), b(this.originalElement), this
        }, _mouseCapture:function (b) {
            var c = !1;
            for (var d in this.handles)a(this.handles[d])[0] == b.target && (c = !0);
            return!this.options.disabled && c
        }, _mouseStart:function (b) {
            var d = this.options, e = this.element.position(), f = this.element;
            this.resizing = !0, this.documentScroll = {top:a(document).scrollTop(), left:a(document).scrollLeft()}, (f.is(".ui-draggable") || /absolute/.test(f.css("position"))) && f.css({position:"absolute", top:e.top, left:e.left}), this._renderProxy();
            var g = c(this.helper.css("left")), h = c(this.helper.css("top"));
            d.containment && (g += a(d.containment).scrollLeft() || 0, h += a(d.containment).scrollTop() || 0), this.offset = this.helper.offset(), this.position = {left:g, top:h}, this.size = this._helper ? {width:f.outerWidth(), height:f.outerHeight()} : {width:f.width(), height:f.height()}, this.originalSize = this._helper ? {width:f.outerWidth(), height:f.outerHeight()} : {width:f.width(), height:f.height()}, this.originalPosition = {left:g, top:h}, this.sizeDiff = {width:f.outerWidth() - f.width(), height:f.outerHeight() - f.height()}, this.originalMousePosition = {left:b.pageX, top:b.pageY}, this.aspectRatio = typeof d.aspectRatio == "number" ? d.aspectRatio : this.originalSize.width / this.originalSize.height || 1;
            var i = a(".ui-resizable-" + this.axis).css("cursor");
            return a("body").css("cursor", i == "auto" ? this.axis + "-resize" : i), f.addClass("ui-resizable-resizing"), this._propagate("start", b), !0
        }, _mouseDrag:function (b) {
            var c = this.helper, d = this.options, e = {}, f = this, g = this.originalMousePosition, h = this.axis, i = b.pageX - g.left || 0, j = b.pageY - g.top || 0, k = this._change[h];
            if (!k)return!1;
            var l = k.apply(this, [b, i, j]), m = a.browser.msie && a.browser.version < 7, n = this.sizeDiff;
            this._updateVirtualBoundaries(b.shiftKey);
            if (this._aspectRatio || b.shiftKey)l = this._updateRatio(l, b);
            return l = this._respectSize(l, b), this._propagate("resize", b), c.css({top:this.position.top + "px", left:this.position.left + "px", width:this.size.width + "px", height:this.size.height + "px"}), !this._helper && this._proportionallyResizeElements.length && this._proportionallyResize(), this._updateCache(l), this._trigger("resize", b, this.ui()), !1
        }, _mouseStop:function (b) {
            this.resizing = !1;
            var c = this.options, d = this;
            if (this._helper) {
                var e = this._proportionallyResizeElements, f = e.length && /textarea/i.test(e[0].nodeName), g = f && a.ui.hasScroll(e[0], "left") ? 0 : d.sizeDiff.height, h = f ? 0 : d.sizeDiff.width, i = {width:d.helper.width() - h, height:d.helper.height() - g}, j = parseInt(d.element.css("left"), 10) + (d.position.left - d.originalPosition.left) || null, k = parseInt(d.element.css("top"), 10) + (d.position.top - d.originalPosition.top) || null;
                c.animate || this.element.css(a.extend(i, {top:k, left:j})), d.helper.height(d.size.height), d.helper.width(d.size.width), this._helper && !c.animate && this._proportionallyResize()
            }
            return a("body").css("cursor", "auto"), this.element.removeClass("ui-resizable-resizing"), this._propagate("stop", b), this._helper && this.helper.remove(), !1
        }, _updateVirtualBoundaries:function (a) {
            var b = this.options, c, e, f, g, h;
            h = {minWidth:d(b.minWidth) ? b.minWidth : 0, maxWidth:d(b.maxWidth) ? b.maxWidth : Infinity, minHeight:d(b.minHeight) ? b.minHeight : 0, maxHeight:d(b.maxHeight) ? b.maxHeight : Infinity};
            if (this._aspectRatio || a)c = h.minHeight * this.aspectRatio, f = h.minWidth / this.aspectRatio, e = h.maxHeight * this.aspectRatio, g = h.maxWidth / this.aspectRatio, c > h.minWidth && (h.minWidth = c), f > h.minHeight && (h.minHeight = f), e < h.maxWidth && (h.maxWidth = e), g < h.maxHeight && (h.maxHeight = g);
            this._vBoundaries = h
        }, _updateCache:function (a) {
            var b = this.options;
            this.offset = this.helper.offset(), d(a.left) && (this.position.left = a.left), d(a.top) && (this.position.top = a.top), d(a.height) && (this.size.height = a.height), d(a.width) && (this.size.width = a.width)
        }, _updateRatio:function (a, b) {
            var c = this.options, e = this.position, f = this.size, g = this.axis;
            return d(a.height) ? a.width = a.height * this.aspectRatio : d(a.width) && (a.height = a.width / this.aspectRatio), g == "sw" && (a.left = e.left + (f.width - a.width), a.top = null), g == "nw" && (a.top = e.top + (f.height - a.height), a.left = e.left + (f.width - a.width)), a
        }, _respectSize:function (a, b) {
            var c = this.helper, e = this._vBoundaries, f = this._aspectRatio || b.shiftKey, g = this.axis, h = d(a.width) && e.maxWidth && e.maxWidth < a.width, i = d(a.height) && e.maxHeight && e.maxHeight < a.height, j = d(a.width) && e.minWidth && e.minWidth > a.width, k = d(a.height) && e.minHeight && e.minHeight > a.height;
            j && (a.width = e.minWidth), k && (a.height = e.minHeight), h && (a.width = e.maxWidth), i && (a.height = e.maxHeight);
            var l = this.originalPosition.left + this.originalSize.width, m = this.position.top + this.size.height, n = /sw|nw|w/.test(g), o = /nw|ne|n/.test(g);
            j && n && (a.left = l - e.minWidth), h && n && (a.left = l - e.maxWidth), k && o && (a.top = m - e.minHeight), i && o && (a.top = m - e.maxHeight);
            var p = !a.width && !a.height;
            return p && !a.left && a.top ? a.top = null : p && !a.top && a.left && (a.left = null), a
        }, _proportionallyResize:function () {
            var b = this.options;
            if (!this._proportionallyResizeElements.length)return;
            var c = this.helper || this.element;
            for (var d = 0; d < this._proportionallyResizeElements.length; d++) {
                var e = this._proportionallyResizeElements[d];
                if (!this.borderDif) {
                    var f = [e.css("borderTopWidth"), e.css("borderRightWidth"), e.css("borderBottomWidth"), e.css("borderLeftWidth")], g = [e.css("paddingTop"), e.css("paddingRight"), e.css("paddingBottom"), e.css("paddingLeft")];
                    this.borderDif = a.map(f, function (a, b) {
                        var c = parseInt(a, 10) || 0, d = parseInt(g[b], 10) || 0;
                        return c + d
                    })
                }
                if (!a.browser.msie || !a(c).is(":hidden") && !a(c).parents(":hidden").length)e.css({height:c.height() - this.borderDif[0] - this.borderDif[2] || 0, width:c.width() - this.borderDif[1] - this.borderDif[3] || 0}); else continue
            }
        }, _renderProxy:function () {
            var b = this.element, c = this.options;
            this.elementOffset = b.offset();
            if (this._helper) {
                this.helper = this.helper || a('<div style="overflow:hidden;"></div>');
                var d = a.browser.msie && a.browser.version < 7, e = d ? 1 : 0, f = d ? 2 : -1;
                this.helper.addClass(this._helper).css({width:this.element.outerWidth() + f, height:this.element.outerHeight() + f, position:"absolute", left:this.elementOffset.left - e + "px", top:this.elementOffset.top - e + "px", zIndex:++c.zIndex}), this.helper.appendTo("body").disableSelection()
            } else this.helper = this.element
        }, _change:{e:function (a, b, c) {
            return{width:this.originalSize.width + b}
        }, w:function (a, b, c) {
            var d = this.options, e = this.originalSize, f = this.originalPosition;
            return{left:f.left + b, width:e.width - b}
        }, n:function (a, b, c) {
            var d = this.options, e = this.originalSize, f = this.originalPosition;
            return{top:f.top + c, height:e.height - c}
        }, s:function (a, b, c) {
            return{height:this.originalSize.height + c}
        }, se:function (b, c, d) {
            return a.extend(this._change.s.apply(this, arguments), this._change.e.apply(this, [b, c, d]))
        }, sw:function (b, c, d) {
            return a.extend(this._change.s.apply(this, arguments), this._change.w.apply(this, [b, c, d]))
        }, ne:function (b, c, d) {
            return a.extend(this._change.n.apply(this, arguments), this._change.e.apply(this, [b, c, d]))
        }, nw:function (b, c, d) {
            return a.extend(this._change.n.apply(this, arguments), this._change.w.apply(this, [b, c, d]))
        }}, _propagate:function (b, c) {
            a.ui.plugin.call(this, b, [c, this.ui()]), b != "resize" && this._trigger(b, c, this.ui())
        }, plugins:{}, ui:function () {
            return{originalElement:this.originalElement, element:this.element, helper:this.helper, position:this.position, size:this.size, originalSize:this.originalSize, originalPosition:this.originalPosition}
        }}), a.extend(a.ui.resizable, {version:"1.8.19"}), a.ui.plugin.add("resizable", "alsoResize", {start:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = function (b) {
                a(b).each(function () {
                    var b = a(this);
                    b.data("resizable-alsoresize", {width:parseInt(b.width(), 10), height:parseInt(b.height(), 10), left:parseInt(b.css("left"), 10), top:parseInt(b.css("top"), 10)})
                })
            };
            typeof e.alsoResize == "object" && !e.alsoResize.parentNode ? e.alsoResize.length ? (e.alsoResize = e.alsoResize[0], f(e.alsoResize)) : a.each(e.alsoResize, function (a) {
                f(a)
            }) : f(e.alsoResize)
        }, resize:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = d.originalSize, g = d.originalPosition, h = {height:d.size.height - f.height || 0, width:d.size.width - f.width || 0, top:d.position.top - g.top || 0, left:d.position.left - g.left || 0}, i = function (b, d) {
                a(b).each(function () {
                    var b = a(this), e = a(this).data("resizable-alsoresize"), f = {}, g = d && d.length ? d : b.parents(c.originalElement[0]).length ? ["width", "height"] : ["width", "height", "top", "left"];
                    a.each(g, function (a, b) {
                        var c = (e[b] || 0) + (h[b] || 0);
                        c && c >= 0 && (f[b] = c || null)
                    }), b.css(f)
                })
            };
            typeof e.alsoResize == "object" && !e.alsoResize.nodeType ? a.each(e.alsoResize, function (a, b) {
                i(a, b)
            }) : i(e.alsoResize)
        }, stop:function (b, c) {
            a(this).removeData("resizable-alsoresize")
        }}), a.ui.plugin.add("resizable", "animate", {stop:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = d._proportionallyResizeElements, g = f.length && /textarea/i.test(f[0].nodeName), h = g && a.ui.hasScroll(f[0], "left") ? 0 : d.sizeDiff.height, i = g ? 0 : d.sizeDiff.width, j = {width:d.size.width - i, height:d.size.height - h}, k = parseInt(d.element.css("left"), 10) + (d.position.left - d.originalPosition.left) || null, l = parseInt(d.element.css("top"), 10) + (d.position.top - d.originalPosition.top) || null;
            d.element.animate(a.extend(j, l && k ? {top:l, left:k} : {}), {duration:e.animateDuration, easing:e.animateEasing, step:function () {
                var c = {width:parseInt(d.element.css("width"), 10), height:parseInt(d.element.css("height"), 10), top:parseInt(d.element.css("top"), 10), left:parseInt(d.element.css("left"), 10)};
                f && f.length && a(f[0]).css({width:c.width, height:c.height}), d._updateCache(c), d._propagate("resize", b)
            }})
        }}), a.ui.plugin.add("resizable", "containment", {start:function (b, d) {
            var e = a(this).data("resizable"), f = e.options, g = e.element, h = f.containment, i = h instanceof a ? h.get(0) : /parent/.test(h) ? g.parent().get(0) : h;
            if (!i)return;
            e.containerElement = a(i);
            if (/document/.test(h) || h == document)e.containerOffset = {left:0, top:0}, e.containerPosition = {left:0, top:0}, e.parentData = {element:a(document), left:0, top:0, width:a(document).width(), height:a(document).height() || document.body.parentNode.scrollHeight}; else {
                var j = a(i), k = [];
                a(["Top", "Right", "Left", "Bottom"]).each(function (a, b) {
                    k[a] = c(j.css("padding" + b))
                }), e.containerOffset = j.offset(), e.containerPosition = j.position(), e.containerSize = {height:j.innerHeight() - k[3], width:j.innerWidth() - k[1]};
                var l = e.containerOffset, m = e.containerSize.height, n = e.containerSize.width, o = a.ui.hasScroll(i, "left") ? i.scrollWidth : n, p = a.ui.hasScroll(i) ? i.scrollHeight : m;
                e.parentData = {element:i, left:l.left, top:l.top, width:o, height:p}
            }
        }, resize:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = d.containerSize, g = d.containerOffset, h = d.size, i = d.position, j = d._aspectRatio || b.shiftKey, k = {top:0, left:0}, l = d.containerElement;
            l[0] != document && /static/.test(l.css("position")) && (k = g), i.left < (d._helper ? g.left : 0) && (d.size.width = d.size.width + (d._helper ? d.position.left - g.left : d.position.left - k.left), j && (d.size.height = d.size.width / d.aspectRatio), d.position.left = e.helper ? g.left : 0), i.top < (d._helper ? g.top : 0) && (d.size.height = d.size.height + (d._helper ? d.position.top - g.top : d.position.top), j && (d.size.width = d.size.height * d.aspectRatio), d.position.top = d._helper ? g.top : 0), d.offset.left = d.parentData.left + d.position.left, d.offset.top = d.parentData.top + d.position.top;
            var m = Math.abs((d._helper ? d.offset.left - k.left : d.offset.left - k.left) + d.sizeDiff.width), n = Math.abs((d._helper ? d.offset.top - k.top : d.offset.top - g.top) + d.sizeDiff.height), o = d.containerElement.get(0) == d.element.parent().get(0), p = /relative|absolute/.test(d.containerElement.css("position"));
            o && p && (m -= d.parentData.left), m + d.size.width >= d.parentData.width && (d.size.width = d.parentData.width - m, j && (d.size.height = d.size.width / d.aspectRatio)), n + d.size.height >= d.parentData.height && (d.size.height = d.parentData.height - n, j && (d.size.width = d.size.height * d.aspectRatio))
        }, stop:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = d.position, g = d.containerOffset, h = d.containerPosition, i = d.containerElement, j = a(d.helper), k = j.offset(), l = j.outerWidth() - d.sizeDiff.width, m = j.outerHeight() - d.sizeDiff.height;
            d._helper && !e.animate && /relative/.test(i.css("position")) && a(this).css({left:k.left - h.left - g.left, width:l, height:m}), d._helper && !e.animate && /static/.test(i.css("position")) && a(this).css({left:k.left - h.left - g.left, width:l, height:m})
        }}), a.ui.plugin.add("resizable", "ghost", {start:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = d.size;
            d.ghost = d.originalElement.clone(), d.ghost.css({opacity:.25, display:"block", position:"relative", height:f.height, width:f.width, margin:0, left:0, top:0}).addClass("ui-resizable-ghost").addClass(typeof e.ghost == "string" ? e.ghost : ""), d.ghost.appendTo(d.helper)
        }, resize:function (b, c) {
            var d = a(this).data("resizable"), e = d.options;
            d.ghost && d.ghost.css({position:"relative", height:d.size.height, width:d.size.width})
        }, stop:function (b, c) {
            var d = a(this).data("resizable"), e = d.options;
            d.ghost && d.helper && d.helper.get(0).removeChild(d.ghost.get(0))
        }}), a.ui.plugin.add("resizable", "grid", {resize:function (b, c) {
            var d = a(this).data("resizable"), e = d.options, f = d.size, g = d.originalSize, h = d.originalPosition, i = d.axis, j = e._aspectRatio || b.shiftKey;
            e.grid = typeof e.grid == "number" ? [e.grid, e.grid] : e.grid;
            var k = Math.round((f.width - g.width) / (e.grid[0] || 1)) * (e.grid[0] || 1), l = Math.round((f.height - g.height) / (e.grid[1] || 1)) * (e.grid[1] || 1);
            /^(se|s|e)$/.test(i) ? (d.size.width = g.width + k, d.size.height = g.height + l) : /^(ne)$/.test(i) ? (d.size.width = g.width + k, d.size.height = g.height + l, d.position.top = h.top - l) : /^(sw)$/.test(i) ? (d.size.width = g.width + k, d.size.height = g.height + l, d.position.left = h.left - k) : (d.size.width = g.width + k, d.size.height = g.height + l, d.position.top = h.top - l, d.position.left = h.left - k)
        }});
        var c = function (a) {
            return parseInt(a, 10) || 0
        }, d = function (a) {
            return!isNaN(parseInt(a, 10))
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.selectable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.selectable", a.ui.mouse, {options:{appendTo:"body", autoRefresh:!0, distance:0, filter:"*", tolerance:"touch"}, _create:function () {
            var b = this;
            this.element.addClass("ui-selectable"), this.dragged = !1;
            var c;
            this.refresh = function () {
                c = a(b.options.filter, b.element[0]), c.addClass("ui-selectee"), c.each(function () {
                    var b = a(this), c = b.offset();
                    a.data(this, "selectable-item", {element:this, $element:b, left:c.left, top:c.top, right:c.left + b.outerWidth(), bottom:c.top + b.outerHeight(), startselected:!1, selected:b.hasClass("ui-selected"), selecting:b.hasClass("ui-selecting"), unselecting:b.hasClass("ui-unselecting")})
                })
            }, this.refresh(), this.selectees = c.addClass("ui-selectee"), this._mouseInit(), this.helper = a("<div class='ui-selectable-helper'></div>")
        }, destroy:function () {
            return this.selectees.removeClass("ui-selectee").removeData("selectable-item"), this.element.removeClass("ui-selectable ui-selectable-disabled").removeData("selectable").unbind(".selectable"), this._mouseDestroy(), this
        }, _mouseStart:function (b) {
            var c = this;
            this.opos = [b.pageX, b.pageY];
            if (this.options.disabled)return;
            var d = this.options;
            this.selectees = a(d.filter, this.element[0]), this._trigger("start", b), a(d.appendTo).append(this.helper), this.helper.css({left:b.clientX, top:b.clientY, width:0, height:0}), d.autoRefresh && this.refresh(), this.selectees.filter(".ui-selected").each(function () {
                var d = a.data(this, "selectable-item");
                d.startselected = !0, !b.metaKey && !b.ctrlKey && (d.$element.removeClass("ui-selected"), d.selected = !1, d.$element.addClass("ui-unselecting"), d.unselecting = !0, c._trigger("unselecting", b, {unselecting:d.element}))
            }), a(b.target).parents().andSelf().each(function () {
                var d = a.data(this, "selectable-item");
                if (d) {
                    var e = !b.metaKey && !b.ctrlKey || !d.$element.hasClass("ui-selected");
                    return d.$element.removeClass(e ? "ui-unselecting" : "ui-selected").addClass(e ? "ui-selecting" : "ui-unselecting"), d.unselecting = !e, d.selecting = e, d.selected = e, e ? c._trigger("selecting", b, {selecting:d.element}) : c._trigger("unselecting", b, {unselecting:d.element}), !1
                }
            })
        }, _mouseDrag:function (b) {
            var c = this;
            this.dragged = !0;
            if (this.options.disabled)return;
            var d = this.options, e = this.opos[0], f = this.opos[1], g = b.pageX, h = b.pageY;
            if (e > g) {
                var i = g;
                g = e, e = i
            }
            if (f > h) {
                var i = h;
                h = f, f = i
            }
            return this.helper.css({left:e, top:f, width:g - e, height:h - f}), this.selectees.each(function () {
                var i = a.data(this, "selectable-item");
                if (!i || i.element == c.element[0])return;
                var j = !1;
                d.tolerance == "touch" ? j = !(i.left > g || i.right < e || i.top > h || i.bottom < f) : d.tolerance == "fit" && (j = i.left > e && i.right < g && i.top > f && i.bottom < h), j ? (i.selected && (i.$element.removeClass("ui-selected"), i.selected = !1), i.unselecting && (i.$element.removeClass("ui-unselecting"), i.unselecting = !1), i.selecting || (i.$element.addClass("ui-selecting"), i.selecting = !0, c._trigger("selecting", b, {selecting:i.element}))) : (i.selecting && ((b.metaKey || b.ctrlKey) && i.startselected ? (i.$element.removeClass("ui-selecting"), i.selecting = !1, i.$element.addClass("ui-selected"), i.selected = !0) : (i.$element.removeClass("ui-selecting"), i.selecting = !1, i.startselected && (i.$element.addClass("ui-unselecting"), i.unselecting = !0), c._trigger("unselecting", b, {unselecting:i.element}))), i.selected && !b.metaKey && !b.ctrlKey && !i.startselected && (i.$element.removeClass("ui-selected"), i.selected = !1, i.$element.addClass("ui-unselecting"), i.unselecting = !0, c._trigger("unselecting", b, {unselecting:i.element})))
            }), !1
        }, _mouseStop:function (b) {
            var c = this;
            this.dragged = !1;
            var d = this.options;
            return a(".ui-unselecting", this.element[0]).each(function () {
                var d = a.data(this, "selectable-item");
                d.$element.removeClass("ui-unselecting"), d.unselecting = !1, d.startselected = !1, c._trigger("unselected", b, {unselected:d.element})
            }), a(".ui-selecting", this.element[0]).each(function () {
                var d = a.data(this, "selectable-item");
                d.$element.removeClass("ui-selecting").addClass("ui-selected"), d.selecting = !1, d.selected = !0, d.startselected = !0, c._trigger("selected", b, {selected:d.element})
            }), this._trigger("stop", b), this.helper.remove(), !1
        }}), a.extend(a.ui.selectable, {version:"1.8.19"})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.sortable.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.sortable", a.ui.mouse, {widgetEventPrefix:"sort", ready:!1, options:{appendTo:"parent", axis:!1, connectWith:!1, containment:!1, cursor:"auto", cursorAt:!1, dropOnEmpty:!0, forcePlaceholderSize:!1, forceHelperSize:!1, grid:!1, handle:!1, helper:"original", items:"> *", opacity:!1, placeholder:!1, revert:!1, scroll:!0, scrollSensitivity:20, scrollSpeed:20, scope:"default", tolerance:"intersect", zIndex:1e3}, _create:function () {
            var a = this.options;
            this.containerCache = {}, this.element.addClass("ui-sortable"), this.refresh(), this.floating = this.items.length ? a.axis === "x" || /left|right/.test(this.items[0].item.css("float")) || /inline|table-cell/.test(this.items[0].item.css("display")) : !1, this.offset = this.element.offset(), this._mouseInit(), this.ready = !0
        }, destroy:function () {
            a.Widget.prototype.destroy.call(this), this.element.removeClass("ui-sortable ui-sortable-disabled"), this._mouseDestroy();
            for (var b = this.items.length - 1; b >= 0; b--)this.items[b].item.removeData(this.widgetName + "-item");
            return this
        }, _setOption:function (b, c) {
            b === "disabled" ? (this.options[b] = c, this.widget()[c ? "addClass" : "removeClass"]("ui-sortable-disabled")) : a.Widget.prototype._setOption.apply(this, arguments)
        }, _mouseCapture:function (b, c) {
            var d = this;
            if (this.reverting)return!1;
            if (this.options.disabled || this.options.type == "static")return!1;
            this._refreshItems(b);
            var e = null, f = this, g = a(b.target).parents().each(function () {
                if (a.data(this, d.widgetName + "-item") == f)return e = a(this), !1
            });
            a.data(b.target, d.widgetName + "-item") == f && (e = a(b.target));
            if (!e)return!1;
            if (this.options.handle && !c) {
                var h = !1;
                a(this.options.handle, e).find("*").andSelf().each(function () {
                    this == b.target && (h = !0)
                });
                if (!h)return!1
            }
            return this.currentItem = e, this._removeCurrentsFromItems(), !0
        }, _mouseStart:function (b, c, d) {
            var e = this.options, f = this;
            this.currentContainer = this, this.refreshPositions(), this.helper = this._createHelper(b), this._cacheHelperProportions(), this._cacheMargins(), this.scrollParent = this.helper.scrollParent(), this.offset = this.currentItem.offset(), this.offset = {top:this.offset.top - this.margins.top, left:this.offset.left - this.margins.left}, this.helper.css("position", "absolute"), this.cssPosition = this.helper.css("position"), a.extend(this.offset, {click:{left:b.pageX - this.offset.left, top:b.pageY - this.offset.top}, parent:this._getParentOffset(), relative:this._getRelativeOffset()}), this.originalPosition = this._generatePosition(b), this.originalPageX = b.pageX, this.originalPageY = b.pageY, e.cursorAt && this._adjustOffsetFromHelper(e.cursorAt), this.domPosition = {prev:this.currentItem.prev()[0], parent:this.currentItem.parent()[0]}, this.helper[0] != this.currentItem[0] && this.currentItem.hide(), this._createPlaceholder(), e.containment && this._setContainment(), e.cursor && (a("body").css("cursor") && (this._storedCursor = a("body").css("cursor")), a("body").css("cursor", e.cursor)), e.opacity && (this.helper.css("opacity") && (this._storedOpacity = this.helper.css("opacity")), this.helper.css("opacity", e.opacity)), e.zIndex && (this.helper.css("zIndex") && (this._storedZIndex = this.helper.css("zIndex")), this.helper.css("zIndex", e.zIndex)), this.scrollParent[0] != document && this.scrollParent[0].tagName != "HTML" && (this.overflowOffset = this.scrollParent.offset()), this._trigger("start", b, this._uiHash()), this._preserveHelperProportions || this._cacheHelperProportions();
            if (!d)for (var g = this.containers.length - 1; g >= 0; g--)this.containers[g]._trigger("activate", b, f._uiHash(this));
            return a.ui.ddmanager && (a.ui.ddmanager.current = this), a.ui.ddmanager && !e.dropBehaviour && a.ui.ddmanager.prepareOffsets(this, b), this.dragging = !0, this.helper.addClass("ui-sortable-helper"), this._mouseDrag(b), !0
        }, _mouseDrag:function (b) {
            this.position = this._generatePosition(b), this.positionAbs = this._convertPositionTo("absolute"), this.lastPositionAbs || (this.lastPositionAbs = this.positionAbs);
            if (this.options.scroll) {
                var c = this.options, d = !1;
                this.scrollParent[0] != document && this.scrollParent[0].tagName != "HTML" ? (this.overflowOffset.top + this.scrollParent[0].offsetHeight - b.pageY < c.scrollSensitivity ? this.scrollParent[0].scrollTop = d = this.scrollParent[0].scrollTop + c.scrollSpeed : b.pageY - this.overflowOffset.top < c.scrollSensitivity && (this.scrollParent[0].scrollTop = d = this.scrollParent[0].scrollTop - c.scrollSpeed), this.overflowOffset.left + this.scrollParent[0].offsetWidth - b.pageX < c.scrollSensitivity ? this.scrollParent[0].scrollLeft = d = this.scrollParent[0].scrollLeft + c.scrollSpeed : b.pageX - this.overflowOffset.left < c.scrollSensitivity && (this.scrollParent[0].scrollLeft = d = this.scrollParent[0].scrollLeft - c.scrollSpeed)) : (b.pageY - a(document).scrollTop() < c.scrollSensitivity ? d = a(document).scrollTop(a(document).scrollTop() - c.scrollSpeed) : a(window).height() - (b.pageY - a(document).scrollTop()) < c.scrollSensitivity && (d = a(document).scrollTop(a(document).scrollTop() + c.scrollSpeed)), b.pageX - a(document).scrollLeft() < c.scrollSensitivity ? d = a(document).scrollLeft(a(document).scrollLeft() - c.scrollSpeed) : a(window).width() - (b.pageX - a(document).scrollLeft()) < c.scrollSensitivity && (d = a(document).scrollLeft(a(document).scrollLeft() + c.scrollSpeed))), d !== !1 && a.ui.ddmanager && !c.dropBehaviour && a.ui.ddmanager.prepareOffsets(this, b)
            }
            this.positionAbs = this._convertPositionTo("absolute");
            if (!this.options.axis || this.options.axis != "y")this.helper[0].style.left = this.position.left + "px";
            if (!this.options.axis || this.options.axis != "x")this.helper[0].style.top = this.position.top + "px";
            for (var e = this.items.length - 1; e >= 0; e--) {
                var f = this.items[e], g = f.item[0], h = this._intersectsWithPointer(f);
                if (!h)continue;
                if (g != this.currentItem[0] && this.placeholder[h == 1 ? "next" : "prev"]()[0] != g && !a.ui.contains(this.placeholder[0], g) && (this.options.type == "semi-dynamic" ? !a.ui.contains(this.element[0], g) : !0)) {
                    this.direction = h == 1 ? "down" : "up";
                    if (this.options.tolerance == "pointer" || this._intersectsWithSides(f))this._rearrange(b, f); else break;
                    this._trigger("change", b, this._uiHash());
                    break
                }
            }
            return this._contactContainers(b), a.ui.ddmanager && a.ui.ddmanager.drag(this, b), this._trigger("sort", b, this._uiHash()), this.lastPositionAbs = this.positionAbs, !1
        }, _mouseStop:function (b, c) {
            if (!b)return;
            a.ui.ddmanager && !this.options.dropBehaviour && a.ui.ddmanager.drop(this, b);
            if (this.options.revert) {
                var d = this, e = d.placeholder.offset();
                d.reverting = !0, a(this.helper).animate({left:e.left - this.offset.parent.left - d.margins.left + (this.offsetParent[0] == document.body ? 0 : this.offsetParent[0].scrollLeft), top:e.top - this.offset.parent.top - d.margins.top + (this.offsetParent[0] == document.body ? 0 : this.offsetParent[0].scrollTop)}, parseInt(this.options.revert, 10) || 500, function () {
                    d._clear(b)
                })
            } else this._clear(b, c);
            return!1
        }, cancel:function () {
            var b = this;
            if (this.dragging) {
                this._mouseUp({target:null}), this.options.helper == "original" ? this.currentItem.css(this._storedCSS).removeClass("ui-sortable-helper") : this.currentItem.show();
                for (var c = this.containers.length - 1; c >= 0; c--)this.containers[c]._trigger("deactivate", null, b._uiHash(this)), this.containers[c].containerCache.over && (this.containers[c]._trigger("out", null, b._uiHash(this)), this.containers[c].containerCache.over = 0)
            }
            return this.placeholder && (this.placeholder[0].parentNode && this.placeholder[0].parentNode.removeChild(this.placeholder[0]), this.options.helper != "original" && this.helper && this.helper[0].parentNode && this.helper.remove(), a.extend(this, {helper:null, dragging:!1, reverting:!1, _noFinalSort:null}), this.domPosition.prev ? a(this.domPosition.prev).after(this.currentItem) : a(this.domPosition.parent).prepend(this.currentItem)), this
        }, serialize:function (b) {
            var c = this._getItemsAsjQuery(b && b.connected), d = [];
            return b = b || {}, a(c).each(function () {
                var c = (a(b.item || this).attr(b.attribute || "id") || "").match(b.expression || /(.+)[-=_](.+)/);
                c && d.push((b.key || c[1] + "[]") + "=" + (b.key && b.expression ? c[1] : c[2]))
            }), !d.length && b.key && d.push(b.key + "="), d.join("&")
        }, toArray:function (b) {
            var c = this._getItemsAsjQuery(b && b.connected), d = [];
            return b = b || {}, c.each(function () {
                d.push(a(b.item || this).attr(b.attribute || "id") || "")
            }), d
        }, _intersectsWith:function (a) {
            var b = this.positionAbs.left, c = b + this.helperProportions.width, d = this.positionAbs.top, e = d + this.helperProportions.height, f = a.left, g = f + a.width, h = a.top, i = h + a.height, j = this.offset.click.top, k = this.offset.click.left, l = d + j > h && d + j < i && b + k > f && b + k < g;
            return this.options.tolerance == "pointer" || this.options.forcePointerForContainers || this.options.tolerance != "pointer" && this.helperProportions[this.floating ? "width" : "height"] > a[this.floating ? "width" : "height"] ? l : f < b + this.helperProportions.width / 2 && c - this.helperProportions.width / 2 < g && h < d + this.helperProportions.height / 2 && e - this.helperProportions.height / 2 < i
        }, _intersectsWithPointer:function (b) {
            var c = a.ui.isOverAxis(this.positionAbs.top + this.offset.click.top, b.top, b.height), d = a.ui.isOverAxis(this.positionAbs.left + this.offset.click.left, b.left, b.width), e = c && d, f = this._getDragVerticalDirection(), g = this._getDragHorizontalDirection();
            return e ? this.floating ? g && g == "right" || f == "down" ? 2 : 1 : f && (f == "down" ? 2 : 1) : !1
        }, _intersectsWithSides:function (b) {
            var c = a.ui.isOverAxis(this.positionAbs.top + this.offset.click.top, b.top + b.height / 2, b.height), d = a.ui.isOverAxis(this.positionAbs.left + this.offset.click.left, b.left + b.width / 2, b.width), e = this._getDragVerticalDirection(), f = this._getDragHorizontalDirection();
            return this.floating && f ? f == "right" && d || f == "left" && !d : e && (e == "down" && c || e == "up" && !c)
        }, _getDragVerticalDirection:function () {
            var a = this.positionAbs.top - this.lastPositionAbs.top;
            return a != 0 && (a > 0 ? "down" : "up")
        }, _getDragHorizontalDirection:function () {
            var a = this.positionAbs.left - this.lastPositionAbs.left;
            return a != 0 && (a > 0 ? "right" : "left")
        }, refresh:function (a) {
            return this._refreshItems(a), this.refreshPositions(), this
        }, _connectWith:function () {
            var a = this.options;
            return a.connectWith.constructor == String ? [a.connectWith] : a.connectWith
        }, _getItemsAsjQuery:function (b) {
            var c = this, d = [], e = [], f = this._connectWith();
            if (f && b)for (var g = f.length - 1; g >= 0; g--) {
                var h = a(f[g]);
                for (var i = h.length - 1; i >= 0; i--) {
                    var j = a.data(h[i], this.widgetName);
                    j && j != this && !j.options.disabled && e.push([a.isFunction(j.options.items) ? j.options.items.call(j.element) : a(j.options.items, j.element).not(".ui-sortable-helper").not(".ui-sortable-placeholder"), j])
                }
            }
            e.push([a.isFunction(this.options.items) ? this.options.items.call(this.element, null, {options:this.options, item:this.currentItem}) : a(this.options.items, this.element).not(".ui-sortable-helper").not(".ui-sortable-placeholder"), this]);
            for (var g = e.length - 1; g >= 0; g--)e[g][0].each(function () {
                d.push(this)
            });
            return a(d)
        }, _removeCurrentsFromItems:function () {
            var a = this.currentItem.find(":data(" + this.widgetName + "-item)");
            for (var b = 0; b < this.items.length; b++)for (var c = 0; c < a.length; c++)a[c] == this.items[b].item[0] && this.items.splice(b, 1)
        }, _refreshItems:function (b) {
            this.items = [], this.containers = [this];
            var c = this.items, d = this, e = [
                [a.isFunction(this.options.items) ? this.options.items.call(this.element[0], b, {item:this.currentItem}) : a(this.options.items, this.element), this]
            ], f = this._connectWith();
            if (f && this.ready)for (var g = f.length - 1; g >= 0; g--) {
                var h = a(f[g]);
                for (var i = h.length - 1; i >= 0; i--) {
                    var j = a.data(h[i], this.widgetName);
                    j && j != this && !j.options.disabled && (e.push([a.isFunction(j.options.items) ? j.options.items.call(j.element[0], b, {item:this.currentItem}) : a(j.options.items, j.element), j]), this.containers.push(j))
                }
            }
            for (var g = e.length - 1; g >= 0; g--) {
                var k = e[g][1], l = e[g][0];
                for (var i = 0, m = l.length; i < m; i++) {
                    var n = a(l[i]);
                    n.data(this.widgetName + "-item", k), c.push({item:n, instance:k, width:0, height:0, left:0, top:0})
                }
            }
        }, refreshPositions:function (b) {
            this.offsetParent && this.helper && (this.offset.parent = this._getParentOffset());
            for (var c = this.items.length - 1; c >= 0; c--) {
                var d = this.items[c];
                if (d.instance != this.currentContainer && this.currentContainer && d.item[0] != this.currentItem[0])continue;
                var e = this.options.toleranceElement ? a(this.options.toleranceElement, d.item) : d.item;
                b || (d.width = e.outerWidth(), d.height = e.outerHeight());
                var f = e.offset();
                d.left = f.left, d.top = f.top
            }
            if (this.options.custom && this.options.custom.refreshContainers)this.options.custom.refreshContainers.call(this); else for (var c = this.containers.length - 1; c >= 0; c--) {
                var f = this.containers[c].element.offset();
                this.containers[c].containerCache.left = f.left, this.containers[c].containerCache.top = f.top, this.containers[c].containerCache.width = this.containers[c].element.outerWidth(), this.containers[c].containerCache.height = this.containers[c].element.outerHeight()
            }
            return this
        }, _createPlaceholder:function (b) {
            var c = b || this, d = c.options;
            if (!d.placeholder || d.placeholder.constructor == String) {
                var e = d.placeholder;
                d.placeholder = {element:function () {
                    var b = a(document.createElement(c.currentItem[0].nodeName)).addClass(e || c.currentItem[0].className + " ui-sortable-placeholder").removeClass("ui-sortable-helper").html("&nbsp;")[0];
                    return e || (b.style.visibility = "hidden"), b
                }, update:function (a, b) {
                    if (e && !d.forcePlaceholderSize)return;
                    b.height() || b.height(c.currentItem.innerHeight() - parseInt(c.currentItem.css("paddingTop") || 0, 10) - parseInt(c.currentItem.css("paddingBottom") || 0, 10)), b.width() || b.width(c.currentItem.innerWidth() - parseInt(c.currentItem.css("paddingLeft") || 0, 10) - parseInt(c.currentItem.css("paddingRight") || 0, 10))
                }}
            }
            c.placeholder = a(d.placeholder.element.call(c.element, c.currentItem)), c.currentItem.after(c.placeholder), d.placeholder.update(c, c.placeholder)
        }, _contactContainers:function (b) {
            var c = null, d = null;
            for (var e = this.containers.length - 1; e >= 0; e--) {
                if (a.ui.contains(this.currentItem[0], this.containers[e].element[0]))continue;
                if (this._intersectsWith(this.containers[e].containerCache)) {
                    if (c && a.ui.contains(this.containers[e].element[0], c.element[0]))continue;
                    c = this.containers[e], d = e
                } else this.containers[e].containerCache.over && (this.containers[e]._trigger("out", b, this._uiHash(this)), this.containers[e].containerCache.over = 0)
            }
            if (!c)return;
            if (this.containers.length === 1)this.containers[d]._trigger("over", b, this._uiHash(this)), this.containers[d].containerCache.over = 1; else if (this.currentContainer != this.containers[d]) {
                var f = 1e4, g = null, h = this.positionAbs[this.containers[d].floating ? "left" : "top"];
                for (var i = this.items.length - 1; i >= 0; i--) {
                    if (!a.ui.contains(this.containers[d].element[0], this.items[i].item[0]))continue;
                    var j = this.items[i][this.containers[d].floating ? "left" : "top"];
                    Math.abs(j - h) < f && (f = Math.abs(j - h), g = this.items[i])
                }
                if (!g && !this.options.dropOnEmpty)return;
                this.currentContainer = this.containers[d], g ? this._rearrange(b, g, null, !0) : this._rearrange(b, null, this.containers[d].element, !0), this._trigger("change", b, this._uiHash()), this.containers[d]._trigger("change", b, this._uiHash(this)), this.options.placeholder.update(this.currentContainer, this.placeholder), this.containers[d]._trigger("over", b, this._uiHash(this)), this.containers[d].containerCache.over = 1
            }
        }, _createHelper:function (b) {
            var c = this.options, d = a.isFunction(c.helper) ? a(c.helper.apply(this.element[0], [b, this.currentItem])) : c.helper == "clone" ? this.currentItem.clone() : this.currentItem;
            return d.parents("body").length || a(c.appendTo != "parent" ? c.appendTo : this.currentItem[0].parentNode)[0].appendChild(d[0]), d[0] == this.currentItem[0] && (this._storedCSS = {width:this.currentItem[0].style.width, height:this.currentItem[0].style.height, position:this.currentItem.css("position"), top:this.currentItem.css("top"), left:this.currentItem.css("left")}), (d[0].style.width == "" || c.forceHelperSize) && d.width(this.currentItem.width()), (d[0].style.height == "" || c.forceHelperSize) && d.height(this.currentItem.height()), d
        }, _adjustOffsetFromHelper:function (b) {
            typeof b == "string" && (b = b.split(" ")), a.isArray(b) && (b = {left:+b[0], top:+b[1] || 0}), "left"in b && (this.offset.click.left = b.left + this.margins.left), "right"in b && (this.offset.click.left = this.helperProportions.width - b.right + this.margins.left), "top"in b && (this.offset.click.top = b.top + this.margins.top), "bottom"in b && (this.offset.click.top = this.helperProportions.height - b.bottom + this.margins.top)
        }, _getParentOffset:function () {
            this.offsetParent = this.helper.offsetParent();
            var b = this.offsetParent.offset();
            this.cssPosition == "absolute" && this.scrollParent[0] != document && a.ui.contains(this.scrollParent[0], this.offsetParent[0]) && (b.left += this.scrollParent.scrollLeft(), b.top += this.scrollParent.scrollTop());
            if (this.offsetParent[0] == document.body || this.offsetParent[0].tagName && this.offsetParent[0].tagName.toLowerCase() == "html" && a.browser.msie)b = {top:0, left:0};
            return{top:b.top + (parseInt(this.offsetParent.css("borderTopWidth"), 10) || 0), left:b.left + (parseInt(this.offsetParent.css("borderLeftWidth"), 10) || 0)}
        }, _getRelativeOffset:function () {
            if (this.cssPosition == "relative") {
                var a = this.currentItem.position();
                return{top:a.top - (parseInt(this.helper.css("top"), 10) || 0) + this.scrollParent.scrollTop(), left:a.left - (parseInt(this.helper.css("left"), 10) || 0) + this.scrollParent.scrollLeft()}
            }
            return{top:0, left:0}
        }, _cacheMargins:function () {
            this.margins = {left:parseInt(this.currentItem.css("marginLeft"), 10) || 0, top:parseInt(this.currentItem.css("marginTop"), 10) || 0}
        }, _cacheHelperProportions:function () {
            this.helperProportions = {width:this.helper.outerWidth(), height:this.helper.outerHeight()}
        }, _setContainment:function () {
            var b = this.options;
            b.containment == "parent" && (b.containment = this.helper[0].parentNode);
            if (b.containment == "document" || b.containment == "window")this.containment = [0 - this.offset.relative.left - this.offset.parent.left, 0 - this.offset.relative.top - this.offset.parent.top, a(b.containment == "document" ? document : window).width() - this.helperProportions.width - this.margins.left, (a(b.containment == "document" ? document : window).height() || document.body.parentNode.scrollHeight) - this.helperProportions.height - this.margins.top];
            if (!/^(document|window|parent)$/.test(b.containment)) {
                var c = a(b.containment)[0], d = a(b.containment).offset(), e = a(c).css("overflow") != "hidden";
                this.containment = [d.left + (parseInt(a(c).css("borderLeftWidth"), 10) || 0) + (parseInt(a(c).css("paddingLeft"), 10) || 0) - this.margins.left, d.top + (parseInt(a(c).css("borderTopWidth"), 10) || 0) + (parseInt(a(c).css("paddingTop"), 10) || 0) - this.margins.top, d.left + (e ? Math.max(c.scrollWidth, c.offsetWidth) : c.offsetWidth) - (parseInt(a(c).css("borderLeftWidth"), 10) || 0) - (parseInt(a(c).css("paddingRight"), 10) || 0) - this.helperProportions.width - this.margins.left, d.top + (e ? Math.max(c.scrollHeight, c.offsetHeight) : c.offsetHeight) - (parseInt(a(c).css("borderTopWidth"), 10) || 0) - (parseInt(a(c).css("paddingBottom"), 10) || 0) - this.helperProportions.height - this.margins.top]
            }
        }, _convertPositionTo:function (b, c) {
            c || (c = this.position);
            var d = b == "absolute" ? 1 : -1, e = this.options, f = this.cssPosition == "absolute" && (this.scrollParent[0] == document || !a.ui.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent, g = /(html|body)/i.test(f[0].tagName);
            return{top:c.top + this.offset.relative.top * d + this.offset.parent.top * d - (a.browser.safari && this.cssPosition == "fixed" ? 0 : (this.cssPosition == "fixed" ? -this.scrollParent.scrollTop() : g ? 0 : f.scrollTop()) * d), left:c.left + this.offset.relative.left * d + this.offset.parent.left * d - (a.browser.safari && this.cssPosition == "fixed" ? 0 : (this.cssPosition == "fixed" ? -this.scrollParent.scrollLeft() : g ? 0 : f.scrollLeft()) * d)}
        }, _generatePosition:function (b) {
            var c = this.options, d = this.cssPosition == "absolute" && (this.scrollParent[0] == document || !a.ui.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent, e = /(html|body)/i.test(d[0].tagName);
            this.cssPosition == "relative" && (this.scrollParent[0] == document || this.scrollParent[0] == this.offsetParent[0]) && (this.offset.relative = this._getRelativeOffset());
            var f = b.pageX, g = b.pageY;
            if (this.originalPosition) {
                this.containment && (b.pageX - this.offset.click.left < this.containment[0] && (f = this.containment[0] + this.offset.click.left), b.pageY - this.offset.click.top < this.containment[1] && (g = this.containment[1] + this.offset.click.top), b.pageX - this.offset.click.left > this.containment[2] && (f = this.containment[2] + this.offset.click.left), b.pageY - this.offset.click.top > this.containment[3] && (g = this.containment[3] + this.offset.click.top));
                if (c.grid) {
                    var h = this.originalPageY + Math.round((g - this.originalPageY) / c.grid[1]) * c.grid[1];
                    g = this.containment ? h - this.offset.click.top < this.containment[1] || h - this.offset.click.top > this.containment[3] ? h - this.offset.click.top < this.containment[1] ? h + c.grid[1] : h - c.grid[1] : h : h;
                    var i = this.originalPageX + Math.round((f - this.originalPageX) / c.grid[0]) * c.grid[0];
                    f = this.containment ? i - this.offset.click.left < this.containment[0] || i - this.offset.click.left > this.containment[2] ? i - this.offset.click.left < this.containment[0] ? i + c.grid[0] : i - c.grid[0] : i : i
                }
            }
            return{top:g - this.offset.click.top - this.offset.relative.top - this.offset.parent.top + (a.browser.safari && this.cssPosition == "fixed" ? 0 : this.cssPosition == "fixed" ? -this.scrollParent.scrollTop() : e ? 0 : d.scrollTop()), left:f - this.offset.click.left - this.offset.relative.left - this.offset.parent.left + (a.browser.safari && this.cssPosition == "fixed" ? 0 : this.cssPosition == "fixed" ? -this.scrollParent.scrollLeft() : e ? 0 : d.scrollLeft())}
        }, _rearrange:function (a, b, c, d) {
            c ? c[0].appendChild(this.placeholder[0]) : b.item[0].parentNode.insertBefore(this.placeholder[0], this.direction == "down" ? b.item[0] : b.item[0].nextSibling), this.counter = this.counter ? ++this.counter : 1;
            var e = this, f = this.counter;
            window.setTimeout(function () {
                f == e.counter && e.refreshPositions(!d)
            }, 0)
        }, _clear:function (b, c) {
            this.reverting = !1;
            var d = [], e = this;
            !this._noFinalSort && this.currentItem.parent().length && this.placeholder.before(this.currentItem), this._noFinalSort = null;
            if (this.helper[0] == this.currentItem[0]) {
                for (var f in this._storedCSS)if (this._storedCSS[f] == "auto" || this._storedCSS[f] == "static")this._storedCSS[f] = "";
                this.currentItem.css(this._storedCSS).removeClass("ui-sortable-helper")
            } else this.currentItem.show();
            this.fromOutside && !c && d.push(function (a) {
                this._trigger("receive", a, this._uiHash(this.fromOutside))
            }), (this.fromOutside || this.domPosition.prev != this.currentItem.prev().not(".ui-sortable-helper")[0] || this.domPosition.parent != this.currentItem.parent()[0]) && !c && d.push(function (a) {
                this._trigger("update", a, this._uiHash())
            });
            if (!a.ui.contains(this.element[0], this.currentItem[0])) {
                c || d.push(function (a) {
                    this._trigger("remove", a, this._uiHash())
                });
                for (var f = this.containers.length - 1; f >= 0; f--)a.ui.contains(this.containers[f].element[0], this.currentItem[0]) && !c && (d.push(function (a) {
                    return function (b) {
                        a._trigger("receive", b, this._uiHash(this))
                    }
                }.call(this, this.containers[f])), d.push(function (a) {
                    return function (b) {
                        a._trigger("update", b, this._uiHash(this))
                    }
                }.call(this, this.containers[f])))
            }
            for (var f = this.containers.length - 1; f >= 0; f--)c || d.push(function (a) {
                return function (b) {
                    a._trigger("deactivate", b, this._uiHash(this))
                }
            }.call(this, this.containers[f])), this.containers[f].containerCache.over && (d.push(function (a) {
                return function (b) {
                    a._trigger("out", b, this._uiHash(this))
                }
            }.call(this, this.containers[f])), this.containers[f].containerCache.over = 0);
            this._storedCursor && a("body").css("cursor", this._storedCursor), this._storedOpacity && this.helper.css("opacity", this._storedOpacity), this._storedZIndex && this.helper.css("zIndex", this._storedZIndex == "auto" ? "" : this._storedZIndex), this.dragging = !1;
            if (this.cancelHelperRemoval) {
                if (!c) {
                    this._trigger("beforeStop", b, this._uiHash());
                    for (var f = 0; f < d.length; f++)d[f].call(this, b);
                    this._trigger("stop", b, this._uiHash())
                }
                return!1
            }
            c || this._trigger("beforeStop", b, this._uiHash()), this.placeholder[0].parentNode.removeChild(this.placeholder[0]), this.helper[0] != this.currentItem[0] && this.helper.remove(), this.helper = null;
            if (!c) {
                for (var f = 0; f < d.length; f++)d[f].call(this, b);
                this._trigger("stop", b, this._uiHash())
            }
            return this.fromOutside = !1, !0
        }, _trigger:function () {
            a.Widget.prototype._trigger.apply(this, arguments) === !1 && this.cancel()
        }, _uiHash:function (b) {
            var c = b || this;
            return{helper:c.helper, placeholder:c.placeholder || a([]), position:c.position, originalPosition:c.originalPosition, offset:c.positionAbs, item:c.currentItem, sender:b ? b.element : null}
        }}), a.extend(a.ui.sortable, {version:"1.8.19"})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.accordion.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.accordion", {options:{active:0, animated:"slide", autoHeight:!0, clearStyle:!1, collapsible:!1, event:"click", fillSpace:!1, header:"> li > :first-child,> :not(li):even", icons:{header:"ui-icon-triangle-1-e", headerSelected:"ui-icon-triangle-1-s"}, navigation:!1, navigationFilter:function () {
            return this.href.toLowerCase() === location.href.toLowerCase()
        }}, _create:function () {
            var b = this, c = b.options;
            b.running = 0, b.element.addClass("ui-accordion ui-widget ui-helper-reset").children("li").addClass("ui-accordion-li-fix"), b.headers = b.element.find(c.header).addClass("ui-accordion-header ui-helper-reset ui-state-default ui-corner-all").bind("mouseenter.accordion",function () {
                if (c.disabled)return;
                a(this).addClass("ui-state-hover")
            }).bind("mouseleave.accordion",function () {
                if (c.disabled)return;
                a(this).removeClass("ui-state-hover")
            }).bind("focus.accordion",function () {
                if (c.disabled)return;
                a(this).addClass("ui-state-focus")
            }).bind("blur.accordion", function () {
                if (c.disabled)return;
                a(this).removeClass("ui-state-focus")
            }), b.headers.next().addClass("ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom");
            if (c.navigation) {
                var d = b.element.find("a").filter(c.navigationFilter).eq(0);
                if (d.length) {
                    var e = d.closest(".ui-accordion-header");
                    e.length ? b.active = e : b.active = d.closest(".ui-accordion-content").prev()
                }
            }
            b.active = b._findActive(b.active || c.active).addClass("ui-state-default ui-state-active").toggleClass("ui-corner-all").toggleClass("ui-corner-top"), b.active.next().addClass("ui-accordion-content-active"), b._createIcons(), b.resize(), b.element.attr("role", "tablist"), b.headers.attr("role", "tab").bind("keydown.accordion",function (a) {
                return b._keydown(a)
            }).next().attr("role", "tabpanel"), b.headers.not(b.active || "").attr({"aria-expanded":"false", "aria-selected":"false", tabIndex:-1}).next().hide(), b.active.length ? b.active.attr({"aria-expanded":"true", "aria-selected":"true", tabIndex:0}) : b.headers.eq(0).attr("tabIndex", 0), a.browser.safari || b.headers.find("a").attr("tabIndex", -1), c.event && b.headers.bind(c.event.split(" ").join(".accordion ") + ".accordion", function (a) {
                b._clickHandler.call(b, a, this), a.preventDefault()
            })
        }, _createIcons:function () {
            var b = this.options;
            b.icons && (a("<span></span>").addClass("ui-icon " + b.icons.header).prependTo(this.headers), this.active.children(".ui-icon").toggleClass(b.icons.header).toggleClass(b.icons.headerSelected), this.element.addClass("ui-accordion-icons"))
        }, _destroyIcons:function () {
            this.headers.children(".ui-icon").remove(), this.element.removeClass("ui-accordion-icons")
        }, destroy:function () {
            var b = this.options;
            this.element.removeClass("ui-accordion ui-widget ui-helper-reset").removeAttr("role"), this.headers.unbind(".accordion").removeClass("ui-accordion-header ui-accordion-disabled ui-helper-reset ui-state-default ui-corner-all ui-state-active ui-state-disabled ui-corner-top").removeAttr("role").removeAttr("aria-expanded").removeAttr("aria-selected").removeAttr("tabIndex"), this.headers.find("a").removeAttr("tabIndex"), this._destroyIcons();
            var c = this.headers.next().css("display", "").removeAttr("role").removeClass("ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content ui-accordion-content-active ui-accordion-disabled ui-state-disabled");
            return(b.autoHeight || b.fillHeight) && c.css("height", ""), a.Widget.prototype.destroy.call(this)
        }, _setOption:function (b, c) {
            a.Widget.prototype._setOption.apply(this, arguments), b == "active" && this.activate(c), b == "icons" && (this._destroyIcons(), c && this._createIcons()), b == "disabled" && this.headers.add(this.headers.next())[c ? "addClass" : "removeClass"]("ui-accordion-disabled ui-state-disabled")
        }, _keydown:function (b) {
            if (this.options.disabled || b.altKey || b.ctrlKey)return;
            var c = a.ui.keyCode, d = this.headers.length, e = this.headers.index(b.target), f = !1;
            switch (b.keyCode) {
                case c.RIGHT:
                case c.DOWN:
                    f = this.headers[(e + 1) % d];
                    break;
                case c.LEFT:
                case c.UP:
                    f = this.headers[(e - 1 + d) % d];
                    break;
                case c.SPACE:
                case c.ENTER:
                    this._clickHandler({target:b.target}, b.target), b.preventDefault()
            }
            return f ? (a(b.target).attr("tabIndex", -1), a(f).attr("tabIndex", 0), f.focus(), !1) : !0
        }, resize:function () {
            var b = this.options, c;
            if (b.fillSpace) {
                if (a.browser.msie) {
                    var d = this.element.parent().css("overflow");
                    this.element.parent().css("overflow", "hidden")
                }
                c = this.element.parent().height(), a.browser.msie && this.element.parent().css("overflow", d), this.headers.each(function () {
                    c -= a(this).outerHeight(!0)
                }), this.headers.next().each(function () {
                    a(this).height(Math.max(0, c - a(this).innerHeight() + a(this).height()))
                }).css("overflow", "auto")
            } else b.autoHeight && (c = 0, this.headers.next().each(function () {
                c = Math.max(c, a(this).height("").height())
            }).height(c));
            return this
        }, activate:function (a) {
            this.options.active = a;
            var b = this._findActive(a)[0];
            return this._clickHandler({target:b}, b), this
        }, _findActive:function (b) {
            return b ? typeof b == "number" ? this.headers.filter(":eq(" + b + ")") : this.headers.not(this.headers.not(b)) : b === !1 ? a([]) : this.headers.filter(":eq(0)")
        }, _clickHandler:function (b, c) {
            var d = this.options;
            if (d.disabled)return;
            if (!b.target) {
                if (!d.collapsible)return;
                this.active.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all").children(".ui-icon").removeClass(d.icons.headerSelected).addClass(d.icons.header), this.active.next().addClass("ui-accordion-content-active");
                var e = this.active.next(), f = {options:d, newHeader:a([]), oldHeader:d.active, newContent:a([]), oldContent:e}, g = this.active = a([]);
                this._toggle(g, e, f);
                return
            }
            var h = a(b.currentTarget || c), i = h[0] === this.active[0];
            d.active = d.collapsible && i ? !1 : this.headers.index(h);
            if (this.running || !d.collapsible && i)return;
            var j = this.active, g = h.next(), e = this.active.next(), f = {options:d, newHeader:i && d.collapsible ? a([]) : h, oldHeader:this.active, newContent:i && d.collapsible ? a([]) : g, oldContent:e}, k = this.headers.index(this.active[0]) > this.headers.index(h[0]);
            this.active = i ? a([]) : h, this._toggle(g, e, f, i, k), j.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all").children(".ui-icon").removeClass(d.icons.headerSelected).addClass(d.icons.header), i || (h.removeClass("ui-state-default ui-corner-all").addClass("ui-state-active ui-corner-top").children(".ui-icon").removeClass(d.icons.header).addClass(d.icons.headerSelected), h.next().addClass("ui-accordion-content-active"));
            return
        }, _toggle:function (b, c, d, e, f) {
            var g = this, h = g.options;
            g.toShow = b, g.toHide = c, g.data = d;
            var i = function () {
                if (!g)return;
                return g._completed.apply(g, arguments)
            };
            g._trigger("changestart", null, g.data), g.running = c.size() === 0 ? b.size() : c.size();
            if (h.animated) {
                var j = {};
                h.collapsible && e ? j = {toShow:a([]), toHide:c, complete:i, down:f, autoHeight:h.autoHeight || h.fillSpace} : j = {toShow:b, toHide:c, complete:i, down:f, autoHeight:h.autoHeight || h.fillSpace}, h.proxied || (h.proxied = h.animated), h.proxiedDuration || (h.proxiedDuration = h.duration), h.animated = a.isFunction(h.proxied) ? h.proxied(j) : h.proxied, h.duration = a.isFunction(h.proxiedDuration) ? h.proxiedDuration(j) : h.proxiedDuration;
                var k = a.ui.accordion.animations, l = h.duration, m = h.animated;
                m && !k[m] && !a.easing[m] && (m = "slide"), k[m] || (k[m] = function (a) {
                    this.slide(a, {easing:m, duration:l || 700})
                }), k[m](j)
            } else h.collapsible && e ? b.toggle() : (c.hide(), b.show()), i(!0);
            c.prev().attr({"aria-expanded":"false", "aria-selected":"false", tabIndex:-1}).blur(), b.prev().attr({"aria-expanded":"true", "aria-selected":"true", tabIndex:0}).focus()
        }, _completed:function (a) {
            this.running = a ? 0 : --this.running;
            if (this.running)return;
            this.options.clearStyle && this.toShow.add(this.toHide).css({height:"", overflow:""}), this.toHide.removeClass("ui-accordion-content-active"), this.toHide.length && (this.toHide.parent()[0].className = this.toHide.parent()[0].className), this._trigger("change", null, this.data)
        }}), a.extend(a.ui.accordion, {version:"1.8.19", animations:{slide:function (b, c) {
            b = a.extend({easing:"swing", duration:300}, b, c);
            if (!b.toHide.size()) {
                b.toShow.animate({height:"show", paddingTop:"show", paddingBottom:"show"}, b);
                return
            }
            if (!b.toShow.size()) {
                b.toHide.animate({height:"hide", paddingTop:"hide", paddingBottom:"hide"}, b);
                return
            }
            var d = b.toShow.css("overflow"), e = 0, f = {}, g = {}, h = ["height", "paddingTop", "paddingBottom"], i, j = b.toShow;
            i = j[0].style.width, j.width(j.parent().width() - parseFloat(j.css("paddingLeft")) - parseFloat(j.css("paddingRight")) - (parseFloat(j.css("borderLeftWidth")) || 0) - (parseFloat(j.css("borderRightWidth")) || 0)), a.each(h, function (c, d) {
                g[d] = "hide";
                var e = ("" + a.css(b.toShow[0], d)).match(/^([\d+-.]+)(.*)$/);
                f[d] = {value:e[1], unit:e[2] || "px"}
            }), b.toShow.css({height:0, overflow:"hidden"}).show(), b.toHide.filter(":hidden").each(b.complete).end().filter(":visible").animate(g, {step:function (a, c) {
                c.prop == "height" && (e = c.end - c.start === 0 ? 0 : (c.now - c.start) / (c.end - c.start)), b.toShow[0].style[c.prop] = e * f[c.prop].value + f[c.prop].unit
            }, duration:b.duration, easing:b.easing, complete:function () {
                b.autoHeight || b.toShow.css("height", ""), b.toShow.css({width:i, overflow:d}), b.complete()
            }})
        }, bounceslide:function (a) {
            this.slide(a, {easing:a.down ? "easeOutBounce" : "swing", duration:a.down ? 1e3 : 200})
        }}})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.autocomplete.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        var c = 0;
        a.widget("ui.autocomplete", {options:{appendTo:"body", autoFocus:!1, delay:300, minLength:1, position:{my:"left top", at:"left bottom", collision:"none"}, source:null}, pending:0, _create:function () {
            var b = this, c = this.element[0].ownerDocument, d;
            this.isMultiLine = this.element.is("textarea"), this.element.addClass("ui-autocomplete-input").attr("autocomplete", "off").attr({role:"textbox", "aria-autocomplete":"list", "aria-haspopup":"true"}).bind("keydown.autocomplete",function (c) {
                if (b.options.disabled || b.element.propAttr("readOnly"))return;
                d = !1;
                var e = a.ui.keyCode;
                switch (c.keyCode) {
                    case e.PAGE_UP:
                        b._move("previousPage", c);
                        break;
                    case e.PAGE_DOWN:
                        b._move("nextPage", c);
                        break;
                    case e.UP:
                        b._keyEvent("previous", c);
                        break;
                    case e.DOWN:
                        b._keyEvent("next", c);
                        break;
                    case e.ENTER:
                    case e.NUMPAD_ENTER:
                        b.menu.active && (d = !0, c.preventDefault());
                    case e.TAB:
                        if (!b.menu.active)return;
                        b.menu.select(c);
                        break;
                    case e.ESCAPE:
                        b.element.val(b.term), b.close(c);
                        break;
                    default:
                        clearTimeout(b.searching), b.searching = setTimeout(function () {
                            b.term != b.element.val() && (b.selectedItem = null, b.search(null, c))
                        }, b.options.delay)
                }
            }).bind("keypress.autocomplete",function (a) {
                d && (d = !1, a.preventDefault())
            }).bind("focus.autocomplete",function () {
                if (b.options.disabled)return;
                b.selectedItem = null, b.previous = b.element.val()
            }).bind("blur.autocomplete", function (a) {
                if (b.options.disabled)return;
                clearTimeout(b.searching), b.closing = setTimeout(function () {
                    b.close(a), b._change(a)
                }, 150)
            }), this._initSource(), this.menu = a("<ul></ul>").addClass("ui-autocomplete").appendTo(a(this.options.appendTo || "body", c)[0]).mousedown(function (c) {
                var d = b.menu.element[0];
                a(c.target).closest(".ui-menu-item").length || setTimeout(function () {
                    a(document).one("mousedown", function (c) {
                        c.target !== b.element[0] && c.target !== d && !a.ui.contains(d, c.target) && b.close()
                    })
                }, 1), setTimeout(function () {
                    clearTimeout(b.closing)
                }, 13)
            }).menu({focus:function (a, c) {
                var d = c.item.data("item.autocomplete");
                !1 !== b._trigger("focus", a, {item:d}) && /^key/.test(a.originalEvent.type) && b.element.val(d.value)
            }, selected:function (a, d) {
                var e = d.item.data("item.autocomplete"), f = b.previous;
                b.element[0] !== c.activeElement && (b.element.focus(), b.previous = f, setTimeout(function () {
                    b.previous = f, b.selectedItem = e
                }, 1)), !1 !== b._trigger("select", a, {item:e}) && b.element.val(e.value), b.term = b.element.val(), b.close(a), b.selectedItem = e
            }, blur:function (a, c) {
                b.menu.element.is(":visible") && b.element.val() !== b.term && b.element.val(b.term)
            }}).zIndex(this.element.zIndex() + 1).css({top:0, left:0}).hide().data("menu"), a.fn.bgiframe && this.menu.element.bgiframe(), b.beforeunloadHandler = function () {
                b.element.removeAttr("autocomplete")
            }, a(window).bind("beforeunload", b.beforeunloadHandler)
        }, destroy:function () {
            this.element.removeClass("ui-autocomplete-input").removeAttr("autocomplete").removeAttr("role").removeAttr("aria-autocomplete").removeAttr("aria-haspopup"), this.menu.element.remove(), a(window).unbind("beforeunload", this.beforeunloadHandler), a.Widget.prototype.destroy.call(this)
        }, _setOption:function (b, c) {
            a.Widget.prototype._setOption.apply(this, arguments), b === "source" && this._initSource(), b === "appendTo" && this.menu.element.appendTo(a(c || "body", this.element[0].ownerDocument)[0]), b === "disabled" && c && this.xhr && this.xhr.abort()
        }, _initSource:function () {
            var b = this, c, d;
            a.isArray(this.options.source) ? (c = this.options.source, this.source = function (b, d) {
                d(a.ui.autocomplete.filter(c, b.term))
            }) : typeof this.options.source == "string" ? (d = this.options.source, this.source = function (c, e) {
                b.xhr && b.xhr.abort(), b.xhr = a.ajax({url:d, data:c, dataType:"json", success:function (a, b) {
                    e(a)
                }, error:function () {
                    e([])
                }})
            }) : this.source = this.options.source
        }, search:function (a, b) {
            a = a != null ? a : this.element.val(), this.term = this.element.val();
            if (a.length < this.options.minLength)return this.close(b);
            clearTimeout(this.closing);
            if (this._trigger("search", b) === !1)return;
            return this._search(a)
        }, _search:function (a) {
            this.pending++, this.element.addClass("ui-autocomplete-loading"), this.source({term:a}, this._response())
        }, _response:function () {
            var a = this, b = ++c;
            return function (d) {
                b === c && a.__response(d), a.pending--, a.pending || a.element.removeClass("ui-autocomplete-loading")
            }
        }, __response:function (a) {
            !this.options.disabled && a && a.length ? (a = this._normalize(a), this._suggest(a), this._trigger("open")) : this.close()
        }, close:function (a) {
            clearTimeout(this.closing), this.menu.element.is(":visible") && (this.menu.element.hide(), this.menu.deactivate(), this._trigger("close", a))
        }, _change:function (a) {
            this.previous !== this.element.val() && this._trigger("change", a, {item:this.selectedItem})
        }, _normalize:function (b) {
            return b.length && b[0].label && b[0].value ? b : a.map(b, function (b) {
                return typeof b == "string" ? {label:b, value:b} : a.extend({label:b.label || b.value, value:b.value || b.label}, b)
            })
        }, _suggest:function (b) {
            var c = this.menu.element.empty().zIndex(this.element.zIndex() + 1);
            this._renderMenu(c, b), this.menu.deactivate(), this.menu.refresh(), c.show(), this._resizeMenu(), c.position(a.extend({of:this.element}, this.options.position)), this.options.autoFocus && this.menu.next(new a.Event("mouseover"))
        }, _resizeMenu:function () {
            var a = this.menu.element;
            a.outerWidth(Math.max(a.width("").outerWidth() + 1, this.element.outerWidth()))
        }, _renderMenu:function (b, c) {
            var d = this;
            a.each(c, function (a, c) {
                d._renderItem(b, c)
            })
        }, _renderItem:function (b, c) {
            return a("<li></li>").data("item.autocomplete", c).append(a("<a></a>").text(c.label)).appendTo(b)
        }, _move:function (a, b) {
            if (!this.menu.element.is(":visible")) {
                this.search(null, b);
                return
            }
            if (this.menu.first() && /^previous/.test(a) || this.menu.last() && /^next/.test(a)) {
                this.element.val(this.term), this.menu.deactivate();
                return
            }
            this.menu[a](b)
        }, widget:function () {
            return this.menu.element
        }, _keyEvent:function (a, b) {
            if (!this.isMultiLine || this.menu.element.is(":visible"))this._move(a, b), b.preventDefault()
        }}), a.extend(a.ui.autocomplete, {escapeRegex:function (a) {
            return a.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&")
        }, filter:function (b, c) {
            var d = new RegExp(a.ui.autocomplete.escapeRegex(c), "i");
            return a.grep(b, function (a) {
                return d.test(a.label || a.value || a)
            })
        }})
    })(jQuery), function (a) {
        a.widget("ui.menu", {_create:function () {
            var b = this;
            this.element.addClass("ui-menu ui-widget ui-widget-content ui-corner-all").attr({role:"listbox", "aria-activedescendant":"ui-active-menuitem"}).click(function (c) {
                if (!a(c.target).closest(".ui-menu-item a").length)return;
                c.preventDefault(), b.select(c)
            }), this.refresh()
        }, refresh:function () {
            var b = this, c = this.element.children("li:not(.ui-menu-item):has(a)").addClass("ui-menu-item").attr("role", "menuitem");
            c.children("a").addClass("ui-corner-all").attr("tabindex", -1).mouseenter(function (c) {
                b.activate(c, a(this).parent())
            }).mouseleave(function () {
                b.deactivate()
            })
        }, activate:function (a, b) {
            this.deactivate();
            if (this.hasScroll()) {
                var c = b.offset().top - this.element.offset().top, d = this.element.scrollTop(), e = this.element.height();
                c < 0 ? this.element.scrollTop(d + c) : c >= e && this.element.scrollTop(d + c - e + b.height())
            }
            this.active = b.eq(0).children("a").addClass("ui-state-hover").attr("id", "ui-active-menuitem").end(), this._trigger("focus", a, {item:b})
        }, deactivate:function () {
            if (!this.active)return;
            this.active.children("a").removeClass("ui-state-hover").removeAttr("id"), this._trigger("blur"), this.active = null
        }, next:function (a) {
            this.move("next", ".ui-menu-item:first", a)
        }, previous:function (a) {
            this.move("prev", ".ui-menu-item:last", a)
        }, first:function () {
            return this.active && !this.active.prevAll(".ui-menu-item").length
        }, last:function () {
            return this.active && !this.active.nextAll(".ui-menu-item").length
        }, move:function (a, b, c) {
            if (!this.active) {
                this.activate(c, this.element.children(b));
                return
            }
            var d = this.active[a + "All"](".ui-menu-item").eq(0);
            d.length ? this.activate(c, d) : this.activate(c, this.element.children(b))
        }, nextPage:function (b) {
            if (this.hasScroll()) {
                if (!this.active || this.last()) {
                    this.activate(b, this.element.children(".ui-menu-item:first"));
                    return
                }
                var c = this.active.offset().top, d = this.element.height(), e = this.element.children(".ui-menu-item").filter(function () {
                    var b = a(this).offset().top - c - d + a(this).height();
                    return b < 10 && b > -10
                });
                e.length || (e = this.element.children(".ui-menu-item:last")), this.activate(b, e)
            } else this.activate(b, this.element.children(".ui-menu-item").filter(!this.active || this.last() ? ":first" : ":last"))
        }, previousPage:function (b) {
            if (this.hasScroll()) {
                if (!this.active || this.first()) {
                    this.activate(b, this.element.children(".ui-menu-item:last"));
                    return
                }
                var c = this.active.offset().top, d = this.element.height(), e = this.element.children(".ui-menu-item").filter(function () {
                    var b = a(this).offset().top - c + d - a(this).height();
                    return b < 10 && b > -10
                });
                e.length || (e = this.element.children(".ui-menu-item:first")), this.activate(b, e)
            } else this.activate(b, this.element.children(".ui-menu-item").filter(!this.active || this.first() ? ":last" : ":first"))
        }, hasScroll:function () {
            return this.element.height() < this.element[a.fn.prop ? "prop" : "attr"]("scrollHeight")
        }, select:function (a) {
            this._trigger("selected", a, {item:this.active})
        }})
    }(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.button.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        var c, d, e, f, g = "ui-button ui-widget ui-state-default ui-corner-all", h = "ui-state-hover ui-state-active ", i = "ui-button-icons-only ui-button-icon-only ui-button-text-icons ui-button-text-icon-primary ui-button-text-icon-secondary ui-button-text-only", j = function () {
            var b = a(this).find(":ui-button");
            setTimeout(function () {
                b.button("refresh")
            }, 1)
        }, k = function (b) {
            var c = b.name, d = b.form, e = a([]);
            return c && (d ? e = a(d).find("[name='" + c + "']") : e = a("[name='" + c + "']", b.ownerDocument).filter(function () {
                return!this.form
            })), e
        };
        a.widget("ui.button", {options:{disabled:null, text:!0, label:null, icons:{primary:null, secondary:null}}, _create:function () {
            this.element.closest("form").unbind("reset.button").bind("reset.button", j), typeof this.options.disabled != "boolean" ? this.options.disabled = !!this.element.propAttr("disabled") : this.element.propAttr("disabled", this.options.disabled), this._determineButtonType(), this.hasTitle = !!this.buttonElement.attr("title");
            var b = this, h = this.options, i = this.type === "checkbox" || this.type === "radio", l = "ui-state-hover" + (i ? "" : " ui-state-active"), m = "ui-state-focus";
            h.label === null && (h.label = this.buttonElement.html()), this.buttonElement.addClass(g).attr("role", "button").bind("mouseenter.button",function () {
                if (h.disabled)return;
                a(this).addClass("ui-state-hover"), this === c && a(this).addClass("ui-state-active")
            }).bind("mouseleave.button",function () {
                if (h.disabled)return;
                a(this).removeClass(l)
            }).bind("click.button", function (a) {
                h.disabled && (a.preventDefault(), a.stopImmediatePropagation())
            }), this.element.bind("focus.button",function () {
                b.buttonElement.addClass(m)
            }).bind("blur.button", function () {
                b.buttonElement.removeClass(m)
            }), i && (this.element.bind("change.button", function () {
                if (f)return;
                b.refresh()
            }), this.buttonElement.bind("mousedown.button",function (a) {
                if (h.disabled)return;
                f = !1, d = a.pageX, e = a.pageY
            }).bind("mouseup.button", function (a) {
                if (h.disabled)return;
                if (d !== a.pageX || e !== a.pageY)f = !0
            })), this.type === "checkbox" ? this.buttonElement.bind("click.button", function () {
                if (h.disabled || f)return!1;
                a(this).toggleClass("ui-state-active"), b.buttonElement.attr("aria-pressed", b.element[0].checked)
            }) : this.type === "radio" ? this.buttonElement.bind("click.button", function () {
                if (h.disabled || f)return!1;
                a(this).addClass("ui-state-active"), b.buttonElement.attr("aria-pressed", "true");
                var c = b.element[0];
                k(c).not(c).map(function () {
                    return a(this).button("widget")[0]
                }).removeClass("ui-state-active").attr("aria-pressed", "false")
            }) : (this.buttonElement.bind("mousedown.button",function () {
                if (h.disabled)return!1;
                a(this).addClass("ui-state-active"), c = this, a(document).one("mouseup", function () {
                    c = null
                })
            }).bind("mouseup.button",function () {
                if (h.disabled)return!1;
                a(this).removeClass("ui-state-active")
            }).bind("keydown.button",function (b) {
                if (h.disabled)return!1;
                (b.keyCode == a.ui.keyCode.SPACE || b.keyCode == a.ui.keyCode.ENTER) && a(this).addClass("ui-state-active")
            }).bind("keyup.button", function () {
                a(this).removeClass("ui-state-active")
            }), this.buttonElement.is("a") && this.buttonElement.keyup(function (b) {
                b.keyCode === a.ui.keyCode.SPACE && a(this).click()
            })), this._setOption("disabled", h.disabled), this._resetButton()
        }, _determineButtonType:function () {
            this.element.is(":checkbox") ? this.type = "checkbox" : this.element.is(":radio") ? this.type = "radio" : this.element.is("input") ? this.type = "input" : this.type = "button";
            if (this.type === "checkbox" || this.type === "radio") {
                var a = this.element.parents().filter(":last"), b = "label[for='" + this.element.attr("id") + "']";
                this.buttonElement = a.find(b), this.buttonElement.length || (a = a.length ? a.siblings() : this.element.siblings(), this.buttonElement = a.filter(b), this.buttonElement.length || (this.buttonElement = a.find(b))), this.element.addClass("ui-helper-hidden-accessible");
                var c = this.element.is(":checked");
                c && this.buttonElement.addClass("ui-state-active"), this.buttonElement.attr("aria-pressed", c)
            } else this.buttonElement = this.element
        }, widget:function () {
            return this.buttonElement
        }, destroy:function () {
            this.element.removeClass("ui-helper-hidden-accessible"), this.buttonElement.removeClass(g + " " + h + " " + i).removeAttr("role").removeAttr("aria-pressed").html(this.buttonElement.find(".ui-button-text").html()), this.hasTitle || this.buttonElement.removeAttr("title"), a.Widget.prototype.destroy.call(this)
        }, _setOption:function (b, c) {
            a.Widget.prototype._setOption.apply(this, arguments);
            if (b === "disabled") {
                c ? this.element.propAttr("disabled", !0) : this.element.propAttr("disabled", !1);
                return
            }
            this._resetButton()
        }, refresh:function () {
            var b = this.element.is(":disabled");
            b !== this.options.disabled && this._setOption("disabled", b), this.type === "radio" ? k(this.element[0]).each(function () {
                a(this).is(":checked") ? a(this).button("widget").addClass("ui-state-active").attr("aria-pressed", "true") : a(this).button("widget").removeClass("ui-state-active").attr("aria-pressed", "false")
            }) : this.type === "checkbox" && (this.element.is(":checked") ? this.buttonElement.addClass("ui-state-active").attr("aria-pressed", "true") : this.buttonElement.removeClass("ui-state-active").attr("aria-pressed", "false"))
        }, _resetButton:function () {
            if (this.type === "input") {
                this.options.label && this.element.val(this.options.label);
                return
            }
            var b = this.buttonElement.removeClass(i), c = a("<span></span>", this.element[0].ownerDocument).addClass("ui-button-text").html(this.options.label).appendTo(b.empty()).text(), d = this.options.icons, e = d.primary && d.secondary, f = [];
            d.primary || d.secondary ? (this.options.text && f.push("ui-button-text-icon" + (e ? "s" : d.primary ? "-primary" : "-secondary")), d.primary && b.prepend("<span class='ui-button-icon-primary ui-icon " + d.primary + "'></span>"), d.secondary && b.append("<span class='ui-button-icon-secondary ui-icon " + d.secondary + "'></span>"), this.options.text || (f.push(e ? "ui-button-icons-only" : "ui-button-icon-only"), this.hasTitle || b.attr("title", c))) : f.push("ui-button-text-only"), b.addClass(f.join(" "))
        }}), a.widget("ui.buttonset", {options:{items:":button, :submit, :reset, :checkbox, :radio, a, :data(button)"}, _create:function () {
            this.element.addClass("ui-buttonset")
        }, _init:function () {
            this.refresh()
        }, _setOption:function (b, c) {
            b === "disabled" && this.buttons.button("option", b, c), a.Widget.prototype._setOption.apply(this, arguments)
        }, refresh:function () {
            var b = this.element.css("direction") === "rtl";
            this.buttons = this.element.find(this.options.items).filter(":ui-button").button("refresh").end().not(":ui-button").button().end().map(function () {
                return a(this).button("widget")[0]
            }).removeClass("ui-corner-all ui-corner-left ui-corner-right").filter(":first").addClass(b ? "ui-corner-right" : "ui-corner-left").end().filter(":last").addClass(b ? "ui-corner-left" : "ui-corner-right").end().end()
        }, destroy:function () {
            this.element.removeClass("ui-buttonset"), this.buttons.map(function () {
                return a(this).button("widget")[0]
            }).removeClass("ui-corner-left ui-corner-right").end().button("destroy"), a.Widget.prototype.destroy.call(this)
        }})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.dialog.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        var c = "ui-dialog ui-widget ui-widget-content ui-corner-all ", d = {buttons:!0, height:!0, maxHeight:!0, maxWidth:!0, minHeight:!0, minWidth:!0, width:!0}, e = {maxHeight:!0, maxWidth:!0, minHeight:!0, minWidth:!0}, f = a.attrFn || {val:!0, css:!0, html:!0, text:!0, data:!0, width:!0, height:!0, offset:!0, click:!0};
        a.widget("ui.dialog", {options:{autoOpen:!0, buttons:{}, closeOnEscape:!0, closeText:"close", dialogClass:"", draggable:!0, hide:null, height:"auto", maxHeight:!1, maxWidth:!1, minHeight:150, minWidth:150, modal:!1, position:{my:"center", at:"center", collision:"fit", using:function (b) {
            var c = a(this).css(b).offset().top;
            c < 0 && a(this).css("top", b.top - c)
        }}, resizable:!0, show:null, stack:!0, title:"", width:300, zIndex:1e3}, _create:function () {
            this.originalTitle = this.element.attr("title"), typeof this.originalTitle != "string" && (this.originalTitle = ""), this.options.title = this.options.title || this.originalTitle;
            var b = this, d = b.options, e = d.title || "&#160;", f = a.ui.dialog.getTitleId(b.element), g = (b.uiDialog = a("<div></div>")).appendTo(document.body).hide().addClass(c + d.dialogClass).css({zIndex:d.zIndex}).attr("tabIndex", -1).css("outline", 0).keydown(function (c) {
                d.closeOnEscape && !c.isDefaultPrevented() && c.keyCode && c.keyCode === a.ui.keyCode.ESCAPE && (b.close(c), c.preventDefault())
            }).attr({role:"dialog", "aria-labelledby":f}).mousedown(function (a) {
                b.moveToTop(!1, a)
            }), h = b.element.show().removeAttr("title").addClass("ui-dialog-content ui-widget-content").appendTo(g), i = (b.uiDialogTitlebar = a("<div></div>")).addClass("ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix").prependTo(g), j = a('<a href="#"></a>').addClass("ui-dialog-titlebar-close ui-corner-all").attr("role", "button").hover(function () {
                j.addClass("ui-state-hover")
            },function () {
                j.removeClass("ui-state-hover")
            }).focus(function () {
                j.addClass("ui-state-focus")
            }).blur(function () {
                j.removeClass("ui-state-focus")
            }).click(function (a) {
                return b.close(a), !1
            }).appendTo(i), k = (b.uiDialogTitlebarCloseText = a("<span></span>")).addClass("ui-icon ui-icon-closethick").text(d.closeText).appendTo(j), l = a("<span></span>").addClass("ui-dialog-title").attr("id", f).html(e).prependTo(i);
            a.isFunction(d.beforeclose) && !a.isFunction(d.beforeClose) && (d.beforeClose = d.beforeclose), i.find("*").add(i).disableSelection(), d.draggable && a.fn.draggable && b._makeDraggable(), d.resizable && a.fn.resizable && b._makeResizable(), b._createButtons(d.buttons), b._isOpen = !1, a.fn.bgiframe && g.bgiframe()
        }, _init:function () {
            this.options.autoOpen && this.open()
        }, destroy:function () {
            var a = this;
            return a.overlay && a.overlay.destroy(), a.uiDialog.hide(), a.element.unbind(".dialog").removeData("dialog").removeClass("ui-dialog-content ui-widget-content").hide().appendTo("body"), a.uiDialog.remove(), a.originalTitle && a.element.attr("title", a.originalTitle), a
        }, widget:function () {
            return this.uiDialog
        }, close:function (b) {
            var c = this, d, e;
            if (!1 === c._trigger("beforeClose", b))return;
            return c.overlay && c.overlay.destroy(), c.uiDialog.unbind("keypress.ui-dialog"), c._isOpen = !1, c.options.hide ? c.uiDialog.hide(c.options.hide, function () {
                c._trigger("close", b)
            }) : (c.uiDialog.hide(), c._trigger("close", b)), a.ui.dialog.overlay.resize(), c.options.modal && (d = 0, a(".ui-dialog").each(function () {
                this !== c.uiDialog[0] && (e = a(this).css("z-index"), isNaN(e) || (d = Math.max(d, e)))
            }), a.ui.dialog.maxZ = d), c
        }, isOpen:function () {
            return this._isOpen
        }, moveToTop:function (b, c) {
            var d = this, e = d.options, f;
            return e.modal && !b || !e.stack && !e.modal ? d._trigger("focus", c) : (e.zIndex > a.ui.dialog.maxZ && (a.ui.dialog.maxZ = e.zIndex), d.overlay && (a.ui.dialog.maxZ += 1, d.overlay.$el.css("z-index", a.ui.dialog.overlay.maxZ = a.ui.dialog.maxZ)), f = {scrollTop:d.element.scrollTop(), scrollLeft:d.element.scrollLeft()}, a.ui.dialog.maxZ += 1, d.uiDialog.css("z-index", a.ui.dialog.maxZ), d.element.attr(f), d._trigger("focus", c), d)
        }, open:function () {
            if (this._isOpen)return;
            var b = this, c = b.options, d = b.uiDialog;
            return b.overlay = c.modal ? new a.ui.dialog.overlay(b) : null, b._size(), b._position(c.position), d.show(c.show), b.moveToTop(!0), c.modal && d.bind("keydown.ui-dialog", function (b) {
                if (b.keyCode !== a.ui.keyCode.TAB)return;
                var c = a(":tabbable", this), d = c.filter(":first"), e = c.filter(":last");
                if (b.target === e[0] && !b.shiftKey)return d.focus(1), !1;
                if (b.target === d[0] && b.shiftKey)return e.focus(1), !1
            }), a(b.element.find(":tabbable").get().concat(d.find(".ui-dialog-buttonpane :tabbable").get().concat(d.get()))).eq(0).focus(), b._isOpen = !0, b._trigger("open"), b
        }, _createButtons:function (b) {
            var c = this, d = !1, e = a("<div></div>").addClass("ui-dialog-buttonpane ui-widget-content ui-helper-clearfix"), g = a("<div></div>").addClass("ui-dialog-buttonset").appendTo(e);
            c.uiDialog.find(".ui-dialog-buttonpane").remove(), typeof b == "object" && b !== null && a.each(b, function () {
                return!(d = !0)
            }), d && (a.each(b, function (b, d) {
                d = a.isFunction(d) ? {click:d, text:b} : d;
                var e = a('<button type="button"></button>').click(function () {
                    d.click.apply(c.element[0], arguments)
                }).appendTo(g);
                a.each(d, function (a, b) {
                    if (a === "click")return;
                    a in f ? e[a](b) : e.attr(a, b)
                }), a.fn.button && e.button()
            }), e.appendTo(c.uiDialog))
        }, _makeDraggable:function () {
            function f(a) {
                return{position:a.position, offset:a.offset}
            }

            var b = this, c = b.options, d = a(document), e;
            b.uiDialog.draggable({cancel:".ui-dialog-content, .ui-dialog-titlebar-close", handle:".ui-dialog-titlebar", containment:"document", start:function (d, g) {
                e = c.height === "auto" ? "auto" : a(this).height(), a(this).height(a(this).height()).addClass("ui-dialog-dragging"), b._trigger("dragStart", d, f(g))
            }, drag:function (a, c) {
                b._trigger("drag", a, f(c))
            }, stop:function (g, h) {
                c.position = [h.position.left - d.scrollLeft(), h.position.top - d.scrollTop()], a(this).removeClass("ui-dialog-dragging").height(e), b._trigger("dragStop", g, f(h)), a.ui.dialog.overlay.resize()
            }})
        }, _makeResizable:function (c) {
            function h(a) {
                return{originalPosition:a.originalPosition, originalSize:a.originalSize, position:a.position, size:a.size}
            }

            c = c === b ? this.options.resizable : c;
            var d = this, e = d.options, f = d.uiDialog.css("position"), g = typeof c == "string" ? c : "n,e,s,w,se,sw,ne,nw";
            d.uiDialog.resizable({cancel:".ui-dialog-content", containment:"document", alsoResize:d.element, maxWidth:e.maxWidth, maxHeight:e.maxHeight, minWidth:e.minWidth, minHeight:d._minHeight(), handles:g, start:function (b, c) {
                a(this).addClass("ui-dialog-resizing"), d._trigger("resizeStart", b, h(c))
            }, resize:function (a, b) {
                d._trigger("resize", a, h(b))
            }, stop:function (b, c) {
                a(this).removeClass("ui-dialog-resizing"), e.height = a(this).height(), e.width = a(this).width(), d._trigger("resizeStop", b, h(c)), a.ui.dialog.overlay.resize()
            }}).css("position", f).find(".ui-resizable-se").addClass("ui-icon ui-icon-grip-diagonal-se")
        }, _minHeight:function () {
            var a = this.options;
            return a.height === "auto" ? a.minHeight : Math.min(a.minHeight, a.height)
        }, _position:function (b) {
            var c = [], d = [0, 0], e;
            if (b) {
                if (typeof b == "string" || typeof b == "object" && "0"in b)c = b.split ? b.split(" ") : [b[0], b[1]], c.length === 1 && (c[1] = c[0]), a.each(["left", "top"], function (a, b) {
                    +c[a] === c[a] && (d[a] = c[a], c[a] = b)
                }), b = {my:c.join(" "), at:c.join(" "), offset:d.join(" ")};
                b = a.extend({}, a.ui.dialog.prototype.options.position, b)
            } else b = a.ui.dialog.prototype.options.position;
            e = this.uiDialog.is(":visible"), e || this.uiDialog.show(), this.uiDialog.css({top:0, left:0}).position(a.extend({of:window}, b)), e || this.uiDialog.hide()
        }, _setOptions:function (b) {
            var c = this, f = {}, g = !1;
            a.each(b, function (a, b) {
                c._setOption(a, b), a in d && (g = !0), a in e && (f[a] = b)
            }), g && this._size(), this.uiDialog.is(":data(resizable)") && this.uiDialog.resizable("option", f)
        }, _setOption:function (b, d) {
            var e = this, f = e.uiDialog;
            switch (b) {
                case"beforeclose":
                    b = "beforeClose";
                    break;
                case"buttons":
                    e._createButtons(d);
                    break;
                case"closeText":
                    e.uiDialogTitlebarCloseText.text("" + d);
                    break;
                case"dialogClass":
                    f.removeClass(e.options.dialogClass).addClass(c + d);
                    break;
                case"disabled":
                    d ? f.addClass("ui-dialog-disabled") : f.removeClass("ui-dialog-disabled");
                    break;
                case"draggable":
                    var g = f.is(":data(draggable)");
                    g && !d && f.draggable("destroy"), !g && d && e._makeDraggable();
                    break;
                case"position":
                    e._position(d);
                    break;
                case"resizable":
                    var h = f.is(":data(resizable)");
                    h && !d && f.resizable("destroy"), h && typeof d == "string" && f.resizable("option", "handles", d), !h && d !== !1 && e._makeResizable(d);
                    break;
                case"title":
                    a(".ui-dialog-title", e.uiDialogTitlebar).html("" + (d || "&#160;"))
            }
            a.Widget.prototype._setOption.apply(e, arguments)
        }, _size:function () {
            var b = this.options, c, d, e = this.uiDialog.is(":visible");
            this.element.show().css({width:"auto", minHeight:0, height:0}), b.minWidth > b.width && (b.width = b.minWidth), c = this.uiDialog.css({height:"auto", width:b.width}).height(), d = Math.max(0, b.minHeight - c);
            if (b.height === "auto")if (a.support.minHeight)this.element.css({minHeight:d, height:"auto"}); else {
                this.uiDialog.show();
                var f = this.element.css("height", "auto").height();
                e || this.uiDialog.hide(), this.element.height(Math.max(f, d))
            } else this.element.height(Math.max(b.height - c, 0));
            this.uiDialog.is(":data(resizable)") && this.uiDialog.resizable("option", "minHeight", this._minHeight())
        }}), a.extend(a.ui.dialog, {version:"1.8.19", uuid:0, maxZ:0, getTitleId:function (a) {
            var b = a.attr("id");
            return b || (this.uuid += 1, b = this.uuid), "ui-dialog-title-" + b
        }, overlay:function (b) {
            this.$el = a.ui.dialog.overlay.create(b)
        }}), a.extend(a.ui.dialog.overlay, {instances:[], oldInstances:[], maxZ:0, events:a.map("focus,mousedown,mouseup,keydown,keypress,click".split(","),function (a) {
            return a + ".dialog-overlay"
        }).join(" "), create:function (b) {
            this.instances.length === 0 && (setTimeout(function () {
                a.ui.dialog.overlay.instances.length && a(document).bind(a.ui.dialog.overlay.events, function (b) {
                    if (a(b.target).zIndex() < a.ui.dialog.overlay.maxZ)return!1
                })
            }, 1), a(document).bind("keydown.dialog-overlay", function (c) {
                b.options.closeOnEscape && !c.isDefaultPrevented() && c.keyCode && c.keyCode === a.ui.keyCode.ESCAPE && (b.close(c), c.preventDefault())
            }), a(window).bind("resize.dialog-overlay", a.ui.dialog.overlay.resize));
            var c = (this.oldInstances.pop() || a("<div></div>").addClass("ui-widget-overlay")).appendTo(document.body).css({width:this.width(), height:this.height()});
            return a.fn.bgiframe && c.bgiframe(), this.instances.push(c), c
        }, destroy:function (b) {
            var c = a.inArray(b, this.instances);
            c != -1 && this.oldInstances.push(this.instances.splice(c, 1)[0]), this.instances.length === 0 && a([document, window]).unbind(".dialog-overlay"), b.remove();
            var d = 0;
            a.each(this.instances, function () {
                d = Math.max(d, this.css("z-index"))
            }), this.maxZ = d
        }, height:function () {
            var b, c;
            return a.browser.msie && a.browser.version < 7 ? (b = Math.max(document.documentElement.scrollHeight, document.body.scrollHeight), c = Math.max(document.documentElement.offsetHeight, document.body.offsetHeight), b < c ? a(window).height() + "px" : b + "px") : a(document).height() + "px"
        }, width:function () {
            var b, c;
            return a.browser.msie ? (b = Math.max(document.documentElement.scrollWidth, document.body.scrollWidth), c = Math.max(document.documentElement.offsetWidth, document.body.offsetWidth), b < c ? a(window).width() + "px" : b + "px") : a(document).width() + "px"
        }, resize:function () {
            var b = a([]);
            a.each(a.ui.dialog.overlay.instances, function () {
                b = b.add(this)
            }), b.css({width:0, height:0}).css({width:a.ui.dialog.overlay.width(), height:a.ui.dialog.overlay.height()})
        }}), a.extend(a.ui.dialog.overlay.prototype, {destroy:function () {
            a.ui.dialog.overlay.destroy(this.$el)
        }})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.slider.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        var c = 5;
        a.widget("ui.slider", a.ui.mouse, {widgetEventPrefix:"slide", options:{animate:!1, distance:0, max:100, min:0, orientation:"horizontal", range:!1, step:1, value:0, values:null}, _create:function () {
            var b = this, d = this.options, e = this.element.find(".ui-slider-handle").addClass("ui-state-default ui-corner-all"), f = "<a class='ui-slider-handle ui-state-default ui-corner-all' href='#'></a>", g = d.values && d.values.length || 1, h = [];
            this._keySliding = !1, this._mouseSliding = !1, this._animateOff = !0, this._handleIndex = null, this._detectOrientation(), this._mouseInit(), this.element.addClass("ui-slider ui-slider-" + this.orientation + " ui-widget" + " ui-widget-content" + " ui-corner-all" + (d.disabled ? " ui-slider-disabled ui-disabled" : "")), this.range = a([]), d.range && (d.range === !0 && (d.values || (d.values = [this._valueMin(), this._valueMin()]), d.values.length && d.values.length !== 2 && (d.values = [d.values[0], d.values[0]])), this.range = a("<div></div>").appendTo(this.element).addClass("ui-slider-range ui-widget-header" + (d.range === "min" || d.range === "max" ? " ui-slider-range-" + d.range : "")));
            for (var i = e.length; i < g; i += 1)h.push(f);
            this.handles = e.add(a(h.join("")).appendTo(b.element)), this.handle = this.handles.eq(0), this.handles.add(this.range).filter("a").click(function (a) {
                a.preventDefault()
            }).hover(function () {
                d.disabled || a(this).addClass("ui-state-hover")
            },function () {
                a(this).removeClass("ui-state-hover")
            }).focus(function () {
                d.disabled ? a(this).blur() : (a(".ui-slider .ui-state-focus").removeClass("ui-state-focus"), a(this).addClass("ui-state-focus"))
            }).blur(function () {
                a(this).removeClass("ui-state-focus")
            }), this.handles.each(function (b) {
                a(this).data("index.ui-slider-handle", b)
            }), this.handles.keydown(function (d) {
                var e = a(this).data("index.ui-slider-handle"), f, g, h, i;
                if (b.options.disabled)return;
                switch (d.keyCode) {
                    case a.ui.keyCode.HOME:
                    case a.ui.keyCode.END:
                    case a.ui.keyCode.PAGE_UP:
                    case a.ui.keyCode.PAGE_DOWN:
                    case a.ui.keyCode.UP:
                    case a.ui.keyCode.RIGHT:
                    case a.ui.keyCode.DOWN:
                    case a.ui.keyCode.LEFT:
                        d.preventDefault();
                        if (!b._keySliding) {
                            b._keySliding = !0, a(this).addClass("ui-state-active"), f = b._start(d, e);
                            if (f === !1)return
                        }
                }
                i = b.options.step, b.options.values && b.options.values.length ? g = h = b.values(e) : g = h = b.value();
                switch (d.keyCode) {
                    case a.ui.keyCode.HOME:
                        h = b._valueMin();
                        break;
                    case a.ui.keyCode.END:
                        h = b._valueMax();
                        break;
                    case a.ui.keyCode.PAGE_UP:
                        h = b._trimAlignValue(g + (b._valueMax() - b._valueMin()) / c);
                        break;
                    case a.ui.keyCode.PAGE_DOWN:
                        h = b._trimAlignValue(g - (b._valueMax() - b._valueMin()) / c);
                        break;
                    case a.ui.keyCode.UP:
                    case a.ui.keyCode.RIGHT:
                        if (g === b._valueMax())return;
                        h = b._trimAlignValue(g + i);
                        break;
                    case a.ui.keyCode.DOWN:
                    case a.ui.keyCode.LEFT:
                        if (g === b._valueMin())return;
                        h = b._trimAlignValue(g - i)
                }
                b._slide(d, e, h)
            }).keyup(function (c) {
                var d = a(this).data("index.ui-slider-handle");
                b._keySliding && (b._keySliding = !1, b._stop(c, d), b._change(c, d), a(this).removeClass("ui-state-active"))
            }), this._refreshValue(), this._animateOff = !1
        }, destroy:function () {
            return this.handles.remove(), this.range.remove(), this.element.removeClass("ui-slider ui-slider-horizontal ui-slider-vertical ui-slider-disabled ui-widget ui-widget-content ui-corner-all").removeData("slider").unbind(".slider"), this._mouseDestroy(), this
        }, _mouseCapture:function (b) {
            var c = this.options, d, e, f, g, h, i, j, k, l;
            return c.disabled ? !1 : (this.elementSize = {width:this.element.outerWidth(), height:this.element.outerHeight()}, this.elementOffset = this.element.offset(), d = {x:b.pageX, y:b.pageY}, e = this._normValueFromMouse(d), f = this._valueMax() - this._valueMin() + 1, h = this, this.handles.each(function (b) {
                var c = Math.abs(e - h.values(b));
                f > c && (f = c, g = a(this), i = b)
            }), c.range === !0 && this.values(1) === c.min && (i += 1, g = a(this.handles[i])), j = this._start(b, i), j === !1 ? !1 : (this._mouseSliding = !0, h._handleIndex = i, g.addClass("ui-state-active").focus(), k = g.offset(), l = !a(b.target).parents().andSelf().is(".ui-slider-handle"), this._clickOffset = l ? {left:0, top:0} : {left:b.pageX - k.left - g.width() / 2, top:b.pageY - k.top - g.height() / 2 - (parseInt(g.css("borderTopWidth"), 10) || 0) - (parseInt(g.css("borderBottomWidth"), 10) || 0) + (parseInt(g.css("marginTop"), 10) || 0)}, this.handles.hasClass("ui-state-hover") || this._slide(b, i, e), this._animateOff = !0, !0))
        }, _mouseStart:function (a) {
            return!0
        }, _mouseDrag:function (a) {
            var b = {x:a.pageX, y:a.pageY}, c = this._normValueFromMouse(b);
            return this._slide(a, this._handleIndex, c), !1
        }, _mouseStop:function (a) {
            return this.handles.removeClass("ui-state-active"), this._mouseSliding = !1, this._stop(a, this._handleIndex), this._change(a, this._handleIndex), this._handleIndex = null, this._clickOffset = null, this._animateOff = !1, !1
        }, _detectOrientation:function () {
            this.orientation = this.options.orientation === "vertical" ? "vertical" : "horizontal"
        }, _normValueFromMouse:function (a) {
            var b, c, d, e, f;
            return this.orientation === "horizontal" ? (b = this.elementSize.width, c = a.x - this.elementOffset.left - (this._clickOffset ? this._clickOffset.left : 0)) : (b = this.elementSize.height, c = a.y - this.elementOffset.top - (this._clickOffset ? this._clickOffset.top : 0)), d = c / b, d > 1 && (d = 1), d < 0 && (d = 0), this.orientation === "vertical" && (d = 1 - d), e = this._valueMax() - this._valueMin(), f = this._valueMin() + d * e, this._trimAlignValue(f)
        }, _start:function (a, b) {
            var c = {handle:this.handles[b], value:this.value()};
            return this.options.values && this.options.values.length && (c.value = this.values(b), c.values = this.values()), this._trigger("start", a, c)
        }, _slide:function (a, b, c) {
            var d, e, f;
            this.options.values && this.options.values.length ? (d = this.values(b ? 0 : 1), this.options.values.length === 2 && this.options.range === !0 && (b === 0 && c > d || b === 1 && c < d) && (c = d), c !== this.values(b) && (e = this.values(), e[b] = c, f = this._trigger("slide", a, {handle:this.handles[b], value:c, values:e}), d = this.values(b ? 0 : 1), f !== !1 && this.values(b, c, !0))) : c !== this.value() && (f = this._trigger("slide", a, {handle:this.handles[b], value:c}), f !== !1 && this.value(c))
        }, _stop:function (a, b) {
            var c = {handle:this.handles[b], value:this.value()};
            this.options.values && this.options.values.length && (c.value = this.values(b), c.values = this.values()), this._trigger("stop", a, c)
        }, _change:function (a, b) {
            if (!this._keySliding && !this._mouseSliding) {
                var c = {handle:this.handles[b], value:this.value()};
                this.options.values && this.options.values.length && (c.value = this.values(b), c.values = this.values()), this._trigger("change", a, c)
            }
        }, value:function (a) {
            if (arguments.length) {
                this.options.value = this._trimAlignValue(a), this._refreshValue(), this._change(null, 0);
                return
            }
            return this._value()
        }, values:function (b, c) {
            var d, e, f;
            if (arguments.length > 1) {
                this.options.values[b] = this._trimAlignValue(c), this._refreshValue(), this._change(null, b);
                return
            }
            if (!arguments.length)return this._values();
            if (!a.isArray(arguments[0]))return this.options.values && this.options.values.length ? this._values(b) : this.value();
            d = this.options.values, e = arguments[0];
            for (f = 0; f < d.length; f += 1)d[f] = this._trimAlignValue(e[f]), this._change(null, f);
            this._refreshValue()
        }, _setOption:function (b, c) {
            var d, e = 0;
            a.isArray(this.options.values) && (e = this.options.values.length), a.Widget.prototype._setOption.apply(this, arguments);
            switch (b) {
                case"disabled":
                    c ? (this.handles.filter(".ui-state-focus").blur(), this.handles.removeClass("ui-state-hover"), this.handles.propAttr("disabled", !0), this.element.addClass("ui-disabled")) : (this.handles.propAttr("disabled", !1), this.element.removeClass("ui-disabled"));
                    break;
                case"orientation":
                    this._detectOrientation(), this.element.removeClass("ui-slider-horizontal ui-slider-vertical").addClass("ui-slider-" + this.orientation), this._refreshValue();
                    break;
                case"value":
                    this._animateOff = !0, this._refreshValue(), this._change(null, 0), this._animateOff = !1;
                    break;
                case"values":
                    this._animateOff = !0, this._refreshValue();
                    for (d = 0; d < e; d += 1)this._change(null, d);
                    this._animateOff = !1
            }
        }, _value:function () {
            var a = this.options.value;
            return a = this._trimAlignValue(a), a
        }, _values:function (a) {
            var b, c, d;
            if (arguments.length)return b = this.options.values[a], b = this._trimAlignValue(b), b;
            c = this.options.values.slice();
            for (d = 0; d < c.length; d += 1)c[d] = this._trimAlignValue(c[d]);
            return c
        }, _trimAlignValue:function (a) {
            if (a <= this._valueMin())return this._valueMin();
            if (a >= this._valueMax())return this._valueMax();
            var b = this.options.step > 0 ? this.options.step : 1, c = (a - this._valueMin()) % b, d = a - c;
            return Math.abs(c) * 2 >= b && (d += c > 0 ? b : -b), parseFloat(d.toFixed(5))
        }, _valueMin:function () {
            return this.options.min
        }, _valueMax:function () {
            return this.options.max
        }, _refreshValue:function () {
            var b = this.options.range, c = this.options, d = this, e = this._animateOff ? !1 : c.animate, f, g = {}, h, i, j, k;
            this.options.values && this.options.values.length ? this.handles.each(function (b, i) {
                f = (d.values(b) - d._valueMin()) / (d._valueMax() - d._valueMin()) * 100, g[d.orientation === "horizontal" ? "left" : "bottom"] = f + "%", a(this).stop(1, 1)[e ? "animate" : "css"](g, c.animate), d.options.range === !0 && (d.orientation === "horizontal" ? (b === 0 && d.range.stop(1, 1)[e ? "animate" : "css"]({left:f + "%"}, c.animate), b === 1 && d.range[e ? "animate" : "css"]({width:f - h + "%"}, {queue:!1, duration:c.animate})) : (b === 0 && d.range.stop(1, 1)[e ? "animate" : "css"]({bottom:f + "%"}, c.animate), b === 1 && d.range[e ? "animate" : "css"]({height:f - h + "%"}, {queue:!1, duration:c.animate}))), h = f
            }) : (i = this.value(), j = this._valueMin(), k = this._valueMax(), f = k !== j ? (i - j) / (k - j) * 100 : 0, g[d.orientation === "horizontal" ? "left" : "bottom"] = f + "%", this.handle.stop(1, 1)[e ? "animate" : "css"](g, c.animate), b === "min" && this.orientation === "horizontal" && this.range.stop(1, 1)[e ? "animate" : "css"]({width:f + "%"}, c.animate), b === "max" && this.orientation === "horizontal" && this.range[e ? "animate" : "css"]({width:100 - f + "%"}, {queue:!1, duration:c.animate}), b === "min" && this.orientation === "vertical" && this.range.stop(1, 1)[e ? "animate" : "css"]({height:f + "%"}, c.animate), b === "max" && this.orientation === "vertical" && this.range[e ? "animate" : "css"]({height:100 - f + "%"}, {queue:!1, duration:c.animate}))
        }}), a.extend(a.ui.slider, {version:"1.8.19"})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.tabs.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        function e() {
            return++c
        }

        function f() {
            return++d
        }

        var c = 0, d = 0;
        a.widget("ui.tabs", {options:{add:null, ajaxOptions:null, cache:!1, cookie:null, collapsible:!1, disable:null, disabled:[], enable:null, event:"click", fx:null, idPrefix:"ui-tabs-", load:null, panelTemplate:"<div></div>", remove:null, select:null, show:null, spinner:"<em>Loading&#8230;</em>", tabTemplate:"<li><a href='#{href}'><span>#{label}</span></a></li>"}, _create:function () {
            this._tabify(!0)
        }, _setOption:function (a, b) {
            if (a == "selected") {
                if (this.options.collapsible && b == this.options.selected)return;
                this.select(b)
            } else this.options[a] = b, this._tabify()
        }, _tabId:function (a) {
            return a.title && a.title.replace(/\s/g, "_").replace(/[^\w\u00c0-\uFFFF-]/g, "") || this.options.idPrefix + e()
        }, _sanitizeSelector:function (a) {
            return a.replace(/:/g, "\\:")
        }, _cookie:function () {
            var b = this.cookie || (this.cookie = this.options.cookie.name || "ui-tabs-" + f());
            return a.cookie.apply(null, [b].concat(a.makeArray(arguments)))
        }, _ui:function (a, b) {
            return{tab:a, panel:b, index:this.anchors.index(a)}
        }, _cleanup:function () {
            this.lis.filter(".ui-state-processing").removeClass("ui-state-processing").find("span:data(label.tabs)").each(function () {
                var b = a(this);
                b.html(b.data("label.tabs")).removeData("label.tabs")
            })
        }, _tabify:function (c) {
            function m(b, c) {
                b.css("display", ""), !a.support.opacity && c.opacity && b[0].style.removeAttribute("filter")
            }

            var d = this, e = this.options, f = /^#.+/;
            this.list = this.element.find("ol,ul").eq(0), this.lis = a(" > li:has(a[href])", this.list), this.anchors = this.lis.map(function () {
                return a("a", this)[0]
            }), this.panels = a([]), this.anchors.each(function (b, c) {
                var g = a(c).attr("href"), h = g.split("#")[0], i;
                h && (h === location.toString().split("#")[0] || (i = a("base")[0]) && h === i.href) && (g = c.hash, c.href = g);
                if (f.test(g))d.panels = d.panels.add(d.element.find(d._sanitizeSelector(g))); else if (g && g !== "#") {
                    a.data(c, "href.tabs", g), a.data(c, "load.tabs", g.replace(/#.*$/, ""));
                    var j = d._tabId(c);
                    c.href = "#" + j;
                    var k = d.element.find("#" + j);
                    k.length || (k = a(e.panelTemplate).attr("id", j).addClass("ui-tabs-panel ui-widget-content ui-corner-bottom").insertAfter(d.panels[b - 1] || d.list), k.data("destroy.tabs", !0)), d.panels = d.panels.add(k)
                } else e.disabled.push(b)
            }), c ? (this.element.addClass("ui-tabs ui-widget ui-widget-content ui-corner-all"), this.list.addClass("ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"), this.lis.addClass("ui-state-default ui-corner-top"), this.panels.addClass("ui-tabs-panel ui-widget-content ui-corner-bottom"), e.selected === b ? (location.hash && this.anchors.each(function (a, b) {
                if (b.hash == location.hash)return e.selected = a, !1
            }), typeof e.selected != "number" && e.cookie && (e.selected = parseInt(d._cookie(), 10)), typeof e.selected != "number" && this.lis.filter(".ui-tabs-selected").length && (e.selected = this.lis.index(this.lis.filter(".ui-tabs-selected"))), e.selected = e.selected || (this.lis.length ? 0 : -1)) : e.selected === null && (e.selected = -1), e.selected = e.selected >= 0 && this.anchors[e.selected] || e.selected < 0 ? e.selected : 0, e.disabled = a.unique(e.disabled.concat(a.map(this.lis.filter(".ui-state-disabled"), function (a, b) {
                return d.lis.index(a)
            }))).sort(), a.inArray(e.selected, e.disabled) != -1 && e.disabled.splice(a.inArray(e.selected, e.disabled), 1), this.panels.addClass("ui-tabs-hide"), this.lis.removeClass("ui-tabs-selected ui-state-active"), e.selected >= 0 && this.anchors.length && (d.element.find(d._sanitizeSelector(d.anchors[e.selected].hash)).removeClass("ui-tabs-hide"), this.lis.eq(e.selected).addClass("ui-tabs-selected ui-state-active"), d.element.queue("tabs", function () {
                d._trigger("show", null, d._ui(d.anchors[e.selected], d.element.find(d._sanitizeSelector(d.anchors[e.selected].hash))[0]))
            }), this.load(e.selected)), a(window).bind("unload", function () {
                d.lis.add(d.anchors).unbind(".tabs"), d.lis = d.anchors = d.panels = null
            })) : e.selected = this.lis.index(this.lis.filter(".ui-tabs-selected")), this.element[e.collapsible ? "addClass" : "removeClass"]("ui-tabs-collapsible"), e.cookie && this._cookie(e.selected, e.cookie);
            for (var g = 0, h; h = this.lis[g]; g++)a(h)[a.inArray(g, e.disabled) != -1 && !a(h).hasClass("ui-tabs-selected") ? "addClass" : "removeClass"]("ui-state-disabled");
            e.cache === !1 && this.anchors.removeData("cache.tabs"), this.lis.add(this.anchors).unbind(".tabs");
            if (e.event !== "mouseover") {
                var i = function (a, b) {
                    b.is(":not(.ui-state-disabled)") && b.addClass("ui-state-" + a)
                }, j = function (a, b) {
                    b.removeClass("ui-state-" + a)
                };
                this.lis.bind("mouseover.tabs", function () {
                    i("hover", a(this))
                }), this.lis.bind("mouseout.tabs", function () {
                    j("hover", a(this))
                }), this.anchors.bind("focus.tabs", function () {
                    i("focus", a(this).closest("li"))
                }), this.anchors.bind("blur.tabs", function () {
                    j("focus", a(this).closest("li"))
                })
            }
            var k, l;
            e.fx && (a.isArray(e.fx) ? (k = e.fx[0], l = e.fx[1]) : k = l = e.fx);
            var n = l ? function (b, c) {
                a(b).closest("li").addClass("ui-tabs-selected ui-state-active"), c.hide().removeClass("ui-tabs-hide").animate(l, l.duration || "normal", function () {
                    m(c, l), d._trigger("show", null, d._ui(b, c[0]))
                })
            } : function (b, c) {
                a(b).closest("li").addClass("ui-tabs-selected ui-state-active"), c.removeClass("ui-tabs-hide"), d._trigger("show", null, d._ui(b, c[0]))
            }, o = k ? function (a, b) {
                b.animate(k, k.duration || "normal", function () {
                    d.lis.removeClass("ui-tabs-selected ui-state-active"), b.addClass("ui-tabs-hide"), m(b, k), d.element.dequeue("tabs")
                })
            } : function (a, b, c) {
                d.lis.removeClass("ui-tabs-selected ui-state-active"), b.addClass("ui-tabs-hide"), d.element.dequeue("tabs")
            };
            this.anchors.bind(e.event + ".tabs", function () {
                var b = this, c = a(b).closest("li"), f = d.panels.filter(":not(.ui-tabs-hide)"), g = d.element.find(d._sanitizeSelector(b.hash));
                if (c.hasClass("ui-tabs-selected") && !e.collapsible || c.hasClass("ui-state-disabled") || c.hasClass("ui-state-processing") || d.panels.filter(":animated").length || d._trigger("select", null, d._ui(this, g[0])) === !1)return this.blur(), !1;
                e.selected = d.anchors.index(this), d.abort();
                if (e.collapsible) {
                    if (c.hasClass("ui-tabs-selected"))return e.selected = -1, e.cookie && d._cookie(e.selected, e.cookie), d.element.queue("tabs",function () {
                        o(b, f)
                    }).dequeue("tabs"), this.blur(), !1;
                    if (!f.length)return e.cookie && d._cookie(e.selected, e.cookie), d.element.queue("tabs", function () {
                        n(b, g)
                    }), d.load(d.anchors.index(this)), this.blur(), !1
                }
                e.cookie && d._cookie(e.selected, e.cookie);
                if (g.length)f.length && d.element.queue("tabs", function () {
                    o(b, f)
                }), d.element.queue("tabs", function () {
                    n(b, g)
                }), d.load(d.anchors.index(this)); else throw"jQuery UI Tabs: Mismatching fragment identifier.";
                a.browser.msie && this.blur()
            }), this.anchors.bind("click.tabs", function () {
                return!1
            })
        }, _getIndex:function (a) {
            return typeof a == "string" && (a = this.anchors.index(this.anchors.filter("[href$='" + a + "']"))), a
        }, destroy:function () {
            var b = this.options;
            return this.abort(), this.element.unbind(".tabs").removeClass("ui-tabs ui-widget ui-widget-content ui-corner-all ui-tabs-collapsible").removeData("tabs"), this.list.removeClass("ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"), this.anchors.each(function () {
                var b = a.data(this, "href.tabs");
                b && (this.href = b);
                var c = a(this).unbind(".tabs");
                a.each(["href", "load", "cache"], function (a, b) {
                    c.removeData(b + ".tabs")
                })
            }), this.lis.unbind(".tabs").add(this.panels).each(function () {
                a.data(this, "destroy.tabs") ? a(this).remove() : a(this).removeClass(["ui-state-default", "ui-corner-top", "ui-tabs-selected", "ui-state-active", "ui-state-hover", "ui-state-focus", "ui-state-disabled", "ui-tabs-panel", "ui-widget-content", "ui-corner-bottom", "ui-tabs-hide"].join(" "))
            }), b.cookie && this._cookie(null, b.cookie), this
        }, add:function (c, d, e) {
            e === b && (e = this.anchors.length);
            var f = this, g = this.options, h = a(g.tabTemplate.replace(/#\{href\}/g, c).replace(/#\{label\}/g, d)), i = c.indexOf("#") ? this._tabId(a("a", h)[0]) : c.replace("#", "");
            h.addClass("ui-state-default ui-corner-top").data("destroy.tabs", !0);
            var j = f.element.find("#" + i);
            return j.length || (j = a(g.panelTemplate).attr("id", i).data("destroy.tabs", !0)), j.addClass("ui-tabs-panel ui-widget-content ui-corner-bottom ui-tabs-hide"), e >= this.lis.length ? (h.appendTo(this.list), j.appendTo(this.list[0].parentNode)) : (h.insertBefore(this.lis[e]), j.insertBefore(this.panels[e])), g.disabled = a.map(g.disabled, function (a, b) {
                return a >= e ? ++a : a
            }), this._tabify(), this.anchors.length == 1 && (g.selected = 0, h.addClass("ui-tabs-selected ui-state-active"), j.removeClass("ui-tabs-hide"), this.element.queue("tabs", function () {
                f._trigger("show", null, f._ui(f.anchors[0], f.panels[0]))
            }), this.load(0)), this._trigger("add", null, this._ui(this.anchors[e], this.panels[e])), this
        }, remove:function (b) {
            b = this._getIndex(b);
            var c = this.options, d = this.lis.eq(b).remove(), e = this.panels.eq(b).remove();
            return d.hasClass("ui-tabs-selected") && this.anchors.length > 1 && this.select(b + (b + 1 < this.anchors.length ? 1 : -1)), c.disabled = a.map(a.grep(c.disabled, function (a, c) {
                return a != b
            }), function (a, c) {
                return a >= b ? --a : a
            }), this._tabify(), this._trigger("remove", null, this._ui(d.find("a")[0], e[0])), this
        }, enable:function (b) {
            b = this._getIndex(b);
            var c = this.options;
            if (a.inArray(b, c.disabled) == -1)return;
            return this.lis.eq(b).removeClass("ui-state-disabled"), c.disabled = a.grep(c.disabled, function (a, c) {
                return a != b
            }), this._trigger("enable", null, this._ui(this.anchors[b], this.panels[b])), this
        }, disable:function (a) {
            a = this._getIndex(a);
            var b = this, c = this.options;
            return a != c.selected && (this.lis.eq(a).addClass("ui-state-disabled"), c.disabled.push(a), c.disabled.sort(), this._trigger("disable", null, this._ui(this.anchors[a], this.panels[a]))), this
        }, select:function (a) {
            a = this._getIndex(a);
            if (a == -1)if (this.options.collapsible && this.options.selected != -1)a = this.options.selected; else return this;
            return this.anchors.eq(a).trigger(this.options.event + ".tabs"), this
        }, load:function (b) {
            b = this._getIndex(b);
            var c = this, d = this.options, e = this.anchors.eq(b)[0], f = a.data(e, "load.tabs");
            this.abort();
            if (!f || this.element.queue("tabs").length !== 0 && a.data(e, "cache.tabs")) {
                this.element.dequeue("tabs");
                return
            }
            this.lis.eq(b).addClass("ui-state-processing");
            if (d.spinner) {
                var g = a("span", e);
                g.data("label.tabs", g.html()).html(d.spinner)
            }
            return this.xhr = a.ajax(a.extend({}, d.ajaxOptions, {url:f, success:function (f, g) {
                c.element.find(c._sanitizeSelector(e.hash)).html(f), c._cleanup(), d.cache && a.data(e, "cache.tabs", !0), c._trigger("load", null, c._ui(c.anchors[b], c.panels[b]));
                try {
                    d.ajaxOptions.success(f, g)
                } catch (h) {
                }
            }, error:function (a, f, g) {
                c._cleanup(), c._trigger("load", null, c._ui(c.anchors[b], c.panels[b]));
                try {
                    d.ajaxOptions.error(a, f, b, e)
                } catch (g) {
                }
            }})), c.element.dequeue("tabs"), this
        }, abort:function () {
            return this.element.queue([]), this.panels.stop(!1, !0), this.element.queue("tabs", this.element.queue("tabs").splice(-2, 2)), this.xhr && (this.xhr.abort(), delete this.xhr), this._cleanup(), this
        }, url:function (a, b) {
            return this.anchors.eq(a).removeData("cache.tabs").data("load.tabs", b), this
        }, length:function () {
            return this.anchors.length
        }}), a.extend(a.ui.tabs, {version:"1.8.19"}), a.extend(a.ui.tabs.prototype, {rotation:null, rotate:function (a, b) {
            var c = this, d = this.options, e = c._rotate || (c._rotate = function (b) {
                clearTimeout(c.rotation), c.rotation = setTimeout(function () {
                    var a = d.selected;
                    c.select(++a < c.anchors.length ? a : 0)
                }, a), b && b.stopPropagation()
            }), f = c._unrotate || (c._unrotate = b ? function (a) {
                e()
            } : function (a) {
                a.clientX && c.rotate(null)
            });
            return a ? (this.element.bind("tabsshow", e), this.anchors.bind(d.event + ".tabs", f), e()) : (clearTimeout(c.rotation), this.element.unbind("tabsshow", e), this.anchors.unbind(d.event + ".tabs", f), delete this._rotate, delete this._unrotate), this
        }})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.datepicker.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function ($, undefined) {
        function Datepicker() {
            this.debug = !1, this._curInst = null, this._keyEvent = !1, this._disabledInputs = [], this._datepickerShowing = !1, this._inDialog = !1, this._mainDivId = "ui-datepicker-div", this._inlineClass = "ui-datepicker-inline", this._appendClass = "ui-datepicker-append", this._triggerClass = "ui-datepicker-trigger", this._dialogClass = "ui-datepicker-dialog", this._disableClass = "ui-datepicker-disabled", this._unselectableClass = "ui-datepicker-unselectable", this._currentClass = "ui-datepicker-current-day", this._dayOverClass = "ui-datepicker-days-cell-over", this.regional = [], this.regional[""] = {closeText:"Done", prevText:"Prev", nextText:"Next", currentText:"Today", monthNames:["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], monthNamesShort:["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], dayNames:["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], dayNamesShort:["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], dayNamesMin:["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"], weekHeader:"Wk", dateFormat:"mm/dd/yy", firstDay:0, isRTL:!1, showMonthAfterYear:!1, yearSuffix:""}, this._defaults = {showOn:"focus", showAnim:"fadeIn", showOptions:{}, defaultDate:null, appendText:"", buttonText:"...", buttonImage:"", buttonImageOnly:!1, hideIfNoPrevNext:!1, navigationAsDateFormat:!1, gotoCurrent:!1, changeMonth:!1, changeYear:!1, yearRange:"c-10:c+10", showOtherMonths:!1, selectOtherMonths:!1, showWeek:!1, calculateWeek:this.iso8601Week, shortYearCutoff:"+10", minDate:null, maxDate:null, duration:"fast", beforeShowDay:null, beforeShow:null, onSelect:null, onChangeMonthYear:null, onClose:null, numberOfMonths:1, showCurrentAtPos:0, stepMonths:1, stepBigMonths:12, altField:"", altFormat:"", constrainInput:!0, showButtonPanel:!1, autoSize:!1, disabled:!1}, $.extend(this._defaults, this.regional[""]), this.dpDiv = bindHover($('<div id="' + this._mainDivId + '" class="ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"></div>'))
        }

        function bindHover(a) {
            var b = "button, .ui-datepicker-prev, .ui-datepicker-next, .ui-datepicker-calendar td a";
            return a.bind("mouseout",function (a) {
                var c = $(a.target).closest(b);
                if (!c.length)return;
                c.removeClass("ui-state-hover ui-datepicker-prev-hover ui-datepicker-next-hover")
            }).bind("mouseover", function (c) {
                var d = $(c.target).closest(b);
                if ($.datepicker._isDisabledDatepicker(instActive.inline ? a.parent()[0] : instActive.input[0]) || !d.length)return;
                d.parents(".ui-datepicker-calendar").find("a").removeClass("ui-state-hover"), d.addClass("ui-state-hover"), d.hasClass("ui-datepicker-prev") && d.addClass("ui-datepicker-prev-hover"), d.hasClass("ui-datepicker-next") && d.addClass("ui-datepicker-next-hover")
            })
        }

        function extendRemove(a, b) {
            $.extend(a, b);
            for (var c in b)if (b[c] == null || b[c] == undefined)a[c] = b[c];
            return a
        }

        function isArray(a) {
            return a && ($.browser.safari && typeof a == "object" && a.length || a.constructor && a.constructor.toString().match(/\Array\(\)/))
        }

        $.extend($.ui, {datepicker:{version:"1.8.19"}});
        var PROP_NAME = "datepicker", dpuuid = (new Date).getTime(), instActive;
        $.extend(Datepicker.prototype, {markerClassName:"hasDatepicker", maxRows:4, log:function () {
            this.debug && console.log.apply("", arguments)
        }, _widgetDatepicker:function () {
            return this.dpDiv
        }, setDefaults:function (a) {
            return extendRemove(this._defaults, a || {}), this
        }, _attachDatepicker:function (target, settings) {
            var inlineSettings = null;
            for (var attrName in this._defaults) {
                var attrValue = target.getAttribute("date:" + attrName);
                if (attrValue) {
                    inlineSettings = inlineSettings || {};
                    try {
                        inlineSettings[attrName] = eval(attrValue)
                    } catch (err) {
                        inlineSettings[attrName] = attrValue
                    }
                }
            }
            var nodeName = target.nodeName.toLowerCase(), inline = nodeName == "div" || nodeName == "span";
            target.id || (this.uuid += 1, target.id = "dp" + this.uuid);
            var inst = this._newInst($(target), inline);
            inst.settings = $.extend({}, settings || {}, inlineSettings || {}), nodeName == "input" ? this._connectDatepicker(target, inst) : inline && this._inlineDatepicker(target, inst)
        }, _newInst:function (a, b) {
            var c = a[0].id.replace(/([^A-Za-z0-9_-])/g, "\\\\$1");
            return{id:c, input:a, selectedDay:0, selectedMonth:0, selectedYear:0, drawMonth:0, drawYear:0, inline:b, dpDiv:b ? bindHover($('<div class="' + this._inlineClass + ' ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"></div>')) : this.dpDiv}
        }, _connectDatepicker:function (a, b) {
            var c = $(a);
            b.append = $([]), b.trigger = $([]);
            if (c.hasClass(this.markerClassName))return;
            this._attachments(c, b), c.addClass(this.markerClassName).keydown(this._doKeyDown).keypress(this._doKeyPress).keyup(this._doKeyUp).bind("setData.datepicker",function (a, c, d) {
                b.settings[c] = d
            }).bind("getData.datepicker", function (a, c) {
                return this._get(b, c)
            }), this._autoSize(b), $.data(a, PROP_NAME, b), b.settings.disabled && this._disableDatepicker(a)
        }, _attachments:function (a, b) {
            var c = this._get(b, "appendText"), d = this._get(b, "isRTL");
            b.append && b.append.remove(), c && (b.append = $('<span class="' + this._appendClass + '">' + c + "</span>"), a[d ? "before" : "after"](b.append)), a.unbind("focus", this._showDatepicker), b.trigger && b.trigger.remove();
            var e = this._get(b, "showOn");
            (e == "focus" || e == "both") && a.focus(this._showDatepicker);
            if (e == "button" || e == "both") {
                var f = this._get(b, "buttonText"), g = this._get(b, "buttonImage");
                b.trigger = $(this._get(b, "buttonImageOnly") ? $("<img/>").addClass(this._triggerClass).attr({src:g, alt:f, title:f}) : $('<button type="button"></button>').addClass(this._triggerClass).html(g == "" ? f : $("<img/>").attr({src:g, alt:f, title:f}))), a[d ? "before" : "after"](b.trigger), b.trigger.click(function () {
                    return $.datepicker._datepickerShowing && $.datepicker._lastInput == a[0] ? $.datepicker._hideDatepicker() : $.datepicker._datepickerShowing && $.datepicker._lastInput != a[0] ? ($.datepicker._hideDatepicker(), $.datepicker._showDatepicker(a[0])) : $.datepicker._showDatepicker(a[0]), !1
                })
            }
        }, _autoSize:function (a) {
            if (this._get(a, "autoSize") && !a.inline) {
                var b = new Date(2009, 11, 20), c = this._get(a, "dateFormat");
                if (c.match(/[DM]/)) {
                    var d = function (a) {
                        var b = 0, c = 0;
                        for (var d = 0; d < a.length; d++)a[d].length > b && (b = a[d].length, c = d);
                        return c
                    };
                    b.setMonth(d(this._get(a, c.match(/MM/) ? "monthNames" : "monthNamesShort"))), b.setDate(d(this._get(a, c.match(/DD/) ? "dayNames" : "dayNamesShort")) + 20 - b.getDay())
                }
                a.input.attr("size", this._formatDate(a, b).length)
            }
        }, _inlineDatepicker:function (a, b) {
            var c = $(a);
            if (c.hasClass(this.markerClassName))return;
            c.addClass(this.markerClassName).append(b.dpDiv).bind("setData.datepicker",function (a, c, d) {
                b.settings[c] = d
            }).bind("getData.datepicker", function (a, c) {
                return this._get(b, c)
            }), $.data(a, PROP_NAME, b), this._setDate(b, this._getDefaultDate(b), !0), this._updateDatepicker(b), this._updateAlternate(b), b.settings.disabled && this._disableDatepicker(a), b.dpDiv.css("display", "block")
        }, _dialogDatepicker:function (a, b, c, d, e) {
            var f = this._dialogInst;
            if (!f) {
                this.uuid += 1;
                var g = "dp" + this.uuid;
                this._dialogInput = $('<input type="text" id="' + g + '" style="position: absolute; top: -100px; width: 0px; z-index: -10;"/>'), this._dialogInput.keydown(this._doKeyDown), $("body").append(this._dialogInput), f = this._dialogInst = this._newInst(this._dialogInput, !1), f.settings = {}, $.data(this._dialogInput[0], PROP_NAME, f)
            }
            extendRemove(f.settings, d || {}), b = b && b.constructor == Date ? this._formatDate(f, b) : b, this._dialogInput.val(b), this._pos = e ? e.length ? e : [e.pageX, e.pageY] : null;
            if (!this._pos) {
                var h = document.documentElement.clientWidth, i = document.documentElement.clientHeight, j = document.documentElement.scrollLeft || document.body.scrollLeft, k = document.documentElement.scrollTop || document.body.scrollTop;
                this._pos = [h / 2 - 100 + j, i / 2 - 150 + k]
            }
            return this._dialogInput.css("left", this._pos[0] + 20 + "px").css("top", this._pos[1] + "px"), f.settings.onSelect = c, this._inDialog = !0, this.dpDiv.addClass(this._dialogClass), this._showDatepicker(this._dialogInput[0]), $.blockUI && $.blockUI(this.dpDiv), $.data(this._dialogInput[0], PROP_NAME, f), this
        }, _destroyDatepicker:function (a) {
            var b = $(a), c = $.data(a, PROP_NAME);
            if (!b.hasClass(this.markerClassName))return;
            var d = a.nodeName.toLowerCase();
            $.removeData(a, PROP_NAME), d == "input" ? (c.append.remove(), c.trigger.remove(), b.removeClass(this.markerClassName).unbind("focus", this._showDatepicker).unbind("keydown", this._doKeyDown).unbind("keypress", this._doKeyPress).unbind("keyup", this._doKeyUp)) : (d == "div" || d == "span") && b.removeClass(this.markerClassName).empty()
        }, _enableDatepicker:function (a) {
            var b = $(a), c = $.data(a, PROP_NAME);
            if (!b.hasClass(this.markerClassName))return;
            var d = a.nodeName.toLowerCase();
            if (d == "input")a.disabled = !1, c.trigger.filter("button").each(function () {
                this.disabled = !1
            }).end().filter("img").css({opacity:"1.0", cursor:""}); else if (d == "div" || d == "span") {
                var e = b.children("." + this._inlineClass);
                e.children().removeClass("ui-state-disabled"), e.find("select.ui-datepicker-month, select.ui-datepicker-year").removeAttr("disabled")
            }
            this._disabledInputs = $.map(this._disabledInputs, function (b) {
                return b == a ? null : b
            })
        }, _disableDatepicker:function (a) {
            var b = $(a), c = $.data(a, PROP_NAME);
            if (!b.hasClass(this.markerClassName))return;
            var d = a.nodeName.toLowerCase();
            if (d == "input")a.disabled = !0, c.trigger.filter("button").each(function () {
                this.disabled = !0
            }).end().filter("img").css({opacity:"0.5", cursor:"default"}); else if (d == "div" || d == "span") {
                var e = b.children("." + this._inlineClass);
                e.children().addClass("ui-state-disabled"), e.find("select.ui-datepicker-month, select.ui-datepicker-year").attr("disabled", "disabled")
            }
            this._disabledInputs = $.map(this._disabledInputs, function (b) {
                return b == a ? null : b
            }), this._disabledInputs[this._disabledInputs.length] = a
        }, _isDisabledDatepicker:function (a) {
            if (!a)return!1;
            for (var b = 0; b < this._disabledInputs.length; b++)if (this._disabledInputs[b] == a)return!0;
            return!1
        }, _getInst:function (a) {
            try {
                return $.data(a, PROP_NAME)
            } catch (b) {
                throw"Missing instance data for this datepicker"
            }
        }, _optionDatepicker:function (a, b, c) {
            var d = this._getInst(a);
            if (arguments.length == 2 && typeof b == "string")return b == "defaults" ? $.extend({}, $.datepicker._defaults) : d ? b == "all" ? $.extend({}, d.settings) : this._get(d, b) : null;
            var e = b || {};
            typeof b == "string" && (e = {}, e[b] = c);
            if (d) {
                this._curInst == d && this._hideDatepicker();
                var f = this._getDateDatepicker(a, !0), g = this._getMinMaxDate(d, "min"), h = this._getMinMaxDate(d, "max");
                extendRemove(d.settings, e), g !== null && e.dateFormat !== undefined && e.minDate === undefined && (d.settings.minDate = this._formatDate(d, g)), h !== null && e.dateFormat !== undefined && e.maxDate === undefined && (d.settings.maxDate = this._formatDate(d, h)), this._attachments($(a), d), this._autoSize(d), this._setDate(d, f), this._updateAlternate(d), this._updateDatepicker(d)
            }
        }, _changeDatepicker:function (a, b, c) {
            this._optionDatepicker(a, b, c)
        }, _refreshDatepicker:function (a) {
            var b = this._getInst(a);
            b && this._updateDatepicker(b)
        }, _setDateDatepicker:function (a, b) {
            var c = this._getInst(a);
            c && (this._setDate(c, b), this._updateDatepicker(c), this._updateAlternate(c))
        }, _getDateDatepicker:function (a, b) {
            var c = this._getInst(a);
            return c && !c.inline && this._setDateFromField(c, b), c ? this._getDate(c) : null
        }, _doKeyDown:function (a) {
            var b = $.datepicker._getInst(a.target), c = !0, d = b.dpDiv.is(".ui-datepicker-rtl");
            b._keyEvent = !0;
            if ($.datepicker._datepickerShowing)switch (a.keyCode) {
                case 9:
                    $.datepicker._hideDatepicker(), c = !1;
                    break;
                case 13:
                    var e = $("td." + $.datepicker._dayOverClass + ":not(." + $.datepicker._currentClass + ")", b.dpDiv);
                    e[0] && $.datepicker._selectDay(a.target, b.selectedMonth, b.selectedYear, e[0]);
                    var f = $.datepicker._get(b, "onSelect");
                    if (f) {
                        var g = $.datepicker._formatDate(b);
                        f.apply(b.input ? b.input[0] : null, [g, b])
                    } else $.datepicker._hideDatepicker();
                    return!1;
                case 27:
                    $.datepicker._hideDatepicker();
                    break;
                case 33:
                    $.datepicker._adjustDate(a.target, a.ctrlKey ? -$.datepicker._get(b, "stepBigMonths") : -$.datepicker._get(b, "stepMonths"), "M");
                    break;
                case 34:
                    $.datepicker._adjustDate(a.target, a.ctrlKey ? +$.datepicker._get(b, "stepBigMonths") : +$.datepicker._get(b, "stepMonths"), "M");
                    break;
                case 35:
                    (a.ctrlKey || a.metaKey) && $.datepicker._clearDate(a.target), c = a.ctrlKey || a.metaKey;
                    break;
                case 36:
                    (a.ctrlKey || a.metaKey) && $.datepicker._gotoToday(a.target), c = a.ctrlKey || a.metaKey;
                    break;
                case 37:
                    (a.ctrlKey || a.metaKey) && $.datepicker._adjustDate(a.target, d ? 1 : -1, "D"), c = a.ctrlKey || a.metaKey, a.originalEvent.altKey && $.datepicker._adjustDate(a.target, a.ctrlKey ? -$.datepicker._get(b, "stepBigMonths") : -$.datepicker._get(b, "stepMonths"), "M");
                    break;
                case 38:
                    (a.ctrlKey || a.metaKey) && $.datepicker._adjustDate(a.target, -7, "D"), c = a.ctrlKey || a.metaKey;
                    break;
                case 39:
                    (a.ctrlKey || a.metaKey) && $.datepicker._adjustDate(a.target, d ? -1 : 1, "D"), c = a.ctrlKey || a.metaKey, a.originalEvent.altKey && $.datepicker._adjustDate(a.target, a.ctrlKey ? +$.datepicker._get(b, "stepBigMonths") : +$.datepicker._get(b, "stepMonths"), "M");
                    break;
                case 40:
                    (a.ctrlKey || a.metaKey) && $.datepicker._adjustDate(a.target, 7, "D"), c = a.ctrlKey || a.metaKey;
                    break;
                default:
                    c = !1
            } else a.keyCode == 36 && a.ctrlKey ? $.datepicker._showDatepicker(this) : c = !1;
            c && (a.preventDefault(), a.stopPropagation())
        }, _doKeyPress:function (a) {
            var b = $.datepicker._getInst(a.target);
            if ($.datepicker._get(b, "constrainInput")) {
                var c = $.datepicker._possibleChars($.datepicker._get(b, "dateFormat")), d = String.fromCharCode(a.charCode == undefined ? a.keyCode : a.charCode);
                return a.ctrlKey || a.metaKey || d < " " || !c || c.indexOf(d) > -1
            }
        }, _doKeyUp:function (a) {
            var b = $.datepicker._getInst(a.target);
            if (b.input.val() != b.lastVal)try {
                var c = $.datepicker.parseDate($.datepicker._get(b, "dateFormat"), b.input ? b.input.val() : null, $.datepicker._getFormatConfig(b));
                c && ($.datepicker._setDateFromField(b), $.datepicker._updateAlternate(b), $.datepicker._updateDatepicker(b))
            } catch (d) {
                $.datepicker.log(d)
            }
            return!0
        }, _showDatepicker:function (a) {
            a = a.target || a, a.nodeName.toLowerCase() != "input" && (a = $("input", a.parentNode)[0]);
            if ($.datepicker._isDisabledDatepicker(a) || $.datepicker._lastInput == a)return;
            var b = $.datepicker._getInst(a);
            $.datepicker._curInst && $.datepicker._curInst != b && ($.datepicker._curInst.dpDiv.stop(!0, !0), b && $.datepicker._datepickerShowing && $.datepicker._hideDatepicker($.datepicker._curInst.input[0]));
            var c = $.datepicker._get(b, "beforeShow"), d = c ? c.apply(a, [a, b]) : {};
            if (d === !1)return;
            extendRemove(b.settings, d), b.lastVal = null, $.datepicker._lastInput = a, $.datepicker._setDateFromField(b), $.datepicker._inDialog && (a.value = ""), $.datepicker._pos || ($.datepicker._pos = $.datepicker._findPos(a), $.datepicker._pos[1] += a.offsetHeight);
            var e = !1;
            $(a).parents().each(function () {
                return e |= $(this).css("position") == "fixed", !e
            }), e && $.browser.opera && ($.datepicker._pos[0] -= document.documentElement.scrollLeft, $.datepicker._pos[1] -= document.documentElement.scrollTop);
            var f = {left:$.datepicker._pos[0], top:$.datepicker._pos[1]};
            $.datepicker._pos = null, b.dpDiv.empty(), b.dpDiv.css({position:"absolute", display:"block", top:"-1000px"}), $.datepicker._updateDatepicker(b), f = $.datepicker._checkOffset(b, f, e), b.dpDiv.css({position:$.datepicker._inDialog && $.blockUI ? "static" : e ? "fixed" : "absolute", display:"none", left:f.left + "px", top:f.top + "px"});
            if (!b.inline) {
                var g = $.datepicker._get(b, "showAnim"), h = $.datepicker._get(b, "duration"), i = function () {
                    var a = b.dpDiv.find("iframe.ui-datepicker-cover");
                    if (!!a.length) {
                        var c = $.datepicker._getBorders(b.dpDiv);
                        a.css({left:-c[0], top:-c[1], width:b.dpDiv.outerWidth(), height:b.dpDiv.outerHeight()})
                    }
                };
                b.dpDiv.zIndex($(a).zIndex() + 1), $.datepicker._datepickerShowing = !0, $.effects && $.effects[g] ? b.dpDiv.show(g, $.datepicker._get(b, "showOptions"), h, i) : b.dpDiv[g || "show"](g ? h : null, i), (!g || !h) && i(), b.input.is(":visible") && !b.input.is(":disabled") && b.input.focus(), $.datepicker._curInst = b
            }
        }, _updateDatepicker:function (a) {
            var b = this;
            b.maxRows = 4;
            var c = $.datepicker._getBorders(a.dpDiv);
            instActive = a, a.dpDiv.empty().append(this._generateHTML(a));
            var d = a.dpDiv.find("iframe.ui-datepicker-cover");
            !d.length || d.css({left:-c[0], top:-c[1], width:a.dpDiv.outerWidth(), height:a.dpDiv.outerHeight()}), a.dpDiv.find("." + this._dayOverClass + " a").mouseover();
            var e = this._getNumberOfMonths(a), f = e[1], g = 17;
            a.dpDiv.removeClass("ui-datepicker-multi-2 ui-datepicker-multi-3 ui-datepicker-multi-4").width(""), f > 1 && a.dpDiv.addClass("ui-datepicker-multi-" + f).css("width", g * f + "em"), a.dpDiv[(e[0] != 1 || e[1] != 1 ? "add" : "remove") + "Class"]("ui-datepicker-multi"), a.dpDiv[(this._get(a, "isRTL") ? "add" : "remove") + "Class"]("ui-datepicker-rtl"), a == $.datepicker._curInst && $.datepicker._datepickerShowing && a.input && a.input.is(":visible") && !a.input.is(":disabled") && a.input[0] != document.activeElement && a.input.focus();
            if (a.yearshtml) {
                var h = a.yearshtml;
                setTimeout(function () {
                    h === a.yearshtml && a.yearshtml && a.dpDiv.find("select.ui-datepicker-year:first").replaceWith(a.yearshtml), h = a.yearshtml = null
                }, 0)
            }
        }, _getBorders:function (a) {
            var b = function (a) {
                return{thin:1, medium:2, thick:3}[a] || a
            };
            return[parseFloat(b(a.css("border-left-width"))), parseFloat(b(a.css("border-top-width")))]
        }, _checkOffset:function (a, b, c) {
            var d = a.dpDiv.outerWidth(), e = a.dpDiv.outerHeight(), f = a.input ? a.input.outerWidth() : 0, g = a.input ? a.input.outerHeight() : 0, h = document.documentElement.clientWidth + $(document).scrollLeft(), i = document.documentElement.clientHeight + $(document).scrollTop();
            return b.left -= this._get(a, "isRTL") ? d - f : 0, b.left -= c && b.left == a.input.offset().left ? $(document).scrollLeft() : 0, b.top -= c && b.top == a.input.offset().top + g ? $(document).scrollTop() : 0, b.left -= Math.min(b.left, b.left + d > h && h > d ? Math.abs(b.left + d - h) : 0), b.top -= Math.min(b.top, b.top + e > i && i > e ? Math.abs(e + g) : 0), b
        }, _findPos:function (a) {
            var b = this._getInst(a), c = this._get(b, "isRTL");
            while (a && (a.type == "hidden" || a.nodeType != 1 || $.expr.filters.hidden(a)))a = a[c ? "previousSibling" : "nextSibling"];
            var d = $(a).offset();
            return[d.left, d.top]
        }, _hideDatepicker:function (a) {
            var b = this._curInst;
            if (!b || a && b != $.data(a, PROP_NAME))return;
            if (this._datepickerShowing) {
                var c = this._get(b, "showAnim"), d = this._get(b, "duration"), e = function () {
                    $.datepicker._tidyDialog(b)
                };
                $.effects && $.effects[c] ? b.dpDiv.hide(c, $.datepicker._get(b, "showOptions"), d, e) : b.dpDiv[c == "slideDown" ? "slideUp" : c == "fadeIn" ? "fadeOut" : "hide"](c ? d : null, e), c || e(), this._datepickerShowing = !1;
                var f = this._get(b, "onClose");
                f && f.apply(b.input ? b.input[0] : null, [b.input ? b.input.val() : "", b]), this._lastInput = null, this._inDialog && (this._dialogInput.css({position:"absolute", left:"0", top:"-100px"}), $.blockUI && ($.unblockUI(), $("body").append(this.dpDiv))), this._inDialog = !1
            }
        }, _tidyDialog:function (a) {
            a.dpDiv.removeClass(this._dialogClass).unbind(".ui-datepicker-calendar")
        }, _checkExternalClick:function (a) {
            if (!$.datepicker._curInst)return;
            var b = $(a.target), c = $.datepicker._getInst(b[0]);
            (b[0].id != $.datepicker._mainDivId && b.parents("#" + $.datepicker._mainDivId).length == 0 && !b.hasClass($.datepicker.markerClassName) && !b.closest("." + $.datepicker._triggerClass).length && $.datepicker._datepickerShowing && (!$.datepicker._inDialog || !$.blockUI) || b.hasClass($.datepicker.markerClassName) && $.datepicker._curInst != c) && $.datepicker._hideDatepicker()
        }, _adjustDate:function (a, b, c) {
            var d = $(a), e = this._getInst(d[0]);
            if (this._isDisabledDatepicker(d[0]))return;
            this._adjustInstDate(e, b + (c == "M" ? this._get(e, "showCurrentAtPos") : 0), c), this._updateDatepicker(e)
        }, _gotoToday:function (a) {
            var b = $(a), c = this._getInst(b[0]);
            if (this._get(c, "gotoCurrent") && c.currentDay)c.selectedDay = c.currentDay, c.drawMonth = c.selectedMonth = c.currentMonth, c.drawYear = c.selectedYear = c.currentYear; else {
                var d = new Date;
                c.selectedDay = d.getDate(), c.drawMonth = c.selectedMonth = d.getMonth(), c.drawYear = c.selectedYear = d.getFullYear()
            }
            this._notifyChange(c), this._adjustDate(b)
        }, _selectMonthYear:function (a, b, c) {
            var d = $(a), e = this._getInst(d[0]);
            e["selected" + (c == "M" ? "Month" : "Year")] = e["draw" + (c == "M" ? "Month" : "Year")] = parseInt(b.options[b.selectedIndex].value, 10), this._notifyChange(e), this._adjustDate(d)
        }, _selectDay:function (a, b, c, d) {
            var e = $(a);
            if ($(d).hasClass(this._unselectableClass) || this._isDisabledDatepicker(e[0]))return;
            var f = this._getInst(e[0]);
            f.selectedDay = f.currentDay = $("a", d).html(), f.selectedMonth = f.currentMonth = b, f.selectedYear = f.currentYear = c, this._selectDate(a, this._formatDate(f, f.currentDay, f.currentMonth, f.currentYear))
        }, _clearDate:function (a) {
            var b = $(a), c = this._getInst(b[0]);
            this._selectDate(b, "")
        }, _selectDate:function (a, b) {
            var c = $(a), d = this._getInst(c[0]);
            b = b != null ? b : this._formatDate(d), d.input && d.input.val(b), this._updateAlternate(d);
            var e = this._get(d, "onSelect");
            e ? e.apply(d.input ? d.input[0] : null, [b, d]) : d.input && d.input.trigger("change"), d.inline ? this._updateDatepicker(d) : (this._hideDatepicker(), this._lastInput = d.input[0], typeof d.input[0] != "object" && d.input.focus(), this._lastInput = null)
        }, _updateAlternate:function (a) {
            var b = this._get(a, "altField");
            if (b) {
                var c = this._get(a, "altFormat") || this._get(a, "dateFormat"), d = this._getDate(a), e = this.formatDate(c, d, this._getFormatConfig(a));
                $(b).each(function () {
                    $(this).val(e)
                })
            }
        }, noWeekends:function (a) {
            var b = a.getDay();
            return[b > 0 && b < 6, ""]
        }, iso8601Week:function (a) {
            var b = new Date(a.getTime());
            b.setDate(b.getDate() + 4 - (b.getDay() || 7));
            var c = b.getTime();
            return b.setMonth(0), b.setDate(1), Math.floor(Math.round((c - b) / 864e5) / 7) + 1
        }, parseDate:function (a, b, c) {
            if (a == null || b == null)throw"Invalid arguments";
            b = typeof b == "object" ? b.toString() : b + "";
            if (b == "")return null;
            var d = (c ? c.shortYearCutoff : null) || this._defaults.shortYearCutoff;
            d = typeof d != "string" ? d : (new Date).getFullYear() % 100 + parseInt(d, 10);
            var e = (c ? c.dayNamesShort : null) || this._defaults.dayNamesShort, f = (c ? c.dayNames : null) || this._defaults.dayNames, g = (c ? c.monthNamesShort : null) || this._defaults.monthNamesShort, h = (c ? c.monthNames : null) || this._defaults.monthNames, i = -1, j = -1, k = -1, l = -1, m = !1, n = function (b) {
                var c = s + 1 < a.length && a.charAt(s + 1) == b;
                return c && s++, c
            }, o = function (a) {
                var c = n(a), d = a == "@" ? 14 : a == "!" ? 20 : a == "y" && c ? 4 : a == "o" ? 3 : 2, e = new RegExp("^\\d{1," + d + "}"), f = b.substring(r).match(e);
                if (!f)throw"Missing number at position " + r;
                return r += f[0].length, parseInt(f[0], 10)
            }, p = function (a, c, d) {
                var e = $.map(n(a) ? d : c,function (a, b) {
                    return[
                        [b, a]
                    ]
                }).sort(function (a, b) {
                    return-(a[1].length - b[1].length)
                }), f = -1;
                $.each(e, function (a, c) {
                    var d = c[1];
                    if (b.substr(r, d.length).toLowerCase() == d.toLowerCase())return f = c[0], r += d.length, !1
                });
                if (f != -1)return f + 1;
                throw"Unknown name at position " + r
            }, q = function () {
                if (b.charAt(r) != a.charAt(s))throw"Unexpected literal at position " + r;
                r++
            }, r = 0;
            for (var s = 0; s < a.length; s++)if (m)a.charAt(s) == "'" && !n("'") ? m = !1 : q(); else switch (a.charAt(s)) {
                case"d":
                    k = o("d");
                    break;
                case"D":
                    p("D", e, f);
                    break;
                case"o":
                    l = o("o");
                    break;
                case"m":
                    j = o("m");
                    break;
                case"M":
                    j = p("M", g, h);
                    break;
                case"y":
                    i = o("y");
                    break;
                case"@":
                    var t = new Date(o("@"));
                    i = t.getFullYear(), j = t.getMonth() + 1, k = t.getDate();
                    break;
                case"!":
                    var t = new Date((o("!") - this._ticksTo1970) / 1e4);
                    i = t.getFullYear(), j = t.getMonth() + 1, k = t.getDate();
                    break;
                case"'":
                    n("'") ? q() : m = !0;
                    break;
                default:
                    q()
            }
            if (r < b.length)throw"Extra/unparsed characters found in date: " + b.substring(r);
            i == -1 ? i = (new Date).getFullYear() : i < 100 && (i += (new Date).getFullYear() - (new Date).getFullYear() % 100 + (i <= d ? 0 : -100));
            if (l > -1) {
                j = 1, k = l;
                do {
                    var u = this._getDaysInMonth(i, j - 1);
                    if (k <= u)break;
                    j++, k -= u
                } while (!0)
            }
            var t = this._daylightSavingAdjust(new Date(i, j - 1, k));
            if (t.getFullYear() != i || t.getMonth() + 1 != j || t.getDate() != k)throw"Invalid date";
            return t
        }, ATOM:"yy-mm-dd", COOKIE:"D, dd M yy", ISO_8601:"yy-mm-dd", RFC_822:"D, d M y", RFC_850:"DD, dd-M-y", RFC_1036:"D, d M y", RFC_1123:"D, d M yy", RFC_2822:"D, d M yy", RSS:"D, d M y", TICKS:"!", TIMESTAMP:"@", W3C:"yy-mm-dd", _ticksTo1970:(718685 + Math.floor(492.5) - Math.floor(19.7) + Math.floor(4.925)) * 24 * 60 * 60 * 1e7, formatDate:function (a, b, c) {
            if (!b)return"";
            var d = (c ? c.dayNamesShort : null) || this._defaults.dayNamesShort, e = (c ? c.dayNames : null) || this._defaults.dayNames, f = (c ? c.monthNamesShort : null) || this._defaults.monthNamesShort, g = (c ? c.monthNames : null) || this._defaults.monthNames, h = function (b) {
                var c = m + 1 < a.length && a.charAt(m + 1) == b;
                return c && m++, c
            }, i = function (a, b, c) {
                var d = "" + b;
                if (h(a))while (d.length < c)d = "0" + d;
                return d
            }, j = function (a, b, c, d) {
                return h(a) ? d[b] : c[b]
            }, k = "", l = !1;
            if (b)for (var m = 0; m < a.length; m++)if (l)a.charAt(m) == "'" && !h("'") ? l = !1 : k += a.charAt(m); else switch (a.charAt(m)) {
                case"d":
                    k += i("d", b.getDate(), 2);
                    break;
                case"D":
                    k += j("D", b.getDay(), d, e);
                    break;
                case"o":
                    k += i("o", Math.round(((new Date(b.getFullYear(), b.getMonth(), b.getDate())).getTime() - (new Date(b.getFullYear(), 0, 0)).getTime()) / 864e5), 3);
                    break;
                case"m":
                    k += i("m", b.getMonth() + 1, 2);
                    break;
                case"M":
                    k += j("M", b.getMonth(), f, g);
                    break;
                case"y":
                    k += h("y") ? b.getFullYear() : (b.getYear() % 100 < 10 ? "0" : "") + b.getYear() % 100;
                    break;
                case"@":
                    k += b.getTime();
                    break;
                case"!":
                    k += b.getTime() * 1e4 + this._ticksTo1970;
                    break;
                case"'":
                    h("'") ? k += "'" : l = !0;
                    break;
                default:
                    k += a.charAt(m)
            }
            return k
        }, _possibleChars:function (a) {
            var b = "", c = !1, d = function (b) {
                var c = e + 1 < a.length && a.charAt(e + 1) == b;
                return c && e++, c
            };
            for (var e = 0; e < a.length; e++)if (c)a.charAt(e) == "'" && !d("'") ? c = !1 : b += a.charAt(e); else switch (a.charAt(e)) {
                case"d":
                case"m":
                case"y":
                case"@":
                    b += "0123456789";
                    break;
                case"D":
                case"M":
                    return null;
                case"'":
                    d("'") ? b += "'" : c = !0;
                    break;
                default:
                    b += a.charAt(e)
            }
            return b
        }, _get:function (a, b) {
            return a.settings[b] !== undefined ? a.settings[b] : this._defaults[b]
        }, _setDateFromField:function (a, b) {
            if (a.input.val() == a.lastVal)return;
            var c = this._get(a, "dateFormat"), d = a.lastVal = a.input ? a.input.val() : null, e, f;
            e = f = this._getDefaultDate(a);
            var g = this._getFormatConfig(a);
            try {
                e = this.parseDate(c, d, g) || f
            } catch (h) {
                this.log(h), d = b ? "" : d
            }
            a.selectedDay = e.getDate(), a.drawMonth = a.selectedMonth = e.getMonth(), a.drawYear = a.selectedYear = e.getFullYear(), a.currentDay = d ? e.getDate() : 0, a.currentMonth = d ? e.getMonth() : 0, a.currentYear = d ? e.getFullYear() : 0, this._adjustInstDate(a)
        }, _getDefaultDate:function (a) {
            return this._restrictMinMax(a, this._determineDate(a, this._get(a, "defaultDate"), new Date))
        }, _determineDate:function (a, b, c) {
            var d = function (a) {
                var b = new Date;
                return b.setDate(b.getDate() + a), b
            }, e = function (b) {
                try {
                    return $.datepicker.parseDate($.datepicker._get(a, "dateFormat"), b, $.datepicker._getFormatConfig(a))
                } catch (c) {
                }
                var d = (b.toLowerCase().match(/^c/) ? $.datepicker._getDate(a) : null) || new Date, e = d.getFullYear(), f = d.getMonth(), g = d.getDate(), h = /([+-]?[0-9]+)\s*(d|D|w|W|m|M|y|Y)?/g, i = h.exec(b);
                while (i) {
                    switch (i[2] || "d") {
                        case"d":
                        case"D":
                            g += parseInt(i[1], 10);
                            break;
                        case"w":
                        case"W":
                            g += parseInt(i[1], 10) * 7;
                            break;
                        case"m":
                        case"M":
                            f += parseInt(i[1], 10), g = Math.min(g, $.datepicker._getDaysInMonth(e, f));
                            break;
                        case"y":
                        case"Y":
                            e += parseInt(i[1], 10), g = Math.min(g, $.datepicker._getDaysInMonth(e, f))
                    }
                    i = h.exec(b)
                }
                return new Date(e, f, g)
            }, f = b == null || b === "" ? c : typeof b == "string" ? e(b) : typeof b == "number" ? isNaN(b) ? c : d(b) : new Date(b.getTime());
            return f = f && f.toString() == "Invalid Date" ? c : f, f && (f.setHours(0), f.setMinutes(0), f.setSeconds(0), f.setMilliseconds(0)), this._daylightSavingAdjust(f)
        }, _daylightSavingAdjust:function (a) {
            return a ? (a.setHours(a.getHours() > 12 ? a.getHours() + 2 : 0), a) : null
        }, _setDate:function (a, b, c) {
            var d = !b, e = a.selectedMonth, f = a.selectedYear, g = this._restrictMinMax(a, this._determineDate(a, b, new Date));
            a.selectedDay = a.currentDay = g.getDate(), a.drawMonth = a.selectedMonth = a.currentMonth = g.getMonth(), a.drawYear = a.selectedYear = a.currentYear = g.getFullYear(), (e != a.selectedMonth || f != a.selectedYear) && !c && this._notifyChange(a), this._adjustInstDate(a), a.input && a.input.val(d ? "" : this._formatDate(a))
        }, _getDate:function (a) {
            var b = !a.currentYear || a.input && a.input.val() == "" ? null : this._daylightSavingAdjust(new Date(a.currentYear, a.currentMonth, a.currentDay));
            return b
        }, _generateHTML:function (a) {
            var b = new Date;
            b = this._daylightSavingAdjust(new Date(b.getFullYear(), b.getMonth(), b.getDate()));
            var c = this._get(a, "isRTL"), d = this._get(a, "showButtonPanel"), e = this._get(a, "hideIfNoPrevNext"), f = this._get(a, "navigationAsDateFormat"), g = this._getNumberOfMonths(a), h = this._get(a, "showCurrentAtPos"), i = this._get(a, "stepMonths"), j = g[0] != 1 || g[1] != 1, k = this._daylightSavingAdjust(a.currentDay ? new Date(a.currentYear, a.currentMonth, a.currentDay) : new Date(9999, 9, 9)), l = this._getMinMaxDate(a, "min"), m = this._getMinMaxDate(a, "max"), n = a.drawMonth - h, o = a.drawYear;
            n < 0 && (n += 12, o--);
            if (m) {
                var p = this._daylightSavingAdjust(new Date(m.getFullYear(), m.getMonth() - g[0] * g[1] + 1, m.getDate()));
                p = l && p < l ? l : p;
                while (this._daylightSavingAdjust(new Date(o, n, 1)) > p)n--, n < 0 && (n = 11, o--)
            }
            a.drawMonth = n, a.drawYear = o;
            var q = this._get(a, "prevText");
            q = f ? this.formatDate(q, this._daylightSavingAdjust(new Date(o, n - i, 1)), this._getFormatConfig(a)) : q;
            var r = this._canAdjustMonth(a, -1, o, n) ? '<a class="ui-datepicker-prev ui-corner-all" onclick="DP_jQuery_' + dpuuid + ".datepicker._adjustDate('#" + a.id + "', -" + i + ", 'M');\"" + ' title="' + q + '"><span class="ui-icon ui-icon-circle-triangle-' + (c ? "e" : "w") + '">' + q + "</span></a>" : e ? "" : '<a class="ui-datepicker-prev ui-corner-all ui-state-disabled" title="' + q + '"><span class="ui-icon ui-icon-circle-triangle-' + (c ? "e" : "w") + '">' + q + "</span></a>", s = this._get(a, "nextText");
            s = f ? this.formatDate(s, this._daylightSavingAdjust(new Date(o, n + i, 1)), this._getFormatConfig(a)) : s;
            var t = this._canAdjustMonth(a, 1, o, n) ? '<a class="ui-datepicker-next ui-corner-all" onclick="DP_jQuery_' + dpuuid + ".datepicker._adjustDate('#" + a.id + "', +" + i + ", 'M');\"" + ' title="' + s + '"><span class="ui-icon ui-icon-circle-triangle-' + (c ? "w" : "e") + '">' + s + "</span></a>" : e ? "" : '<a class="ui-datepicker-next ui-corner-all ui-state-disabled" title="' + s + '"><span class="ui-icon ui-icon-circle-triangle-' + (c ? "w" : "e") + '">' + s + "</span></a>", u = this._get(a, "currentText"), v = this._get(a, "gotoCurrent") && a.currentDay ? k : b;
            u = f ? this.formatDate(u, v, this._getFormatConfig(a)) : u;
            var w = a.inline ? "" : '<button type="button" class="ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all" onclick="DP_jQuery_' + dpuuid + '.datepicker._hideDatepicker();">' + this._get(a, "closeText") + "</button>", x = d ? '<div class="ui-datepicker-buttonpane ui-widget-content">' + (c ? w : "") + (this._isInRange(a, v) ? '<button type="button" class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all" onclick="DP_jQuery_' + dpuuid + ".datepicker._gotoToday('#" + a.id + "');\"" + ">" + u + "</button>" : "") + (c ? "" : w) + "</div>" : "", y = parseInt(this._get(a, "firstDay"), 10);
            y = isNaN(y) ? 0 : y;
            var z = this._get(a, "showWeek"), A = this._get(a, "dayNames"), B = this._get(a, "dayNamesShort"), C = this._get(a, "dayNamesMin"), D = this._get(a, "monthNames"), E = this._get(a, "monthNamesShort"), F = this._get(a, "beforeShowDay"), G = this._get(a, "showOtherMonths"), H = this._get(a, "selectOtherMonths"), I = this._get(a, "calculateWeek") || this.iso8601Week, J = this._getDefaultDate(a), K = "";
            for (var L = 0; L < g[0]; L++) {
                var M = "";
                this.maxRows = 4;
                for (var N = 0; N < g[1]; N++) {
                    var O = this._daylightSavingAdjust(new Date(o, n, a.selectedDay)), P = " ui-corner-all", Q = "";
                    if (j) {
                        Q += '<div class="ui-datepicker-group';
                        if (g[1] > 1)switch (N) {
                            case 0:
                                Q += " ui-datepicker-group-first", P = " ui-corner-" + (c ? "right" : "left");
                                break;
                            case g[1] - 1:
                                Q += " ui-datepicker-group-last", P = " ui-corner-" + (c ? "left" : "right");
                                break;
                            default:
                                Q += " ui-datepicker-group-middle", P = ""
                        }
                        Q += '">'
                    }
                    Q += '<div class="ui-datepicker-header ui-widget-header ui-helper-clearfix' + P + '">' + (/all|left/.test(P) && L == 0 ? c ? t : r : "") + (/all|right/.test(P) && L == 0 ? c ? r : t : "") + this._generateMonthYearHeader(a, n, o, l, m, L > 0 || N > 0, D, E) + '</div><table class="ui-datepicker-calendar"><thead>' + "<tr>";
                    var R = z ? '<th class="ui-datepicker-week-col">' + this._get(a, "weekHeader") + "</th>" : "";
                    for (var S = 0; S < 7; S++) {
                        var T = (S + y) % 7;
                        R += "<th" + ((S + y + 6) % 7 >= 5 ? ' class="ui-datepicker-week-end"' : "") + ">" + '<span title="' + A[T] + '">' + C[T] + "</span></th>"
                    }
                    Q += R + "</tr></thead><tbody>";
                    var U = this._getDaysInMonth(o, n);
                    o == a.selectedYear && n == a.selectedMonth && (a.selectedDay = Math.min(a.selectedDay, U));
                    var V = (this._getFirstDayOfMonth(o, n) - y + 7) % 7, W = Math.ceil((V + U) / 7), X = j ? this.maxRows > W ? this.maxRows : W : W;
                    this.maxRows = X;
                    var Y = this._daylightSavingAdjust(new Date(o, n, 1 - V));
                    for (var Z = 0; Z < X; Z++) {
                        Q += "<tr>";
                        var _ = z ? '<td class="ui-datepicker-week-col">' + this._get(a, "calculateWeek")(Y) + "</td>" : "";
                        for (var S = 0; S < 7; S++) {
                            var ba = F ? F.apply(a.input ? a.input[0] : null, [Y]) : [!0, ""], bb = Y.getMonth() != n, bc = bb && !H || !ba[0] || l && Y < l || m && Y > m;
                            _ += '<td class="' + ((S + y + 6) % 7 >= 5 ? " ui-datepicker-week-end" : "") + (bb ? " ui-datepicker-other-month" : "") + (Y.getTime() == O.getTime() && n == a.selectedMonth && a._keyEvent || J.getTime() == Y.getTime() && J.getTime() == O.getTime() ? " " + this._dayOverClass : "") + (bc ? " " + this._unselectableClass + " ui-state-disabled" : "") + (bb && !G ? "" : " " + ba[1] + (Y.getTime() == k.getTime() ? " " + this._currentClass : "") + (Y.getTime() == b.getTime() ? " ui-datepicker-today" : "")) + '"' + ((!bb || G) && ba[2] ? ' title="' + ba[2] + '"' : "") + (bc ? "" : ' onclick="DP_jQuery_' + dpuuid + ".datepicker._selectDay('#" + a.id + "'," + Y.getMonth() + "," + Y.getFullYear() + ', this);return false;"') + ">" + (bb && !G ? "&#xa0;" : bc ? '<span class="ui-state-default">' + Y.getDate() + "</span>" : '<a class="ui-state-default' + (Y.getTime() == b.getTime() ? " ui-state-highlight" : "") + (Y.getTime() == k.getTime() ? " ui-state-active" : "") + (bb ? " ui-priority-secondary" : "") + '" href="#">' + Y.getDate() + "</a>") + "</td>", Y.setDate(Y.getDate() + 1), Y = this._daylightSavingAdjust(Y)
                        }
                        Q += _ + "</tr>"
                    }
                    n++, n > 11 && (n = 0, o++), Q += "</tbody></table>" + (j ? "</div>" + (g[0] > 0 && N == g[1] - 1 ? '<div class="ui-datepicker-row-break"></div>' : "") : ""), M += Q
                }
                K += M
            }
            return K += x + ($.browser.msie && parseInt($.browser.version, 10) < 7 && !a.inline ? '<iframe src="javascript:false;" class="ui-datepicker-cover" frameborder="0"></iframe>' : ""), a._keyEvent = !1, K
        }, _generateMonthYearHeader:function (a, b, c, d, e, f, g, h) {
            var i = this._get(a, "changeMonth"), j = this._get(a, "changeYear"), k = this._get(a, "showMonthAfterYear"), l = '<div class="ui-datepicker-title">', m = "";
            if (f || !i)m += '<span class="ui-datepicker-month">' + g[b] + "</span>"; else {
                var n = d && d.getFullYear() == c, o = e && e.getFullYear() == c;
                m += '<select class="ui-datepicker-month" onchange="DP_jQuery_' + dpuuid + ".datepicker._selectMonthYear('#" + a.id + "', this, 'M');\" " + ">";
                for (var p = 0; p < 12; p++)(!n || p >= d.getMonth()) && (!o || p <= e.getMonth()) && (m += '<option value="' + p + '"' + (p == b ? ' selected="selected"' : "") + ">" + h[p] + "</option>");
                m += "</select>"
            }
            k || (l += m + (f || !i || !j ? "&#xa0;" : ""));
            if (!a.yearshtml) {
                a.yearshtml = "";
                if (f || !j)l += '<span class="ui-datepicker-year">' + c + "</span>"; else {
                    var q = this._get(a, "yearRange").split(":"), r = (new Date).getFullYear(), s = function (a) {
                        var b = a.match(/c[+-].*/) ? c + parseInt(a.substring(1), 10) : a.match(/[+-].*/) ? r + parseInt(a, 10) : parseInt(a, 10);
                        return isNaN(b) ? r : b
                    }, t = s(q[0]), u = Math.max(t, s(q[1] || ""));
                    t = d ? Math.max(t, d.getFullYear()) : t, u = e ? Math.min(u, e.getFullYear()) : u, a.yearshtml += '<select class="ui-datepicker-year" onchange="DP_jQuery_' + dpuuid + ".datepicker._selectMonthYear('#" + a.id + "', this, 'Y');\" " + ">";
                    for (; t <= u; t++)a.yearshtml += '<option value="' + t + '"' + (t == c ? ' selected="selected"' : "") + ">" + t + "</option>";
                    a.yearshtml += "</select>", l += a.yearshtml, a.yearshtml = null
                }
            }
            return l += this._get(a, "yearSuffix"), k && (l += (f || !i || !j ? "&#xa0;" : "") + m), l += "</div>", l
        }, _adjustInstDate:function (a, b, c) {
            var d = a.drawYear + (c == "Y" ? b : 0), e = a.drawMonth + (c == "M" ? b : 0), f = Math.min(a.selectedDay, this._getDaysInMonth(d, e)) + (c == "D" ? b : 0), g = this._restrictMinMax(a, this._daylightSavingAdjust(new Date(d, e, f)));
            a.selectedDay = g.getDate(), a.drawMonth = a.selectedMonth = g.getMonth(), a.drawYear = a.selectedYear = g.getFullYear(), (c == "M" || c == "Y") && this._notifyChange(a)
        }, _restrictMinMax:function (a, b) {
            var c = this._getMinMaxDate(a, "min"), d = this._getMinMaxDate(a, "max"), e = c && b < c ? c : b;
            return e = d && e > d ? d : e, e
        }, _notifyChange:function (a) {
            var b = this._get(a, "onChangeMonthYear");
            b && b.apply(a.input ? a.input[0] : null, [a.selectedYear, a.selectedMonth + 1, a])
        }, _getNumberOfMonths:function (a) {
            var b = this._get(a, "numberOfMonths");
            return b == null ? [1, 1] : typeof b == "number" ? [1, b] : b
        }, _getMinMaxDate:function (a, b) {
            return this._determineDate(a, this._get(a, b + "Date"), null)
        }, _getDaysInMonth:function (a, b) {
            return 32 - this._daylightSavingAdjust(new Date(a, b, 32)).getDate()
        }, _getFirstDayOfMonth:function (a, b) {
            return(new Date(a, b, 1)).getDay()
        }, _canAdjustMonth:function (a, b, c, d) {
            var e = this._getNumberOfMonths(a), f = this._daylightSavingAdjust(new Date(c, d + (b < 0 ? b : e[0] * e[1]), 1));
            return b < 0 && f.setDate(this._getDaysInMonth(f.getFullYear(), f.getMonth())), this._isInRange(a, f)
        }, _isInRange:function (a, b) {
            var c = this._getMinMaxDate(a, "min"), d = this._getMinMaxDate(a, "max");
            return(!c || b.getTime() >= c.getTime()) && (!d || b.getTime() <= d.getTime())
        }, _getFormatConfig:function (a) {
            var b = this._get(a, "shortYearCutoff");
            return b = typeof b != "string" ? b : (new Date).getFullYear() % 100 + parseInt(b, 10), {shortYearCutoff:b, dayNamesShort:this._get(a, "dayNamesShort"), dayNames:this._get(a, "dayNames"), monthNamesShort:this._get(a, "monthNamesShort"), monthNames:this._get(a, "monthNames")}
        }, _formatDate:function (a, b, c, d) {
            b || (a.currentDay = a.selectedDay, a.currentMonth = a.selectedMonth, a.currentYear = a.selectedYear);
            var e = b ? typeof b == "object" ? b : this._daylightSavingAdjust(new Date(d, c, b)) : this._daylightSavingAdjust(new Date(a.currentYear, a.currentMonth, a.currentDay));
            return this.formatDate(this._get(a, "dateFormat"), e, this._getFormatConfig(a))
        }}), $.fn.datepicker = function (a) {
            if (!this.length)return this;
            $.datepicker.initialized || ($(document).mousedown($.datepicker._checkExternalClick).find("body").append($.datepicker.dpDiv), $.datepicker.initialized = !0);
            var b = Array.prototype.slice.call(arguments, 1);
            return typeof a != "string" || a != "isDisabled" && a != "getDate" && a != "widget" ? a == "option" && arguments.length == 2 && typeof arguments[1] == "string" ? $.datepicker["_" + a + "Datepicker"].apply($.datepicker, [this[0]].concat(b)) : this.each(function () {
                typeof a == "string" ? $.datepicker["_" + a + "Datepicker"].apply($.datepicker, [this].concat(b)) : $.datepicker._attachDatepicker(this, a)
            }) : $.datepicker["_" + a + "Datepicker"].apply($.datepicker, [this[0]].concat(b))
        }, $.datepicker = new Datepicker, $.datepicker.initialized = !1, $.datepicker.uuid = (new Date).getTime(), $.datepicker.version = "@VERSION", window["DP_jQuery_" + dpuuid] = $
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.ui.progressbar.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.widget("ui.progressbar", {options:{value:0, max:100}, min:0, _create:function () {
            this.element.addClass("ui-progressbar ui-widget ui-widget-content ui-corner-all").attr({role:"progressbar", "aria-valuemin":this.min, "aria-valuemax":this.options.max, "aria-valuenow":this._value()}), this.valueDiv = a("<div class='ui-progressbar-value ui-widget-header ui-corner-left'></div>").appendTo(this.element), this.oldValue = this._value(), this._refreshValue()
        }, destroy:function () {
            this.element.removeClass("ui-progressbar ui-widget ui-widget-content ui-corner-all").removeAttr("role").removeAttr("aria-valuemin").removeAttr("aria-valuemax").removeAttr("aria-valuenow"), this.valueDiv.remove(), a.Widget.prototype.destroy.apply(this, arguments)
        }, value:function (a) {
            return a === b ? this._value() : (this._setOption("value", a), this)
        }, _setOption:function (b, c) {
            b === "value" && (this.options.value = c, this._refreshValue(), this._value() === this.options.max && this._trigger("complete")), a.Widget.prototype._setOption.apply(this, arguments)
        }, _value:function () {
            var a = this.options.value;
            return typeof a != "number" && (a = 0), Math.min(this.options.max, Math.max(this.min, a))
        }, _percentage:function () {
            return 100 * this._value() / this.options.max
        }, _refreshValue:function () {
            var a = this.value(), b = this._percentage();
            this.oldValue !== a && (this.oldValue = a, this._trigger("change")), this.valueDiv.toggle(a > this.min).toggleClass("ui-corner-right", a === this.options.max).width(b.toFixed(0) + "%"), this.element.attr("aria-valuenow", a)
        }}), a.extend(a.ui.progressbar, {version:"1.8.19"})
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.core.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    jQuery.effects || function (a, b) {
        function c(b) {
            var c;
            return b && b.constructor == Array && b.length == 3 ? b : (c = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(b)) ? [parseInt(c[1], 10), parseInt(c[2], 10), parseInt(c[3], 10)] : (c = /rgb\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*\)/.exec(b)) ? [parseFloat(c[1]) * 2.55, parseFloat(c[2]) * 2.55, parseFloat(c[3]) * 2.55] : (c = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/.exec(b)) ? [parseInt(c[1], 16), parseInt(c[2], 16), parseInt(c[3], 16)] : (c = /#([a-fA-F0-9])([a-fA-F0-9])([a-fA-F0-9])/.exec(b)) ? [parseInt(c[1] + c[1], 16), parseInt(c[2] + c[2], 16), parseInt(c[3] + c[3], 16)] : (c = /rgba\(0, 0, 0, 0\)/.exec(b)) ? e.transparent : e[a.trim(b).toLowerCase()]
        }

        function d(b, d) {
            var e;
            do {
                e = a.curCSS(b, d);
                if (e != "" && e != "transparent" || a.nodeName(b, "body"))break;
                d = "backgroundColor"
            } while (b = b.parentNode);
            return c(e)
        }

        function h() {
            var a = document.defaultView ? document.defaultView.getComputedStyle(this, null) : this.currentStyle, b = {}, c, d;
            if (a && a.length && a[0] && a[a[0]]) {
                var e = a.length;
                while (e--)c = a[e], typeof a[c] == "string" && (d = c.replace(/\-(\w)/g, function (a, b) {
                    return b.toUpperCase()
                }), b[d] = a[c])
            } else for (c in a)typeof a[c] == "string" && (b[c] = a[c]);
            return b
        }

        function i(b) {
            var c, d;
            for (c in b)d = b[c], (d == null || a.isFunction(d) || c in g || /scrollbar/.test(c) || !/color/i.test(c) && isNaN(parseFloat(d))) && delete b[c];
            return b
        }

        function j(a, b) {
            var c = {_:0}, d;
            for (d in b)a[d] != b[d] && (c[d] = b[d]);
            return c
        }

        function k(b, c, d, e) {
            typeof b == "object" && (e = c, d = null, c = b, b = c.effect), a.isFunction(c) && (e = c, d = null, c = {});
            if (typeof c == "number" || a.fx.speeds[c])e = d, d = c, c = {};
            return a.isFunction(d) && (e = d, d = null), c = c || {}, d = d || c.duration, d = a.fx.off ? 0 : typeof d == "number" ? d : d in a.fx.speeds ? a.fx.speeds[d] : a.fx.speeds._default, e = e || c.complete, [b, c, d, e]
        }

        function l(b) {
            return!b || typeof b == "number" || a.fx.speeds[b] ? !0 : typeof b == "string" && !a.effects[b] ? !0 : !1
        }

        a.effects = {}, a.each(["backgroundColor", "borderBottomColor", "borderLeftColor", "borderRightColor", "borderTopColor", "borderColor", "color", "outlineColor"], function (b, e) {
            a.fx.step[e] = function (a) {
                a.colorInit || (a.start = d(a.elem, e), a.end = c(a.end), a.colorInit = !0), a.elem.style[e] = "rgb(" + Math.max(Math.min(parseInt(a.pos * (a.end[0] - a.start[0]) + a.start[0], 10), 255), 0) + "," + Math.max(Math.min(parseInt(a.pos * (a.end[1] - a.start[1]) + a.start[1], 10), 255), 0) + "," + Math.max(Math.min(parseInt(a.pos * (a.end[2] - a.start[2]) + a.start[2], 10), 255), 0) + ")"
            }
        });
        var e = {aqua:[0, 255, 255], azure:[240, 255, 255], beige:[245, 245, 220], black:[0, 0, 0], blue:[0, 0, 255], brown:[165, 42, 42], cyan:[0, 255, 255], darkblue:[0, 0, 139], darkcyan:[0, 139, 139], darkgrey:[169, 169, 169], darkgreen:[0, 100, 0], darkkhaki:[189, 183, 107], darkmagenta:[139, 0, 139], darkolivegreen:[85, 107, 47], darkorange:[255, 140, 0], darkorchid:[153, 50, 204], darkred:[139, 0, 0], darksalmon:[233, 150, 122], darkviolet:[148, 0, 211], fuchsia:[255, 0, 255], gold:[255, 215, 0], green:[0, 128, 0], indigo:[75, 0, 130], khaki:[240, 230, 140], lightblue:[173, 216, 230], lightcyan:[224, 255, 255], lightgreen:[144, 238, 144], lightgrey:[211, 211, 211], lightpink:[255, 182, 193], lightyellow:[255, 255, 224], lime:[0, 255, 0], magenta:[255, 0, 255], maroon:[128, 0, 0], navy:[0, 0, 128], olive:[128, 128, 0], orange:[255, 165, 0], pink:[255, 192, 203], purple:[128, 0, 128], violet:[128, 0, 128], red:[255, 0, 0], silver:[192, 192, 192], white:[255, 255, 255], yellow:[255, 255, 0], transparent:[255, 255, 255]}, f = ["add", "remove", "toggle"], g = {border:1, borderBottom:1, borderColor:1, borderLeft:1, borderRight:1, borderTop:1, borderWidth:1, margin:1, padding:1};
        a.effects.animateClass = function (b, c, d, e) {
            return a.isFunction(d) && (e = d, d = null), this.queue(function () {
                var g = a(this), k = g.attr("style") || " ", l = i(h.call(this)), m, n = g.attr("class") || "";
                a.each(f, function (a, c) {
                    b[c] && g[c + "Class"](b[c])
                }), m = i(h.call(this)), g.attr("class", n), g.animate(j(l, m), {queue:!1, duration:c, easing:d, complete:function () {
                    a.each(f, function (a, c) {
                        b[c] && g[c + "Class"](b[c])
                    }), typeof g.attr("style") == "object" ? (g.attr("style").cssText = "", g.attr("style").cssText = k) : g.attr("style", k), e && e.apply(this, arguments), a.dequeue(this)
                }})
            })
        }, a.fn.extend({_addClass:a.fn.addClass, addClass:function (b, c, d, e) {
            return c ? a.effects.animateClass.apply(this, [
                {add:b},
                c,
                d,
                e
            ]) : this._addClass(b)
        }, _removeClass:a.fn.removeClass, removeClass:function (b, c, d, e) {
            return c ? a.effects.animateClass.apply(this, [
                {remove:b},
                c,
                d,
                e
            ]) : this._removeClass(b)
        }, _toggleClass:a.fn.toggleClass, toggleClass:function (c, d, e, f, g) {
            return typeof d == "boolean" || d === b ? e ? a.effects.animateClass.apply(this, [d ? {add:c} : {remove:c}, e, f, g]) : this._toggleClass(c, d) : a.effects.animateClass.apply(this, [
                {toggle:c},
                d,
                e,
                f
            ])
        }, switchClass:function (b, c, d, e, f) {
            return a.effects.animateClass.apply(this, [
                {add:c, remove:b},
                d,
                e,
                f
            ])
        }}), a.extend(a.effects, {version:"1.8.19", save:function (a, b) {
            for (var c = 0; c < b.length; c++)b[c] !== null && a.data("ec.storage." + b[c], a[0].style[b[c]])
        }, restore:function (a, b) {
            for (var c = 0; c < b.length; c++)b[c] !== null && a.css(b[c], a.data("ec.storage." + b[c]))
        }, setMode:function (a, b) {
            return b == "toggle" && (b = a.is(":hidden") ? "show" : "hide"), b
        }, getBaseline:function (a, b) {
            var c, d;
            switch (a[0]) {
                case"top":
                    c = 0;
                    break;
                case"middle":
                    c = .5;
                    break;
                case"bottom":
                    c = 1;
                    break;
                default:
                    c = a[0] / b.height
            }
            switch (a[1]) {
                case"left":
                    d = 0;
                    break;
                case"center":
                    d = .5;
                    break;
                case"right":
                    d = 1;
                    break;
                default:
                    d = a[1] / b.width
            }
            return{x:d, y:c}
        }, createWrapper:function (b) {
            if (b.parent().is(".ui-effects-wrapper"))return b.parent();
            var c = {width:b.outerWidth(!0), height:b.outerHeight(!0), "float":b.css("float")}, d = a("<div></div>").addClass("ui-effects-wrapper").css({fontSize:"100%", background:"transparent", border:"none", margin:0, padding:0}), e = document.activeElement;
            return b.wrap(d), (b[0] === e || a.contains(b[0], e)) && a(e).focus(), d = b.parent(), b.css("position") == "static" ? (d.css({position:"relative"}), b.css({position:"relative"})) : (a.extend(c, {position:b.css("position"), zIndex:b.css("z-index")}), a.each(["top", "left", "bottom", "right"], function (a, d) {
                c[d] = b.css(d), isNaN(parseInt(c[d], 10)) && (c[d] = "auto")
            }), b.css({position:"relative", top:0, left:0, right:"auto", bottom:"auto"})), d.css(c).show()
        }, removeWrapper:function (b) {
            var c, d = document.activeElement;
            return b.parent().is(".ui-effects-wrapper") ? (c = b.parent().replaceWith(b), (b[0] === d || a.contains(b[0], d)) && a(d).focus(), c) : b
        }, setTransition:function (b, c, d, e) {
            return e = e || {}, a.each(c, function (a, c) {
                var f = b.cssUnit(c);
                f[0] > 0 && (e[c] = f[0] * d + f[1])
            }), e
        }}), a.fn.extend({effect:function (b, c, d, e) {
            var f = k.apply(this, arguments), g = {options:f[1], duration:f[2], callback:f[3]}, h = g.options.mode, i = a.effects[b];
            return a.fx.off || !i ? h ? this[h](g.duration, g.callback) : this.each(function () {
                g.callback && g.callback.call(this)
            }) : i.call(this, g)
        }, _show:a.fn.show, show:function (a) {
            if (l(a))return this._show.apply(this, arguments);
            var b = k.apply(this, arguments);
            return b[1].mode = "show", this.effect.apply(this, b)
        }, _hide:a.fn.hide, hide:function (a) {
            if (l(a))return this._hide.apply(this, arguments);
            var b = k.apply(this, arguments);
            return b[1].mode = "hide", this.effect.apply(this, b)
        }, __toggle:a.fn.toggle, toggle:function (b) {
            if (l(b) || typeof b == "boolean" || a.isFunction(b))return this.__toggle.apply(this, arguments);
            var c = k.apply(this, arguments);
            return c[1].mode = "toggle", this.effect.apply(this, c)
        }, cssUnit:function (b) {
            var c = this.css(b), d = [];
            return a.each(["em", "px", "%", "pt"], function (a, b) {
                c.indexOf(b) > 0 && (d = [parseFloat(c), b])
            }), d
        }}), a.easing.jswing = a.easing.swing, a.extend(a.easing, {def:"easeOutQuad", swing:function (b, c, d, e, f) {
            return a.easing[a.easing.def](b, c, d, e, f)
        }, easeInQuad:function (a, b, c, d, e) {
            return d * (b /= e) * b + c
        }, easeOutQuad:function (a, b, c, d, e) {
            return-d * (b /= e) * (b - 2) + c
        }, easeInOutQuad:function (a, b, c, d, e) {
            return(b /= e / 2) < 1 ? d / 2 * b * b + c : -d / 2 * (--b * (b - 2) - 1) + c
        }, easeInCubic:function (a, b, c, d, e) {
            return d * (b /= e) * b * b + c
        }, easeOutCubic:function (a, b, c, d, e) {
            return d * ((b = b / e - 1) * b * b + 1) + c
        }, easeInOutCubic:function (a, b, c, d, e) {
            return(b /= e / 2) < 1 ? d / 2 * b * b * b + c : d / 2 * ((b -= 2) * b * b + 2) + c
        }, easeInQuart:function (a, b, c, d, e) {
            return d * (b /= e) * b * b * b + c
        }, easeOutQuart:function (a, b, c, d, e) {
            return-d * ((b = b / e - 1) * b * b * b - 1) + c
        }, easeInOutQuart:function (a, b, c, d, e) {
            return(b /= e / 2) < 1 ? d / 2 * b * b * b * b + c : -d / 2 * ((b -= 2) * b * b * b - 2) + c
        }, easeInQuint:function (a, b, c, d, e) {
            return d * (b /= e) * b * b * b * b + c
        }, easeOutQuint:function (a, b, c, d, e) {
            return d * ((b = b / e - 1) * b * b * b * b + 1) + c
        }, easeInOutQuint:function (a, b, c, d, e) {
            return(b /= e / 2) < 1 ? d / 2 * b * b * b * b * b + c : d / 2 * ((b -= 2) * b * b * b * b + 2) + c
        }, easeInSine:function (a, b, c, d, e) {
            return-d * Math.cos(b / e * (Math.PI / 2)) + d + c
        }, easeOutSine:function (a, b, c, d, e) {
            return d * Math.sin(b / e * (Math.PI / 2)) + c
        }, easeInOutSine:function (a, b, c, d, e) {
            return-d / 2 * (Math.cos(Math.PI * b / e) - 1) + c
        }, easeInExpo:function (a, b, c, d, e) {
            return b == 0 ? c : d * Math.pow(2, 10 * (b / e - 1)) + c
        }, easeOutExpo:function (a, b, c, d, e) {
            return b == e ? c + d : d * (-Math.pow(2, -10 * b / e) + 1) + c
        }, easeInOutExpo:function (a, b, c, d, e) {
            return b == 0 ? c : b == e ? c + d : (b /= e / 2) < 1 ? d / 2 * Math.pow(2, 10 * (b - 1)) + c : d / 2 * (-Math.pow(2, -10 * --b) + 2) + c
        }, easeInCirc:function (a, b, c, d, e) {
            return-d * (Math.sqrt(1 - (b /= e) * b) - 1) + c
        }, easeOutCirc:function (a, b, c, d, e) {
            return d * Math.sqrt(1 - (b = b / e - 1) * b) + c
        }, easeInOutCirc:function (a, b, c, d, e) {
            return(b /= e / 2) < 1 ? -d / 2 * (Math.sqrt(1 - b * b) - 1) + c : d / 2 * (Math.sqrt(1 - (b -= 2) * b) + 1) + c
        }, easeInElastic:function (a, b, c, d, e) {
            var f = 1.70158, g = 0, h = d;
            if (b == 0)return c;
            if ((b /= e) == 1)return c + d;
            g || (g = e * .3);
            if (h < Math.abs(d)) {
                h = d;
                var f = g / 4
            } else var f = g / (2 * Math.PI) * Math.asin(d / h);
            return-(h * Math.pow(2, 10 * (b -= 1)) * Math.sin((b * e - f) * 2 * Math.PI / g)) + c
        }, easeOutElastic:function (a, b, c, d, e) {
            var f = 1.70158, g = 0, h = d;
            if (b == 0)return c;
            if ((b /= e) == 1)return c + d;
            g || (g = e * .3);
            if (h < Math.abs(d)) {
                h = d;
                var f = g / 4
            } else var f = g / (2 * Math.PI) * Math.asin(d / h);
            return h * Math.pow(2, -10 * b) * Math.sin((b * e - f) * 2 * Math.PI / g) + d + c
        }, easeInOutElastic:function (a, b, c, d, e) {
            var f = 1.70158, g = 0, h = d;
            if (b == 0)return c;
            if ((b /= e / 2) == 2)return c + d;
            g || (g = e * .3 * 1.5);
            if (h < Math.abs(d)) {
                h = d;
                var f = g / 4
            } else var f = g / (2 * Math.PI) * Math.asin(d / h);
            return b < 1 ? -0.5 * h * Math.pow(2, 10 * (b -= 1)) * Math.sin((b * e - f) * 2 * Math.PI / g) + c : h * Math.pow(2, -10 * (b -= 1)) * Math.sin((b * e - f) * 2 * Math.PI / g) * .5 + d + c
        }, easeInBack:function (a, c, d, e, f, g) {
            return g == b && (g = 1.70158), e * (c /= f) * c * ((g + 1) * c - g) + d
        }, easeOutBack:function (a, c, d, e, f, g) {
            return g == b && (g = 1.70158), e * ((c = c / f - 1) * c * ((g + 1) * c + g) + 1) + d
        }, easeInOutBack:function (a, c, d, e, f, g) {
            return g == b && (g = 1.70158), (c /= f / 2) < 1 ? e / 2 * c * c * (((g *= 1.525) + 1) * c - g) + d : e / 2 * ((c -= 2) * c * (((g *= 1.525) + 1) * c + g) + 2) + d
        }, easeInBounce:function (b, c, d, e, f) {
            return e - a.easing.easeOutBounce(b, f - c, 0, e, f) + d
        }, easeOutBounce:function (a, b, c, d, e) {
            return(b /= e) < 1 / 2.75 ? d * 7.5625 * b * b + c : b < 2 / 2.75 ? d * (7.5625 * (b -= 1.5 / 2.75) * b + .75) + c : b < 2.5 / 2.75 ? d * (7.5625 * (b -= 2.25 / 2.75) * b + .9375) + c : d * (7.5625 * (b -= 2.625 / 2.75) * b + .984375) + c
        }, easeInOutBounce:function (b, c, d, e, f) {
            return c < f / 2 ? a.easing.easeInBounce(b, c * 2, 0, e, f) * .5 + d : a.easing.easeOutBounce(b, c * 2 - f, 0, e, f) * .5 + e * .5 + d
        }})
    }(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.blind.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.blind = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right"], e = a.effects.setMode(c, b.options.mode || "hide"), f = b.options.direction || "vertical";
                a.effects.save(c, d), c.show();
                var g = a.effects.createWrapper(c).css({overflow:"hidden"}), h = f == "vertical" ? "height" : "width", i = f == "vertical" ? g.height() : g.width();
                e == "show" && g.css(h, 0);
                var j = {};
                j[h] = e == "show" ? i : 0, g.animate(j, b.duration, b.options.easing, function () {
                    e == "hide" && c.hide(), a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(c[0], arguments), c.dequeue()
                })
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.bounce.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.bounce = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right"], e = a.effects.setMode(c, b.options.mode || "effect"), f = b.options.direction || "up", g = b.options.distance || 20, h = b.options.times || 5, i = b.duration || 250;
                /show|hide/.test(e) && d.push("opacity"), a.effects.save(c, d), c.show(), a.effects.createWrapper(c);
                var j = f == "up" || f == "down" ? "top" : "left", k = f == "up" || f == "left" ? "pos" : "neg", g = b.options.distance || (j == "top" ? c.outerHeight({margin:!0}) / 3 : c.outerWidth({margin:!0}) / 3);
                e == "show" && c.css("opacity", 0).css(j, k == "pos" ? -g : g), e == "hide" && (g = g / (h * 2)), e != "hide" && h--;
                if (e == "show") {
                    var l = {opacity:1};
                    l[j] = (k == "pos" ? "+=" : "-=") + g, c.animate(l, i / 2, b.options.easing), g = g / 2, h--
                }
                for (var m = 0; m < h; m++) {
                    var n = {}, p = {};
                    n[j] = (k == "pos" ? "-=" : "+=") + g, p[j] = (k == "pos" ? "+=" : "-=") + g, c.animate(n, i / 2, b.options.easing).animate(p, i / 2, b.options.easing), g = e == "hide" ? g * 2 : g / 2
                }
                if (e == "hide") {
                    var l = {opacity:0};
                    l[j] = (k == "pos" ? "-=" : "+=") + g, c.animate(l, i / 2, b.options.easing, function () {
                        c.hide(), a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(this, arguments)
                    })
                } else {
                    var n = {}, p = {};
                    n[j] = (k == "pos" ? "-=" : "+=") + g, p[j] = (k == "pos" ? "+=" : "-=") + g, c.animate(n, i / 2, b.options.easing).animate(p, i / 2, b.options.easing, function () {
                        a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(this, arguments)
                    })
                }
                c.queue("fx", function () {
                    c.dequeue()
                }), c.dequeue()
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.clip.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.clip = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right", "height", "width"], e = a.effects.setMode(c, b.options.mode || "hide"), f = b.options.direction || "vertical";
                a.effects.save(c, d), c.show();
                var g = a.effects.createWrapper(c).css({overflow:"hidden"}), h = c[0].tagName == "IMG" ? g : c, i = {size:f == "vertical" ? "height" : "width", position:f == "vertical" ? "top" : "left"}, j = f == "vertical" ? h.height() : h.width();
                e == "show" && (h.css(i.size, 0), h.css(i.position, j / 2));
                var k = {};
                k[i.size] = e == "show" ? j : 0, k[i.position] = e == "show" ? 0 : j / 2, h.animate(k, {queue:!1, duration:b.duration, easing:b.options.easing, complete:function () {
                    e == "hide" && c.hide(), a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(c[0], arguments), c.dequeue()
                }})
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.drop.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.drop = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right", "opacity"], e = a.effects.setMode(c, b.options.mode || "hide"), f = b.options.direction || "left";
                a.effects.save(c, d), c.show(), a.effects.createWrapper(c);
                var g = f == "up" || f == "down" ? "top" : "left", h = f == "up" || f == "left" ? "pos" : "neg", i = b.options.distance || (g == "top" ? c.outerHeight({margin:!0}) / 2 : c.outerWidth({margin:!0}) / 2);
                e == "show" && c.css("opacity", 0).css(g, h == "pos" ? -i : i);
                var j = {opacity:e == "show" ? 1 : 0};
                j[g] = (e == "show" ? h == "pos" ? "+=" : "-=" : h == "pos" ? "-=" : "+=") + i, c.animate(j, {queue:!1, duration:b.duration, easing:b.options.easing, complete:function () {
                    e == "hide" && c.hide(), a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(this, arguments), c.dequeue()
                }})
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.explode.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.explode = function (b) {
            return this.queue(function () {
                var c = b.options.pieces ? Math.round(Math.sqrt(b.options.pieces)) : 3, d = b.options.pieces ? Math.round(Math.sqrt(b.options.pieces)) : 3;
                b.options.mode = b.options.mode == "toggle" ? a(this).is(":visible") ? "hide" : "show" : b.options.mode;
                var e = a(this).show().css("visibility", "hidden"), f = e.offset();
                f.top -= parseInt(e.css("marginTop"), 10) || 0, f.left -= parseInt(e.css("marginLeft"), 10) || 0;
                var g = e.outerWidth(!0), h = e.outerHeight(!0);
                for (var i = 0; i < c; i++)for (var j = 0; j < d; j++)e.clone().appendTo("body").wrap("<div></div>").css({position:"absolute", visibility:"visible", left:-j * (g / d), top:-i * (h / c)}).parent().addClass("ui-effects-explode").css({position:"absolute", overflow:"hidden", width:g / d, height:h / c, left:f.left + j * (g / d) + (b.options.mode == "show" ? (j - Math.floor(d / 2)) * (g / d) : 0), top:f.top + i * (h / c) + (b.options.mode == "show" ? (i - Math.floor(c / 2)) * (h / c) : 0), opacity:b.options.mode == "show" ? 0 : 1}).animate({left:f.left + j * (g / d) + (b.options.mode == "show" ? 0 : (j - Math.floor(d / 2)) * (g / d)), top:f.top + i * (h / c) + (b.options.mode == "show" ? 0 : (i - Math.floor(c / 2)) * (h / c)), opacity:b.options.mode == "show" ? 1 : 0}, b.duration || 500);
                setTimeout(function () {
                    b.options.mode == "show" ? e.css({visibility:"visible"}) : e.css({visibility:"visible"}).hide(), b.callback && b.callback.apply(e[0]), e.dequeue(), a("div.ui-effects-explode").remove()
                }, b.duration || 500)
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.fade.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.fade = function (b) {
            return this.queue(function () {
                var c = a(this), d = a.effects.setMode(c, b.options.mode || "hide");
                c.animate({opacity:d}, {queue:!1, duration:b.duration, easing:b.options.easing, complete:function () {
                    b.callback && b.callback.apply(this, arguments), c.dequeue()
                }})
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.fold.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.fold = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right"], e = a.effects.setMode(c, b.options.mode || "hide"), f = b.options.size || 15, g = !!b.options.horizFirst, h = b.duration ? b.duration / 2 : a.fx.speeds._default / 2;
                a.effects.save(c, d), c.show();
                var i = a.effects.createWrapper(c).css({overflow:"hidden"}), j = e == "show" != g, k = j ? ["width", "height"] : ["height", "width"], l = j ? [i.width(), i.height()] : [i.height(), i.width()], m = /([0-9]+)%/.exec(f);
                m && (f = parseInt(m[1], 10) / 100 * l[e == "hide" ? 0 : 1]), e == "show" && i.css(g ? {height:0, width:f} : {height:f, width:0});
                var n = {}, p = {};
                n[k[0]] = e == "show" ? l[0] : f, p[k[1]] = e == "show" ? l[1] : 0, i.animate(n, h, b.options.easing).animate(p, h, b.options.easing, function () {
                    e == "hide" && c.hide(), a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(c[0], arguments), c.dequeue()
                })
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.highlight.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.highlight = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["backgroundImage", "backgroundColor", "opacity"], e = a.effects.setMode(c, b.options.mode || "show"), f = {backgroundColor:c.css("backgroundColor")};
                e == "hide" && (f.opacity = 0), a.effects.save(c, d), c.show().css({backgroundImage:"none", backgroundColor:b.options.color || "#ffff99"}).animate(f, {queue:!1, duration:b.duration, easing:b.options.easing, complete:function () {
                    e == "hide" && c.hide(), a.effects.restore(c, d), e == "show" && !a.support.opacity && this.style.removeAttribute("filter"), b.callback && b.callback.apply(this, arguments), c.dequeue()
                }})
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.pulsate.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.pulsate = function (b) {
            return this.queue(function () {
                var c = a(this), d = a.effects.setMode(c, b.options.mode || "show"), e = (b.options.times || 5) * 2 - 1, f = b.duration ? b.duration / 2 : a.fx.speeds._default / 2, g = c.is(":visible"), h = 0;
                g || (c.css("opacity", 0).show(), h = 1), (d == "hide" && g || d == "show" && !g) && e--;
                for (var i = 0; i < e; i++)c.animate({opacity:h}, f, b.options.easing), h = (h + 1) % 2;
                c.animate({opacity:h}, f, b.options.easing, function () {
                    h == 0 && c.hide(), b.callback && b.callback.apply(this, arguments)
                }), c.queue("fx",function () {
                    c.dequeue()
                }).dequeue()
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.scale.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.puff = function (b) {
            return this.queue(function () {
                var c = a(this), d = a.effects.setMode(c, b.options.mode || "hide"), e = parseInt(b.options.percent, 10) || 150, f = e / 100, g = {height:c.height(), width:c.width()};
                a.extend(b.options, {fade:!0, mode:d, percent:d == "hide" ? e : 100, from:d == "hide" ? g : {height:g.height * f, width:g.width * f}}), c.effect("scale", b.options, b.duration, b.callback), c.dequeue()
            })
        }, a.effects.scale = function (b) {
            return this.queue(function () {
                var c = a(this), d = a.extend(!0, {}, b.options), e = a.effects.setMode(c, b.options.mode || "effect"), f = parseInt(b.options.percent, 10) || (parseInt(b.options.percent, 10) == 0 ? 0 : e == "hide" ? 0 : 100), g = b.options.direction || "both", h = b.options.origin;
                e != "effect" && (d.origin = h || ["middle", "center"], d.restore = !0);
                var i = {height:c.height(), width:c.width()};
                c.from = b.options.from || (e == "show" ? {height:0, width:0} : i);
                var j = {y:g != "horizontal" ? f / 100 : 1, x:g != "vertical" ? f / 100 : 1};
                c.to = {height:i.height * j.y, width:i.width * j.x}, b.options.fade && (e == "show" && (c.from.opacity = 0, c.to.opacity = 1), e == "hide" && (c.from.opacity = 1, c.to.opacity = 0)), d.from = c.from, d.to = c.to, d.mode = e, c.effect("size", d, b.duration, b.callback), c.dequeue()
            })
        }, a.effects.size = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right", "width", "height", "overflow", "opacity"], e = ["position", "top", "bottom", "left", "right", "overflow", "opacity"], f = ["width", "height", "overflow"], g = ["fontSize"], h = ["borderTopWidth", "borderBottomWidth", "paddingTop", "paddingBottom"], i = ["borderLeftWidth", "borderRightWidth", "paddingLeft", "paddingRight"], j = a.effects.setMode(c, b.options.mode || "effect"), k = b.options.restore || !1, l = b.options.scale || "both", m = b.options.origin, n = {height:c.height(), width:c.width()};
                c.from = b.options.from || n, c.to = b.options.to || n;
                if (m) {
                    var p = a.effects.getBaseline(m, n);
                    c.from.top = (n.height - c.from.height) * p.y, c.from.left = (n.width - c.from.width) * p.x, c.to.top = (n.height - c.to.height) * p.y, c.to.left = (n.width - c.to.width) * p.x
                }
                var q = {from:{y:c.from.height / n.height, x:c.from.width / n.width}, to:{y:c.to.height / n.height, x:c.to.width / n.width}};
                if (l == "box" || l == "both")q.from.y != q.to.y && (d = d.concat(h), c.from = a.effects.setTransition(c, h, q.from.y, c.from), c.to = a.effects.setTransition(c, h, q.to.y, c.to)), q.from.x != q.to.x && (d = d.concat(i), c.from = a.effects.setTransition(c, i, q.from.x, c.from), c.to = a.effects.setTransition(c, i, q.to.x, c.to));
                (l == "content" || l == "both") && q.from.y != q.to.y && (d = d.concat(g), c.from = a.effects.setTransition(c, g, q.from.y, c.from), c.to = a.effects.setTransition(c, g, q.to.y, c.to)), a.effects.save(c, k ? d : e), c.show(), a.effects.createWrapper(c), c.css("overflow", "hidden").css(c.from);
                if (l == "content" || l == "both")h = h.concat(["marginTop", "marginBottom"]).concat(g), i = i.concat(["marginLeft", "marginRight"]), f = d.concat(h).concat(i), c.find("*[width]").each(function () {
                    var c = a(this);
                    k && a.effects.save(c, f);
                    var d = {height:c.height(), width:c.width()};
                    c.from = {height:d.height * q.from.y, width:d.width * q.from.x}, c.to = {height:d.height * q.to.y, width:d.width * q.to.x}, q.from.y != q.to.y && (c.from = a.effects.setTransition(c, h, q.from.y, c.from), c.to = a.effects.setTransition(c, h, q.to.y, c.to)), q.from.x != q.to.x && (c.from = a.effects.setTransition(c, i, q.from.x, c.from), c.to = a.effects.setTransition(c, i, q.to.x, c.to)), c.css(c.from), c.animate(c.to, b.duration, b.options.easing, function () {
                        k && a.effects.restore(c, f)
                    })
                });
                c.animate(c.to, {queue:!1, duration:b.duration, easing:b.options.easing, complete:function () {
                    c.to.opacity === 0 && c.css("opacity", c.from.opacity), j == "hide" && c.hide(), a.effects.restore(c, k ? d : e), a.effects.removeWrapper(c), b.callback && b.callback.apply(this, arguments), c.dequeue()
                }})
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.shake.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.shake = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right"], e = a.effects.setMode(c, b.options.mode || "effect"), f = b.options.direction || "left", g = b.options.distance || 20, h = b.options.times || 3, i = b.duration || b.options.duration || 140;
                a.effects.save(c, d), c.show(), a.effects.createWrapper(c);
                var j = f == "up" || f == "down" ? "top" : "left", k = f == "up" || f == "left" ? "pos" : "neg", l = {}, m = {}, n = {};
                l[j] = (k == "pos" ? "-=" : "+=") + g, m[j] = (k == "pos" ? "+=" : "-=") + g * 2, n[j] = (k == "pos" ? "-=" : "+=") + g * 2, c.animate(l, i, b.options.easing);
                for (var p = 1; p < h; p++)c.animate(m, i, b.options.easing).animate(n, i, b.options.easing);
                c.animate(m, i, b.options.easing).animate(l, i / 2, b.options.easing, function () {
                    a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(this, arguments)
                }), c.queue("fx", function () {
                    c.dequeue()
                }), c.dequeue()
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.slide.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.slide = function (b) {
            return this.queue(function () {
                var c = a(this), d = ["position", "top", "bottom", "left", "right"], e = a.effects.setMode(c, b.options.mode || "show"), f = b.options.direction || "left";
                a.effects.save(c, d), c.show(), a.effects.createWrapper(c).css({overflow:"hidden"});
                var g = f == "up" || f == "down" ? "top" : "left", h = f == "up" || f == "left" ? "pos" : "neg", i = b.options.distance || (g == "top" ? c.outerHeight({margin:!0}) : c.outerWidth({margin:!0}));
                e == "show" && c.css(g, h == "pos" ? isNaN(i) ? "-" + i : -i : i);
                var j = {};
                j[g] = (e == "show" ? h == "pos" ? "+=" : "-=" : h == "pos" ? "-=" : "+=") + i, c.animate(j, {queue:!1, duration:b.duration, easing:b.options.easing, complete:function () {
                    e == "hide" && c.hide(), a.effects.restore(c, d), a.effects.removeWrapper(c), b.callback && b.callback.apply(this, arguments), c.dequeue()
                }})
            })
        }
    })(jQuery);
    /*! jQuery UI - v1.8.19 - 2012-04-16
     * https://github.com/jquery/jquery-ui
     * Includes: jquery.effects.transfer.js
     * Copyright (c) 2012 AUTHORS.txt; Licensed MIT, GPL */
    (function (a, b) {
        a.effects.transfer = function (b) {
            return this.queue(function () {
                var c = a(this), d = a(b.options.to), e = d.offset(), f = {top:e.top, left:e.left, height:d.innerHeight(), width:d.innerWidth()}, g = c.offset(), h = a('<div class="ui-effects-transfer"></div>').appendTo(document.body).addClass(b.options.className).css({top:g.top, left:g.left, height:c.innerHeight(), width:c.innerWidth(), position:"absolute"}).animate(f, b.duration, b.options.easing, function () {
                    h.remove(), b.callback && b.callback.apply(c[0], arguments), c.dequeue()
                })
            })
        }
    })(jQuery);

    return jq;
});

/*!
/*
 * jQuery UI Sortable 1.8.16
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Sortables
 *
 * Depends:
 *	jquery.ui.core.js
 *	jquery.ui.mouse.js
 *	jquery.ui.widget.js
 */

define('libs/jquery/jquery.ui.sortable.performance',["libs/jquery/jquery","libs/jquery/jquery.ui"],function(jQuery){return function(d){d.widget("ui.sortable",d.ui.mouse,{widgetEventPrefix:"sort",options:{appendTo:"parent",axis:!1,connectWith:!1,containment:!1,cursor:"auto",cursorAt:!1,dropOnEmpty:!0,forcePlaceholderSize:!1,forceHelperSize:!1,grid:!1,handle:!1,helper:"original",items:"> *",opacity:!1,placeholder:!1,revert:!1,scroll:!0,scrollSensitivity:20,scrollSpeed:20,scope:"default",tolerance:"intersect",zIndex:1e3},_create:function(){var a=this.options;this.containerCache={},this.element.addClass("ui-sortable"),this.refresh(),this.floating=this.items.length?a.axis==="x"||/left|right/.test(this.items[0].item.css("float"))||/inline|table-cell/.test(this.items[0].item.css("display")):!1,this.offset=this.element.offset(),this._mouseInit()},destroy:function(){this.element.removeClass("ui-sortable ui-sortable-disabled").removeData("sortable").unbind(".sortable"),this._mouseDestroy();for(var a=this.items.length-1;a>=0;a--)this.items[a].item.removeData("sortable-item");return this},_setOption:function(a,b){a==="disabled"?(this.options[a]=b,this.widget()[b?"addClass":"removeClass"]("ui-sortable-disabled")):d.Widget.prototype._setOption.apply(this,arguments)},_mouseCapture:function(a,b){if(this.reverting)return!1;if(this.options.disabled||this.options.type=="static")return!1;this._refreshItems(a);var c=null,e=this;d(a.target).parents().each(function(){if(d.data(this,"sortable-item")==e)return c=d(this),!1}),d.data(a.target,"sortable-item")==e&&(c=d(a.target));if(!c)return!1;if(this.options.handle&&!b){var f=!1;d(this.options.handle,c).find("*").andSelf().each(function(){this==a.target&&(f=!0)});if(!f)return!1}return this.currentItem=c,this._removeCurrentsFromItems(),!0},_mouseStart:function(a,b,c){b=this.options;var e=this;this.currentContainer=this,this.refreshPositions(),this.helper=this._createHelper(a),this._cacheHelperProportions(),this._cacheMargins(),this.scrollParent=this.helper.scrollParent(),this.offset=this.currentItem.offset(),this.offset={top:this.offset.top-this.margins.top,left:this.offset.left-this.margins.left},this.helper.css("position","absolute"),this.cssPosition=this.helper.css("position"),d.extend(this.offset,{click:{left:a.pageX-this.offset.left,top:a.pageY-this.offset.top},parent:this._getParentOffset(),relative:this._getRelativeOffset()}),this.originalPosition=this._generatePosition(a),this.originalPageX=a.pageX,this.originalPageY=a.pageY,b.cursorAt&&this._adjustOffsetFromHelper(b.cursorAt),this.domPosition={prev:this.currentItem.prev()[0],parent:this.currentItem.parent()[0]},this.helper[0]!=this.currentItem[0]&&this.currentItem.hide(),this._createPlaceholder(),b.containment&&this._setContainment(),b.cursor&&(d("body").css("cursor")&&(this._storedCursor=d("body").css("cursor")),d("body").css("cursor",b.cursor)),b.opacity&&(this.helper.css("opacity")&&(this._storedOpacity=this.helper.css("opacity")),this.helper.css("opacity",b.opacity)),b.zIndex&&(this.helper.css("zIndex")&&(this._storedZIndex=this.helper.css("zIndex")),this.helper.css("zIndex",b.zIndex)),this.scrollParent[0]!=document&&this.scrollParent[0].tagName!="HTML"&&(this.overflowOffset=this.scrollParent.offset()),this._trigger("start",a,this._uiHash()),this._preserveHelperProportions||this._cacheHelperProportions();if(!c)for(c=this.containers.length-1;c>=0;c--)this.containers[c]._trigger("activate",a,e._uiHash(this));return d.ui.ddmanager&&(d.ui.ddmanager.current=this),d.ui.ddmanager&&!b.dropBehaviour&&d.ui.ddmanager.prepareOffsets(this,a),this.dragging=!0,this.helper.addClass("ui-sortable-helper"),this._mouseDrag(a),!0},_mouseDrag:function(a){this.position=this._generatePosition(a),this.positionAbs=this._convertPositionTo("absolute"),this.lastPositionAbs||(this.lastPositionAbs=this.positionAbs);if(this.options.scroll){var b=this.options,c=!1;this.scrollParent[0]!=document&&this.scrollParent[0].tagName!="HTML"?(this.overflowOffset.top+this.scrollParent[0].offsetHeight-a.pageY<b.scrollSensitivity?this.scrollParent[0].scrollTop=c=this.scrollParent[0].scrollTop+b.scrollSpeed:a.pageY-this.overflowOffset.top<b.scrollSensitivity&&(this.scrollParent[0].scrollTop=c=this.scrollParent[0].scrollTop-b.scrollSpeed),this.overflowOffset.left+this.scrollParent[0].offsetWidth-a.pageX<b.scrollSensitivity?this.scrollParent[0].scrollLeft=c=this.scrollParent[0].scrollLeft+b.scrollSpeed:a.pageX-this.overflowOffset.left<b.scrollSensitivity&&(this.scrollParent[0].scrollLeft=c=this.scrollParent[0].scrollLeft-b.scrollSpeed)):(a.pageY-d(document).scrollTop()<b.scrollSensitivity?c=d(document).scrollTop(d(document).scrollTop()-b.scrollSpeed):d(window).height()-(a.pageY-d(document).scrollTop())<b.scrollSensitivity&&(c=d(document).scrollTop(d(document).scrollTop()+b.scrollSpeed)),a.pageX-d(document).scrollLeft()<b.scrollSensitivity?c=d(document).scrollLeft(d(document).scrollLeft()-b.scrollSpeed):d(window).width()-(a.pageX-d(document).scrollLeft())<b.scrollSensitivity&&(c=d(document).scrollLeft(d(document).scrollLeft()+b.scrollSpeed))),c!==!1&&d.ui.ddmanager&&!b.dropBehaviour&&d.ui.ddmanager.prepareOffsets(this,a)}this.positionAbs=this._convertPositionTo("absolute");if(!this.options.axis||this.options.axis!="y")this.helper[0].style.left=this.position.left+"px";if(!this.options.axis||this.options.axis!="x")this.helper[0].style.top=this.position.top+"px";for(b=this.items.length-1;b>=0;b--){c=this.items[b];var e=c.item[0],f=this._intersectsWithPointer(c);if(f&&e!=this.currentItem[0]&&this.placeholder[f==1?"next":"prev"]()[0]!=e&&!d.ui.contains(this.placeholder[0],e)&&(this.options.type=="semi-dynamic"?!d.ui.contains(this.element[0],e):!0)){this.direction=f==1?"down":"up";if(this.options.tolerance!="pointer"&&!this._intersectsWithSides(c))break;this._rearrange(a,c),this._trigger("change",a,this._uiHash());break}}return this._contactContainers(a),d.ui.ddmanager&&d.ui.ddmanager.drag(this,a),this._trigger("sort",a,this._uiHash()),this.lastPositionAbs=this.positionAbs,!1},_mouseStop:function(a,b){if(a){d.ui.ddmanager&&!this.options.dropBehaviour&&d.ui.ddmanager.drop(this,a);if(this.options.revert){var c=this;b=c.placeholder.offset(),c.reverting=!0,d(this.helper).animate({left:b.left-this.offset.parent.left-c.margins.left+(this.offsetParent[0]==document.body?0:this.offsetParent[0].scrollLeft),top:b.top-this.offset.parent.top-c.margins.top+(this.offsetParent[0]==document.body?0:this.offsetParent[0].scrollTop)},parseInt(this.options.revert,10)||500,function(){c._clear(a)})}else this._clear(a,b);return!1}},cancel:function(){var a=this;if(this.dragging){this._mouseUp({target:null}),this.options.helper=="original"?this.currentItem.css(this._storedCSS).removeClass("ui-sortable-helper"):this.currentItem.show();for(var b=this.containers.length-1;b>=0;b--)this.containers[b]._trigger("deactivate",null,a._uiHash(this)),this.containers[b].containerCache.over&&(this.containers[b]._trigger("out",null,a._uiHash(this)),this.containers[b].containerCache.over=0)}return this.placeholder&&(this.placeholder[0].parentNode&&this.placeholder[0].parentNode.removeChild(this.placeholder[0]),this.options.helper!="original"&&this.helper&&this.helper[0].parentNode&&this.helper.remove(),d.extend(this,{helper:null,dragging:!1,reverting:!1,_noFinalSort:null}),this.domPosition.prev?d(this.domPosition.prev).after(this.currentItem):d(this.domPosition.parent).prepend(this.currentItem)),this},serialize:function(a){var b=this._getItemsAsjQuery(a&&a.connected),c=[];return a=a||{},d(b).each(function(){var e=(d(a.item||this).attr(a.attribute||"id")||"").match(a.expression||/(.+)[-=_](.+)/);e&&c.push((a.key||e[1]+"[]")+"="+(a.key&&a.expression?e[1]:e[2]))}),!c.length&&a.key&&c.push(a.key+"="),c.join("&")},toArray:function(a){var b=this._getItemsAsjQuery(a&&a.connected),c=[];return a=a||{},b.each(function(){c.push(d(a.item||this).attr(a.attribute||"id")||"")}),c},_intersectsWith:function(a){var b=this.positionAbs.left,c=b+this.helperProportions.width,e=this.positionAbs.top,f=e+this.helperProportions.height,g=a.left,h=g+a.width,i=a.top,k=i+a.height,j=this.offset.click.top,l=this.offset.click.left;return j=e+j>i&&e+j<k&&b+l>g&&b+l<h,this.options.tolerance=="pointer"||this.options.forcePointerForContainers||this.options.tolerance!="pointer"&&this.helperProportions[this.floating?"width":"height"]>a[this.floating?"width":"height"]?j:g<b+this.helperProportions.width/2&&c-this.helperProportions.width/2<h&&i<e+this.helperProportions.height/2&&f-this.helperProportions.height/2<k},_intersectsWithPointer:function(a){var b=d.ui.isOverAxis(this.positionAbs.top+this.offset.click.top,a.top,a.height);a=d.ui.isOverAxis(this.positionAbs.left+this.offset.click.left,a.left,a.width),b=b&&a,a=this._getDragVerticalDirection();var c=this._getDragHorizontalDirection();return b?this.floating?c&&c=="right"||a=="down"?2:1:a&&(a=="down"?2:1):!1},_intersectsWithSides:function(a){var b=d.ui.isOverAxis(this.positionAbs.top+this.offset.click.top,a.top+a.height/2,a.height);a=d.ui.isOverAxis(this.positionAbs.left+this.offset.click.left,a.left+a.width/2,a.width);var c=this._getDragVerticalDirection(),e=this._getDragHorizontalDirection();return this.floating&&e?e=="right"&&a||e=="left"&&!a:c&&(c=="down"&&b||c=="up"&&!b)},_getDragVerticalDirection:function(){var a=this.positionAbs.top-this.lastPositionAbs.top;return a!=0&&(a>0?"down":"up")},_getDragHorizontalDirection:function(){var a=this.positionAbs.left-this.lastPositionAbs.left;return a!=0&&(a>0?"right":"left")},refresh:function(a){return this._refreshItems(a),this.refreshPositions(!0),this},_connectWith:function(){var a=this.options;return a.connectWith.constructor==String?[a.connectWith]:a.connectWith},_getItemsAsjQuery:function(a){var b=[],c=[],e=this._connectWith();if(e&&a)for(a=e.length-1;a>=0;a--)for(var f=d(e[a]),g=f.length-1;g>=0;g--){var h=d.data(f[g],"sortable");h&&h!=this&&!h.options.disabled&&c.push([d.isFunction(h.options.items)?h.options.items.call(h.element):d(h.options.items,h.element).not(".ui-sortable-helper").not(".ui-sortable-placeholder"),h])}c.push([d.isFunction(this.options.items)?this.options.items.call(this.element,null,{options:this.options,item:this.currentItem}):d(this.options.items,this.element).not(".ui-sortable-helper").not(".ui-sortable-placeholder"),this]);for(a=c.length-1;a>=0;a--)c[a][0].each(function(){b.push(this)});return d(b)},_removeCurrentsFromItems:function(){for(var a=this.currentItem.find(":data(sortable-item)"),b=0;b<this.items.length;b++)for(var c=0;c<a.length;c++)a[c]==this.items[b].item[0]&&this.items.splice(b,1)},_refreshItems:function(a){this.items=[],this.containers=[this];var b=this.items,c=[[d.isFunction(this.options.items)?this.options.items.call(this.element[0],a,{item:this.currentItem}):d(this.options.items,this.element),this]],e=this._connectWith();if(e)for(var f=e.length-1;f>=0;f--)for(var g=d(e[f]),h=g.length-1;h>=0;h--){var i=d.data(g[h],"sortable");i&&i!=this&&!i.options.disabled&&(c.push([d.isFunction(i.options.items)?i.options.items.call(i.element[0],a,{item:this.currentItem}):d(i.options.items,i.element),i]),this.containers.push(i))}for(f=c.length-1;f>=0;f--){a=c[f][1],e=c[f][0],h=0;for(g=e.length;h<g;h++)i=d(e[h]),i.data("sortable-item",a),b.push({item:i,instance:a,width:0,height:0,left:0,top:0})}},refreshPositions:function(a){this.offsetParent&&this.helper&&(this.offset.parent=this._getParentOffset());for(var b=this.items.length-1;b>=0;b--){var c=this.items[b];if(c.instance==this.currentContainer||!this.currentContainer||c.item[0]==this.currentItem[0]){var e=this.options.toleranceElement?d(this.options.toleranceElement,c.item):c.item;a||(c.width=e.outerWidth(),c.height=e.outerHeight()),e=e.offset(),c.left=e.left,c.top=e.top}}if(this.options.custom&&this.options.custom.refreshContainers)this.options.custom.refreshContainers.call(this);else for(b=this.containers.length-1;b>=0;b--)e=this.containers[b].element.offset(),this.containers[b].containerCache.left=e.left,this.containers[b].containerCache.top=e.top,this.containers[b].containerCache.width=this.containers[b].element.outerWidth(),this.containers[b].containerCache.height=this.containers[b].element.outerHeight();return this},_createPlaceholder:function(a){var b=a||this,c=b.options;if(!c.placeholder||c.placeholder.constructor==String){var e=c.placeholder;c.placeholder={element:function(){var f=d(document.createElement(b.currentItem[0].nodeName)).addClass(e||b.currentItem[0].className+" ui-sortable-placeholder").removeClass("ui-sortable-helper")[0];return e||(f.style.visibility="hidden"),f},update:function(f,g){if(!e||!!c.forcePlaceholderSize)g.height()||g.height(b.currentItem.innerHeight()-parseInt(b.currentItem.css("paddingTop")||0,10)-parseInt(b.currentItem.css("paddingBottom")||0,10)),g.width()||g.width(b.currentItem.innerWidth()-parseInt(b.currentItem.css("paddingLeft")||0,10)-parseInt(b.currentItem.css("paddingRight")||0,10))}}}b.placeholder=d(c.placeholder.element.call(b.element,b.currentItem)),b.currentItem.after(b.placeholder),c.placeholder.update(b,b.placeholder)},_contactContainers:function(a){for(var b=null,c=null,e=this.containers.length-1;e>=0;e--)if(!d.ui.contains(this.currentItem[0],this.containers[e].element[0]))if(this._intersectsWith(this.containers[e].containerCache)){if(!b||!d.ui.contains(this.containers[e].element[0],b.element[0]))b=this.containers[e],c=e}else this.containers[e].containerCache.over&&(this.containers[e]._trigger("out",a,this._uiHash(this)),this.containers[e].containerCache.over=0);if(b)if(this.containers.length===1)this.containers[c]._trigger("over",a,this._uiHash(this)),this.containers[c].containerCache.over=1;else if(this.currentContainer!=this.containers[c]){b=1e4,e=null;for(var f=this.positionAbs[this.containers[c].floating?"left":"top"],g=this.items.length-1;g>=0;g--)if(d.ui.contains(this.containers[c].element[0],this.items[g].item[0])){var h=this.items[g][this.containers[c].floating?"left":"top"];Math.abs(h-f)<b&&(b=Math.abs(h-f),e=this.items[g])}if(e||this.options.dropOnEmpty)this.currentContainer=this.containers[c],e?this._rearrange(a,e,null,!0):this._rearrange(a,null,this.containers[c].element,!0),this._trigger("change",a,this._uiHash()),this.containers[c]._trigger("change",a,this._uiHash(this)),this.options.placeholder.update(this.currentContainer,this.placeholder),this.containers[c]._trigger("over",a,this._uiHash(this)),this.containers[c].containerCache.over=1}},_createHelper:function(a){var b=this.options;return a=d.isFunction(b.helper)?d(b.helper.apply(this.element[0],[a,this.currentItem])):b.helper=="clone"?this.currentItem.clone():this.currentItem,a.parents("body").length||d(b.appendTo!="parent"?b.appendTo:this.currentItem[0].parentNode)[0].appendChild(a[0]),a[0]==this.currentItem[0]&&(this._storedCSS={width:this.currentItem[0].style.width,height:this.currentItem[0].style.height,position:this.currentItem.css("position"),top:this.currentItem.css("top"),left:this.currentItem.css("left")}),(a[0].style.width==""||b.forceHelperSize)&&a.width(this.currentItem.width()),(a[0].style.height==""||b.forceHelperSize)&&a.height(this.currentItem.height()),a},_adjustOffsetFromHelper:function(a){typeof a=="string"&&(a=a.split(" ")),d.isArray(a)&&(a={left:+a[0],top:+a[1]||0}),"left"in a&&(this.offset.click.left=a.left+this.margins.left),"right"in a&&(this.offset.click.left=this.helperProportions.width-a.right+this.margins.left),"top"in a&&(this.offset.click.top=a.top+this.margins.top),"bottom"in a&&(this.offset.click.top=this.helperProportions.height-a.bottom+this.margins.top)},_getParentOffset:function(){this.offsetParent=this.helper.offsetParent();var a=this.offsetParent.offset();this.cssPosition=="absolute"&&this.scrollParent[0]!=document&&d.ui.contains(this.scrollParent[0],this.offsetParent[0])&&(a.left+=this.scrollParent.scrollLeft(),a.top+=this.scrollParent.scrollTop());if(this.offsetParent[0]==document.body||this.offsetParent[0].tagName&&this.offsetParent[0].tagName.toLowerCase()=="html"&&d.browser.msie)a={top:0,left:0};return{top:a.top+(parseInt(this.offsetParent.css("borderTopWidth"),10)||0),left:a.left+(parseInt(this.offsetParent.css("borderLeftWidth"),10)||0)}},_getRelativeOffset:function(){if(this.cssPosition=="relative"){var a=this.currentItem.position();return{top:a.top-(parseInt(this.helper.css("top"),10)||0)+this.scrollParent.scrollTop(),left:a.left-(parseInt(this.helper.css("left"),10)||0)+this.scrollParent.scrollLeft()}}return{top:0,left:0}},_cacheMargins:function(){this.margins={left:parseInt(this.currentItem.css("marginLeft"),10)||0,top:parseInt(this.currentItem.css("marginTop"),10)||0}},_cacheHelperProportions:function(){this.helperProportions={width:this.helper.outerWidth(),height:this.helper.outerHeight()}},_setContainment:function(){var a=this.options;a.containment=="parent"&&(a.containment=this.helper[0].parentNode);if(a.containment=="document"||a.containment=="window")this.containment=[0-this.offset.relative.left-this.offset.parent.left,0-this.offset.relative.top-this.offset.parent.top,d(a.containment=="document"?document:window).width()-this.helperProportions.width-this.margins.left,(d(a.containment=="document"?document:window).height()||document.body.parentNode.scrollHeight)-this.helperProportions.height-this.margins.top];if(!/^(document|window|parent)$/.test(a.containment)){var b=d(a.containment)[0];a=d(a.containment).offset();var c=d(b).css("overflow")!="hidden";this.containment=[a.left+(parseInt(d(b).css("borderLeftWidth"),10)||0)+(parseInt(d(b).css("paddingLeft"),10)||0)-this.margins.left,a.top+(parseInt(d(b).css("borderTopWidth"),10)||0)+(parseInt(d(b).css("paddingTop"),10)||0)-this.margins.top,a.left+(c?Math.max(b.scrollWidth,b.offsetWidth):b.offsetWidth)-(parseInt(d(b).css("borderLeftWidth"),10)||0)-(parseInt(d(b).css("paddingRight"),10)||0)-this.helperProportions.width-this.margins.left,a.top+(c?Math.max(b.scrollHeight,b.offsetHeight):b.offsetHeight)-(parseInt(d(b).css("borderTopWidth"),10)||0)-(parseInt(d(b).css("paddingBottom"),10)||0)-this.helperProportions.height-this.margins.top]}},_convertPositionTo:function(a,b){b||(b=this.position),a=a=="absolute"?1:-1;var c=this.cssPosition!="absolute"||this.scrollParent[0]!=document&&!!d.ui.contains(this.scrollParent[0],this.offsetParent[0])?this.scrollParent:this.offsetParent,e=/(html|body)/i.test(c[0].tagName);return{top:b.top+this.offset.relative.top*a+this.offset.parent.top*a-(d.browser.safari&&this.cssPosition=="fixed"?0:(this.cssPosition=="fixed"?-this.scrollParent.scrollTop():e?0:c.scrollTop())*a),left:b.left+this.offset.relative.left*a+this.offset.parent.left*a-(d.browser.safari&&this.cssPosition=="fixed"?0:(this.cssPosition=="fixed"?-this.scrollParent.scrollLeft():e?0:c.scrollLeft())*a)}},_generatePosition:function(a){var b=this.options,c=this.cssPosition!="absolute"||this.scrollParent[0]!=document&&!!d.ui.contains(this.scrollParent[0],this.offsetParent[0])?this.scrollParent:this.offsetParent,e=/(html|body)/i.test(c[0].tagName);this.cssPosition=="relative"&&(this.scrollParent[0]==document||this.scrollParent[0]==this.offsetParent[0])&&(this.offset.relative=this._getRelativeOffset());var f=a.pageX,g=a.pageY;return this.originalPosition&&(this.containment&&(a.pageX-this.offset.click.left<this.containment[0]&&(f=this.containment[0]+this.offset.click.left),a.pageY-this.offset.click.top<this.containment[1]&&(g=this.containment[1]+this.offset.click.top),a.pageX-this.offset.click.left>this.containment[2]&&(f=this.containment[2]+this.offset.click.left),a.pageY-this.offset.click.top>this.containment[3]&&(g=this.containment[3]+this.offset.click.top)),b.grid&&(g=this.originalPageY+Math.round((g-this.originalPageY)/b.grid[1])*b.grid[1],g=this.containment?g-this.offset.click.top<this.containment[1]||g-this.offset.click.top>this.containment[3]?g-this.offset.click.top<this.containment[1]?g+b.grid[1]:g-b.grid[1]:g:g,f=this.originalPageX+Math.round((f-this.originalPageX)/b.grid[0])*b.grid[0],f=this.containment?f-this.offset.click.left<this.containment[0]||f-this.offset.click.left>this.containment[2]?f-this.offset.click.left<this.containment[0]?f+b.grid[0]:f-b.grid[0]:f:f)),{top:g-this.offset.click.top-this.offset.relative.top-this.offset.parent.top+(d.browser.safari&&this.cssPosition=="fixed"?0:this.cssPosition=="fixed"?-this.scrollParent.scrollTop():e?0:c.scrollTop()),left:f-this.offset.click.left-this.offset.relative.left-this.offset.parent.left+(d.browser.safari&&this.cssPosition=="fixed"?0:this.cssPosition=="fixed"?-this.scrollParent.scrollLeft():e?0:c.scrollLeft())}},_rearrange:function(a,b,c,e){c?c[0].appendChild(this.placeholder[0]):b.item[0].parentNode.insertBefore(this.placeholder[0],this.direction=="down"?b.item[0]:b.item[0].nextSibling),this.counter=this.counter?++this.counter:1;var f=this,g=this.counter;window.setTimeout(function(){g==f.counter&&f.refreshPositions(!e)},0)},_clear:function(a,b){this.reverting=!1;var c=[];!this._noFinalSort&&this.currentItem.parent().length&&this.placeholder.before(this.currentItem),this._noFinalSort=null;if(this.helper[0]==this.currentItem[0]){for(var e in this._storedCSS)if(this._storedCSS[e]=="auto"||this._storedCSS[e]=="static")this._storedCSS[e]="";this.currentItem.css(this._storedCSS).removeClass("ui-sortable-helper")}else this.currentItem.show();this.fromOutside&&!b&&c.push(function(f){this._trigger("receive",f,this._uiHash(this.fromOutside))}),(this.fromOutside||this.domPosition.prev!=this.currentItem.prev().not(".ui-sortable-helper")[0]||this.domPosition.parent!=this.currentItem.parent()[0])&&!b&&c.push(function(f){this._trigger("update",f,this._uiHash())});if(!d.ui.contains(this.element[0],this.currentItem[0])){b||c.push(function(f){this._trigger("remove",f,this._uiHash())});for(e=this.containers.length-1;e>=0;e--)d.ui.contains(this.containers[e].element[0],this.currentItem[0])&&!b&&(c.push(function(f){return function(g){f._trigger("receive",g,this._uiHash(this))}}.call(this,this.containers[e])),c.push(function(f){return function(g){f._trigger("update",g,this._uiHash(this))}}.call(this,this.containers[e])))}for(e=this.containers.length-1;e>=0;e--)b||c.push(function(f){return function(g){f._trigger("deactivate",g,this._uiHash(this))}}.call(this,this.containers[e])),this.containers[e].containerCache.over&&(c.push(function(f){return function(g){f._trigger("out",g,this._uiHash(this))}}.call(this,this.containers[e])),this.containers[e].containerCache.over=0);this._storedCursor&&d("body").css("cursor",this._storedCursor),this._storedOpacity&&this.helper.css("opacity",this._storedOpacity),this._storedZIndex&&this.helper.css("zIndex",this._storedZIndex=="auto"?"":this._storedZIndex),this.dragging=!1;if(this.cancelHelperRemoval){if(!b){this._trigger("beforeStop",a,this._uiHash());for(e=0;e<c.length;e++)c[e].call(this,a);this._trigger("stop",a,this._uiHash())}return!1}b||this._trigger("beforeStop",a,this._uiHash()),this.placeholder[0].parentNode.removeChild(this.placeholder[0]),this.helper[0]!=this.currentItem[0]&&this.helper.remove(),this.helper=null;if(!b){for(e=0;e<c.length;e++)c[e].call(this,a);this._trigger("stop",a,this._uiHash())}return this.fromOutside=!1,!0},_trigger:function(){d.Widget.prototype._trigger.apply(this,arguments)===!1&&this.cancel()},_uiHash:function(a){var b=a||this;return{helper:b.helper,placeholder:b.placeholder||d([]),position:b.position,originalPosition:b.originalPosition,offset:b.positionAbs,item:b.currentItem,sender:a?a.element:null}}}),d.extend(d.ui.sortable,{version:"1.8.16"})}(jQuery),jQuery});
/*
 * jQuery Templating Plugin
 * Copyright 2010, John Resig
 * Dual licensed under the MIT or GPL Version 2 licenses.
 */

define('libs/jquery/jquery.tmpl',["libs/jquery/jquery"],function(jq){return function(jQuery,undefined){function newTmplItem(options,parentItem,fn,data){var newItem={data:data||(parentItem?parentItem.data:{}),_wrap:parentItem?parentItem._wrap:null,tmpl:null,parent:parentItem||null,nodes:[],calls:tiCalls,nest:tiNest,wrap:tiWrap,html:tiHtml,update:tiUpdate};return options&&jQuery.extend(newItem,options,{nodes:[],parent:parentItem}),fn&&(newItem.tmpl=fn,newItem._ctnt=newItem._ctnt||newItem.tmpl(jQuery,newItem),newItem.key=++itemKey,(stack.length?wrappedItems:newTmplItems)[itemKey]=newItem),newItem}function build(tmplItem,nested,content){var frag,ret=content?jQuery.map(content,function(item){return typeof item=="string"?tmplItem.key?item.replace(/(<\w+)(?=[\s>])(?![^>]*_tmplitem)([^>]*)/g,"$1 "+tmplItmAtt+'="'+tmplItem.key+'" $2'):item:build(item,tmplItem,item._ctnt)}):tmplItem;return nested?ret:(ret=ret.join(""),ret.replace(/^\s*([^<\s][^<]*)?(<[\w\W]+>)([^>]*[^>\s])?\s*$/,function(all,before,middle,after){frag=jQuery(middle).get(),storeTmplItems(frag),before&&(frag=unencode(before).concat(frag)),after&&(frag=frag.concat(unencode(after)))}),frag?frag:unencode(ret))}function unencode(text){var el=document.createElement("div");return el.innerHTML=text,jQuery.makeArray(el.childNodes)}function buildTmplFn(markup){return new Function("jQuery","$item","var $=jQuery,call,_=[],$data=$item.data;with($data){_.push('"+jQuery.trim(markup).replace(/([\\'])/g,"\\$1").replace(/[\r\t\n]/g," ").replace(/\$\{([^\}]*)\}/g,"{{= $1}}").replace(/\{\{(\/?)(\w+|.)(?:\(((?:[^\}]|\}(?!\}))*?)?\))?(?:\s+(.*?)?)?(\(((?:[^\}]|\}(?!\}))*?)\))?\s*\}\}/g,function(all,slash,type,fnargs,target,parens,args){var tag=jQuery.tmpl.tag[type],def,expr,exprAutoFnDetect;if(!tag)throw"Template command not found: "+type;return def=tag._default||[],parens&&!/\w$/.test(target)&&(target+=parens,parens=""),target?(target=unescape(target),args=args?","+unescape(args)+")":parens?")":"",expr=parens?target.indexOf(".")>-1?target+parens:"("+target+").call($item"+args:target,exprAutoFnDetect=parens?expr:"(typeof("+target+")==='function'?("+target+").call($item):("+target+"))"):exprAutoFnDetect=expr=def.$1||"null",fnargs=unescape(fnargs),"');"+tag[slash?"close":"open"].split("$notnull_1").join(target?"typeof("+target+")!=='undefined' && ("+target+")!=null":"true").split("$1a").join(exprAutoFnDetect).split("$1").join(expr).split("$2").join(fnargs?fnargs.replace(/\s*([^\(]+)\s*(\((.*?)\))?/g,function(all,name,parens,params){return params=params?","+params+")":parens?")":"",params?"("+name+").call($item"+params:all}):def.$2||"")+"_.push('"})+"');}return _;")}function updateWrapped(options,wrapped){options._wrap=build(options,!0,jQuery.isArray(wrapped)?wrapped:[htmlExpr.test(wrapped)?wrapped:jQuery(wrapped).html()]).join("")}function unescape(args){return args?args.replace(/\\'/g,"'").replace(/\\\\/g,"\\"):null}function outerHtml(elem){var div=document.createElement("div");return div.appendChild(elem.cloneNode(!0)),div.innerHTML}function storeTmplItems(content){function processItemKey(el){function cloneTmplItem(key){key+=keySuffix,tmplItem=newClonedItems[key]=newClonedItems[key]||newTmplItem(tmplItem,newTmplItems[tmplItem.parent.key+keySuffix]||tmplItem.parent,null,!0)}var pntKey,pntNode=el,pntItem,tmplItem,key;if(key=el.getAttribute(tmplItmAtt)){while(pntNode.parentNode&&(pntNode=pntNode.parentNode).nodeType===1&&!(pntKey=pntNode.getAttribute(tmplItmAtt)));pntKey!==key&&(pntNode=pntNode.parentNode?pntNode.nodeType===11?0:pntNode.getAttribute(tmplItmAtt)||0:0,(tmplItem=newTmplItems[key])||(tmplItem=wrappedItems[key],tmplItem=newTmplItem(tmplItem,newTmplItems[pntNode]||wrappedItems[pntNode],null,!0),tmplItem.key=++itemKey,newTmplItems[itemKey]=tmplItem),cloneIndex&&cloneTmplItem(key)),el.removeAttribute(tmplItmAtt)}else cloneIndex&&(tmplItem=jQuery.data(el,"tmplItem"))&&(cloneTmplItem(tmplItem.key),newTmplItems[tmplItem.key]=tmplItem,pntNode=jQuery.data(el.parentNode,"tmplItem"),pntNode=pntNode?pntNode.key:0);if(tmplItem){pntItem=tmplItem;while(pntItem&&pntItem.key!=pntNode)pntItem.nodes.push(el),pntItem=pntItem.parent;delete tmplItem._ctnt,delete tmplItem._wrap,jQuery.data(el,"tmplItem",tmplItem)}}var keySuffix="_"+cloneIndex,elem,elems,newClonedItems={},i,l,m;for(i=0,l=content.length;i<l;i++){if((elem=content[i]).nodeType!==1)continue;elems=elem.getElementsByTagName("*");for(m=elems.length-1;m>=0;m--)processItemKey(elems[m]);processItemKey(elem)}}function tiCalls(content,tmpl,data,options){if(!content)return stack.pop();stack.push({_:content,tmpl:tmpl,item:this,data:data,options:options})}function tiNest(tmpl,data,options){return jQuery.tmpl(jQuery.template(tmpl),data,options,this)}function tiWrap(call,wrapped){var options=call.options||{};return options.wrapped=wrapped,jQuery.tmpl(jQuery.template(call.tmpl),call.data,options,call.item)}function tiHtml(filter,textOnly){var wrapped=this._wrap;return jQuery.map(jQuery(jQuery.isArray(wrapped)?wrapped.join(""):wrapped).filter(filter||"*"),function(e){return textOnly?e.innerText||e.textContent:e.outerHTML||outerHtml(e)})}function tiUpdate(){var coll=this.nodes;jQuery.tmpl(null,null,null,this).insertBefore(coll[0]),jQuery(coll).remove()}var oldManip=jQuery.fn.domManip,tmplItmAtt="_tmplitem",htmlExpr=/^[^<]*(<[\w\W]+>)[^>]*$|\{\{\! /,newTmplItems={},wrappedItems={},appendToTmplItems,topTmplItem={key:0,data:{}},itemKey=0,cloneIndex=0,stack=[];jQuery.each({appendTo:"append",prependTo:"prepend",insertBefore:"before",insertAfter:"after",replaceAll:"replaceWith"},function(name,original){jQuery.fn[name]=function(selector){var ret=[],insert=jQuery(selector),elems,i,l,tmplItems,parent=this.length===1&&this[0].parentNode;appendToTmplItems=newTmplItems||{};if(parent&&parent.nodeType===11&&parent.childNodes.length===1&&insert.length===1)insert[original](this[0]),ret=this;else{for(i=0,l=insert.length;i<l;i++)cloneIndex=i,elems=(i>0?this.clone(!0):this).get(),jQuery.fn[original].apply(jQuery(insert[i]),elems),ret=ret.concat(elems);cloneIndex=0,ret=this.pushStack(ret,name,insert.selector)}return tmplItems=appendToTmplItems,appendToTmplItems=null,jQuery.tmpl.complete(tmplItems),ret}}),jQuery.fn.extend({tmpl:function(data,options,parentItem){return jQuery.tmpl(this[0],data,options,parentItem)},tmplItem:function(){return jQuery.tmplItem(this[0])},template:function(name){return jQuery.template(name,this[0])},domManip:function(args,table,callback,options){if(args[0]&&args[0].nodeType){var dmArgs=jQuery.makeArray(arguments),argsLength=args.length,i=0,tmplItem;while(i<argsLength&&!(tmplItem=jQuery.data(args[i++],"tmplItem")));argsLength>1&&(dmArgs[0]=[jQuery.makeArray(args)]),tmplItem&&cloneIndex&&(dmArgs[2]=function(fragClone){jQuery.tmpl.afterManip(this,fragClone,callback)}),oldManip.apply(this,dmArgs)}else oldManip.apply(this,arguments);return cloneIndex=0,appendToTmplItems||jQuery.tmpl.complete(newTmplItems),this}}),jQuery.extend({tmpl:function(tmpl,data,options,parentItem){var ret,topLevel=!parentItem;if(topLevel)parentItem=topTmplItem,tmpl=jQuery.template[tmpl]||jQuery.template(null,tmpl),wrappedItems={};else if(!tmpl)return tmpl=parentItem.tmpl,newTmplItems[parentItem.key]=parentItem,parentItem.nodes=[],parentItem.wrapped&&updateWrapped(parentItem,parentItem.wrapped),jQuery(build(parentItem,null,parentItem.tmpl(jQuery,parentItem)));return tmpl?(typeof data=="function"&&(data=data.call(parentItem||{})),options&&options.wrapped&&updateWrapped(options,options.wrapped),ret=jQuery.isArray(data)?jQuery.map(data,function(dataItem){return dataItem?newTmplItem(options,parentItem,tmpl,dataItem):null}):[newTmplItem(options,parentItem,tmpl,data)],topLevel?jQuery(build(parentItem,null,ret)):ret):[]},tmplItem:function(elem){var tmplItem;elem instanceof jQuery&&(elem=elem[0]);while(elem&&elem.nodeType===1&&!(tmplItem=jQuery.data(elem,"tmplItem"))&&(elem=elem.parentNode));return tmplItem||topTmplItem},template:function(name,tmpl){return tmpl?(typeof tmpl=="string"?tmpl=buildTmplFn(tmpl):tmpl instanceof jQuery&&(tmpl=tmpl[0]||{}),tmpl.nodeType&&(tmpl=jQuery.data(tmpl,"tmpl")||jQuery.data(tmpl,"tmpl",buildTmplFn(tmpl.innerHTML))),typeof name=="string"?jQuery.template[name]=tmpl:tmpl):name?typeof name!="string"?jQuery.template(null,name):jQuery.template[name]||jQuery.template(null,htmlExpr.test(name)?name:jQuery(name)):null},encode:function(text){return(""+text).split("<").join("&lt;").split(">").join("&gt;").split('"').join("&#34;").split("'").join("&#39;")}}),jQuery.extend(jQuery.tmpl,{tag:{tmpl:{_default:{$2:"null"},open:"if($notnull_1){_=_.concat($item.nest($1,$2));}"},wrap:{_default:{$2:"null"},open:"$item.calls(_,$1,$2);_=[];",close:"call=$item.calls();_=call._.concat($item.wrap(call,_));"},each:{_default:{$2:"$index, $value"},open:"if($notnull_1){$.each($1a,function($2){with(this){",close:"}});}"},"if":{open:"if(($notnull_1) && $1a){",close:"}"},"else":{_default:{$1:"true"},open:"}else if(($notnull_1) && $1a){"},html:{open:"if($notnull_1){_.push($1a);}"},"=":{_default:{$1:"$data"},open:"if($notnull_1){_.push($.encode($1a));}"},"!":{open:""}},complete:function(items){newTmplItems={}},afterManip:function(elem,fragClone,callback){var content=fragClone.nodeType===11?jQuery.makeArray(fragClone.childNodes):fragClone.nodeType===1?[fragClone]:[];callback.call(elem,fragClone),storeTmplItems(content),cloneIndex++}})}(jq),jq.fn.tmpl});
/*! Copyright (c) 2011 Brandon Aaron (http://brandonaaron.net)
 * Licensed under the MIT License (LICENSE.txt).
 *
 * Thanks to: http://adomas.org/javascript-mouse-wheel/ for some pointers.
 * Thanks to: Mathias Bank(http://www.mathias-bank.de) for a scope bug fix.
 * Thanks to: Seamus Leahy for adding deltaX and deltaY
 *
 * Version: 3.0.6
 * 
 * Requires: 1.2.2+
 */

define('libs/jquery/jquery.mousewheel',["libs/jquery/jquery.ui"],function(jQuery){(function($){function handler(event){var orgEvent=event||window.event,args=[].slice.call(arguments,1),delta=0,returnValue=!0,deltaX=0,deltaY=0;return event=$.event.fix(orgEvent),event.type="mousewheel",orgEvent.wheelDelta&&(delta=orgEvent.wheelDelta/120),orgEvent.detail&&(delta=-orgEvent.detail/3),deltaY=delta,orgEvent.axis!==undefined&&orgEvent.axis===orgEvent.HORIZONTAL_AXIS&&(deltaY=0,deltaX=-1*delta),orgEvent.wheelDeltaY!==undefined&&(deltaY=orgEvent.wheelDeltaY/120),orgEvent.wheelDeltaX!==undefined&&(deltaX=-1*orgEvent.wheelDeltaX/120),args.unshift(event,delta,deltaX,deltaY),($.event.dispatch||$.event.handle).apply(this,args)}var types=["DOMMouseScroll","mousewheel"];if($.event.fixHooks)for(var i=types.length;i;)$.event.fixHooks[types[--i]]=$.event.mouseHooks;$.event.special.mousewheel={setup:function(){if(this.addEventListener)for(var i=types.length;i;)this.addEventListener(types[--i],handler,!1);else this.onmousewheel=handler},teardown:function(){if(this.removeEventListener)for(var i=types.length;i;)this.removeEventListener(types[--i],handler,!1);else this.onmousewheel=null}},$.fn.extend({mousewheel:function(fn){return fn?this.bind("mousewheel",fn):this.trigger("mousewheel")},unmousewheel:function(fn){return this.unbind("mousewheel",fn)}})})(jQuery)});
/*
* jQuery timepicker addon
* By: Trent Richardson [http://trentrichardson.com]
* Version 0.9.7
* Last Modified: 10/02/2011
*
* Copyright 2011 Trent Richardson
* Dual licensed under the MIT and GPL licenses.
* http://trentrichardson.com/Impromptu/GPL-LICENSE.txt
* http://trentrichardson.com/Impromptu/MIT-LICENSE.txt
*
* HERES THE CSS:
* .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
* .ui-timepicker-div dl { text-align: left; }
* .ui-timepicker-div dl dt { height: 25px; }
* .ui-timepicker-div dl dd { margin: -25px 10px 10px 65px; }
* .ui-timepicker-div td { font-size: 90%; }
* .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
*/

define('libs/jquery/jquery.ui.timepicker',["libs/jquery/jquery.ui"],function($){(function($){function Timepicker(){this.regional=[],this.regional[""]={currentText:"Now",closeText:"Done",ampm:!1,amNames:["AM","A"],pmNames:["PM","P"],timeFormat:"hh:mm tt",timeSuffix:"",timeOnlyTitle:"Choose Time",timeText:"Time",hourText:"Hour",minuteText:"Minute",secondText:"Second",millisecText:"Millisecond",timezoneText:"Time Zone"},this._defaults={showButtonPanel:!0,timeOnly:!1,showHour:!0,showMinute:!0,showSecond:!1,showMillisec:!1,showTimezone:!1,showTime:!0,stepHour:.05,stepMinute:.05,stepSecond:.05,stepMillisec:.5,hour:0,minute:0,second:0,millisec:0,timezone:"+0000",hourMin:0,minuteMin:0,secondMin:0,millisecMin:0,hourMax:23,minuteMax:59,secondMax:59,millisecMax:999,minDateTime:null,maxDateTime:null,onSelect:null,hourGrid:0,minuteGrid:0,secondGrid:0,millisecGrid:0,alwaysSetTime:!0,separator:" ",altFieldTimeOnly:!0,showTimepicker:!0,timezoneIso8609:!1,timezoneList:null},$.extend(this._defaults,this.regional[""])}function extendRemove(target,props){$.extend(target,props);for(var name in props)if(props[name]===null||props[name]===undefined)target[name]=props[name];return target}$.extend($.ui,{timepicker:{version:"0.9.7"}}),$.extend(Timepicker.prototype,{$input:null,$altInput:null,$timeObj:null,inst:null,hour_slider:null,minute_slider:null,second_slider:null,millisec_slider:null,timezone_select:null,hour:0,minute:0,second:0,millisec:0,timezone:"+0000",hourMinOriginal:null,minuteMinOriginal:null,secondMinOriginal:null,millisecMinOriginal:null,hourMaxOriginal:null,minuteMaxOriginal:null,secondMaxOriginal:null,millisecMaxOriginal:null,ampm:"",formattedDate:"",formattedTime:"",formattedDateTime:"",timezoneList:null,setDefaults:function(settings){return extendRemove(this._defaults,settings||{}),this},_newInst:function($input,o){var tp_inst=new Timepicker,inlineSettings={};for(var attrName in this._defaults){var attrValue=$input.attr("time:"+attrName);if(attrValue)try{inlineSettings[attrName]=eval(attrValue)}catch(err){inlineSettings[attrName]=attrValue}}tp_inst._defaults=$.extend({},this._defaults,inlineSettings,o,{beforeShow:function(input,dp_inst){$.isFunction(o.beforeShow)&&o.beforeShow(input,dp_inst,tp_inst)},onChangeMonthYear:function(year,month,dp_inst){tp_inst._updateDateTime(dp_inst),$.isFunction(o.onChangeMonthYear)&&o.onChangeMonthYear.call($input[0],year,month,dp_inst,tp_inst)},onClose:function(dateText,dp_inst){tp_inst.timeDefined===!0&&$input.val()!=""&&tp_inst._updateDateTime(dp_inst),$.isFunction(o.onClose)&&o.onClose.call($input[0],dateText,dp_inst,tp_inst)},timepicker:tp_inst}),tp_inst.amNames=$.map(tp_inst._defaults.amNames,function(val){return val.toUpperCase()}),tp_inst.pmNames=$.map(tp_inst._defaults.pmNames,function(val){return val.toUpperCase()});if(tp_inst._defaults.timezoneList===null){var timezoneList=[];for(var i=-11;i<=12;i++)timezoneList.push((i>=0?"+":"-")+("0"+Math.abs(i).toString()).slice(-2)+"00");tp_inst._defaults.timezoneIso8609&&(timezoneList=$.map(timezoneList,function(val){return val=="+0000"?"Z":val.substring(0,3)+":"+val.substring(3)})),tp_inst._defaults.timezoneList=timezoneList}tp_inst.hour=tp_inst._defaults.hour,tp_inst.minute=tp_inst._defaults.minute,tp_inst.second=tp_inst._defaults.second,tp_inst.millisec=tp_inst._defaults.millisec,tp_inst.ampm="",tp_inst.$input=$input,o.altField&&(tp_inst.$altInput=$(o.altField).css({cursor:"pointer"}).focus(function(){$input.trigger("focus")}));if(tp_inst._defaults.minDate==0||tp_inst._defaults.minDateTime==0)tp_inst._defaults.minDate=new Date;if(tp_inst._defaults.maxDate==0||tp_inst._defaults.maxDateTime==0)tp_inst._defaults.maxDate=new Date;return tp_inst._defaults.minDate!==undefined&&tp_inst._defaults.minDate instanceof Date&&(tp_inst._defaults.minDateTime=new Date(tp_inst._defaults.minDate.getTime())),tp_inst._defaults.minDateTime!==undefined&&tp_inst._defaults.minDateTime instanceof Date&&(tp_inst._defaults.minDate=new Date(tp_inst._defaults.minDateTime.getTime())),tp_inst._defaults.maxDate!==undefined&&tp_inst._defaults.maxDate instanceof Date&&(tp_inst._defaults.maxDateTime=new Date(tp_inst._defaults.maxDate.getTime())),tp_inst._defaults.maxDateTime!==undefined&&tp_inst._defaults.maxDateTime instanceof Date&&(tp_inst._defaults.maxDate=new Date(tp_inst._defaults.maxDateTime.getTime())),tp_inst},_addTimePicker:function(dp_inst){var currDT=this.$altInput&&this._defaults.altFieldTimeOnly?this.$input.val()+" "+this.$altInput.val():this.$input.val();this.timeDefined=this._parseTime(currDT),this._limitMinMaxDateTime(dp_inst,!1),this._injectTimePicker()},_parseTime:function(timeString,withDate){var regstr=this._defaults.timeFormat.toString().replace(/h{1,2}/ig,"(\\d?\\d)").replace(/m{1,2}/ig,"(\\d?\\d)").replace(/s{1,2}/ig,"(\\d?\\d)").replace(/l{1}/ig,"(\\d?\\d?\\d)").replace(/t{1,2}/ig,this._getPatternAmpm()).replace(/z{1}/ig,"(z|[-+]\\d\\d:?\\d\\d)?").replace(/\s/g,"\\s?")+this._defaults.timeSuffix+"$",order=this._getFormatPositions(),ampm="",treg;this.inst||(this.inst=$.datepicker._getInst(this.$input[0]));if(withDate||!this._defaults.timeOnly){var dp_dateFormat=$.datepicker._get(this.inst,"dateFormat"),specials=new RegExp("[.*+?|()\\[\\]{}\\\\]","g");regstr=".{"+dp_dateFormat.length+",}"+this._defaults.separator.replace(specials,"\\$&")+regstr}treg=timeString.match(new RegExp(regstr,"i"));if(treg){order.t!==-1&&(treg[order.t]===undefined||treg[order.t].length===0?(ampm="",this.ampm=""):(ampm=$.inArray(treg[order.t].toUpperCase(),this.amNames)!==-1?"AM":"PM",this.ampm=this._defaults[ampm=="AM"?"amNames":"pmNames"][0])),order.h!==-1&&(ampm=="AM"&&treg[order.h]=="12"?this.hour=0:ampm=="PM"&&treg[order.h]!="12"?this.hour=(parseFloat(treg[order.h])+12).toFixed(0):this.hour=Number(treg[order.h])),order.m!==-1&&(this.minute=Number(treg[order.m])),order.s!==-1&&(this.second=Number(treg[order.s])),order.l!==-1&&(this.millisec=Number(treg[order.l]));if(order.z!==-1&&treg[order.z]!==undefined){var tz=treg[order.z].toUpperCase();switch(tz.length){case 1:tz=this._defaults.timezoneIso8609?"Z":"+0000";break;case 5:this._defaults.timezoneIso8609&&(tz=tz.substring(1)=="0000"?"Z":tz.substring(0,3)+":"+tz.substring(3));break;case 6:this._defaults.timezoneIso8609?tz.substring(1)=="00:00"&&(tz="Z"):tz=tz=="Z"||tz.substring(1)=="00:00"?"+0000":tz.replace(/:/,"")}this.timezone=tz}return!0}return!1},_getPatternAmpm:function(){var markers=[];return o=this._defaults,o.amNames&&$.merge(markers,o.amNames),o.pmNames&&$.merge(markers,o.pmNames),markers=$.map(markers,function(val){return val.replace(/[.*+?|()\[\]{}\\]/g,"\\$&")}),"("+markers.join("|")+")?"},_getFormatPositions:function(){var finds=this._defaults.timeFormat.toLowerCase().match(/(h{1,2}|m{1,2}|s{1,2}|l{1}|t{1,2}|z)/g),orders={h:-1,m:-1,s:-1,l:-1,t:-1,z:-1};if(finds)for(var i=0;i<finds.length;i++)orders[finds[i].toString().charAt(0)]==-1&&(orders[finds[i].toString().charAt(0)]=i+1);return orders},_injectTimePicker:function(){var $dp=this.inst.dpDiv,o=this._defaults,tp_inst=this,hourMax=(o.hourMax-(o.hourMax-o.hourMin)%o.stepHour).toFixed(0),minMax=(o.minuteMax-(o.minuteMax-o.minuteMin)%o.stepMinute).toFixed(0),secMax=(o.secondMax-(o.secondMax-o.secondMin)%o.stepSecond).toFixed(0),millisecMax=(o.millisecMax-(o.millisecMax-o.millisecMin)%o.stepMillisec).toFixed(0),dp_id=this.inst.id.toString().replace(/([^A-Za-z0-9_])/g,"");if($dp.find("div#ui-timepicker-div-"+dp_id).length===0&&o.showTimepicker){var noDisplay=' style="display:none;"',html='<div class="ui-timepicker-div" id="ui-timepicker-div-'+dp_id+'"><dl>'+'<dt class="ui_tpicker_time_label" id="ui_tpicker_time_label_'+dp_id+'"'+(o.showTime?"":noDisplay)+">"+o.timeText+"</dt>"+'<dd class="ui_tpicker_time" id="ui_tpicker_time_'+dp_id+'"'+(o.showTime?"":noDisplay)+"></dd>"+'<dt class="ui_tpicker_hour_label" id="ui_tpicker_hour_label_'+dp_id+'"'+(o.showHour?"":noDisplay)+">"+o.hourText+"</dt>",hourGridSize=0,minuteGridSize=0,secondGridSize=0,millisecGridSize=0,size;if(o.showHour&&o.hourGrid>0){html+='<dd class="ui_tpicker_hour"><div id="ui_tpicker_hour_'+dp_id+'"'+(o.showHour?"":noDisplay)+"></div>"+'<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';for(var h=o.hourMin;h<=hourMax;h+=parseInt(o.hourGrid,10)){hourGridSize++;var tmph=o.ampm&&h>12?h-12:h;tmph<10&&(tmph="0"+tmph),o.ampm&&(h==0?tmph="12a":h<12?tmph+="a":tmph+="p"),html+="<td>"+tmph+"</td>"}html+="</tr></table></div></dd>"}else html+='<dd class="ui_tpicker_hour" id="ui_tpicker_hour_'+dp_id+'"'+(o.showHour?"":noDisplay)+"></dd>";html+='<dt class="ui_tpicker_minute_label" id="ui_tpicker_minute_label_'+dp_id+'"'+(o.showMinute?"":noDisplay)+">"+o.minuteText+"</dt>";if(o.showMinute&&o.minuteGrid>0){html+='<dd class="ui_tpicker_minute ui_tpicker_minute_'+o.minuteGrid+'">'+'<div id="ui_tpicker_minute_'+dp_id+'"'+(o.showMinute?"":noDisplay)+"></div>"+'<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';for(var m=o.minuteMin;m<=minMax;m+=parseInt(o.minuteGrid,10))minuteGridSize++,html+="<td>"+(m<10?"0":"")+m+"</td>";html+="</tr></table></div></dd>"}else html+='<dd class="ui_tpicker_minute" id="ui_tpicker_minute_'+dp_id+'"'+(o.showMinute?"":noDisplay)+"></dd>";html+='<dt class="ui_tpicker_second_label" id="ui_tpicker_second_label_'+dp_id+'"'+(o.showSecond?"":noDisplay)+">"+o.secondText+"</dt>";if(o.showSecond&&o.secondGrid>0){html+='<dd class="ui_tpicker_second ui_tpicker_second_'+o.secondGrid+'">'+'<div id="ui_tpicker_second_'+dp_id+'"'+(o.showSecond?"":noDisplay)+"></div>"+'<div style="padding-left: 1px"><table><tr>';for(var s=o.secondMin;s<=secMax;s+=parseInt(o.secondGrid,10))secondGridSize++,html+="<td>"+(s<10?"0":"")+s+"</td>";html+="</tr></table></div></dd>"}else html+='<dd class="ui_tpicker_second" id="ui_tpicker_second_'+dp_id+'"'+(o.showSecond?"":noDisplay)+"></dd>";html+='<dt class="ui_tpicker_millisec_label" id="ui_tpicker_millisec_label_'+dp_id+'"'+(o.showMillisec?"":noDisplay)+">"+o.millisecText+"</dt>";if(o.showMillisec&&o.millisecGrid>0){html+='<dd class="ui_tpicker_millisec ui_tpicker_millisec_'+o.millisecGrid+'">'+'<div id="ui_tpicker_millisec_'+dp_id+'"'+(o.showMillisec?"":noDisplay)+"></div>"+'<div style="padding-left: 1px"><table><tr>';for(var l=o.millisecMin;l<=millisecMax;l+=parseInt(o.millisecGrid,10))millisecGridSize++,html+="<td>"+(l<10?"0":"")+s+"</td>";html+="</tr></table></div></dd>"}else html+='<dd class="ui_tpicker_millisec" id="ui_tpicker_millisec_'+dp_id+'"'+(o.showMillisec?"":noDisplay)+"></dd>";html+='<dt class="ui_tpicker_timezone_label" id="ui_tpicker_timezone_label_'+dp_id+'"'+(o.showTimezone?"":noDisplay)+">"+o.timezoneText+"</dt>",html+='<dd class="ui_tpicker_timezone" id="ui_tpicker_timezone_'+dp_id+'"'+(o.showTimezone?"":noDisplay)+"></dd>",html+="</dl></div>",$tp=$(html),o.timeOnly===!0&&($tp.prepend('<div class="ui-widget-header ui-helper-clearfix ui-corner-all"><div class="ui-datepicker-title">'+o.timeOnlyTitle+"</div>"+"</div>"),$dp.find(".ui-datepicker-header, .ui-datepicker-calendar").hide()),this.hour_slider=$tp.find("#ui_tpicker_hour_"+dp_id).slider({orientation:"horizontal",value:this.hour,min:o.hourMin,max:hourMax,step:o.stepHour,slide:function(event,ui){tp_inst.hour_slider.slider("option","value",ui.value),tp_inst._onTimeChange()}}),this.minute_slider=$tp.find("#ui_tpicker_minute_"+dp_id).slider({orientation:"horizontal",value:this.minute,min:o.minuteMin,max:minMax,step:o.stepMinute,slide:function(event,ui){tp_inst.minute_slider.slider("option","value",ui.value),tp_inst._onTimeChange()}}),this.second_slider=$tp.find("#ui_tpicker_second_"+dp_id).slider({orientation:"horizontal",value:this.second,min:o.secondMin,max:secMax,step:o.stepSecond,slide:function(event,ui){tp_inst.second_slider.slider("option","value",ui.value),tp_inst._onTimeChange()}}),this.millisec_slider=$tp.find("#ui_tpicker_millisec_"+dp_id).slider({orientation:"horizontal",value:this.millisec,min:o.millisecMin,max:millisecMax,step:o.stepMillisec,slide:function(event,ui){tp_inst.millisec_slider.slider("option","value",ui.value),tp_inst._onTimeChange()}}),this.timezone_select=$tp.find("#ui_tpicker_timezone_"+dp_id).append("<select></select>").find("select"),$.fn.append.apply(this.timezone_select,$.map(o.timezoneList,function(val,idx){return $("<option />").val(typeof val=="object"?val.value:val).text(typeof val=="object"?val.label:val)})),this.timezone_select.val(typeof this.timezone!="undefined"&&this.timezone!=null&&this.timezone!=""?this.timezone:o.timezone),this.timezone_select.change(function(){tp_inst._onTimeChange()}),o.showHour&&o.hourGrid>0&&(size=100*hourGridSize*o.hourGrid/(hourMax-o.hourMin),$tp.find(".ui_tpicker_hour table").css({width:size+"%",marginLeft:size/(-2*hourGridSize)+"%",borderCollapse:"collapse"}).find("td").each(function(index){$(this).click(function(){var h=$(this).html();if(o.ampm){var ap=h.substring(2).toLowerCase(),aph=parseInt(h.substring(0,2),10);ap=="a"?aph==12?h=0:h=aph:aph==12?h=12:h=aph+12}tp_inst.hour_slider.slider("option","value",h),tp_inst._onTimeChange(),tp_inst._onSelectHandler()}).css({cursor:"pointer",width:100/hourGridSize+"%",textAlign:"center",overflow:"hidden"})})),o.showMinute&&o.minuteGrid>0&&(size=100*minuteGridSize*o.minuteGrid/(minMax-o.minuteMin),$tp.find(".ui_tpicker_minute table").css({width:size+"%",marginLeft:size/(-2*minuteGridSize)+"%",borderCollapse:"collapse"}).find("td").each(function(index){$(this).click(function(){tp_inst.minute_slider.slider("option","value",$(this).html()),tp_inst._onTimeChange(),tp_inst._onSelectHandler()}).css({cursor:"pointer",width:100/minuteGridSize+"%",textAlign:"center",overflow:"hidden"})})),o.showSecond&&o.secondGrid>0&&$tp.find(".ui_tpicker_second table").css({width:size+"%",marginLeft:size/(-2*secondGridSize)+"%",borderCollapse:"collapse"}).find("td").each(function(index){$(this).click(function(){tp_inst.second_slider.slider("option","value",$(this).html()),tp_inst._onTimeChange(),tp_inst._onSelectHandler()}).css({cursor:"pointer",width:100/secondGridSize+"%",textAlign:"center",overflow:"hidden"})}),o.showMillisec&&o.millisecGrid>0&&$tp.find(".ui_tpicker_millisec table").css({width:size+"%",marginLeft:size/(-2*millisecGridSize)+"%",borderCollapse:"collapse"}).find("td").each(function(index){$(this).click(function(){tp_inst.millisec_slider.slider("option","value",$(this).html()),tp_inst._onTimeChange(),tp_inst._onSelectHandler()}).css({cursor:"pointer",width:100/millisecGridSize+"%",textAlign:"center",overflow:"hidden"})});var $buttonPanel=$dp.find(".ui-datepicker-buttonpane");$buttonPanel.length?$buttonPanel.before($tp):$dp.append($tp),this.$timeObj=$tp.find("#ui_tpicker_time_"+dp_id);if(this.inst!==null){var timeDefined=this.timeDefined;this._onTimeChange(),this.timeDefined=timeDefined}var onSelectDelegate=function(){tp_inst._onSelectHandler()};this.hour_slider.bind("slidestop",onSelectDelegate),this.minute_slider.bind("slidestop",onSelectDelegate),this.second_slider.bind("slidestop",onSelectDelegate),this.millisec_slider.bind("slidestop",onSelectDelegate)}},_limitMinMaxDateTime:function(dp_inst,adjustSliders){var o=this._defaults,dp_date=new Date(dp_inst.selectedYear,dp_inst.selectedMonth,dp_inst.selectedDay);if(!this._defaults.showTimepicker)return;if($.datepicker._get(dp_inst,"minDateTime")!==null&&$.datepicker._get(dp_inst,"minDateTime")!==undefined&&dp_date){var minDateTime=$.datepicker._get(dp_inst,"minDateTime"),minDateTimeDate=new Date(minDateTime.getFullYear(),minDateTime.getMonth(),minDateTime.getDate(),0,0,0,0);if(this.hourMinOriginal===null||this.minuteMinOriginal===null||this.secondMinOriginal===null||this.millisecMinOriginal===null)this.hourMinOriginal=o.hourMin,this.minuteMinOriginal=o.minuteMin,this.secondMinOriginal=o.secondMin,this.millisecMinOriginal=o.millisecMin;dp_inst.settings.timeOnly||minDateTimeDate.getTime()==dp_date.getTime()?(this._defaults.hourMin=minDateTime.getHours(),this.hour<=this._defaults.hourMin?(this.hour=this._defaults.hourMin,this._defaults.minuteMin=minDateTime.getMinutes(),this.minute<=this._defaults.minuteMin?(this.minute=this._defaults.minuteMin,this._defaults.secondMin=minDateTime.getSeconds()):this.second<=this._defaults.secondMin?(this.second=this._defaults.secondMin,this._defaults.millisecMin=minDateTime.getMilliseconds()):(this.millisec<this._defaults.millisecMin&&(this.millisec=this._defaults.millisecMin),this._defaults.millisecMin=this.millisecMinOriginal)):(this._defaults.minuteMin=this.minuteMinOriginal,this._defaults.secondMin=this.secondMinOriginal,this._defaults.millisecMin=this.millisecMinOriginal)):(this._defaults.hourMin=this.hourMinOriginal,this._defaults.minuteMin=this.minuteMinOriginal,this._defaults.secondMin=this.secondMinOriginal,this._defaults.millisecMin=this.millisecMinOriginal)}if($.datepicker._get(dp_inst,"maxDateTime")!==null&&$.datepicker._get(dp_inst,"maxDateTime")!==undefined&&dp_date){var maxDateTime=$.datepicker._get(dp_inst,"maxDateTime"),maxDateTimeDate=new Date(maxDateTime.getFullYear(),maxDateTime.getMonth(),maxDateTime.getDate(),0,0,0,0);if(this.hourMaxOriginal===null||this.minuteMaxOriginal===null||this.secondMaxOriginal===null)this.hourMaxOriginal=o.hourMax,this.minuteMaxOriginal=o.minuteMax,this.secondMaxOriginal=o.secondMax,this.millisecMaxOriginal=o.millisecMax;dp_inst.settings.timeOnly||maxDateTimeDate.getTime()==dp_date.getTime()?(this._defaults.hourMax=maxDateTime.getHours(),this.hour>=this._defaults.hourMax?(this.hour=this._defaults.hourMax,this._defaults.minuteMax=maxDateTime.getMinutes(),this.minute>=this._defaults.minuteMax?(this.minute=this._defaults.minuteMax,this._defaults.secondMax=maxDateTime.getSeconds()):this.second>=this._defaults.secondMax?(this.second=this._defaults.secondMax,this._defaults.millisecMax=maxDateTime.getMilliseconds()):(this.millisec>this._defaults.millisecMax&&(this.millisec=this._defaults.millisecMax),this._defaults.millisecMax=this.millisecMaxOriginal)):(this._defaults.minuteMax=this.minuteMaxOriginal,this._defaults.secondMax=this.secondMaxOriginal,this._defaults.millisecMax=this.millisecMaxOriginal)):(this._defaults.hourMax=this.hourMaxOriginal,this._defaults.minuteMax=this.minuteMaxOriginal,this._defaults.secondMax=this.secondMaxOriginal,this._defaults.millisecMax=this.millisecMaxOriginal)}if(adjustSliders!==undefined&&adjustSliders===!0){var hourMax=(this._defaults.hourMax-(this._defaults.hourMax-this._defaults.hourMin)%this._defaults.stepHour).toFixed(0),minMax=(this._defaults.minuteMax-(this._defaults.minuteMax-this._defaults.minuteMin)%this._defaults.stepMinute).toFixed(0),secMax=(this._defaults.secondMax-(this._defaults.secondMax-this._defaults.secondMin)%this._defaults.stepSecond).toFixed(0),millisecMax=(this._defaults.millisecMax-(this._defaults.millisecMax-this._defaults.millisecMin)%this._defaults.stepMillisec).toFixed(0);this.hour_slider&&this.hour_slider.slider("option",{min:this._defaults.hourMin,max:hourMax}).slider("value",this.hour),this.minute_slider&&this.minute_slider.slider("option",{min:this._defaults.minuteMin,max:minMax}).slider("value",this.minute),this.second_slider&&this.second_slider.slider("option",{min:this._defaults.secondMin,max:secMax}).slider("value",this.second),this.millisec_slider&&this.millisec_slider.slider("option",{min:this._defaults.millisecMin,max:millisecMax}).slider("value",this.millisec)}},_onTimeChange:function(){var hour=this.hour_slider?this.hour_slider.slider("value"):!1,minute=this.minute_slider?this.minute_slider.slider("value"):!1,second=this.second_slider?this.second_slider.slider("value"):!1,millisec=this.millisec_slider?this.millisec_slider.slider("value"):!1,timezone=this.timezone_select?this.timezone_select.val():!1,o=this._defaults;typeof hour=="object"&&(hour=!1),typeof minute=="object"&&(minute=!1),typeof second=="object"&&(second=!1),typeof millisec=="object"&&(millisec=!1),typeof timezone=="object"&&(timezone=!1),hour!==!1&&(hour=parseInt(hour,10)),minute!==!1&&(minute=parseInt(minute,10)),second!==!1&&(second=parseInt(second,10)),millisec!==!1&&(millisec=parseInt(millisec,10));var ampm=o[hour<12?"amNames":"pmNames"][0],hasChanged=hour!=this.hour||minute!=this.minute||second!=this.second||millisec!=this.millisec||this.ampm.length>0&&hour<12!=($.inArray(this.ampm.toUpperCase(),this.amNames)!==-1)||timezone!=this.timezone;hasChanged&&(hour!==!1&&(this.hour=hour),minute!==!1&&(this.minute=minute),second!==!1&&(this.second=second),millisec!==!1&&(this.millisec=millisec),timezone!==!1&&(this.timezone=timezone),this.inst||(this.inst=$.datepicker._getInst(this.$input[0])),this._limitMinMaxDateTime(this.inst,!0)),o.ampm&&(this.ampm=ampm),this._formatTime(),this.$timeObj&&this.$timeObj.text(this.formattedTime+o.timeSuffix),this.timeDefined=!0,hasChanged&&this._updateDateTime()},_onSelectHandler:function(){var onSelect=this._defaults.onSelect,inputEl=this.$input?this.$input[0]:null;onSelect&&inputEl&&onSelect.apply(inputEl,[this.formattedDateTime,this])},_formatTime:function(time,format,ampm){ampm==undefined&&(ampm=this._defaults.ampm),time=time||{hour:this.hour,minute:this.minute,second:this.second,millisec:this.millisec,ampm:this.ampm,timezone:this.timezone};var tmptime=(format||this._defaults.timeFormat).toString(),hour=parseInt(time.hour,10);ampm&&(!$.inArray(time.ampm.toUpperCase(),this.amNames)!==-1&&(hour%=12),hour===0&&(hour=12)),tmptime=tmptime.replace(/(?:hh?|mm?|ss?|[tT]{1,2}|[lz])/g,function(match){switch(match.toLowerCase()){case"hh":return("0"+hour).slice(-2);case"h":return hour;case"mm":return("0"+time.minute).slice(-2);case"m":return time.minute;case"ss":return("0"+time.second).slice(-2);case"s":return time.second;case"l":return("00"+time.millisec).slice(-3);case"z":return time.timezone;case"t":case"tt":if(ampm){var _ampm=time.ampm;return match.length==1&&(_ampm=_ampm.charAt(0)),match.charAt(0)=="T"?_ampm.toUpperCase():_ampm.toLowerCase()}return""}});if(arguments.length)return tmptime;this.formattedTime=tmptime},_updateDateTime:function(dp_inst){dp_inst=this.inst||dp_inst,dt=new Date(dp_inst.selectedYear,dp_inst.selectedMonth,dp_inst.selectedDay),dateFmt=$.datepicker._get(dp_inst,"dateFormat"),formatCfg=$.datepicker._getFormatConfig(dp_inst),timeAvailable=dt!==null&&this.timeDefined,this.formattedDate=$.datepicker.formatDate(dateFmt,dt===null?new Date:dt,formatCfg);var formattedDateTime=this.formattedDate;if(dp_inst.lastVal!==undefined&&dp_inst.lastVal.length>0&&this.$input.val().length===0)return;this._defaults.timeOnly===!0?formattedDateTime=this.formattedTime:this._defaults.timeOnly!==!0&&(this._defaults.alwaysSetTime||timeAvailable)&&(formattedDateTime+=this._defaults.separator+this.formattedTime+this._defaults.timeSuffix),this.formattedDateTime=formattedDateTime,this._defaults.showTimepicker?this.$altInput&&this._defaults.altFieldTimeOnly===!0?(this.$altInput.val(this.formattedTime),this.$input.val(this.formattedDate)):this.$altInput?(this.$altInput.val(formattedDateTime),this.$input.val(formattedDateTime)):this.$input.val(formattedDateTime):this.$input.val(this.formattedDate),this.$input.trigger("change")}}),$.fn.extend({timepicker:function(o){o=o||{};var tmp_args=arguments;return typeof o=="object"&&(tmp_args[0]=$.extend(o,{timeOnly:!0})),$(this).each(function(){$.fn.datetimepicker.apply($(this),tmp_args)})},datetimepicker:function(o){o=o||{};var $input=this,tmp_args=arguments;return typeof o=="string"?o=="getDate"?$.fn.datepicker.apply($(this[0]),tmp_args):this.each(function(){var $t=$(this);$t.datepicker.apply($t,tmp_args)}):this.each(function(){var $t=$(this);$t.datepicker($.timepicker._newInst($t,o)._defaults)})}}),$.datepicker._base_selectDate=$.datepicker._selectDate,$.datepicker._selectDate=function(id,dateStr){var inst=this._getInst($(id)[0]),tp_inst=this._get(inst,"timepicker");tp_inst?(tp_inst._limitMinMaxDateTime(inst,!0),inst.inline=inst.stay_open=!0,this._base_selectDate(id,dateStr),inst.inline=inst.stay_open=!1,this._notifyChange(inst),this._updateDatepicker(inst)):this._base_selectDate(id,dateStr)},$.datepicker._base_updateDatepicker=$.datepicker._updateDatepicker,$.datepicker._updateDatepicker=function(inst){var input=inst.input[0];if($.datepicker._curInst&&$.datepicker._curInst!=inst&&$.datepicker._datepickerShowing&&$.datepicker._lastInput!=input)return;if(typeof inst.stay_open!="boolean"||inst.stay_open===!1){this._base_updateDatepicker(inst);var tp_inst=this._get(inst,"timepicker");tp_inst&&tp_inst._addTimePicker(inst)}},$.datepicker._base_doKeyPress=$.datepicker._doKeyPress,$.datepicker._doKeyPress=function(event){var inst=$.datepicker._getInst(event.target),tp_inst=$.datepicker._get(inst,"timepicker");if(tp_inst&&$.datepicker._get(inst,"constrainInput")){var ampm=tp_inst._defaults.ampm,dateChars=$.datepicker._possibleChars($.datepicker._get(inst,"dateFormat")),datetimeChars=tp_inst._defaults.timeFormat.toString().replace(/[hms]/g,"").replace(/TT/g,ampm?"APM":"").replace(/Tt/g,ampm?"AaPpMm":"").replace(/tT/g,ampm?"AaPpMm":"").replace(/T/g,ampm?"AP":"").replace(/tt/g,ampm?"apm":"").replace(/t/g,ampm?"ap":"")+" "+tp_inst._defaults.separator+tp_inst._defaults.timeSuffix+(tp_inst._defaults.showTimezone?tp_inst._defaults.timezoneList.join(""):"")+tp_inst._defaults.amNames.join("")+tp_inst._defaults.pmNames.join("")+dateChars,chr=String.fromCharCode(event.charCode===undefined?event.keyCode:event.charCode);return event.ctrlKey||chr<" "||!dateChars||datetimeChars.indexOf(chr)>-1}return $.datepicker._base_doKeyPress(event)},$.datepicker._base_doKeyUp=$.datepicker._doKeyUp,$.datepicker._doKeyUp=function(event){var inst=$.datepicker._getInst(event.target),tp_inst=$.datepicker._get(inst,"timepicker");if(tp_inst&&tp_inst._defaults.timeOnly&&inst.input.val()!=inst.lastVal)try{$.datepicker._updateDatepicker(inst)}catch(err){$.datepicker.log(err)}return $.datepicker._base_doKeyUp(event)},$.datepicker._base_gotoToday=$.datepicker._gotoToday,$.datepicker._gotoToday=function(id){var inst=this._getInst($(id)[0]),$dp=inst.dpDiv;this._base_gotoToday(id);var now=new Date,tp_inst=this._get(inst,"timepicker");if(tp_inst._defaults.showTimezone&&tp_inst.timezone_select){var tzoffset=now.getTimezoneOffset(),tzsign=tzoffset>0?"-":"+";tzoffset=Math.abs(tzoffset);var tzmin=tzoffset%60;tzoffset=tzsign+("0"+(tzoffset-tzmin)/60).slice(-2)+("0"+tzmin).slice(-2),tp_inst._defaults.timezoneIso8609&&(tzoffset=tzoffset.substring(0,3)+":"+tzoffset.substring(3)),tp_inst.timezone_select.val(tzoffset)}this._setTime(inst,now),$(".ui-datepicker-today",$dp).click()},$.datepicker._disableTimepickerDatepicker=function(target,date,withDate){var inst=this._getInst(target),tp_inst=this._get(inst,"timepicker");$(target).datepicker("getDate"),tp_inst&&(tp_inst._defaults.showTimepicker=!1,tp_inst._updateDateTime(inst))},$.datepicker._enableTimepickerDatepicker=function(target,date,withDate){var inst=this._getInst(target),tp_inst=this._get(inst,"timepicker");$(target).datepicker("getDate"),tp_inst&&(tp_inst._defaults.showTimepicker=!0,tp_inst._addTimePicker(inst),tp_inst._updateDateTime(inst))},$.datepicker._setTime=function(inst,date){var tp_inst=this._get(inst,"timepicker");if(tp_inst){var defaults=tp_inst._defaults,hour=date?date.getHours():defaults.hour,minute=date?date.getMinutes():defaults.minute,second=date?date.getSeconds():defaults.second,millisec=date?date.getMilliseconds():defaults.millisec;if(hour<defaults.hourMin||hour>defaults.hourMax||minute<defaults.minuteMin||minute>defaults.minuteMax||second<defaults.secondMin||second>defaults.secondMax||millisec<defaults.millisecMin||millisec>defaults.millisecMax)hour=defaults.hourMin,minute=defaults.minuteMin,second=defaults.secondMin,millisec=defaults.millisecMin;tp_inst.hour=hour,tp_inst.minute=minute,tp_inst.second=second,tp_inst.millisec=millisec,tp_inst.hour_slider&&tp_inst.hour_slider.slider("value",hour),tp_inst.minute_slider&&tp_inst.minute_slider.slider("value",minute),tp_inst.second_slider&&tp_inst.second_slider.slider("value",second),tp_inst.millisec_slider&&tp_inst.millisec_slider.slider("value",millisec),tp_inst._onTimeChange(),tp_inst._updateDateTime(inst)}},$.datepicker._setTimeDatepicker=function(target,date,withDate){var inst=this._getInst(target),tp_inst=this._get(inst,"timepicker");if(tp_inst){this._setDateFromField(inst);var tp_date;date&&(typeof date=="string"?(tp_inst._parseTime(date,withDate),tp_date=new Date,tp_date.setHours(tp_inst.hour,tp_inst.minute,tp_inst.second,tp_inst.millisec)):tp_date=new Date(date.getTime()),tp_date.toString()=="Invalid Date"&&(tp_date=undefined),this._setTime(inst,tp_date))}},$.datepicker._base_setDateDatepicker=$.datepicker._setDateDatepicker,$.datepicker._setDateDatepicker=function(target,date){var inst=this._getInst(target),tp_date=date instanceof Date?new Date(date.getTime()):date;this._updateDatepicker(inst),this._base_setDateDatepicker.apply(this,arguments),this._setTimeDatepicker(target,tp_date,!0)},$.datepicker._base_getDateDatepicker=$.datepicker._getDateDatepicker,$.datepicker._getDateDatepicker=function(target,noDefault){var inst=this._getInst(target),tp_inst=this._get(inst,"timepicker");if(tp_inst){this._setDateFromField(inst,noDefault);var date=this._getDate(inst);return date&&tp_inst._parseTime($(target).val(),tp_inst.timeOnly)&&date.setHours(tp_inst.hour,tp_inst.minute,tp_inst.second,tp_inst.millisec),date}return this._base_getDateDatepicker(target,noDefault)},$.datepicker._base_parseDate=$.datepicker.parseDate,$.datepicker.parseDate=function(format,value,settings){var date;try{date=this._base_parseDate(format,value,settings)}catch(err){date=this._base_parseDate(format,value.substring(0,value.length-(err.length-err.indexOf(":")-2)),settings)}return date},$.datepicker._base_formatDate=$.datepicker._formatDate,$.datepicker._formatDate=function(inst,day,month,year){var tp_inst=this._get(inst,"timepicker");if(tp_inst){if(day)var b=this._base_formatDate(inst,day,month,year);return tp_inst._updateDateTime(),tp_inst.$input.val()}return this._base_formatDate(inst)},$.datepicker._base_optionDatepicker=$.datepicker._optionDatepicker,$.datepicker._optionDatepicker=function(target,name,value){var inst=this._getInst(target),tp_inst=this._get(inst,"timepicker");if(tp_inst){var min,max,onselect;typeof name=="string"?name==="minDate"||name==="minDateTime"?min=value:name==="maxDate"||name==="maxDateTime"?max=value:name==="onSelect"&&(onselect=value):typeof name=="object"&&(name.minDate?min=name.minDate:name.minDateTime?min=name.minDateTime:name.maxDate?max=name.maxDate:name.maxDateTime&&(max=name.maxDateTime)),min?(min==0?min=new Date:min=new Date(min),tp_inst._defaults.minDate=min,tp_inst._defaults.minDateTime=min):max?(max==0?max=new Date:max=new Date(max),tp_inst._defaults.maxDate=max,tp_inst._defaults.maxDateTime=max):onselect&&(tp_inst._defaults.onSelect=onselect)}this._base_optionDatepicker(target,name,value)},$.timepicker=new Timepicker,$.timepicker.version="0.9.7"})(jQuery)});
define('libs/jquery/jquery.ui.confirmation',["libs/jquery/jquery.ui"],function(jQuery){return function($,widgetName){$.widget("ui."+widgetName,{options:{message:"Question?",okLabel:"OK",cancelLabel:"Cancel"},_create:function(){if($.type(this.options.message)!="string")throw new TypeError('Invalid confirmation message type. A "string" expected.');this.element.addClass(widgetName+"Host"),this._$widget=this._bindBehaviour(this._createTemplate())},_createTemplate:function(){var $template=$('<div class="'+widgetName+'"/>'),$templateInner=$('<div class="inner"/>'),$message=$("<p/>").text(this.options.message),$ok=$('<button type="button" class="ok button mr-5 danger">').text(this.options.okLabel),$cancel=$('<button type="button" class="cancel button">').text(this.options.cancelLabel);return $template.append($templateInner.append($message,$ok,$cancel))},_bindBehaviour:function($template){return $template.click($.proxy(function(evt){evt.stopPropagation(),evt.preventDefault();var $target=$(evt.target);if($target.is(".ok"))return this._trigger("ok");if($target.is(".cancel"))return this._trigger("cancel")},this)),$template},show:function(){this._$widget.appendTo(this.element),this._trigger("show")},hideConfirmationMessage:function(){this._$widget.find(".inner").hide()},hide:function(){this._$widget.detach(),this._trigger("hide")},destroy:function(){$.Widget.prototype.destroy.apply(this,arguments),this._$widget.remove(),this.element.removeClass(widgetName+"Host")}})}(jQuery,"confirmation"),jQuery});
define('libs/jquery/jquery.notifyBar',["libs/jquery/jquery"],function($){var pluginName="notifyBar";$.fn[pluginName]=function(method){var methods={init:function(options){this[pluginName].settings=$.extend({},this[pluginName].defaults,options);var settings=this[pluginName].settings,$el;if(!this[pluginName].$el||this[pluginName].$el.length==0)$el=$("<div class='jquery-notify-bar'><div class='jquery-notify-bar-inners'></div></div>"),this[pluginName].$el=$el;return $el=this[pluginName].$el,$el.one("click",function(){var $el=$(this);$el.clearQueue().slideUp(settings.animationSpeed)}),this.each(function(){var $element=$(this),element=this;$el.parent().is($element)||$el.prependTo($element),$el.clearQueue(),$el.data("className")&&$el.removeClass($el.data("className")),$el.addClass(settings.className).data("className",settings.className);var $inners=$el.find(".jquery-notify-bar-inners");settings.$node?$inners.empty().append(settings.$node):$inners.html(settings.html),$el.is(":visible")||(settings.disableAutoClose?$el.slideDown(settings.animationSpeed):$el.slideDown(settings.animationSpeed).delay(settings.delay).slideUp(settings.animationSpeed))})},remove:function(){var $el=$.fn[pluginName].$el;$el&&($.fn[pluginName].$el=null,$el.remove())}};if(methods[method])return methods[method].apply(this,Array.prototype.slice.call(arguments,1));if(typeof method=="object"||!method)return methods.init.apply(this,arguments);$.error('Method "'+method+'" does not exist in pluginName plugin!')},$.fn[pluginName].defaults={className:"",$node:null,html:"",animationSpeed:"fast",delay:6e4,disableAutoClose:!1},$.fn[pluginName].settings={}});
/*
     * jQote2 - client-side Javascript templating engine
     * Copyright (C) 2010, aefxx
     * http://aefxx.com/
     *
     * Dual licensed under the WTFPL v2 or MIT (X11) licenses
     * WTFPL v2 Copyright (C) 2004, Sam Hocevar
     *
     * Date: Fri, May 4th, 2012
     * Version: 0.9.8
     */

define('libs/jqote2/jquery.jqote2',["libs/jquery/jquery"],function(jQuery){return function($){function raise(error,ext){throw $.extend(error,ext),error}function dotted_ns(fn){var ns=[];if(type_of.call(fn)!==ARR)return!1;for(var i=0,l=fn.length;i<l;i++)ns[i]=fn[i].jqote_id;return ns.length?ns.sort().join(".").replace(/(\b\d+\b)\.(?:\1(\.|$))+/g,"$1$2"):!1}function lambda(tmpl,t){var f,fn=[],t=t||tag,type=type_of.call(tmpl);if(type===FUNC)return tmpl.jqote_id?[tmpl]:!1;if(type!==ARR)return[$.jqotec(tmpl,t)];if(type===ARR)for(var i=0,l=tmpl.length;i<l;i++)(f=lambda(tmpl[i],t))&&fn.push(f[0]);return fn.length?fn:!1}var JQOTE2_TMPL_UNDEF_ERROR="UndefinedTemplateError",JQOTE2_TMPL_COMP_ERROR="TemplateCompilationError",JQOTE2_TMPL_EXEC_ERROR="TemplateExecutionError",ARR="[object Array]",STR="[object String]",FUNC="[object Function]",n=1,tag="%",qreg=/^[^<]*(<[\w\W]+>)[^>]*$/,type_of=Object.prototype.toString;$.fn.extend({jqote:function(data,t){var data=type_of.call(data)===ARR?data:[data],dom="";return this.each(function(i){var fn=$.jqotec(this,t);for(var j=0;j<data.length;j++)dom+=fn.call(data[j],i,j,data,fn)}),dom}}),$.each({app:"append",pre:"prepend",sub:"html"},function(name,method){$.fn["jqote"+name]=function(elem,data,t){var ns,regexp,str=$.jqote(elem,data,t),$$=qreg.test(str)?$:function(str){return $(document.createTextNode(str))};return!(ns=dotted_ns(lambda(elem)))||(regexp=new RegExp("(^|\\.)"+ns.split(".").join("\\.(.*)?")+"(\\.|$)")),this.each(function(){var dom=$$(str);$(this)[method](dom),(dom[0].nodeType===3?$(this):dom).trigger("jqote."+name,[dom,regexp])})}}),$.extend({jqote:function(elem,data,t){var str="",t=t||tag,fn=lambda(elem,t);fn===!1&&raise(new Error("Empty or undefined template passed to $.jqote"),{type:JQOTE2_TMPL_UNDEF_ERROR}),data=type_of.call(data)!==ARR?[data]:data;for(var i=0,l=fn.length;i<l;i++)for(var j=0;j<data.length;j++)str+=fn[i].call(data[j],i,j,data,fn[i]);return str},jqotec:function(template,t){var cache,elem,tmpl,t=t||tag,type=type_of.call(template);if(type===STR&&qreg.test(template)){elem=tmpl=template;if(cache=$.jqotecache[template])return cache}else{elem=type===STR||template.nodeType?$(template):template instanceof jQuery?template:null,(!elem[0]||!(tmpl=elem[0].innerHTML)&&!(tmpl=elem.text()))&&raise(new Error("Empty or undefined template passed to $.jqotec"),{type:JQOTE2_TMPL_UNDEF_ERROR});if(cache=$.jqotecache[$.data(elem[0],"jqote_id")])return cache}var str="",index,arr=tmpl.replace(/\s*<!\[CDATA\[\s*|\s*\]\]>\s*|[\r\n\t]/g,"").split("<"+t).join(t+">").split(t+">");for(var m=0,l=arr.length;m<l;m++)str+=arr[m].charAt(0)!==""?"out+='"+arr[m].replace(/(\\|["'])/g,"\\$1")+"'":arr[m].charAt(1)==="="?";out+=("+arr[m].substr(2)+");":arr[m].charAt(1)==="!"?";out+=$.jqotenc(("+arr[m].substr(2)+"));":";"+arr[m].substr(1);str="try{"+('var out="";'+str+";return out;").split("out+='';").join("").split('var out="";out+=').join("var out=")+'}catch(e){e.type="'+JQOTE2_TMPL_EXEC_ERROR+'";e.args=arguments;e.template=arguments.callee.toString();throw e;}';try{var fn=new Function("i, j, data, fn",str)}catch(e){raise(e,{type:JQOTE2_TMPL_COMP_ERROR})}return index=elem instanceof jQuery?$.data(elem[0],"jqote_id",n):elem,$.jqotecache[index]=(fn.jqote_id=n++,fn)},jqotefn:function(elem){var type=type_of.call(elem),index=type===STR&&qreg.test(elem)?elem:$.data($(elem)[0],"jqote_id");return $.jqotecache[index]||!1},jqotetag:function(str){type_of.call(str)===STR&&(tag=str)},jqotenc:function(str){return _.asString(str).replace(/&(?!\w+;)/g,"&#38;").split("<").join("&#60;").split(">").join("&#62;").split('"').join("&#34;").split("'").join("&#39;")},jqotecache:{}}),$.event.special.jqote={add:function(obj){var ns,handler=obj.handler,data=obj.data?type_of.call(obj.data)!==ARR?[obj.data]:obj.data:[];obj.namespace||(obj.namespace="app.pre.sub");if(!data.length||!(ns=dotted_ns(lambda(data))))return;obj.handler=function(event,dom,regexp){return!regexp||regexp.test(ns)?handler.apply(this,[event,dom]):null}}}}(jQuery),jQuery});
define('libs/jquery/jquery.ui.animateWithCss',["libs/jquery/jquery","libs/jquery/jquery.isSupport"],function(jQuery){return function($){$.fn.extend({animateWithCss:function(options){var defaults={cssClassName:"",complete:$.noop},o=$.extend(defaults,options),$defer=$.Deferred();return $defer.done(o.complete),this.each(function(){var $el=$(this);$el.removeClass(o.cssClassName).addClass(o.cssClassName);var timeout=parseFloat($el.css("MozAnimationDuration")||$el.css("WebkitAnimationDuration")||1,10)*parseFloat($el.css("MozAnimationIterationCount")||$el.css("WebkitAnimationIterationCount")||1,10)*1e3;_.delay(_.bind(function($d){this.removeClass(o.cssClassName),$d&&$d.resolve()},$el,$defer),timeout)})}})}(jQuery),jQuery});
define('libs/jquery/jquery.ui.sprite',["libs/jquery/jquery.ui"],function($){$.widget("tau.sprite",{options:{fps:25,autoplay:!1,width:null,height:null,frames:null,url:null},_create:function(){this.element.addClass(this._getClass()),this._initStyle=this.element.attr("style")},_init:function(){this.isPlaying()&&this.stop();var sheetUrl=this.options.url||this.element.css("backgroundImage");if(sheetUrl){try{this._initSheetData(sheetUrl)}catch(e){this.element.html(e.message)}this._initDimensions(),this._setFrame(0),this.options.autoplay&&this.play()}},_getClass:function(){return this.namespace+"-"+this.widgetName},_urlToVal:function(url,pattern,errorText){var patternMatch=url.match(pattern);if(!patternMatch)throw Error(errorText);var digitsMatch=patternMatch[0].match(/\d+/);if(!digitsMatch)throw Error(errorText);return parseInt(digitsMatch[0])},_parseFramesCount:function(url){return this._urlToVal(url,/f-\d+/,"Sprite frames count is not determined.")},_parseSheetWidth:function(url){return this._urlToVal(url,/w-\d+/,"Sprite sheet width is not determined.")},_parseSheetHeight:function(url){return this._urlToVal(url,/h-\d+/,"Sprite sheet height is not determined.")},_initSheetData:function(sheetUrl){this._sheetWidth=this.options.width||this._parseSheetWidth(sheetUrl),this._sheetHeight=this.options.height||this._parseSheetHeight(sheetUrl),this._framesCount=this.options.frames||this._parseFramesCount(sheetUrl),this._frameWidth=this._sheetWidth/this._framesCount,this._firstFrameIndex=0,this._lastFrameIndex=this._framesCount-1},_initDimensions:function(){this.element.css({width:this._frameWidth,height:this._sheetHeight})},_setFrame:function(frameIndex){var offset=-((this._currentFrameIndex=frameIndex)*this._frameWidth);this.element.css("backgroundPosition",offset+"px 0px")},_nextFrame:function(){var nextFrameIndex=this._currentFrameIndex+1;nextFrameIndex>this._lastFrameIndex&&(nextFrameIndex=this._firstFrameIndex),this._setFrame(nextFrameIndex)},isPlaying:function(){return!!this._playIntervalId},play:function(){this.isPlaying()||(this._playIntervalId=setInterval($.proxy(this,"_nextFrame"),1e3/this.options.fps))},stop:function(){this.isPlaying()&&(clearInterval(this._playIntervalId),this._playIntervalId=null)},destroy:function(){this.stop(),this._initStyle?this.element.attr("style",this._initStyle):this.element.removeAttr("style"),this.element.removeClass(this._getClass()),$.Widget.prototype.destroy.call(this)}})});
/*
 * jQuery UI Position @VERSION
 *
 * Copyright 2012, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Position
 */

define('libs/jquery/jquery.ui.position',["libs/jquery/jquery"],function(jq){(function($,undefined){$.ui=$.ui||{};var rhorizontal=/left|center|right/,rvertical=/top|center|bottom/,roffset=/[+-]\d+%?/,rposition=/^\w+/,rpercent=/%$/,center="center",_position=$.fn.position;$.position={scrollbarWidth:function(){var w1,w2,div=$("<div style='display:block;width:50px;height:50px;overflow:hidden;'><div style='height:100px;width:auto;'></div></div>"),innerDiv=div.children()[0];return $("body").append(div),w1=innerDiv.offsetWidth,div.css("overflow","scroll"),w2=innerDiv.offsetWidth,w1===w2&&(w2=div[0].clientWidth),div.remove(),w1-w2},getScrollInfo:function(within){var notWindow=within[0]!==window,overflowX=notWindow?within.css("overflow-x"):"",overflowY=notWindow?within.css("overflow-y"):"",scrollbarWidth=overflowX==="auto"||overflowX==="scroll"?$.position.scrollbarWidth():0,scrollbarHeight=overflowY==="auto"||overflowY==="scroll"?$.position.scrollbarWidth():0;return{height:within.height()<within[0].scrollHeight?scrollbarHeight:0,width:within.width()<within[0].scrollWidth?scrollbarWidth:0}}},$.fn.position=function(options){if(!options||!options.of)return _position.apply(this,arguments);options=$.extend({},options);var target=$(options.of),within=$(options.within||window),targetElem=target[0],collision=(options.collision||"flip").split(" "),offsets={},atOffset,targetWidth,targetHeight,basePosition;return targetElem.nodeType===9?(targetWidth=target.width(),targetHeight=target.height(),basePosition={top:0,left:0}):$.isWindow(targetElem)?(targetWidth=target.width(),targetHeight=target.height(),basePosition={top:target.scrollTop(),left:target.scrollLeft()}):targetElem.preventDefault?(options.at="left top",targetWidth=targetHeight=0,basePosition={top:options.of.pageY,left:options.of.pageX}):(targetWidth=target.outerWidth(),targetHeight=target.outerHeight(),basePosition=target.offset()),$.each(["my","at"],function(){var pos=(options[this]||"").split(" "),horizontalOffset,verticalOffset;pos.length===1&&(pos=rhorizontal.test(pos[0])?pos.concat([center]):rvertical.test(pos[0])?[center].concat(pos):[center,center]),pos[0]=rhorizontal.test(pos[0])?pos[0]:center,pos[1]=rvertical.test(pos[1])?pos[1]:center,horizontalOffset=roffset.exec(pos[0]),verticalOffset=roffset.exec(pos[1]),offsets[this]=[horizontalOffset?horizontalOffset[0]:0,verticalOffset?verticalOffset[0]:0],options[this]=[rposition.exec(pos[0])[0],rposition.exec(pos[1])[0]]}),collision.length===1&&(collision[1]=collision[0]),options.at[0]==="right"?basePosition.left+=targetWidth:options.at[0]===center&&(basePosition.left+=targetWidth/2),options.at[1]==="bottom"?basePosition.top+=targetHeight:options.at[1]===center&&(basePosition.top+=targetHeight/2),atOffset=[parseInt(offsets.at[0],10)*(rpercent.test(offsets.at[0])?targetWidth/100:1),parseInt(offsets.at[1],10)*(rpercent.test(offsets.at[1])?targetHeight/100:1)],basePosition.left+=atOffset[0],basePosition.top+=atOffset[1],this.each(function(){var elem=$(this),elemWidth=elem.outerWidth(),elemHeight=elem.outerHeight(),marginLeft=parseInt($.css(this,"marginLeft"))||0,marginTop=parseInt($.css(this,"marginTop"))||0,scrollInfo=$.position.getScrollInfo(within),collisionWidth=elemWidth+marginLeft+(parseInt($.css(this,"marginRight"))||0)+scrollInfo.width,collisionHeight=elemHeight+marginTop+(parseInt($.css(this,"marginBottom"))||0)+scrollInfo.height,position=$.extend({},basePosition),myOffset=[parseInt(offsets.my[0],10)*(rpercent.test(offsets.my[0])?elem.outerWidth()/100:1),parseInt(offsets.my[1],10)*(rpercent.test(offsets.my[1])?elem.outerHeight()/100:1)],collisionPosition;options.my[0]==="right"?position.left-=elemWidth:options.my[0]===center&&(position.left-=elemWidth/2),options.my[1]==="bottom"?position.top-=elemHeight:options.my[1]===center&&(position.top-=elemHeight/2),position.left+=myOffset[0],position.top+=myOffset[1],$.support.offsetFractions||(position.left=Math.round(position.left),position.top=Math.round(position.top)),collisionPosition={marginLeft:marginLeft,marginTop:marginTop},$.each(["left","top"],function(i,dir){$.ui.position[collision[i]]&&$.ui.position[collision[i]][dir](position,{targetWidth:targetWidth,targetHeight:targetHeight,elemWidth:elemWidth,elemHeight:elemHeight,collisionPosition:collisionPosition,collisionWidth:collisionWidth,collisionHeight:collisionHeight,offset:[atOffset[0]+myOffset[0],atOffset[1]+myOffset[1]],my:options.my,at:options.at,within:within,elem:elem})}),$.fn.bgiframe&&elem.bgiframe(),elem.offset($.extend(position,{using:options.using}))})},$.ui.position={fit:{left:function(position,data){var within=data.within,win=$(window),isWindow=$.isWindow(data.within[0]),withinOffset=isWindow?win.scrollLeft():within.offset().left,outerWidth=isWindow?win.width():within.outerWidth(),collisionPosLeft=position.left-data.collisionPosition.marginLeft,overLeft=withinOffset-collisionPosLeft,overRight=collisionPosLeft+data.collisionWidth-outerWidth-withinOffset,newOverRight,newOverLeft;data.collisionWidth>outerWidth?overLeft>0&&overRight<=0?(newOverRight=position.left+overLeft+data.collisionWidth-outerWidth-withinOffset,position.left+=overLeft-newOverRight):overRight>0&&overLeft<=0?position.left=withinOffset:overLeft>overRight?position.left=withinOffset+outerWidth-data.collisionWidth:position.left=withinOffset:overLeft>0?position.left+=overLeft:overRight>0?position.left-=overRight:position.left=Math.max(position.left-collisionPosLeft,position.left)},top:function(position,data){var within=data.within,win=$(window),isWindow=$.isWindow(data.within[0]),withinOffset=isWindow?win.scrollTop():within.offset().top,outerHeight=isWindow?win.height():within.outerHeight(),collisionPosTop=position.top-data.collisionPosition.marginTop,overTop=withinOffset-collisionPosTop,overBottom=collisionPosTop+data.collisionHeight-outerHeight-withinOffset,newOverTop,newOverBottom;data.collisionHeight>outerHeight?overTop>0&&overBottom<=0?(newOverBottom=position.top+overTop+data.collisionHeight-outerHeight-withinOffset,position.top+=overTop-newOverBottom):overBottom>0&&overTop<=0?position.top=withinOffset:overTop>overBottom?position.top=withinOffset+outerHeight-data.collisionHeight:position.top=withinOffset:overTop>0?position.top+=overTop:overBottom>0?position.top-=overBottom:position.top=Math.max(position.top-collisionPosTop,position.top)}},flip:{left:function(position,data){if(data.at[0]===center)return;data.elem.removeClass("ui-flipped-left ui-flipped-right");var within=data.within,win=$(window),isWindow=$.isWindow(data.within[0]),withinOffset=(isWindow?0:within.offset().left)+within.scrollLeft(),outerWidth=isWindow?within.width():within.outerWidth(),collisionPosLeft=position.left-data.collisionPosition.marginLeft,overLeft=collisionPosLeft-withinOffset,overRight=collisionPosLeft+data.collisionWidth-outerWidth-withinOffset,left=data.my[0]==="left",myOffset=data.my[0]==="left"?-data.elemWidth:data.my[0]==="right"?data.elemWidth:0,atOffset=data.at[0]==="left"?data.targetWidth:-data.targetWidth,offset=-2*data.offset[0],newOverRight,newOverLeft;if(overLeft<0){newOverRight=position.left+myOffset+atOffset+offset+data.collisionWidth-outerWidth-withinOffset;if(newOverRight<0||newOverRight<Math.abs(overLeft))data.elem.addClass("ui-flipped-right"),position.left+=myOffset+atOffset+offset}else if(overRight>0){newOverLeft=position.left-data.collisionPosition.marginLeft+myOffset+atOffset+offset-withinOffset;if(newOverLeft>0||Math.abs(newOverLeft)<overRight)data.elem.addClass("ui-flipped-left"),position.left+=myOffset+atOffset+offset}},top:function(position,data){if(data.at[1]===center)return;data.elem.removeClass("ui-flipped-top ui-flipped-bottom");var within=data.within,win=$(window),isWindow=$.isWindow(data.within[0]),withinOffset=(isWindow?0:within.offset().top)+within.scrollTop(),outerHeight=isWindow?within.height():within.outerHeight(),collisionPosTop=position.top-data.collisionPosition.marginTop,overTop=collisionPosTop-withinOffset,overBottom=collisionPosTop+data.collisionHeight-outerHeight-withinOffset,top=data.my[1]==="top",myOffset=top?-data.elemHeight:data.my[1]==="bottom"?data.elemHeight:0,atOffset=data.at[1]==="top"?data.targetHeight:-data.targetHeight,offset=-2*data.offset[1],newOverTop,newOverBottom;overTop<0?(newOverBottom=position.top+myOffset+atOffset+offset+data.collisionHeight-outerHeight-withinOffset,position.top+myOffset+atOffset+offset>overTop&&(newOverBottom<0||newOverBottom<Math.abs(overTop))&&(data.elem.addClass("ui-flipped-bottom"),position.top+=myOffset+atOffset+offset)):overBottom>0&&(newOverTop=position.top-data.collisionPosition.marginTop+myOffset+atOffset+offset-withinOffset,position.top+myOffset+atOffset+offset>overBottom&&(newOverTop>0||Math.abs(newOverTop)<overBottom)&&(data.elem.addClass("ui-flipped-top"),position.top+=myOffset+atOffset+offset))}},flipfit:{left:function(){$.ui.position.flip.left.apply(this,arguments),$.ui.position.fit.left.apply(this,arguments)},top:function(){$.ui.position.flip.top.apply(this,arguments),$.ui.position.fit.top.apply(this,arguments)}}},function(){var testElement,testElementParent,testElementStyle,offsetLeft,i,body=document.getElementsByTagName("body")[0],div=document.createElement("div");testElement=document.createElement(body?"div":"body"),testElementStyle={visibility:"hidden",width:0,height:0,border:0,margin:0,background:"none"},body&&$.extend(testElementStyle,{position:"absolute",left:"-1000px",top:"-1000px"});for(i in testElementStyle)testElement.style[i]=testElementStyle[i];testElement.appendChild(div),testElementParent=body||document.documentElement,testElementParent.insertBefore(testElement,testElementParent.firstChild),div.style.cssText="position: absolute; left: 10.7432222px;",offsetLeft=$(div).offset().left,$.support.offsetFractions=offsetLeft>10&&offsetLeft<11,testElement.innerHTML="",testElementParent.removeChild(testElement)}(),$.uiBackCompat!==!1&&function($){var _position=$.fn.position;$.fn.position=function(options){if(!options||!options.offset)return _position.call(this,options);var offset=options.offset.split(" "),at=options.at.split(" ");return offset.length===1&&(offset[1]=offset[0]),/^\d/.test(offset[0])&&(offset[0]="+"+offset[0]),/^\d/.test(offset[1])&&(offset[1]="+"+offset[1]),at.length===1&&(/left|center|right/.test(at[0])?at[1]="center":(at[1]=at[0],at[0]="center")),_position.call(this,$.extend(options,{at:at[0]+offset[0]+" "+at[1]+offset[1],offset:undefined}))}}(jQuery)})(jQuery)});
define('libs/jquery/jquery.ui.tauBubble',["libs/jquery/jquery.ui","libs/jquery/jquery.ui.position"],function($){(function(jQuery,window,document){var _bubblesCache={};$.widget("ui.tauBubble",{options:{application:null,within:null,appendTo:"body",alignTo:null,zIndex:null,onBeforeShow:$.noop,onShow:$.noop,onHide:$.noop,onPositionConfig:$.noop,onArrowPositionConfig:$.noop,onResize:$.noop,showOnCreation:!1,closeOnEscape:!0,mode:"bubble",showEvent:"click",hideEvent:"click",className:"",content:"",stackName:"default",delay:0,template:['<div class="tau-bubble">','<div class="tau-bubble__arrow"         role="arrow"></div>','<div class="tau-bubble__inner"         role="content"></div>',"</div>"].join(""),dontCloseSelectors:[]},_timeoutToken:null,toggle:function(){var self=this;!self.$popup||!self.$popup.is(":visible")?self.show():self.hide()},show:function(){if(this.options.disabled)return;var self=this;this._initInstance();var $popup=self.$popup,$target=self.$target,opts=self.options;opts.zIndex!=""&&(opts.zIndex=opts.zIndex||$target.zIndex()+1,$popup.zIndex(opts.zIndex)),$target.trigger("show.before",{});if(self.options.onBeforeShow.call(self,$popup)===!1)return;$popup.show(),self.adjustPosition(),self.adjustSize(),$popup.addClass("i-state_visible"),self.options.onShow.call(self,$popup),_bubblesCache[opts.stackName]=_bubblesCache[opts.stackName]||[];for(var i=0;i<_bubblesCache[opts.stackName].length;i++){var cacheItem=_bubblesCache[opts.stackName][i];cacheItem!=self&&(cacheItem.$target.trigger("externalClose",{}),cacheItem.hide())}self._signUpForCloseEvents(),self._windowResizeDelegate||(self._windowResizeDelegate=_.bind(function(){this.adjustPosition()},this)),$(window).bind("resize",self._windowResizeDelegate);var evtData={api:self,target:self.$target,popup:self.$popup};this._trigger("show"),self.$target.trigger("show",evtData),self.$popup.trigger("show",evtData)},hide:function(evt){var self=this,$popup=self.$popup;if(!$popup||!$popup.is(":visible"))return;$popup.removeClass("i-state_visible"),self.options.onHide.call(self,$popup),$popup.hide(),self._documentKeyDown&&($(document).unbind("keydown",self._documentKeyDown),delete self._documentKeyDown),self._documentClickDelegate&&($(document).unbind("click",self._documentClickDelegate),delete self._documentClickDelegate),self._windowResizeDelegate&&($(window).unbind("resize",self._windowResizeDelegate),delete self._windowResizeDelegate),self.$target.trigger("close",{event:evt}),this._trigger("hide")},_create:function(){var opts=this.options;this.$target=this.element,this.$alignTo=this._getAlignTo(),opts.showOnCreation==1&&this.show(),opts.showEvent==opts.hideEvent?this.$target.bind(opts.showEvent+".tauBubble",$.proxy(this._onToggleEvent,this)):(this.$target.bind(opts.showEvent+".tauBubble",$.proxy(opts.delay>0?this._onShowEventWithDelay:this._onShowEvent,this)),this.$target.bind(opts.hideEvent+".tauBubble",$.proxy(this._onHideEvent,this)))},_onShowEventWithDelay:function(evt){var that=this;this._clearTimeout(),this._timeoutToken=setTimeout(function(){that._onShowEvent(evt)},this.options.delay)},_clearTimeout:function(){this._timeoutToken&&clearTimeout(this._timeoutToken)},_onShowEvent:function(evt){var $target=$(evt.target);if(evt.type=="click"&&($target.is("a")||$target.parent().is("a")))return!0;var self=this;this.show()},_onHideEvent:function(evt){this._clearTimeout(),this.hide()},_onToggleEvent:function(evt){if(evt.isPropagationStopped())return;var $target=$(evt.target);if($target.is("a")||$target.parent().is("a"))return!0;evt.stopPropagation(),this.toggle()},_initInstance:function(e){var self=this,opts=this.options,$application=this.$target.parents(".i-role-application:first"),act="insertAfter";$application.length||($application=$("body"),act="appendTo"),opts.appendTo!=="body"&&($application=opts.appendTo,act="appendTo"),this.options.application=this.options.application||$application,this.options.within=null;var hide=$.proxy(this.hide,this),$popup=$(opts.template);if(this.options.showCloseButton){var close=$('<div class="tau-bubble__control-close" role="close"></div>');$popup.append(close),close.click(hide)}$popup[act]($application),this.$popup=$popup;var contentElement=this.$popup.addClass(opts.className).find("[role=content]");opts.content&&(opts.content.jquery?contentElement.append(opts.content):contentElement.html(opts.content)),self.$arrow=$popup.find("[role=arrow]"),_bubblesCache[opts.stackName]=_bubblesCache[opts.stackName]||[],_.include(_bubblesCache[opts.stackName],self)||_bubblesCache[opts.stackName].push(self),$popup.mouseenter(function(e){$popup.data("focus",!0)}),$popup.mouseleave(function(e){$popup.data("focus",!1)}),$application&&$application.scroll(function(){hide(),self.$target.trigger("externalClose",{})}),this._initInstance=function(){}},_onKeyDown:function(evt){if(evt.keyCode!=$.ui.keyCode.ESCAPE)return;this.$target.trigger("externalClose",{}),this.hide(evt)},_signUpForCloseEvents:function(){var self=this;self.options.closeOnEscape&&!self._documentKeyDown&&(self._documentKeyDown=function(ev){self._onKeyDown(ev)},$(document).keydown(self._documentKeyDown)),self._documentClickDelegate||(self._documentClickDelegate=function(ev){self._onDocumentClick(ev)},$(document).on("mousedown",self._documentClickDelegate))},widget:function(){return this.$popup||$()},adjustPosition:function(){var self=this,$of=self._getAlignTo(),$popup=self.$popup,$arrow=self.$arrow,positionConfig=this._getPositionConfig();positionConfig=self.options.onPositionConfig(positionConfig)||positionConfig,$popup.position(positionConfig);var linkPosition=$of[0].getBoundingClientRect(),popupPosition=$popup[0].getBoundingClientRect(),arrowPositionConfig={within:self.options.within,of:$of,collision:"fit flip",offset:"0 0"},currentOrientation="top";$arrow.attr("data-orientation","top"),Math.floor(popupPosition.top)+1<Math.floor(linkPosition.top)&&(currentOrientation="bottom"),popupPosition.right<=linkPosition.left+1?currentOrientation="right":Math.floor(popupPosition.left)>=Math.floor(linkPosition.right)&&(currentOrientation="left");var arrowConf={top:{my:"center top",at:"center bottom"},bottom:{my:"center bottom",at:"center top"},left:{my:"left center",at:"right center"},right:{my:"right center",at:"left center"}};popupPosition.width<=linkPosition.width/1.1&&(arrowConf.top={my:"center top",at:"left bottom",offset:"20 0"},arrowConf.bottom={my:"center bottom",at:"left top",offset:"20 0"}),arrowPositionConfig=_.defaults(arrowConf[currentOrientation],arrowPositionConfig),$arrow.attr("data-orientation",currentOrientation);var c="i-orientation_"+$popup.attr("data-orientaion");$popup.removeClass(c),$popup.addClass("i-orientaion_"+currentOrientation),$popup.attr("data-orientation",currentOrientation),arrowPositionConfig=self.options.onArrowPositionConfig(arrowPositionConfig)||arrowPositionConfig,$arrow.position(arrowPositionConfig)},adjustSize:function(){var self=this,$popup=this.$popup;if(!$popup)return;var $w=$("body")[0],windowRect={height:$w.clientHeight,width:$w.clientWidth},popupRect=$popup[0].getBoundingClientRect(),correctRect={height:windowRect.height-popupRect.top,width:windowRect.width-popupRect.left};self.options.onResize($popup,correctRect)},_getPositionConfig:function(){var self=this,$of=self._getAlignTo(),opts=this.options,positionConfig={within:opts.within,of:$of,collision:"fit flip"};switch(opts.mode){case"tooltip":$.extend(positionConfig,{my:"center top",at:"center bottom",offset:"0 0"});break;case"bubble":default:$.extend(positionConfig,{my:"left top",at:"left bottom",offset:"-30 0"})}return positionConfig},_onDocumentClick:function(ev){var self=this,$elementClick=$(ev.target),notClose=_.any(this.options.dontCloseSelectors,function(item){if($elementClick.closest(item).length)return!0});if(notClose)return!1;if(!self.$popup.is(":visible"))return;if(!self.$popup.is(":visible"))return;var $clicked=$elementClick.parents().andSelf();if($clicked.is(self.$target)||$clicked.is(self.$popup))return;if(self._justActivated){self._justActivated=!1;return}self._trigger("hide"),self.$target.trigger("externalClose",{}),self.hide()},_getAlignTo:function(){var $a=$(this.options.alignTo);return $a&&$a.length?$a:$(this.element)},activate:function(){this._justActivated=!0,this.show()},destroy:function(){var self=this,opts=self.options,$target=self.$target;self.$popup&&(self.hide(),self.$popup.remove(),self.$arrow.remove()),$target.unbind(opts.showEvent+".tauBubble"),$target.unbind(opts.hideEvent+".tauBubble"),_bubblesCache[opts.stackName]=_.without(_bubblesCache[opts.stackName],self)},empty:function(){var self=this;self.hide(),self.$popup.find("[role=content]").empty()},_setOption:function(key,value){key=="content"&&this.$popup&&(this.$popup.find("[role=content]").html(value),this.adjustPosition(),this.adjustSize()),$.Widget.prototype._setOption.call(this,key,value)}})})(jQuery,window,document)});
define('libs/jquery/jquery.textEditor',["libs/jquery/jquery.ui"],function(jq){var getCursorPos=function(editableDiv){var caretPos=0,containerEl=null,sel,range;if(window.getSelection)sel=window.getSelection(),sel.rangeCount&&(range=sel.getRangeAt(0),range.commonAncestorContainer.parentNode==editableDiv&&(caretPos=range.endOffset));else if(document.selection&&document.selection.createRange){range=document.selection.createRange();if(range.parentElement()==editableDiv){var tempEl=document.createElement("span");editableDiv.insertBefore(tempEl,editableDiv.firstChild);var tempRange=range.duplicate();tempRange.moveToElementText(tempEl),tempRange.setEndPoint("EndToEnd",range),caretPos=tempRange.text.length}}return caretPos},getSelectionPos=function(editableDiv){if(window.getSelection){var sel=window.getSelection();if(sel.rangeCount){var range=sel.getRangeAt(0);if(range.commonAncestorContainer.parentNode==editableDiv){var selection={start:range.startOffset,end:range.endOffset};return selection}if(range.commonAncestorContainer==editableDiv){var selection={start:0,end:range.toString().length};return selection}}}};return function($,pluginName){if(pluginName in $)throw new Error('jQuery already has the "'+pluginName+'" property.');var bindNamespacedEvents=function($element,eventMap){for(var eventType in eventMap)$element.bind(eventType+"."+pluginName,eventMap[eventType])},unbindNamespacedEvents=function($element,eventMap){for(var eventType in eventMap)$element.unbind(eventType+"."+pluginName,eventMap[eventType])},TextEditor=function($element,options){this.setOptions(options),this.initElement($element),this.enabled&&this.bindEvents(options)};TextEditor.prototype={constructor:TextEditor,initElement:function($element){this.$element=$element.addClass(this.className)},setOptions:function(options){var _options=options||{};this.onSave=$.isFunction(_options.onSave)?_options.onSave:$.noop,this.onEditStart=$.isFunction(_options.onEditStart)?_options.onEditStart:$.noop,this.onEditEnd=$.isFunction(_options.onEditEnd)?_options.onEditEnd:$.noop,this.maxLength=_options.maxLength||null,this.mask=_options.mask?new RegExp(_options.mask):!1,this.restoreText=typeof _options.restoreText=="undefined"?!0:_options.restoreText,this.enabled=options.enabled||!0,this.className=options.className||"editableText",this.classNameActive=options.className||"active"},enable:function(){this.enabled||(this.enabled=!0,this.bindEvents(this))},disable:function(){this.enabled&&(this.enabled=!1,this.deactivate(),this.unbindEvents(this))},bindEvents:function(options){options=options||{};var self=this;options.resetOnBlur&&(self.onBlur=function(){this._cancelEdit()}),bindNamespacedEvents(self.$element,{blur:$.proxy(self,"onBlur"),keydown:$.proxy(self,"mapKeys"),keypress:$.proxy(self,"mapKeys"),click:$.proxy(self,"onFocus")}),$.browser.msie||$(window).blur(function(){self.$element.blur()})},unbindEvents:function(options){options=options||{};var self=this;options.resetOnBlur&&(self.onBlur=function(){this._cancelEdit()}),unbindNamespacedEvents(self.$element,{blur:$.proxy(self,"onBlur"),keydown:$.proxy(self,"mapKeys"),keypress:$.proxy(self,"mapKeys"),click:$.proxy(self,"onFocus")})},onFocus:function(evt){evt&&evt.stopPropagation(),this.isActive()||(this.activate(),this.onEditStart(),this.storeInitialText())},_performSave:function(noCheck){if(!noCheck&&!this.isActive()){this.deactivate();return}this.deactivate(),this.saveText(),this.onEditEnd()},_cancelEdit:function(){if(!this.isActive())return;this.deactivate(),this.restoreInitialText(),this.onEditEnd()},onBlur:function(){this._performSave()},onEnter:function(event){event.preventDefault(),this._performSave(!0),this.$element.blur()},onEscape:function(event){event.preventDefault(),event.stopPropagation(),this._cancelEdit(),this.$element.blur()},onKeyDefault:function(event){var str=this._getValue(),k=event.which;if(event.ctrlKey||event.altKey||event.metaKey||k<32)return!0;var charValue=String.fromCharCode(k),pos=getCursorPos(this.$element[0]),strParts=[str.substring(0,pos),charValue,str.substring(pos)],newVal=strParts.join("");this.mask.test(newVal)||event.preventDefault()},activate:function(){this.$element.prop("contentEditable",!0).data("testEditorActive",!0).addClass(this.classNameActive).focus()},deactivate:function(){this.$element.prop("contentEditable",!1).data("testEditorActive",!1).removeClass(this.classNameActive)},isActive:function(){return this.$element.data("testEditorActive")||!1},setInitialText:function(text){this.initialText=text},getInitialText:function(){return this.initialText},storeInitialText:function(){this.setInitialText(this._getValue())},restoreInitialText:function(){this._setValue(this.getInitialText())},trimText:function(text){return _.trim(text)},normalizeText:function(text){var normalizedText=this.trimText(text);return normalizedText.length==0&&this.restoreText===!0&&(normalizedText=this.getInitialText()),normalizedText},saveText:function(){var changedText=this.normalizeText(this._getValue());this._setValue(changedText),changedText!=this.getInitialText()&&this.onSave(changedText)},mapKeys:function(event){switch(event.which){case 13:this.onEnter(event);break;case 27:this.onEscape(event);break;default:event.type=="keypress"&&this.mask&&this.onKeyDefault(event)}},_getValue:function(){return this.$element.is("input")?this.$element.val():this.$element.text()},_setValue:function(v){return this.$element.is("input")?this.$element.val(v):this.$element.text(v)}},$.fn[pluginName]=function(options){return this.each(function(index,element){var $element=$(element);$element.data(pluginName,new TextEditor($element,options))})},$[pluginName]=function(element,options){return $(element)[pluginName](options)}}(jq,"textEditor"),jq.fn.textEditor});
define('libs/jquery/jquery.ui.tauSortable',["libs/jquery/jquery.ui"],function(jQuery){(function($,undefined){$.widget("ui.tauSortable",{widgetEventPrefix:"tausortable",options:{groups:"",items:"> *",helper:function(){var $h=$(this).clone().removeAttr("id").wrap('<div class="tau-sortable__helper" />').parent();return $h},distance:50,scroll:!0,zIndex:9999,scrollSensitivity:100,cursor:"move",draggedClassName:"tau-sortable__placeholder",processToGroup:null,useSelectable:!1},dndOptions:{},_create:function(){var o=this.options,dndOptions={};$.extend(!0,dndOptions,this.options),this.dndOptions=dndOptions;var self=this;self.interval=function(){};var $selectable=!1;o.useSelectable&&($selectable=$(o.useSelectable));var globals=$.ui.tauSelectable;dndOptions.start=function(event,ui){globals.active=!0;var $dragged=$(event.target);self.currentGroup=$dragged.parents(o.groups).first();var handler=_.bind(self._updateLocation,self,$dragged,ui);self.interval=_.throttle(handler,100),ui.group=self.currentGroup,ui.item=event.target,ui.sender=self.element,ui&&ui.helper&&$.data(ui.helper,"helper",!0),$dragged.addClass(o.draggedClassName);var $all=$dragged;if($selectable){var $selected=$selectable.tauSelectable("getSelected"),$rest=$selected.not($dragged).not(":data(helper)");$rest.hide(),$all=$all.add($rest)}self._applyDndData($all,"source"),$all.length>1&&ui&&ui.helper&&(ui.helper.addClass("tau-sortable__helper_multiple_true"),ui.helper.children().attr("data-batch_count",_.asString($all.length))),ui.items=$all,self._trigger("start",event,ui)},dndOptions.drag=function(event,ui){self._updatePosition(event,ui),self.interval()},dndOptions.stop=function(event,ui){var $dragged=$(event.target);ui.item=event.target,ui.sender=self.element,self._updatePosition(event,ui),self._updateLocation($dragged,ui),ui.group=self.currentGroup,$dragged.removeClass(o.draggedClassName);var $all=$dragged;if($selectable){var $selected=$selectable.tauSelectable("getSelected"),$rest=$selected.not($dragged);$all=$all.add($rest),ui&&ui.helper&&(ui.helper.removeClass("tau-sortable__helper_multiple_true"),ui.helper.children().removeAttr("data-batch_count"));var i=0;$dragged.show();var _applyDndData=_.bind(self._applyDndData,self);_applyDndData($dragged,"target"),_applyDndData($dragged,"order",i++);var $lastDragged=$dragged;$rest.each(function(){var $card=$(this);$card.insertAfter($lastDragged),$lastDragged=$card,$card.show(),_applyDndData($card,"target"),_applyDndData($card,"order",i++)})}ui.items=$all,self._trigger("stop",event,ui),self._triggerToConnected("stop",event,ui),globals.active=!1};var $el=this.element;$el.addClass("tau-sortable"),jQuery.type(o.items)==="string"?$el.delegate(o.items,"mouseenter.tausortable",function(){globals.active||$(this).draggable(self.dndOptions)}):$(o.items).bind("mouseenter.tausortable",function(){globals.active||$(this).draggable(self.dndOptions)})},_within:function($el,pos){var el=$el[0],r=el.getBoundingClientRect(),rect={},c=getComputedStyle(el);return rect.left=r.left-(Math.round(parseFloat(c.marginLeft))+5),rect.right=r.right+(Math.round(parseFloat(c.marginRight))+5),rect.top=r.top-(Math.round(parseFloat(c.marginTop))+5),rect.bottom=r.bottom+(Math.round(parseFloat(c.marginBottom))+5),pos.x>=rect.left&&pos.x<=rect.right&&pos.y<=rect.bottom&&pos.y>=rect.top},_processToGroup:function($dragged,$fromGroup,$toGroup){var $ch=$toGroup.children();this.options.processToGroup?this.options.processToGroup.call(this,$dragged,$fromGroup,$toGroup):$ch.length?$ch.eq(0).append($dragged):$toGroup.append($dragged)},_updateCurrentGroup:function($dragged,ui){var $currentGroup=this.currentGroup;if(!this._within($currentGroup,this.cursorPosition)){var $newGroup=this._findNewGroup();if($newGroup.length){var nUi=$.extend(ui,{group:$currentGroup});this._trigger("groupleft",null,nUi),this._triggerToConnected("groupleft",null,nUi),this._processToGroup($dragged,$currentGroup,$newGroup),this.currentGroup=$newGroup,nUi=$.extend(ui,{group:$newGroup}),this._trigger("groupentered",null,nUi),this._triggerToConnected("groupentered",null,nUi),this._trigger("groupchanged",null,{item:$dragged,groupFrom:$currentGroup,groupTo:$newGroup}),this._triggerToConnected("groupchanged",null,{item:$dragged,groupFrom:$currentGroup,groupTo:$newGroup})}}},_updateLocation:function($el,ui){this._updateCurrentGroup($el,ui),this._updateSorting($el,ui)},_updateSorting:function($dragged,ui){var $item=this._findNewItem($dragged);$item.length>0?($dragged.index()<$item.index()?$dragged.insertAfter($item):$dragged.insertBefore($item),this._trigger("changed",null,ui),this._triggerToConnected("changed",null,ui)):this._within($dragged,this.cursorPosition)||(this._processToGroup($dragged,this.currentGroup,this.currentGroup),this._trigger("changed",null,ui),this._triggerToConnected("changed",null,ui))},_updatePosition:function(evt,ui){this.cursorPosition={x:evt.pageX,y:evt.pageY}},_findNewGroup:function(){var $groups=$(this.options.groups),pos=this.cursorPosition,$group=$();for(var i=0,n=$groups.length;i<n;i++)if(this._within($groups.eq(i),pos)){$group=$groups.eq(i);break}return $group},_findNewItem:function($dragged){var $items=this.currentGroup.find(this.options.items).not($dragged),pos=this.cursorPosition,$item=$();for(var i=0,n=$items.length;i<n;i++)if(this._within($items.eq(i),pos)){$item=$items.eq(i);break}return $item},_applyDndData:function($cards,name,val){$cards.each(function(){var $card=$(this),data=_.defaults($card.data("boardDnd")||{},{uid:_.uniqueId("boardDnd"),source:{},target:{},order:null});if(name=="source"||name=="target"){var $parentCell=$card.parent(),$prevCard=$card.prev(),$nextCard=$card.next(),cd=$card.data();data[name]={index:$card.index(),cellElementId:$parentCell.attr("id"),x:$parentCell.data("x")||null,y:$parentCell.data("y")||null,sliceId:cd.id,entityType:cd.entityType,entityId:cd.id,elementId:$card.attr("id"),beforeEntityId:$prevCard.data("id"),beforeElementId:$prevCard.attr("id"),afterEntityId:$nextCard.data("id"),afterElementId:$nextCard.attr("id")}}else data[name]=val;$card.data("boardDnd",data)})},_resetDndData:function($cards){$cards.each(function(){var $card=$(this),data=_.defaults({},{uid:_.uniqueId("boardDnd"),source:{},target:{},order:null});$card.data("boardDnd",data)})},_triggerToConnected:function(){var o=this.options,$el=this.element;if(o.connectedSortable){var $otherSortable=this.currentGroup.parents(":data(tauSortable)");if($otherSortable.is($el)==0){var d=$otherSortable.data("tauSortable");d._trigger.apply(d,_.toArray(arguments))}}},destroy:function(){var o=this.options,$items;jQuery.type(o.items)==="string"?($items=this.element.find(o.items+":data(draggable)"),this.element.undelegate(o.items,"mouseenter.tausortable")):($items=$(o.items).filter(":data(draggable)"),$(o.items).unbind("mouseenter.tausortable")),$items.draggable("destroy"),$.Widget.prototype.destroy.call(this)}}),$.extend($.ui.tauSelectable,{active:!1})})(jQuery)});
define('libs/jquery/jquery.ui.tauSelectable',["libs/jquery/jquery.ui"],function(jQuery){(function($,undefined){$.widget("ui.tauSelectable",{widgetEventPrefix:"tauselectable",options:{items:"> *",triggerSelector:null,className:"i-selected_true",dataName:"tauselectable",ctrl:!1,forceSelection:!1,selectOnMouseup:!1,fireEvents:!0},_create:function(){var o=this.options,$el=this.element,self=this,map={};if(o.ctrl){var isCtrl=function(evt){return evt.ctrlKey||evt.metaKey};map.mousedown=$.proxy(function(evt,item){var ctrl=isCtrl(evt),isSelected=this._getData(item,"selected"),$selected=this.getSelected(),$otherSelected=$selected.not(item),alreadySelected=$selected.length,isOnlySelected=isSelected&&alreadySelected==1;this._setData(item,"nextReset",!1),ctrl?(!o.forceSelection||!isOnlySelected)&&this._toggle(item):$otherSelected.length?isSelected?this._setData(item,"nextReset",!0):(this.reset($otherSelected),this._set(0,item)):o.forceSelection?this._set(0,item):this._toggle(item)},this),map.mouseup=$.proxy(function(evt,item){var $selected=this.getSelected(),$otherSelected=$selected.not(item);if(this._getData(item,"nextReset"))this.reset($otherSelected),this._set(0,item);else if(o.selectOnMouseup){var isSelected=!!this._getData(item,"selected");isSelected&&this.raiseChanged($selected,isSelected,!0)}this._setData(item,"nextReset",!1)},this)}else map.mousedown=$.proxy(function(evt,item){this._getData(item,"selected")?this._setData(item,"nextReset",!0):this._toggle(item)},this),map.mouseup=$.proxy(function(evt,item){this._getData(item,"selected")&&this._getData(item,"nextReset")&&(this._reset(0,item),this._setData(item,"nextReset",!1))},this);$el.delegate(o.items,"mousedown",function(evt){if(o.triggerSelector&&!$(evt.target).is(o.triggerSelector))return;map.mousedown(evt,this)}),$el.delegate(o.items,"mouseup",function(evt){if(o.triggerSelector&&!$(evt.target).is(o.triggerSelector))return;map.mouseup(evt,this)})},getSelected:function(){var o=this.options;return this.element.find(":data("+o.dataName+")").filter(function(evt){return $.data(this,o.dataName).selected})},select:function(cb){var o=this.options,$el=this.element.find(o.items);$el=$el.filter(cb),$el.length&&$el.each($.proxy(this._set,this))},unselect:function(cb){var o=this.options,$el=this.element.find(o.items).filter(cb);$el.length&&$el.each($.proxy(this._reset,this))},reset:function($cards,$except){$except=$except||$();var o=this.options,ce=o.fireEvents;o.fireEvents=!1;var $toReset=$();$cards?$toReset=$cards.not($except):$toReset=this.element.find(o.items).not($except),$toReset.each($.proxy(this._reset,this)),o.fireEvents=ce,o.fireEvents&&this.raiseChanged($toReset,!1)},raiseChanged:function($item,isSelected,mouseUp){mouseUp=mouseUp||!1;var raiseChangedOnlyForMouseup=isSelected&&this.options.selectOnMouseup;if(raiseChangedOnlyForMouseup&&!mouseUp)return;this._trigger("changed",{},{item:$item,selected:isSelected})},_toggle:function(el,isSelected){var o=this.options,$el=$(el),d=$el.data(o.dataName)||{},shouldBeSelected=arguments.length==2?!!isSelected:!d.selected;d.className&&d.className!=o.className&&$el.removeClass(d.className),$el.toggleClass(o.className,shouldBeSelected),d.selected=shouldBeSelected,d.className=o.className,shouldBeSelected||(d.nextReset=!1),$el.data(o.dataName,d),o.fireEvents&&this.raiseChanged($el,shouldBeSelected)},_reset:function(i,el){this._toggle(el,!1)},_resetForce:function(i,el){this._toggle(el,!1,!0)},_set:function(i,el){this._toggle(el,!0)},_setData:function(el,name,val){var dataName=this.options.dataName,d=$.data(el,dataName)||{};d[name]=val,$.data(el,dataName,d)},_getData:function(el,name){var dataName=this.options.dataName,d=$.data(el,dataName)||{};return d[name]}})})(jQuery)});
define('libs/jquery/jquery.ui.tauProgressIndicator',["libs/jquery/jquery.ui"],function($){var DATA_KEY="progress-indicator-originalPosition";$.widget("ui.tauProgressIndicator",{_create:function(){},show:function(options){options=options||{};var $placeholder=this.element,isNoLoader=$placeholder.children(".tau-loader").size()==0;if(isNoLoader){var originalPosition=$placeholder.css("position");!$placeholder.is("body")&&originalPosition.toLowerCase()==="static"&&$placeholder.data(DATA_KEY,originalPosition).css("position","relative");var $indicator=$("<div />").addClass("tau-loader").appendTo($placeholder);if(options.hover){var hoverCss={"background-color":"white",opacity:.5,"z-index":$indicator.css("z-index")-1,position:"absolute",top:0,left:0,height:"100%",width:"100%"};$("<div />").addClass("i-role-loader-hover").css(hoverCss).appendTo($placeholder)}var parentTagName=$placeholder.get(0).tagName||"";parentTagName.toLowerCase()!=="body",$indicator.sprite({autoplay:!0,url:!0,frames:10,width:560,height:56})}},hide:function(){var $placeholder=this.element;$placeholder.children(".tau-loader,.i-role-loader-hover").remove();var originalPosition=$placeholder.data(DATA_KEY);!$placeholder.is("body")&&originalPosition&&$placeholder.css("position",originalPosition)}})});
/**
 * jQuery.ScrollTo - Easy element scrolling using jQuery.
 * Copyright (c) 2007-2009 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
 * Dual licensed under MIT and GPL.
 * Date: 5/25/2009
 * @author Ariel Flesler
 * @version 1.4.2
 *
 * http://flesler.blogspot.com/2007/10/jqueryscrollto.html
 */

define('libs/jquery/jquery.scrollTo',["libs/jquery/jquery"],function(jQuery){return function(d){function p(a){return typeof a=="object"?a:{top:a,left:a}}var k=d.scrollTo=function(a,i,e){d(window).scrollTo(a,i,e)};k.defaults={axis:"xy",duration:parseFloat(d.fn.jquery)>=1.3?0:1},k.window=function(a){return d(window)._scrollable()},d.fn._scrollable=function(){return this.map(function(){var a=this,i=!a.nodeName||d.inArray(a.nodeName.toLowerCase(),["iframe","#document","html","body"])!=-1;if(!i)return a;var e=(a.contentWindow||a).document||a.ownerDocument||a;return d.browser.safari||e.compatMode=="BackCompat"?e.body:e.documentElement})},d.fn.scrollTo=function(n,j,b){return typeof j=="object"&&(b=j,j=0),typeof b=="function"&&(b={onAfter:b}),n=="max"&&(n=9e9),b=d.extend({},k.defaults,b),j=j||b.speed||b.duration,b.queue=b.queue&&b.axis.length>1,b.queue&&(j/=2),b.offset=p(b.offset),b.over=p(b.over),this._scrollable().each(function(){function t(a){r.animate(g,j,b.easing,a&&function(){a.call(this,n,b)})}var q=this,r=d(q),f=n,s,g={},u=r.is("html,body");switch(typeof f){case"number":case"string":if(/^([+-]=)?\d+(\.\d+)?(px|%)?$/.test(f)){f=p(f);break}f=d(f,this);case"object":if(f.is||f.style)s=(f=d(f)).offset()}d.each(b.axis.split(""),function(a,i){var e=i=="x"?"Left":"Top",h=e.toLowerCase(),c="scroll"+e,l=q[c],m=k.max(q,i);if(s)g[c]=s[h]+(u?0:l-r.offset()[h]),b.margin&&(g[c]-=parseInt(f.css("margin"+e))||0,g[c]-=parseInt(f.css("border"+e+"Width"))||0),g[c]+=b.offset[h]||0,b.over[h]&&(g[c]+=f[i=="x"?"width":"height"]()*b.over[h]);else{var o=f[h];g[c]=o.slice&&o.slice(-1)=="%"?parseFloat(o)/100*m:o}/^\d+$/.test(g[c])&&(g[c]=g[c]<=0?0:Math.min(g[c],m)),!a&&b.queue&&(l!=g[c]&&t(b.onAfterFirst),delete g[c])}),t(b.onAfter)}).end()},k.max=function(a,i){var e=i=="x"?"Width":"Height",h="scroll"+e;if(!d(a).is("html,body"))return a[h]-d(a)[e.toLowerCase()]();var c="client"+e,l=a.ownerDocument.documentElement,m=a.ownerDocument.body;return Math.max(l[h],m[h])-Math.min(l[c],m[c])}}(jQuery),jQuery});
define('libs/jquery/jquery.gid',["libs/jquery/jquery"],function(jQuery){if(!Object.create)return;(function($,moduleName){if(moduleName in $)throw new Error('The "'+moduleName+'" module is already defined.');var gidDetector={mouse:"onmousedown"in document,touch:"ontouchstart"in document},GIDObserver=function(options){this.__setOptions(options)};GIDObserver.prototype={constructor:GIDObserver,__setOptions:function(options){var _options=$.isPlainObject(options)?options:{};this.__options={dragStart:$.isFunction(_options.dragStart)?_options.dragStart:$.noop,drag:$.isFunction(_options.drag)?_options.drag:$.noop,dragEnd:$.isFunction(_options.dragEnd)?_options.dragEnd:$.noop}},_getOptions:function(){return this.__options},_isPointerInClientArea:function(pointer,element){var boundingRect=element.getBoundingClientRect(),areaLeft=boundingRect.left+element.clientLeft,areaTop=boundingRect.top+element.clientTop,areaRight=areaLeft+element.clientWidth,areaBottom=areaTop+element.clientHeight,inX=pointer.x>=areaLeft&&pointer.x<=areaRight,inY=pointer.y>=areaTop&&pointer.y<=areaBottom;return inX&&inY},_bind:function(eventName,handler){this.__$elements.on(eventName,this.__delegateSelector,handler)},_unbind:function(eventName,handler){this.__$elements.off(eventName,this.__delegateSelector,handler)},_isValidEvent:function(evt){return this.__isDirect?evt.currentTarget==evt.target:!0},_bindDragEventHandler:function(){},_unbindDragEventHandler:function(){},__bindEventHandlers:function(){this._bindDragEventHandler()},__unbindEventHandlers:function(){this._unbindDragEventHandler()},on:function($elements,delegateSelector,isDirect){return this.__enabled||(this.__enabled=!0,this.__delegateSelector=delegateSelector,this.__isDirect=isDirect,this.__$elements=$elements,this.__bindEventHandlers()),this},off:function(){return this.__enabled&&(this.__enabled=!1,this.__unbindEventHandlers(),this.__$elements=null),this}};if(gidDetector.mouse){var MouseObserver=function(options){GIDObserver.call(this,options)};MouseObserver.prototype=$.extend(Object.create(GIDObserver.prototype),{constructor:MouseObserver,__onDragStart:function(evt){if(this._isValidEvent(evt)){var pointer={x:evt.clientX,y:evt.clientY};this._isPointerInClientArea(pointer,evt.target)&&($("body").addClass("textSelectionDisabled"),$.browser.msie&&($("input:focus").blur(),evt.preventDefault()),this._getOptions().dragStart(evt,pointer),$(window).on({mousemove:$.proxy(this,"__onDrag"),mouseup:$.proxy(this,"__onDragEnd")}))}},__onDrag:function(evt){evt.preventDefault(),this._getOptions().drag(evt,{x:evt.clientX,y:evt.clientY})},__onDragEnd:function(evt){evt.preventDefault(),$("body").removeClass("textSelectionDisabled"),this._getOptions().dragEnd(evt,{x:evt.clientX,y:evt.clientY}),$(window).off({mousemove:this.__onDrag,mouseup:this.__onDragEnd})},_bindDragEventHandler:function(){this._bind("mousedown",$.proxy(this,"__onDragStart"))},_unbindDragEventHandler:function(){this._unbind("mousedown",this.__onDragStart)}})}if(gidDetector.touch){var TouchObserver=function(options){GIDObserver.call(this,options)};TouchObserver.prototype=$.extend(Object.create(GIDObserver.prototype),{constructor:TouchObserver,__onDragStart:function(evt){if(this._isValidEvent(evt)){var allTouches=evt.originalEvent.touches;if(allTouches.length==1){var theTouch=allTouches[0],pointer={x:theTouch.clientX,y:theTouch.clientY};this._isPointerInClientArea(pointer,evt.target)&&(evt.preventDefault(),this.__touchId=theTouch.identifier,this._getOptions().dragStart(evt,pointer),$(window).on({touchmove:$.proxy(this,"__onDrag"),touchend:$.proxy(this,"__onDragEnd")}))}}},__onDrag:function(evt){var theTouch=evt.originalEvent.changedTouches[0];theTouch.identifier==this.__touchId&&(evt.preventDefault(),this._getOptions().drag(evt,{x:theTouch.clientX,y:theTouch.clientY}))},__onDragEnd:function(evt){var theTouch=evt.originalEvent.changedTouches[0];theTouch.identifier==this.__touchId&&(evt.preventDefault(),this._getOptions().dragEnd(evt,{x:theTouch.clientX,y:theTouch.clientY}),$(window).off({touchmove:this.__onDrag,touchend:this.__onDragEnd}))},_bindDragEventHandler:function(){this._bind("touchstart",$.proxy(this,"__onDragStart"))},_unbindDragEventHandler:function(){this._unbind("touchstart",this.__onDragStart)}})}var gidObserverFactory={create:function(options){var gidObserver=null;return gidDetector.touch?gidObserver=new TouchObserver(options):gidDetector.mouse&&(gidObserver=new MouseObserver(options)),gidObserver}};$[moduleName]=function(options){return gidObserverFactory.create(options)}})(jQuery,"gid")});
define('libs/jquery/jquery.ui.pscroller',["libs/jquery/jquery.ui"],function(jQuery){(function($,namespace,moduleName){var SCROLL_REFRESH_INTERVAL=50,SCROLL_DETECTION_RATIO=.1,isElementOverflown=function($element,dimension){var e=$element[0],isXOverflown,isYOverflown;return dimension=="x"?isXOverflown=e.scrollWidth>e.clientWidth:dimension=="y"&&(isYOverflown=e.scrollHeight>e.clientHeight),isXOverflown||isYOverflown},getElementClientRect=function($element){var e=$element[0],rect=e.getBoundingClientRect(),w=e.clientWidth,h=e.clientHeight,l=rect.left+e.clientLeft,t=rect.top+e.clientTop,r=l+w,b=t+h;return{left:l,top:t,right:r,bottom:b,width:w,height:h}},squeezeRect=function(rect,ratio){var delta={x:rect.width*ratio,y:rect.height*ratio};return rect.left+=delta.x,rect.right-=delta.x,rect.top+=delta.y,rect.bottom-=delta.y,rect.width-=delta.x*2,rect.height-=delta.x*2,rect},Scroller=function(options){this.__options=options};Scroller.prototype={constructor:Scroller,__scrollLeft:function($element,offset){$element[0].scrollLeft-=offset},__scrollTop:function($element,offset){$element[0].scrollTop-=offset},__scrollRight:function($element,offset){$element[0].scrollLeft+=offset},__scrollDown:function($element,offset){$element[0].scrollTop+=offset},__scroll:function(){var point=this.__options.pointProvider();if(point){var $element=this.__$element,scrollDetectionRect=squeezeRect(getElementClientRect($element),this.__options.detectionRatio);isElementOverflown($element,"x")&&(point.x<scrollDetectionRect.left?this.__scrollLeft($element,scrollDetectionRect.left-point.x):point.x>scrollDetectionRect.right&&this.__scrollRight($element,point.x-scrollDetectionRect.right)),isElementOverflown($element,"y")&&(point.y<scrollDetectionRect.top?this.__scrollTop($element,scrollDetectionRect.top-point.y):point.y>scrollDetectionRect.bottom&&this.__scrollDown($element,point.y-scrollDetectionRect.bottom))}},on:function($element){this.__intervalId||(this.__$element=$element,this.__intervalId=setInterval($.proxy(this,"__scroll"),this.__options.refreshRate))},off:function(){this.__intervalId&&(clearInterval(this.__intervalId),this.__intervalId=null,this.__$element=null)}},$.widget(namespace+"."+moduleName,{options:{disabled:!0,refreshInterval:SCROLL_REFRESH_INTERVAL,detectionRatio:SCROLL_DETECTION_RATIO},__setPoint:function(point){this.__point=point},__getPoint:function(){return this.__point},__pointStart:function(evt,point){this.__setPoint(point)},__pointChange:function(evt,point){this.__setPoint(point)},__pointEnd:function(){this.__setPoint(null)},__on:function(){this.__scroller.on(this.element)},__off:function(){this.__scroller.off()},_create:function(){this.__scroller=new Scroller({refreshRate:this.options.refreshInterval,detectionRatio:this.options.detectionRatio,pointProvider:$.proxy(this,"__getPoint")}),this.__gidObserver=$.gid({dragStart:$.proxy(this,"__pointStart"),drag:$.proxy(this,"__pointChange"),dragEnd:$.proxy(this,"__pointEnd")}).on(this.element),this.options.disabled||this.__on()},_setOption:function(name,value){switch(name){case"disabled":this[value?"disable":"enable"]()}},enable:function(){this.options.disabled&&(this.options.disabled=!1,this.__on())},disable:function(){this.options.disabled||(this.options.disabled=!0,this.__off())},destroy:function(){this.__scroller.off(),this.__gidObserver.off(),this.__scroller=null,this.__gidObserver=null,$.Widget.prototype.destroy.call(this)}})})(jQuery,"ui","pscroller")});
define('libs/jquery/jquery.ui.rselect',["libs/jquery/jquery.ui"],function(jQuery){(function($,namespace,moduleName){var DEFAULT_SELECTION_CLASS="r-selected",SELECTION_AREA_CLASSNAME="rs-area",SELECTION_HOLDER_CLASSNAME="rs-holder",DRAG_INIT_RADIUS=10,SELECTABLE_SPLITTER=">>",EVENT_PREFIX="rs-",InputObserver=function(options){this.__options=options,this.__gidObserver=$.gid({dragStart:$.proxy(this,"__onDragStart"),drag:$.proxy(this,"__onDrag"),dragEnd:$.proxy(this,"__onDragEnd")})};InputObserver.prototype={constructor:InputObserver,__eventToKeyDescriptor:function(evt){var keyDescriptor={shift:evt.shiftKey,ctrl:evt.ctrlKey,meta:evt.metaKey,alt:evt.altKey,space:this.__keyDescriptor?this.__keyDescriptor.space:!1};if(evt.which==32)switch(evt.type){case"keydown":keyDescriptor.space=!0;break;case"keyup":keyDescriptor.space=!1}return keyDescriptor},__areKeyDescriptorsEqual:function(keyDescriptorA,keyDescriptorB){var equal=!0;for(var key in keyDescriptorA)if(keyDescriptorA[key]!=keyDescriptorB[key]){equal=!1;break}return equal},__anyActiveKeys:function(keyDescriptor){var activeKeys=!1;for(var key in keyDescriptor)if(keyDescriptor[key]){activeKeys=!0;break}return activeKeys},__updateMode:function(){var keyDescriptor=this.__keyDescriptor,rectMode="resize";keyDescriptor.space&&(rectMode="move");var addKeyActive=keyDescriptor.shift||keyDescriptor.ctrl||keyDescriptor.meta,subtractKeyActive=keyDescriptor.alt,selectionMode="new";addKeyActive&&!subtractKeyActive?selectionMode="add":subtractKeyActive&&!addKeyActive?selectionMode="subtract":addKeyActive&&subtractKeyActive&&(selectionMode="intersect"),this.__options.modeChange({rect:rectMode,selection:selectionMode})},__startSelection:function(){this.__options.selectionStart(this.__basePoint)},__showSelection:function(){this.__options.selectionShow(this.__basePoint,this.__guidePoint)},__updateSelection:function(){this.__options.selectionChange(this.__guidePoint)},__endSelection:function(){this.__options.selectionEnd(this.__guidePoint)},__bindKeyCapture:function(){$(window).on("keydown keyup",$.proxy(this,"__onKeyChange"))},__unbindKeyCapture:function(){$(window).off("keydown keyup",this.__onKeyChange)},__finalizeKeyCapture:function(evt){this.__anyActiveKeys(this.__keyDescriptor)||(this.__unbindKeyCapture(),this.__unbindKeyCaptureFinalizer())},__bindKeyCaptureFinalizer:function(){$(window).on("keyup",$.proxy(this,"__finalizeKeyCapture"))},__unbindKeyCaptureFinalizer:function(){$(window).off("keyup",this.__finalizeKeyCapture)},__onDragStart:function(evt,pointer){this.__basePoint=pointer,this.__startSelection(),this.__keyDescriptor=this.__eventToKeyDescriptor(evt),this.__bindKeyCapture(),this.__updateMode()},__onDrag:function(evt,pointer){if(!this.__$elements)return;this.__guidePoint=pointer;if(!this.__dragInitialized){var dragXDelta=Math.abs(this.__guidePoint.x-this.__basePoint.x),dragYDelta=Math.abs(this.__guidePoint.y-this.__basePoint.y);dragXDelta>DRAG_INIT_RADIUS&&dragYDelta>DRAG_INIT_RADIUS&&(this.__dragInitialized=!0,this.__showSelection(),this.__$elements.on("scroll",$.proxy(this,"__updateSelection")))}else this.__updateSelection()},__onDragEnd:function(evt,pointer){this.__guidePoint=pointer,this.__dragInitialized&&(this.__dragInitialized=!1,this.__$elements.off("scroll",this.__updateSelection)),this.__anyActiveKeys(this.__keyDescriptor)?this.__bindKeyCaptureFinalizer():this.__unbindKeyCapture(),this.__endSelection()},__onKeyChange:function(evt){evt.preventDefault();var newKeyDescriptor=this.__eventToKeyDescriptor(evt);this.__areKeyDescriptorsEqual(this.__keyDescriptor,newKeyDescriptor)||(this.__keyDescriptor=newKeyDescriptor,this.__updateMode())},on:function($elements,trigger){this.__$elements=$elements,this.__gidObserver.on($elements,trigger,trigger?!0:!1)},off:function(){this.__gidObserver.off(),this.__$elements=null}};var SelectionRect=function($placeholder){this.__$placeholder=$placeholder,this.__mode="resize"};SelectionRect.prototype={constructor:SelectionRect,__createElement:function(){var $element=$("<div/>").addClass(SELECTION_AREA_CLASSNAME);return getComputedStyle($element[0]),$element},__getElement:function(){return this.__$element||(this.__$element=this.__createElement()),this.__$element},__toPlaceholderPoint:function(viewportPoint){var placeholder=this.__$placeholder[0],placeholderRect=placeholder.getBoundingClientRect();return{x:viewportPoint.x-(placeholderRect.left+placeholder.clientLeft)+placeholder.scrollLeft,y:viewportPoint.y-(placeholderRect.top+placeholder.clientTop)+placeholder.scrollTop}},__clipValue:function(value,min,max){var v=value;return v<min?v=min:v>max&&(v=max),v},__clipRect:function(rect){var placeholder=this.__$placeholder[0],phWidth=placeholder.scrollWidth,phHeight=placeholder.scrollHeight,left=this.__clipValue(rect.left,0,phWidth),right=this.__clipValue(rect.left+rect.width,0,phWidth),top=this.__clipValue(rect.top,0,phHeight),bottom=this.__clipValue(rect.top+rect.height,0,phHeight);return{left:left,top:top,width:right-left,height:bottom-top}},__drawRect:function(rect){this.__getElement().css(this.__clipRect(rect))},__updateDimensions:function(guidePoint){var rect=this.__rect,basePoint=this.__basePoint;rect.left=Math.min(basePoint.x,guidePoint.x),rect.top=Math.min(basePoint.y,guidePoint.y),rect.width=Math.abs(guidePoint.x-basePoint.x),rect.height=Math.abs(guidePoint.y-basePoint.y),this.__drawRect(rect)},__updatePosition:function(guidePoint){var currentGuidePoint=this.__guidePoint,dx=guidePoint.x-currentGuidePoint.x,dy=guidePoint.y-currentGuidePoint.y,basePoint=this.__basePoint;basePoint.x+=dx,basePoint.y+=dy;var rect=this.__rect;rect.left+=dx,rect.top+=dy,this.__drawRect(this.__rect)},show:function(basePoint,guidePoint){this.__basePoint=this.__toPlaceholderPoint(basePoint),this.__rect={},this.__updateDimensions(this.__guidePoint=this.__toPlaceholderPoint(guidePoint)),this.__$placeholder.addClass(SELECTION_HOLDER_CLASSNAME).append(this.__getElement())},hide:function(){this.__getElement().detach(),this.__$placeholder.removeClass(SELECTION_HOLDER_CLASSNAME)},update:function(guidePoint){var phGuidePoint=this.__toPlaceholderPoint(guidePoint);switch(this.__mode){case"resize":this.__updateDimensions(phGuidePoint);break;case"move":this.__updatePosition(phGuidePoint)}this.__guidePoint=phGuidePoint},setMode:function(mode){this.__mode=mode},get:function(){return this.__getElement()[0].getBoundingClientRect()}};var selector={__isIntersect:function(rectA,rectB){var xIntersect=rectA.right>=rectB.left&&rectA.left<=rectB.right,yIntersect=rectA.bottom>=rectB.top&&rectA.top<=rectB.bottom;return xIntersect&&yIntersect},getRectSelection:function(selectionRect,$context,selectable){var $selection=$();if(selectionRect.width>0&&selectionRect.height>0){var selectableStack=selectable.split(SELECTABLE_SPLITTER).reverse(),self=this;$selection=$context;while(selectableStack.length>0)$selection=$selection.find(selectableStack.pop()).filter(function(){return self.__isIntersect(selectionRect,this.getBoundingClientRect())})}return $selection},__getNewSelection:function($oldSelection,$newSelection){return $newSelection},__getUnionSelection:function($oldSelection,$newSelection){return $oldSelection.add($newSelection)},__getSubtractSelection:function($oldSelection,$newSelection){return $oldSelection.not($newSelection)},__getIntersectSelection:function($oldSelection,$newSelection){return $oldSelection.add($newSelection).filter(function(){return $oldSelection.is(this)&&$newSelection.is(this)})},getActualSelection:function($oldSelection,$newSelection,mode){switch(mode){case"add":return this.__getUnionSelection($oldSelection,$newSelection);case"subtract":return this.__getSubtractSelection($oldSelection,$newSelection);case"intersect":return this.__getIntersectSelection($oldSelection,$newSelection);case"new":default:return this.__getNewSelection($oldSelection,$newSelection)}}};$.widget(namespace+"."+moduleName,{options:{disabled:!1,trigger:"",selectable:"",selectionClass:DEFAULT_SELECTION_CLASS,start:$.noop,show:$.noop,end:$.noop},__on:function(){this.__inputObserver.on(this.element,this.options.trigger)},__off:function(){this.__inputObserver.off()},__onSelectionStart:function(){if(!this.__isSelectionActive){this.__isSelectionActive=!0;var selectable=$.trim(this.options.selectable.replace(SELECTABLE_SPLITTER,""));this.__$oldSelection=this.element.find(selectable+"."+this.options.selectionClass),this._trigger("start",null,{selection:this.__$oldSelection})}},__onSelectionShow:function(basePoint,guidePoint){if(!this.__selectionRect)return;this.__selectionRect.show(basePoint,guidePoint),this._trigger("show")},__onSelectionChange:function(guidePoint){if(!this.__selectionRect)return;this.__selectionRect.update(guidePoint)},__onSelectionEnd:function(){if(this.__isSelectionActive){this.__isSelectionActive=!1;if(!this.__selectionRect)return;var selectionRect=this.__selectionRect.get();this.__selectionRect.hide();var $newSelection=selector.getRectSelection(selectionRect,this.element,this.options.selectable),$actualSelection=selector.getActualSelection(this.__$oldSelection,$newSelection,this.__selectionMode),$deselection=this.__$oldSelection.not($actualSelection);$deselection.removeClass(this.options.selectionClass).trigger(EVENT_PREFIX+"deselect"),$actualSelection.addClass(this.options.selectionClass).trigger(EVENT_PREFIX+"select"),this.__$oldSelection=null,this._trigger("end",null,{selection:$actualSelection,deselection:$deselection})}},__onModeChange:function(mode){this.__selectionMode=mode.selection,this.__selectionRect.setMode(mode.rect)},_create:function(){this.widgetEventPrefix=EVENT_PREFIX,this.__inputObserver=new InputObserver({selectionStart:$.proxy(this,"__onSelectionStart"),selectionShow:$.proxy(this,"__onSelectionShow"),selectionChange:$.proxy(this,"__onSelectionChange"),selectionEnd:$.proxy(this,"__onSelectionEnd"),modeChange:$.proxy(this,"__onModeChange")}),this.__selectionRect=new SelectionRect(this.element),this.options.disabled||this.__on()},_setOption:function(name,value){switch(name){case"disabled":this[value?"disable":"enable"]()}},enable:function(){this.options.disabled&&(this.options.disabled=!1,this.__on())},disable:function(){this.options.disabled||(this.options.disabled=!0,this.__off())},destroy:function(){this.__selectionRect.hide(),this.__inputObserver.off(),this.__selectionRect=null,this.__inputObserver=null,$.Widget.prototype.destroy.call(this)}})})(jQuery,"ui","rselect")});
/*
 * jQuery Easing v1.3 - http://gsgd.co.uk/sandbox/jquery/easing/
 *
 * Uses the built in easing capabilities added In jQuery 1.1
 * to offer multiple easing options
 *
 * TERMS OF USE - jQuery Easing
 * 
 * Open source under the BSD License. 
 * 
 * Copyright © 2008 George McGinley Smith
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of 
 * conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list 
 * of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * 
 * Neither the name of the author nor the names of contributors may be used to endorse 
 * or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 *  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 *  GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 *
*/

/*
 *
 * TERMS OF USE - EASING EQUATIONS
 * 
 * Open source under the BSD License. 
 * 
 * Copyright © 2001 Robert Penner
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of 
 * conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list 
 * of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * 
 * Neither the name of the author nor the names of contributors may be used to endorse 
 * or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 *  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 *  GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 *
 */

require(["libs/jquery/jquery"],function(){return jQuery.easing.jswing=jQuery.easing.swing,jQuery.extend(jQuery.easing,{def:"easeOutQuad",swing:function(x,t,b,c,d){return jQuery.easing[jQuery.easing.def](x,t,b,c,d)},easeInQuad:function(x,t,b,c,d){return c*(t/=d)*t+b},easeOutQuad:function(x,t,b,c,d){return-c*(t/=d)*(t-2)+b},easeInOutQuad:function(x,t,b,c,d){return(t/=d/2)<1?c/2*t*t+b:-c/2*(--t*(t-2)-1)+b},easeInCubic:function(x,t,b,c,d){return c*(t/=d)*t*t+b},easeOutCubic:function(x,t,b,c,d){return c*((t=t/d-1)*t*t+1)+b},easeInOutCubic:function(x,t,b,c,d){return(t/=d/2)<1?c/2*t*t*t+b:c/2*((t-=2)*t*t+2)+b},easeInQuart:function(x,t,b,c,d){return c*(t/=d)*t*t*t+b},easeOutQuart:function(x,t,b,c,d){return-c*((t=t/d-1)*t*t*t-1)+b},easeInOutQuart:function(x,t,b,c,d){return(t/=d/2)<1?c/2*t*t*t*t+b:-c/2*((t-=2)*t*t*t-2)+b},easeInQuint:function(x,t,b,c,d){return c*(t/=d)*t*t*t*t+b},easeOutQuint:function(x,t,b,c,d){return c*((t=t/d-1)*t*t*t*t+1)+b},easeInOutQuint:function(x,t,b,c,d){return(t/=d/2)<1?c/2*t*t*t*t*t+b:c/2*((t-=2)*t*t*t*t+2)+b},easeInSine:function(x,t,b,c,d){return-c*Math.cos(t/d*(Math.PI/2))+c+b},easeOutSine:function(x,t,b,c,d){return c*Math.sin(t/d*(Math.PI/2))+b},easeInOutSine:function(x,t,b,c,d){return-c/2*(Math.cos(Math.PI*t/d)-1)+b},easeInExpo:function(x,t,b,c,d){return t==0?b:c*Math.pow(2,10*(t/d-1))+b},easeOutExpo:function(x,t,b,c,d){return t==d?b+c:c*(-Math.pow(2,-10*t/d)+1)+b},easeInOutExpo:function(x,t,b,c,d){return t==0?b:t==d?b+c:(t/=d/2)<1?c/2*Math.pow(2,10*(t-1))+b:c/2*(-Math.pow(2,-10*--t)+2)+b},easeInCirc:function(x,t,b,c,d){return-c*(Math.sqrt(1-(t/=d)*t)-1)+b},easeOutCirc:function(x,t,b,c,d){return c*Math.sqrt(1-(t=t/d-1)*t)+b},easeInOutCirc:function(x,t,b,c,d){return(t/=d/2)<1?-c/2*(Math.sqrt(1-t*t)-1)+b:c/2*(Math.sqrt(1-(t-=2)*t)+1)+b},easeInElastic:function(x,t,b,c,d){var s=1.70158,p=0,a=c;if(t==0)return b;if((t/=d)==1)return b+c;p||(p=d*.3);if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return-(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*2*Math.PI/p))+b},easeOutElastic:function(x,t,b,c,d){var s=1.70158,p=0,a=c;if(t==0)return b;if((t/=d)==1)return b+c;p||(p=d*.3);if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return a*Math.pow(2,-10*t)*Math.sin((t*d-s)*2*Math.PI/p)+c+b},easeInOutElastic:function(x,t,b,c,d){var s=1.70158,p=0,a=c;if(t==0)return b;if((t/=d/2)==2)return b+c;p||(p=d*.3*1.5);if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return t<1?-0.5*a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*2*Math.PI/p)+b:a*Math.pow(2,-10*(t-=1))*Math.sin((t*d-s)*2*Math.PI/p)*.5+c+b},easeInBack:function(x,t,b,c,d,s){return s==undefined&&(s=1.70158),c*(t/=d)*t*((s+1)*t-s)+b},easeOutBack:function(x,t,b,c,d,s){return s==undefined&&(s=1.70158),c*((t=t/d-1)*t*((s+1)*t+s)+1)+b},easeInOutBack:function(x,t,b,c,d,s){return s==undefined&&(s=1.70158),(t/=d/2)<1?c/2*t*t*(((s*=1.525)+1)*t-s)+b:c/2*((t-=2)*t*(((s*=1.525)+1)*t+s)+2)+b},easeInBounce:function(x,t,b,c,d){return c-jQuery.easing.easeOutBounce(x,d-t,0,c,d)+b},easeOutBounce:function(x,t,b,c,d){return(t/=d)<1/2.75?c*7.5625*t*t+b:t<2/2.75?c*(7.5625*(t-=1.5/2.75)*t+.75)+b:t<2.5/2.75?c*(7.5625*(t-=2.25/2.75)*t+.9375)+b:c*(7.5625*(t-=2.625/2.75)*t+.984375)+b},easeInOutBounce:function(x,t,b,c,d){return t<d/2?jQuery.easing.easeInBounce(x,t*2,0,c,d)*.5+b:jQuery.easing.easeOutBounce(x,t*2-d,0,c,d)*.5+c*.5+b}}),jQuery.easing.jswing});
define("libs/jquery/jquery.easing.1.3", function(){});

/**
 * Rhinoslider 1.05
 * http://rhinoslider.com/
 *
 * Copyright 2012: Sebastian Pontow, Rene Maas (http://renemaas.de/)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://rhinoslider.com/license/
 */

define('libs/jquery/rhinoslider',["libs/jquery/jquery","libs/jquery/jquery.easing.1.3"],function(jQuery){(function($,window,undefined){$.extend($.easing,{def:"out",out:function(none,currentTime,startValue,endValue,totalTime){return-endValue*(currentTime/=totalTime)*(currentTime-2)+startValue},kick:function(none,currentTime,startValue,endValue,totalTime){return(currentTime/=totalTime/2)<1?endValue/2*Math.pow(2,10*(currentTime-1))+startValue:endValue/2*(-Math.pow(2,-10*--currentTime)+2)+startValue},shuffle:function(none,currentTime,startValue,endValue,totalTime){return(currentTime/=totalTime/2)<1?endValue/2*currentTime*currentTime*currentTime*currentTime*currentTime+startValue:endValue/2*((currentTime-=2)*currentTime*currentTime*currentTime*currentTime+2)+startValue}});var rhinoSlider=function(element,opts){var settings=$.extend({},$.fn.rhinoslider.defaults,opts),$slider=$(element),effects=$.fn.rhinoslider.effects,preparations=$.fn.rhinoslider.preparations,vars={isPlaying:!1,intervalAutoPlay:!1,active:"",next:"",container:"",items:"",buttons:[],prefix:"rhino-",playedArray:[],playedCounter:0,original:element};settings.callBeforeInit();var setUpSettings=function(settings){return settings.controlsPrevNext=String(settings.controlsPrevNext)=="true"?!0:!1,settings.controlsKeyboard=String(settings.controlsKeyboard)=="true"?!0:!1,settings.controlsMousewheel=String(settings.controlsMousewheel)=="true"?!0:!1,settings.controlsPlayPause=String(settings.controlsPlayPause)=="true"?!0:!1,settings.pauseOnHover=String(settings.pauseOnHover)=="true"?!0:!1,settings.animateActive=String(settings.animateActive)=="true"?!0:!1,settings.autoPlay=String(settings.autoPlay)=="true"?!0:!1,settings.cycled=String(settings.cycled)=="true"?!0:!1,settings.showTime=parseInt(settings.showTime,10),settings.effectTime=parseInt(settings.effectTime,10),settings.controlFadeTime=parseInt(settings.controlFadeTime,10),settings.captionsFadeTime=parseInt(settings.captionsFadeTime,10),tmpShiftValue=settings.shiftValue,tmpParts=settings.parts,settings.shiftValue=[],settings.parts=[],settings},init=function($slider,settings,vars){settings=setUpSettings(settings),$slider.wrap('<div class="'+vars.prefix+'container">'),vars.container=$slider.parent("."+vars.prefix+"container"),vars.isPlaying=settings.autoPlay;var buttons="";settings.controlsPrevNext&&(vars.container.addClass(vars.prefix+"controls-prev-next"),buttons='<a class="'+vars.prefix+"prev "+vars.prefix+'btn">'+settings.prevText+'</a><a class="'+vars.prefix+"next "+vars.prefix+'btn">'+settings.nextText+"</a>",vars.container.append(buttons),vars.buttons.prev=vars.container.find("."+vars.prefix+"prev"),vars.buttons.next=vars.container.find("."+vars.prefix+"next"),vars.buttons.prev.click(function(){prev($slider,settings),settings.autoPlay&&pause()}),vars.buttons.next.click(function(){next($slider,settings),settings.autoPlay&&pause()})),settings.controlsPlayPause&&(vars.container.addClass(vars.prefix+"controls-play-pause"),buttons=settings.autoPlay?'<a class="'+vars.prefix+"toggle "+vars.prefix+"pause "+vars.prefix+'btn">'+settings.pauseText+"</a>":'<a class="'+vars.prefix+"toggle "+vars.prefix+"play "+vars.prefix+'btn">'+settings.playText+"</a>",vars.container.append(buttons),vars.buttons.play=vars.container.find("."+vars.prefix+"toggle"),vars.buttons.play.click(function(){vars.isPlaying===!1?play():pause()})),vars.container.find("."+vars.prefix+"btn").css({position:"absolute",display:"block",cursor:"pointer"});if(settings.showControls!=="always"){var allControls=vars.container.find("."+vars.prefix+"btn");allControls.stop(!0,!0).fadeOut(0),settings.showControls==="hover"&&vars.container.mouseenter(function(){allControls.stop(!0,!0).fadeIn(settings.controlFadeTime)}).mouseleave(function(){allControls.delay(200).fadeOut(settings.controlFadeTime)})}settings.showControls!=="never"&&vars.container.addClass(vars.prefix+"show-controls"),vars.items=$slider.children(),vars.items.addClass(vars.prefix+"item"),vars.items.first().addClass(vars.prefix+"active");var sliderStyles=settings.styles.split(","),style;$.each(sliderStyles,function(i,cssAttribute){style=$.trim(cssAttribute),vars.container.css(style,$slider.css(style)),$slider.css(style," ");switch(style){case"width":case"height":$slider.css(style,"100%")}}),vars.container.css("position")=="static"&&vars.container.css("position","relative"),$slider.css({top:"auto",left:"auto",position:"relative"}),vars.items.css({margin:0,width:$slider.css("width"),height:$slider.css("height"),position:"absolute",top:0,left:0,zIndex:0,opacity:0,overflow:"hidden"}),vars.items.each(function(i){$(this).attr("id",vars.prefix+"item"+i)});if(settings.showBullets!=="never"){vars.container.addClass(vars.prefix+"show-bullets");var navi='<ol class="'+vars.prefix+'bullets">';vars.items.each(function(i){var $item=$(this),id=vars.prefix+"item"+i;navi=navi+'<li><a id="'+id+'-bullet" class="'+vars.prefix+'bullet">'+parseInt(i+1,10)+"</a></li>"}),navi+="</ol>",vars.container.append(navi),vars.navigation=vars.container.find("."+vars.prefix+"bullets"),vars.buttons.bullets=vars.navigation.find("."+vars.prefix+"bullet"),vars.buttons.bullets.first().addClass(vars.prefix+"active-bullet "+vars.prefix+"first-bullet"),vars.buttons.bullets.last().addClass(vars.prefix+"last-bullet"),vars.buttons.bullets.click(function(){var itemID=$(this).attr("id").replace("-bullet",""),$next=vars.container.find("#"+itemID),curID=parseInt(vars.navigation.find("."+vars.prefix+"active-bullet").attr("id").replace("-bullet","").replace(vars.prefix+"item",""),10),nextID=parseInt(itemID.replace(vars.prefix+"item",""),10);if(curID<nextID)next($slider,settings,$next);else{if(!(curID>nextID))return!1;prev($slider,settings,$next)}settings.autoPlay&&pause()})}settings.showBullets==="hover"&&(vars.navigation.hide(),vars.container.mouseenter(function(){vars.navigation.stop(!0,!0).fadeIn(settings.controlFadeTime)}).mouseleave(function(){vars.navigation.delay(200).fadeOut(settings.controlFadeTime)})),settings.showCaptions!=="never"&&(vars.container.addClass(vars.prefix+"show-captions"),vars.items.each(function(){var $item=$(this);if($item.children("."+vars.prefix+"caption").length==0&&$item.children("img").length>0){var title=$.trim($item.children("img:first").attr("title"));if(undefined!=title||""==title)$item.append('<div class="'+vars.prefix+'caption">'+title+"</div>"),$item.children("."+vars.prefix+"caption:empty").remove()}}),settings.showCaptions==="hover"?($("."+vars.prefix+"caption").hide(),vars.container.mouseenter(function(){vars.active.find("."+vars.prefix+"caption").stop(!0,!0).fadeTo(settings.captionFadeTime,settings.captionsOpacity)}).mouseleave(function(){vars.active.find("."+vars.prefix+"caption").delay(200).fadeOut(settings.captionFadeTime)})):settings.showCaptions==="always"&&$("."+vars.prefix+"caption").fadeTo(0,settings.captionsOpacity)),vars.items.each(function(){$(this).children("img").removeAttr("title")}),settings.autoPlay?vars.intervalAutoPlay=setInterval(function(){next($slider,settings)},settings.showTime):vars.intervalAutoPlay=!1,settings.pauseOnHover&&(vars.container.addClass(vars.prefix+"pause-on-hover"),$slider.mouseenter(function(){vars.isPlaying&&(clearInterval(vars.intervalAutoPlay),settings.controlsPlayPause&&vars.buttons.play.text(settings.playText).removeClass(vars.prefix+"pause").addClass(vars.prefix+"play"))}).mouseleave(function(){vars.isPlaying&&(vars.intervalAutoPlay=setInterval(function(){next($slider,settings)},settings.showTime),settings.controlsPlayPause&&vars.buttons.play.text(settings.pauseText).removeClass(vars.prefix+"play").addClass(vars.prefix+"pause"))})),settings.controlsKeyboard&&(vars.container.addClass(vars.prefix+"controls-keyboard"),$(document).keyup(function(e){switch(e.keyCode){case 37:pause(),prev($slider,settings);break;case 39:pause(),next($slider,settings);break;case 80:vars.isPlaying===!1?play():pause()}})),settings.controlsMousewheel&&(vars.container.addClass(vars.prefix+"controls-mousewheel"),$.isFunction($.fn.mousewheel)?$slider.mousewheel(function(e,delta){e.preventDefault();if(vars.container.hasClass("inProgress"))return!1;var dir=delta>0?"up":"down";dir==="up"?(pause(),prev($slider,settings)):(pause(),next($slider,settings))}):alert("$.fn.mousewheel is not a function. Please check that you have the mousewheel-plugin installed properly.")),vars.active=$slider.find("."+vars.prefix+"active"),vars.active.css({zIndex:1,opacity:1}),settings.cycled||(vars.items.each(function(){var $item=$(this);$item.is(":first-child")&&$item.addClass(vars.prefix+"firstItem"),$item.is(":last-child")&&$item.addClass(vars.prefix+"lastItem")}),vars.active.is(":first-child")&&settings.controlsPrevNext&&vars.buttons.prev.addClass("disabled"),vars.active.is(":last-child")&&(settings.controlsPrevNext&&(vars.buttons.next.addClass("disabled"),pause()),settings.autoPlay&&vars.buttons.play.addClass("disabled"))),preparations[settings.effect]==undefined?console.log("Effect for "+settings.effect+" not found."):preparations[settings.effect]($slider,settings,vars),$slider.data("slider:vars",vars),settings.callBackInit()},isFirst=function($item){return $item.is(":first-child")},isLast=function($item){return $item.is(":last-child")},pause=function(){var vars=$slider.data("slider:vars");clearInterval(vars.intervalAutoPlay),vars.isPlaying=!1,settings.controlsPlayPause&&vars.buttons.play.text(settings.playText).removeClass(vars.prefix+"pause").addClass(vars.prefix+"play"),settings.callBackPause()},play=function(){var vars=$slider.data("slider:vars");vars.intervalAutoPlay=setInterval(function(){next($slider,settings)},settings.showTime),vars.isPlaying=!0,settings.controlsPlayPause&&vars.buttons.play.text(settings.pauseText).removeClass(vars.prefix+"play").addClass(vars.prefix+"pause"),settings.callBackPlay()},prev=function($slider,settings,$next){var vars=$slider.data("slider:vars");if(!settings.cycled&&isFirst(vars.active))return!1;settings.callBeforePrev();if(vars.container.hasClass("inProgress"))return!1;vars.container.addClass("inProgress");if(!$next)if(settings.randomOrder){var nextID=getRandom(vars);vars.next=vars.container.find("#"+nextID)}else vars.next=vars.items.first().hasClass(vars.prefix+"active")?vars.items.last():vars.active.prev();else vars.next=$next;if(vars.next.hasClass(vars.prefix+"active"))return!1;settings.showCaptions!=="never"&&$("."+vars.prefix+"caption").stop(!0,!0).fadeOut(settings.captionsFadeTime),settings.showBullets!=="never"&&settings.changeBullets=="before"&&(vars.navigation.find("."+vars.prefix+"active-bullet").removeClass(vars.prefix+"active-bullet"),vars.navigation.find("#"+vars.next.attr("id")+"-bullet").addClass(vars.prefix+"active-bullet")),setTimeout(function(){var params=[];params.settings=settings,params.animateActive=settings.animateActive,params.direction=settings.slidePrevDirection,effects[settings.effect]==undefined?console.log("Preparations for "+settings.effect+" not found."):effects[settings.effect]($slider,params,resetElements),setTimeout(function(){settings.showBullets!=="never"&&settings.changeBullets=="after"&&(vars.navigation.find("."+vars.prefix+"active-bullet").removeClass(vars.prefix+"active-bullet"),vars.navigation.find("#"+vars.next.attr("id")+"-bullet").addClass(vars.prefix+"active-bullet")),settings.callBackPrev()},settings.effectTime)},settings.captionsFadeTime),settings.showBullets!=="never"&&settings.changeBullets=="after"&&(vars.navigation.find("."+vars.prefix+"active-bullet").removeClass(vars.prefix+"active-bullet"),vars.navigation.find("#"+vars.next.attr("id")+"-bullet").addClass(vars.prefix+"active-bullet"))},next=function($slider,settings,$next){var vars=$slider.data("slider:vars");if(!settings.cycled&&isLast(vars.active))return!1;settings.callBeforeNext();if(vars.container.hasClass("inProgress"))return!1;vars.container.addClass("inProgress");if(!$next)if(settings.randomOrder){var nextID=getRandom(vars);vars.next=vars.container.find("#"+nextID)}else vars.next=vars.items.last().hasClass(vars.prefix+"active")?vars.items.first():vars.active.next();else vars.next=$next;if(vars.next.hasClass(vars.prefix+"active"))return!1;settings.showCaptions!=="never"&&$("."+vars.prefix+"caption").stop(!0,!0).fadeOut(settings.captionsFadeTime),settings.showBullets!=="never"&&settings.changeBullets=="before"&&(vars.navigation.find("."+vars.prefix+"active-bullet").removeClass(vars.prefix+"active-bullet"),vars.navigation.find("#"+vars.next.attr("id")+"-bullet").addClass(vars.prefix+"active-bullet")),setTimeout(function(){var params=[];params.settings=settings,params.animateActive=settings.animateActive,params.direction=settings.slideNextDirection,effects[settings.effect]==undefined?console.log("Preparations for "+settings.effect+" not found."):effects[settings.effect]($slider,params,resetElements),setTimeout(function(){settings.showBullets!=="never"&&settings.changeBullets=="after"&&(vars.navigation.find("."+vars.prefix+"active-bullet").removeClass(vars.prefix+"active-bullet"),vars.navigation.find("#"+vars.next.attr("id")+"-bullet").addClass(vars.prefix+"active-bullet")),settings.callBackNext()},settings.effectTime)},settings.captionsFadeTime)},getRandom=function(vars){var curID=vars.active.attr("id"),itemCount=vars.items.length,nextID=vars.prefix+"item"+parseInt(Math.random()*itemCount,10),nextKey=nextID.replace(vars.prefix+"item","");return vars.playedCounter>=itemCount&&(vars.playedCounter=0,vars.playedArray=[]),curID==nextID||vars.playedArray[nextKey]===!0?getRandom(vars):(vars.playedArray[nextKey]=!0,vars.playedCounter++,nextID)},resetElements=function($slider,settings){var vars=$slider.data("slider:vars");vars.next.addClass(vars.prefix+"active").css({zIndex:1,top:0,left:0,width:"100%",height:"100%",margin:0,opacity:1}),vars.active.css({zIndex:0,top:0,left:0,margin:0,opacity:0}).removeClass(vars.prefix+"active"),settings.additionalResets(),settings.cycled||(settings.controlsPrevNext&&(isFirst(vars.next)?vars.buttons.prev.addClass("disabled"):vars.buttons.prev.removeClass("disabled"),isLast(vars.next)?(vars.buttons.next.addClass("disabled"),pause()):vars.buttons.next.removeClass("disabled")),settings.controlsPlayPause&&(isLast(vars.next)?(vars.buttons.play.addClass("disabled"),pause()):vars.buttons.play.removeClass("disabled"))),settings.showBullets!=="never"&&(vars.navigation.find("."+vars.prefix+"active-bullet").removeClass(vars.prefix+"active-bullet"),vars.navigation.find("#"+vars.next.attr("id")+"-bullet").addClass(vars.prefix+"active-bullet")),vars.active=vars.next,settings.showCaptions!=="never"&&vars.active.find("."+vars.prefix+"caption").stop(!0,!0).fadeTo(settings.captionsFadeTime,settings.captionsOpacity),vars.container.removeClass("inProgress")};this.pause=function(){pause()},this.play=function(){play()},this.prev=function($next){prev($slider,settings,$next)},this.next=function($next){next($slider,settings,$next)},this.uninit=function(){pause(),vars.container.before($(element).data("slider:original")),$slider.data("slider:vars",null),vars.container.remove(),$(element).data("rhinoslider",null)},init($slider,settings,vars)};$.fn.rhinoslider=function(opts){return this.each(function(){var element=$(this);if(element.data("rhinoslider"))return element.data("rhinoslider");element.data("slider:original",element.clone());var rhinoslider=new rhinoSlider(this,opts);element.data("rhinoslider",rhinoslider)})},$.fn.rhinoslider.defaults={effect:"slide",easing:"swing",randomOrder:!1,controlsMousewheel:!0,controlsKeyboard:!0,controlsPrevNext:!0,controlsPlayPause:!0,pauseOnHover:!0,animateActive:!0,autoPlay:!1,cycled:!0,showTime:3e3,effectTime:1e3,controlFadeTime:650,captionsFadeTime:250,captionsOpacity:.7,partDelay:100,shiftValue:"150",parts:"5,3",showCaptions:"never",showBullets:"hover",changeBullets:"after",showControls:"hover",slidePrevDirection:"toLeft",slideNextDirection:"toRight",prevText:"prev",nextText:"next",playText:"play",pauseText:"pause",styles:"position,top,right,bottom,left,margin-top,margin-right,margin-bottom,margin-left,width,height",callBeforeInit:function(){return!1},callBackInit:function(){return!1},callBeforeNext:function(){return!1},callBeforePrev:function(){return!1},callBackNext:function(){return!1},callBackPrev:function(){return!1},callBackPlay:function(){return!1},callBackPause:function(){return!1},additionalResets:function(){return!1}},$.fn.rhinoslider.effects={none:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings;vars.next.css({zIndex:2,display:"block"}),vars.active.hide(0,function(){callback($slider,settings)})},fade:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings;settings.animateActive&&vars.active.animate({opacity:0},settings.effectTime),vars.next.css({zIndex:2}).animate({opacity:1},settings.effectTime,settings.easing,function(){callback($slider,settings)})},slide:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings,direction=params.direction,values=[];values.width=vars.container.width(),values.height=vars.container.height(),values.easing=settings.showTime===0?"linear":settings.easing,values.nextEasing=settings.showTime===0?"linear":settings.easing,$slider.css("overflow","hidden");switch(direction){case"toTop":values.top=-values.height,values.left=0,values.nextTop=-values.top,values.nextLeft=0;break;case"toBottom":values.top=values.height,values.left=0,values.nextTop=-values.top,values.nextLeft=0;break;case"toRight":values.top=0,values.left=values.width,values.nextTop=0,values.nextLeft=-values.left;break;case"toLeft":values.top=0,values.left=-values.width,values.nextTop=0,values.nextLeft=-values.left}vars.next.css({zIndex:2,opacity:1}),settings.animateActive&&vars.active.css({top:0,left:0}).animate({top:values.top,left:values.left,opacity:1},settings.effectTime,values.easing),vars.next.css({top:values.nextTop,left:values.nextLeft}).animate({top:0,left:0,opacity:1},settings.effectTime,values.nextEasing,function(){callback($slider,settings)})},kick:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings,direction=params.direction,values=[];values.delay=settings.effectTime/2,values.activeEffectTime=settings.effectTime/2,settings.shiftValue.x=settings.shiftValue.x<0?settings.shiftValue.x*-1:settings.shiftValue.x;switch(direction){case"toTop":values.top=-settings.shiftValue.x,values.left=0,values.nextTop=settings.shiftValue.x,values.nextLeft=0;break;case"toBottom":values.top=settings.shiftValue.x,values.left=0,values.nextTop=-settings.shiftValue.x,values.nextLeft=0;break;case"toRight":values.top=0,values.left=settings.shiftValue.x,values.nextTop=0,values.nextLeft=-settings.shiftValue.x;break;case"toLeft":values.top=0,values.left=-settings.shiftValue.x,values.nextTop=0,values.nextLeft=settings.shiftValue.x}vars.next.css({zIndex:2,opacity:0}),vars.active.css({top:0,left:0}),settings.animateActive&&vars.active.delay(values.delay).animate({top:values.top,left:values.left,opacity:0},values.activeEffectTime,"out"),vars.next.css({top:values.nextTop,left:values.nextLeft}).animate({top:0,left:0,opacity:1},settings.effectTime,"kick",function(){callback($slider,settings)})},transfer:function($slider,params,callback){var settings=params.settings,direction=params.direction,vars=$slider.data("slider:vars"),values=[];values.width=$slider.width(),values.height=$slider.height();switch(direction){case"toTop":values.top=-settings.shiftValue.y,values.left=values.width/2,values.nextTop=values.height+settings.shiftValue.y,values.nextLeft=values.width/2;break;case"toBottom":values.top=values.height+settings.shiftValue.y,values.left=values.width/2,values.nextTop=-settings.shiftValue.y,values.nextLeft=values.width/2;break;case"toRight":values.top=values.height/2,values.left=values.width+settings.shiftValue.x,values.nextTop=values.height/2,values.nextLeft=-settings.shiftValue.x;break;case"toLeft":values.top=values.height/2,values.left=-settings.shiftValue.x,values.nextTop=values.height/2,values.nextLeft=values.width+settings.shiftValue.x}vars.next.children().wrapAll('<div id="'+vars.prefix+'nextContainer" class="'+vars.prefix+'tmpContainer"></div>'),vars.active.children().wrapAll('<div id="'+vars.prefix+'activeContainer" class="'+vars.prefix+'tmpContainer"></div>');var $nextContainer=vars.next.find("#"+vars.prefix+"nextContainer"),$activeContainer=vars.active.find("#"+vars.prefix+"activeContainer"),$tmpContainer=vars.container.find("."+vars.prefix+"tmpContainer");$activeContainer.css({width:values.width,height:values.height,position:"absolute",top:"50%",left:"50%",margin:"-"+parseInt(values.height*.5,10)+"px 0 0 -"+parseInt(values.width*.5,10)+"px"}),$nextContainer.css({width:values.width,height:values.height,position:"absolute",top:"50%",left:"50%",margin:"-"+parseInt(values.height*.5,10)+"px 0 0 -"+parseInt(values.width*.5,10)+"px"}),settings.animateActive&&vars.active.css({width:"100%",height:"100%",top:0,left:0}).animate({width:0,height:0,top:values.top,left:values.left,opacity:0},settings.effectTime),vars.next.css({opacity:0,zIndex:2,width:0,height:0,top:values.nextTop,left:values.nextLeft}).animate({width:"100%",height:"100%",top:0,left:0,opacity:1},settings.effectTime,settings.easing,function(){$tmpContainer.children().unwrap(),callback($slider,settings)})},transfer:function($slider,params,callback){var settings=params.settings,direction=params.direction,vars=$slider.data("slider:vars"),values=[];values.width=$slider.width(),values.height=$slider.height();switch(direction){case"toTop":values.top=-settings.shiftValue.y,values.left=values.width/2,values.nextTop=values.height+settings.shiftValue.y,values.nextLeft=values.width/2;break;case"toBottom":values.top=values.height+settings.shiftValue.y,values.left=values.width/2,values.nextTop=-settings.shiftValue.y,values.nextLeft=values.width/2;break;case"toRight":values.top=values.height/2,values.left=values.width+settings.shiftValue.x,values.nextTop=values.height/2,values.nextLeft=-settings.shiftValue.x;break;case"toLeft":values.top=values.height/2,values.left=-settings.shiftValue.x,values.nextTop=values.height/2,values.nextLeft=values.width+settings.shiftValue.x}vars.next.children().wrapAll('<div id="'+vars.prefix+'nextContainer" class="'+vars.prefix+'tmpContainer"></div>'),vars.active.children().wrapAll('<div id="'+vars.prefix+'activeContainer" class="'+vars.prefix+'tmpContainer"></div>');var $nextContainer=vars.next.find("#"+vars.prefix+"nextContainer"),$activeContainer=vars.active.find("#"+vars.prefix+"activeContainer"),$tmpContainer=vars.container.find("."+vars.prefix+"tmpContainer");$activeContainer.css({width:values.width,height:values.height,position:"absolute",top:"50%",left:"50%",margin:"-"+parseInt(values.height*.5,10)+"px 0 0 -"+parseInt(values.width*.5,10)+"px"}),$nextContainer.css({width:values.width,height:values.height,position:"absolute",top:"50%",left:"50%",margin:"-"+parseInt(values.height*.5,10)+"px 0 0 -"+parseInt(values.width*.5,10)+"px"}),settings.animateActive&&vars.active.css({width:"100%",height:"100%",top:0,left:0}).animate({width:0,height:0,top:values.top,left:values.left,opacity:0},settings.effectTime),vars.next.css({opacity:0,zIndex:2,width:0,height:0,top:values.nextTop,left:values.nextLeft}).animate({width:"100%",height:"100%",top:0,left:0,opacity:1},settings.effectTime,settings.easing,function(){$tmpContainer.children().unwrap(),callback($slider,settings)})},shuffle:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings,values=[],preShuffle=function($slider,settings,$li){var vars=$slider.data("slider:vars");$li.html('<div class="'+vars.prefix+'partContainer">'+$li.html()+"</div>");var part=$li.html(),width=$slider.width(),height=$slider.height();for(i=1;i<settings.parts.x*settings.parts.y;i++)$li.html($li.html()+part);var $parts=$li.children("."+vars.prefix+"partContainer"),partValues=[];return partValues.width=$li.width()/settings.parts.x,partValues.height=$li.height()/settings.parts.y,$parts.each(function(i){var $this=$(this);partValues.top=(i-i%settings.parts.x)/settings.parts.x*partValues.height,partValues.left=i%settings.parts.x*partValues.width,partValues.marginTop=-partValues.top,partValues.marginLeft=-partValues.left,$this.css({top:partValues.top,left:partValues.left,width:partValues.width,height:partValues.height,position:"absolute",overflow:"hidden"}).html('<div class="'+vars.prefix+'part">'+$this.html()+"</div>"),$this.children("."+vars.prefix+"part").css({marginTop:partValues.marginTop,marginLeft:partValues.marginLeft,width:width,height:height,background:$li.css("background-image")+" "+$li.parent().css("background-color")})}),$parts},calcParts=function(parts,c){return parts.x*parts.y>36?(c?(parts.x>1?parts.x--:parts.y--,c=!1):(parts.y>1?parts.y--:parts.x--,c=!0),calcParts(parts,c)):parts},shuffle=function($slider,settings){settings.parts.x=settings.parts.x<1?1:settings.parts.x,settings.parts.y=settings.parts.y<1?1:settings.parts.y,settings.parts=calcParts(settings.parts,!0),settings.shiftValue.x=settings.shiftValue.x<0?settings.shiftValue.x*-1:settings.shiftValue.x,settings.shiftValue.y=settings.shiftValue.y<0?settings.shiftValue.y*-1:settings.shiftValue.y;var vars=$slider.data("slider:vars"),activeContent=vars.active.html(),nextContent=vars.next.html(),width=$slider.width(),height=$slider.height(),$activeParts=preShuffle($slider,settings,vars.active),$nextParts=preShuffle($slider,settings,vars.next),activeBackgroundImage=vars.active.css("background-image"),activeBackgroundColor=vars.active.css("background-color"),nextBackgroundImage=vars.next.css("background-image"),nextBackgroundColor=vars.next.css("background-color");vars.active.css({backgroundImage:"none",backgroundColor:"none",opacity:1}),vars.next.css({backgroundImage:"none",backgroundColor:"none",opacity:1,zIndex:2});var partValues=[];partValues.width=vars.next.width()/settings.parts.x,partValues.height=vars.next.height()/settings.parts.y,settings.animateActive&&$activeParts.each(function(i){$this=$(this);var newLeft,newTop;newLeft=Math.random()*settings.shiftValue.x*2-settings.shiftValue.x,newTop=Math.random()*settings.shiftValue.y*2-settings.shiftValue.y,$this.animate({opacity:0,top:"+="+newTop,left:"+="+newLeft},settings.effectTime,settings.easing)}),$nextParts.each(function(i){$this=$(this),partValues.top=(i-i%settings.parts.x)/settings.parts.x*partValues.height,partValues.left=i%settings.parts.x*partValues.width;var newLeft,newTop;newLeft=partValues.left+(Math.random()*settings.shiftValue.x*2-settings.shiftValue.x),newTop=partValues.top+(Math.random()*settings.shiftValue.y*2-settings.shiftValue.y),$this.css({top:newTop,left:newLeft,opacity:0}).animate({top:partValues.top,left:partValues.left,opacity:1},settings.effectTime,settings.easing,function(){i==$activeParts.length-1&&(vars.active.html(activeContent),vars.next.html(nextContent),vars.active.css({backgroundImage:activeBackgroundImage,backgroundColor:activeBackgroundColor,opacity:0}),vars.next.css({backgroundImage:nextBackgroundImage,backgroundColor:nextBackgroundColor,opacity:1}),callback($slider,settings))})})};shuffle($slider,settings)},explode:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings,values=[],preShuffle=function($slider,settings,$li){var vars=$slider.data("slider:vars");$li.html('<div class="'+vars.prefix+'partContainer">'+$li.html()+"</div>");var part=$li.html(),width=$slider.width(),height=$slider.height();for(i=1;i<settings.parts.x*settings.parts.y;i++)$li.html($li.html()+part);var $parts=$li.children("."+vars.prefix+"partContainer"),partValues=[];return partValues.width=$li.width()/settings.parts.x,partValues.height=$li.height()/settings.parts.y,$parts.each(function(i){var $this=$(this);partValues.top=(i-i%settings.parts.x)/settings.parts.x*partValues.height,partValues.left=i%settings.parts.x*partValues.width,partValues.marginTop=-partValues.top,partValues.marginLeft=-partValues.left,$this.css({top:partValues.top,left:partValues.left,width:partValues.width,height:partValues.height,position:"absolute",overflow:"hidden"}).html('<div class="'+vars.prefix+'part">'+$this.html()+"</div>"),$this.children("."+vars.prefix+"part").css({marginTop:partValues.marginTop,marginLeft:partValues.marginLeft,width:width,height:height,background:$li.css("background-image")+" "+$li.parent().css("background-color")})}),$parts},calcParts=function(parts,c){return parts.x*parts.y>36?(c?(parts.x>1?parts.x--:parts.y--,c=!1):(parts.y>1?parts.y--:parts.x--,c=!0),calcParts(parts,c)):parts},explode=function($slider,settings){settings.parts.x=settings.parts.x<1?1:settings.parts.x,settings.parts.y=settings.parts.y<1?1:settings.parts.y,settings.parts=calcParts(settings.parts,!0),settings.shiftValue.x=settings.shiftValue.x<0?settings.shiftValue.x*-1:settings.shiftValue.x,settings.shiftValue.y=settings.shiftValue.y<0?settings.shiftValue.y*-1:settings.shiftValue.y;var vars=$slider.data("slider:vars"),activeContent=vars.active.html(),nextContent=vars.next.html(),width=$slider.width(),height=$slider.height(),$activeParts=preShuffle($slider,settings,vars.active),$nextParts=preShuffle($slider,settings,vars.next),activeBackgroundImage=vars.active.css("background-image"),activeBackgroundColor=vars.active.css("background-color"),nextBackgroundImage=vars.next.css("background-image"),nextBackgroundColor=vars.next.css("background-color");vars.active.css({backgroundImage:"none",backgroundColor:"none",opacity:1}),vars.next.css({backgroundImage:"none",backgroundColor:"none",opacity:1,zIndex:2});var partValues=[];partValues.width=vars.next.width()/settings.parts.x,partValues.height=vars.next.height()/settings.parts.y,settings.animateActive&&$activeParts.each(function(i){$this=$(this);var newLeft,newTop,position=[];position.top=$this.position().top,position.bottom=$this.parent().height()-$this.position().top-$this.height(),position.left=$this.position().left,position.right=$this.parent().width()-$this.position().left-$this.width();var rndX=parseInt(Math.random()*settings.shiftValue.x,10),rndY=parseInt(Math.random()*settings.shiftValue.y,10);newLeft=position.right<=position.left?position.right==position.left?rndX/2:rndX:-rndX,newTop=position.bottom<=position.top?position.top==position.bottom-1?rndY/2:rndY:-rndY,$this.animate({top:"+="+newTop,left:"+="+newLeft,opacity:0},settings.effectTime,settings.easing)}),$nextParts.each(function(i){$this=$(this),partValues.top=(i-i%settings.parts.x)/settings.parts.x*partValues.height,partValues.left=i%settings.parts.x*partValues.width;var newLeft,newTop,position=[];position.top=$this.position().top,position.bottom=$this.parent().height()-$this.position().top-$this.height(),position.left=$this.position().left,position.right=$this.parent().width()-$this.position().left-$this.width();var rndX=parseInt(Math.random()*settings.shiftValue.x,10),rndY=parseInt(Math.random()*settings.shiftValue.y,10);newLeft=position.right<=position.left?position.right==position.left?rndX/2:rndX:-rndX,newTop=position.bottom<=position.top?position.top==position.bottom-1?rndY/2:rndY:-rndY,newLeft=partValues.left+newLeft,newTop=partValues.top+newTop,$this.css({top:newTop,left:newLeft,opacity:0}).animate({top:partValues.top,left:partValues.left,opacity:1},settings.effectTime,settings.easing,function(){i==$activeParts.length-1&&(vars.active.html(activeContent),vars.next.html(nextContent),vars.active.css({backgroundImage:activeBackgroundImage,backgroundColor:activeBackgroundColor,opacity:0}),vars.next.css({backgroundImage:nextBackgroundImage,backgroundColor:nextBackgroundColor,opacity:1}),callback($slider,settings))})})};explode($slider,settings)},turnOver:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings,direction=params.direction,values=[];values.width=vars.container.width(),values.height=vars.container.height();switch(direction){case"toTop":values.top=-values.height,values.left=0;break;case"toBottom":values.top=values.height,values.left=0;break;case"toRight":values.top=0,values.left=values.width;break;case"toLeft":values.top=0,values.left=-values.width}values.timeOut=settings.animateActive?settings.effectTime:0,values.effectTime=settings.animateActive?settings.effectTime/2:settings.effectTime,vars.next.css({zIndex:2,opacity:1}),vars.next.css({top:values.top,left:values.left}),settings.animateActive&&vars.active.css({top:0,left:0}).animate({top:values.top,left:values.left,opacity:1},values.effectTime,settings.easing),setTimeout(function(){vars.next.animate({top:0,left:0,opacity:1},values.effectTime,settings.easing,function(){vars.active.css("opacity",0),callback($slider,settings)})},values.timeOut)},chewyBars:function($slider,params,callback){var vars=$slider.data("slider:vars"),settings=params.settings,direction=params.direction,values=[],preSlide=function($slider,settings,$li){var vars=$slider.data("slider:vars");$li.html('<div class="'+vars.prefix+'partContainer">'+$li.html()+"</div>");var part=$li.html(),width=$slider.width(),height=$slider.height();for(i=1;i<settings.parts;i++)$li.html($li.html()+part);var $parts=$li.children("."+vars.prefix+"partContainer"),partValues=[];switch(direction){case"toLeft":partValues.width=$li.width()/settings.parts,partValues.height=height;break;case"toTop":partValues.width=width,partValues.height=$li.height()/settings.parts}return $parts.each(function(i){var $this=$(this),liWidth=$li.width(),liHeight=$li.height();partValues.left="auto",partValues.marginLeft="auto",partValues.top="auto",partValues.marginTop="auto",partValues.right="auto",partValues.bottom="auto";switch(direction){case"toLeft":partValues.width=liWidth/settings.parts,partValues.height=height,partValues.left=i%settings.parts*partValues.width,partValues.marginLeft=-partValues.left,partValues.top=0,partValues.marginTop=0;break;case"toRight":partValues.width=liWidth/settings.parts,partValues.height=height,partValues.right=i%settings.parts*partValues.width,partValues.marginLeft=-(liWidth-partValues.right-partValues.width),partValues.top=0,partValues.marginTop=0;break;case"toTop":partValues.width=width,partValues.height=liHeight/settings.parts,partValues.left=0,partValues.marginLeft=0,partValues.top=i%settings.parts*partValues.height,partValues.marginTop=-partValues.top;break;case"toBottom":partValues.width=width,partValues.height=liHeight/settings.parts,partValues.left=0,partValues.marginLeft=0,partValues.bottom=i%settings.parts*partValues.height,partValues.marginTop=-(liHeight-partValues.bottom-partValues.height)}$this.css({top:partValues.top,left:partValues.left,bottom:partValues.bottom,right:partValues.right,width:partValues.width,height:partValues.height,position:"absolute",overflow:"hidden"}).html('<div class="'+vars.prefix+'part">'+$this.html()+"</div>"),$this.children("."+vars.prefix+"part").css({marginLeft:partValues.marginLeft,marginTop:partValues.marginTop,width:width,height:height,background:$li.css("background-image")+" "+$li.parent().css("background-color")})}),$parts},slideBars=function($slider,settings){settings.parts=settings.parts<1?1:settings.parts,settings.shiftValue.x=settings.shiftValue.x<0?settings.shiftValue.x*-1:settings.shiftValue.x,settings.shiftValue.y=settings.shiftValue.y<0?settings.shiftValue.y*-1:settings.shiftValue.y;var vars=$slider.data("slider:vars"),partDuration,partDelay=settings.partDelay,activeContent=vars.active.html(),nextContent=vars.next.html(),width=$slider.width(),height=$slider.height(),$activeParts=preSlide($slider,settings,vars.active),$nextParts=preSlide($slider,settings,vars.next),activeBackgroundImage=vars.active.css("background-image"),activeBackgroundColor=vars.active.css("background-color"),nextBackgroundImage=vars.next.css("background-image"),nextBackgroundColor=vars.next.css("background-color"),delay=0;partDuration=settings.effectTime-2*(settings.parts-1)*partDelay,vars.active.css({backgroundImage:"none",backgroundColor:"none",opacity:1}),vars.next.css({backgroundImage:"none",backgroundColor:"none",opacity:1,zIndex:2});var values=[],aniMap={opacity:0},cssMapNext={opacity:0};switch(direction){case"toTop":aniMap.left=-settings.shiftValue.x,aniMap.top=-settings.shiftValue.y,cssMapNext.left=settings.shiftValue.x,cssMapNext.top=height+settings.shiftValue.y,values.width=width,values.height=vars.next.height()/settings.parts;break;case"toRight":values.width=vars.next.width()/settings.parts,values.height=height,aniMap.top=-settings.shiftValue.y,aniMap.right=-settings.shiftValue.x,cssMapNext.top=settings.shiftValue.y,cssMapNext.right=width+settings.shiftValue.x;break;case"toBottom":values.width=width,values.height=vars.next.height()/settings.parts,aniMap.left=-settings.shiftValue.x,aniMap.bottom=-settings.shiftValue.y,cssMapNext.left=settings.shiftValue.x,cssMapNext.bottom=height+settings.shiftValue.y;break;case"toLeft":values.width=vars.next.width()/settings.parts,values.height=height,aniMap.top=-settings.shiftValue.y,aniMap.left=-settings.shiftValue.x,cssMapNext.top=settings.shiftValue.y,cssMapNext.left=width+settings.shiftValue.x}settings.animateActive&&($activeParts.each(function(i){$this=$(this),$this.delay(partDelay*i).animate(aniMap,partDuration,settings.easing)}),delay=settings.parts*partDelay),$nextParts.each(function(i){var $this=$(this),newValues=[],aniMap={opacity:1};switch(direction){case"toTop":aniMap.left=0,aniMap.top=values.height*i;break;case"toRight":aniMap.top=0,aniMap.right=values.width*i;break;case"toBottom":aniMap.left=0,aniMap.bottom=values.height*i;break;case"toLeft":aniMap.top=0,aniMap.left=values.width*i}$this.delay(delay).css(cssMapNext).delay(i*partDelay).animate(aniMap,partDuration,settings.easing,function(){i==settings.parts-1&&(vars.active.html(activeContent),vars.next.html(nextContent),vars.active.css({backgroundImage:activeBackgroundImage,backgroundColor:activeBackgroundColor,opacity:0}),vars.next.css({backgroundImage:nextBackgroundImage,backgroundColor:nextBackgroundColor,opacity:1}),callback($slider,settings))})})};slideBars($slider,settings)}},$.fn.rhinoslider.preparations={none:function($slider,settings,vars){},fade:function($slider,settings,vars){},slide:function($slider,settings,vars){vars.items.css("overflow","hidden"),$slider.css("overflow","hidden")},kick:function($slider,settings,vars){vars.items.css("overflow","hidden"),settings.shiftValue.x=parseInt(tmpShiftValue,10),settings.shiftValue.y=parseInt(tmpShiftValue,10),settings.parts.x=parseInt(tmpParts,10),settings.parts.y=parseInt(tmpParts,10)},transfer:function($slider,settings,vars){var shiftValue=String(tmpShiftValue);if(shiftValue.indexOf(",")>=0){var tmp=shiftValue.split(",");settings.shiftValue.x=parseInt(tmp[0],10),settings.shiftValue.y=parseInt(tmp[1],10)}else settings.shiftValue.x=parseInt(tmpShiftValue,10),settings.shiftValue.y=parseInt(tmpShiftValue,10);vars.items.css("overflow","hidden")},shuffle:function($slider,settings,vars){var shiftValue=String(tmpShiftValue);if(shiftValue.indexOf(",")>=0){var tmp=shiftValue.split(",");settings.shiftValue.x=tmp[0],settings.shiftValue.y=tmp[1]}else settings.shiftValue.x=parseInt(tmpShiftValue,10),settings.shiftValue.y=parseInt(tmpShiftValue,10);var parts=String(tmpParts);if(parts.indexOf(",")>=0){var tmp=parts.split(",");settings.parts.x=tmp[0],settings.parts.y=tmp[1]}else settings.parts.x=parseInt(tmpParts,10),settings.parts.y=parseInt(tmpParts,10);vars.items.css("overflow","visible")},explode:function($slider,settings,vars){var shiftValue=String(tmpShiftValue);if(shiftValue.indexOf(",")>=0){var tmp=shiftValue.split(",");settings.shiftValue.x=tmp[0],settings.shiftValue.y=tmp[1]}else settings.shiftValue.x=parseInt(tmpShiftValue,10),settings.shiftValue.y=parseInt(tmpShiftValue,10);var parts=String(tmpParts);if(parts.indexOf(",")>=0){var tmp=parts.split(",");settings.parts.x=tmp[0],settings.parts.y=tmp[1]}else settings.parts.x=parseInt(tmpParts,10),settings.parts.y=parseInt(tmpParts,10);vars.items.css("overflow","visible")},turnOver:function($slider,settings,vars){vars.items.css("overflow","hidden"),$slider.css("overflow","hidden")},chewyBars:function($slider,settings,vars){var shiftValue=String(tmpShiftValue);if(shiftValue.indexOf(",")>=0){var tmp=shiftValue.split(",");settings.shiftValue.x=parseInt(tmp[0],10),settings.shiftValue.y=parseInt(tmp[1],10)}else settings.shiftValue.x=parseInt(tmpShiftValue,10),settings.shiftValue.y=parseInt(tmpShiftValue,10);var parts=String(tmpParts);if(parts.indexOf(",")>=0){var tmp=parts.split(",");settings.parts=parseInt(tmp[0],10)*parseInt(tmp[1],10)}else settings.parts=parseInt(tmpParts,10);vars.items.css("overflow","visible")}}})(jQuery,window)});
/*! matchMedia() polyfill - Test a CSS media type/query in JS. Authors & copyright (c) 2012: Scott Jehl, Paul Irish, Nicholas Zakas. Dual MIT/BSD license */

/*! NOTE: If you're already including a window.matchMedia polyfill via Modernizr or otherwise, you don't need this part */

/*! Respond.js v1.1.0: min/max-width media query polyfill. (c) Scott Jehl. MIT/GPLv2 Lic. j.mp/respondjs  */

window.matchMedia=window.matchMedia||function(e,f){var c,a=e.documentElement,b=a.firstElementChild||a.firstChild,d=e.createElement("body"),g=e.createElement("div");return g.id="mq-test-1",g.style.cssText="position:absolute;top:-100em",d.style.background="none",d.appendChild(g),function(h){return g.innerHTML='&shy;<style media="'+h+'"> #mq-test-1 { width: 42px; }</style>',a.insertBefore(d,b),c=g.offsetWidth==42,a.removeChild(d),{matches:c,media:h}}}(document),function(e){function t(){j(!0)}e.respond={},respond.update=function(){},respond.mediaQueriesSupported=e.matchMedia&&e.matchMedia("only all").matches;if(respond.mediaQueriesSupported)return;var w=e.document,s=w.documentElement,i=[],k=[],q=[],o={},h=30,f=w.getElementsByTagName("head")[0]||s,g=w.getElementsByTagName("base")[0],b=f.getElementsByTagName("link"),d=[],a=function(){var D=b,y=D.length,B=0,A,z,C,x;for(;B<y;B++)A=D[B],z=A.href,C=A.media,x=A.rel&&A.rel.toLowerCase()==="stylesheet",!!z&&x&&!o[z]&&(A.styleSheet&&A.styleSheet.rawCssText?(m(A.styleSheet.rawCssText,z,C),o[z]=!0):(!/^([a-zA-Z:]*\/\/)/.test(z)&&!g||z.replace(RegExp.$1,"").split("/")[0]===e.location.host)&&d.push({href:z,media:C}));u()},u=function(){if(d.length){var x=d.shift();n(x.href,function(y){m(y,x.href,x.media),o[x.href]=!0,u()})}},m=function(I,x,z){var G=I.match(/@media[^\{]+\{([^\{\}]*\{[^\}\{]*\})+/gi),J=G&&G.length||0,x=x.substring(0,x.lastIndexOf("/")),y=function(K){return K.replace(/(url\()['"]?([^\/\)'"][^:\)'"]+)['"]?(\))/g,"$1"+x+"$2$3")},A=!J&&z,D=0,C,E,F,B,H;x.length&&(x+="/"),A&&(J=1);for(;D<J;D++){C=0,A?(E=z,k.push(y(I))):(E=G[D].match(/@media *([^\{]+)\{([\S\s]+?)$/)&&RegExp.$1,k.push(RegExp.$2&&y(RegExp.$2))),B=E.split(","),H=B.length;for(;C<H;C++)F=B[C],i.push({media:F.split("(")[0].match(/(only\s+)?([a-zA-Z]+)\s?/)&&RegExp.$2||"all",rules:k.length-1,hasquery:F.indexOf("(")>-1,minw:F.match(/\(min\-width:[\s]*([\s]*[0-9\.]+)(px|em)[\s]*\)/)&&parseFloat(RegExp.$1)+(RegExp.$2||""),maxw:F.match(/\(max\-width:[\s]*([\s]*[0-9\.]+)(px|em)[\s]*\)/)&&parseFloat(RegExp.$1)+(RegExp.$2||"")})}j()},l,r,v=function(){var z,A=w.createElement("div"),x=w.body,y=!1;return A.style.cssText="position:absolute;font-size:1em;width:1em",x||(x=y=w.createElement("body"),x.style.background="none"),x.appendChild(A),s.insertBefore(x,s.firstChild),z=A.offsetWidth,y?s.removeChild(x):x.removeChild(A),z=p=parseFloat(z),z},p,j=function(I){var x="clientWidth",B=s[x],H=w.compatMode==="CSS1Compat"&&B||w.body[x]||B,D={},G=b[b.length-1],z=(new Date).getTime();if(I&&l&&z-l<h){clearTimeout(r),r=setTimeout(j,h);return}l=z;for(var E in i){var K=i[E],C=K.minw,J=K.maxw,A=C===null,L=J===null,y="em";!C||(C=parseFloat(C)*(C.indexOf(y)>-1?p||v():1)),!J||(J=parseFloat(J)*(J.indexOf(y)>-1?p||v():1));if(!K.hasquery||(!A||!L)&&(A||H>=C)&&(L||H<=J))D[K.media]||(D[K.media]=[]),D[K.media].push(k[K.rules])}for(var E in q)q[E]&&q[E].parentNode===f&&f.removeChild(q[E]);for(var E in D){var M=w.createElement("style"),F=D[E].join("\n");M.type="text/css",M.media=E,f.insertBefore(M,G.nextSibling),M.styleSheet?M.styleSheet.cssText=F:M.appendChild(w.createTextNode(F)),q.push(M)}},n=function(x,z){var y=c();if(!y)return;y.open("GET",x,!0),y.onreadystatechange=function(){if(y.readyState!=4||y.status!=200&&y.status!=304)return;z(y.responseText)};if(y.readyState==4)return;y.send(null)},c=function(){var x=!1;try{x=new XMLHttpRequest}catch(y){x=new ActiveXObject("Microsoft.XMLHTTP")}return function(){return x}}();a(),respond.update=a,e.addEventListener?e.addEventListener("resize",t,!1):e.attachEvent&&e.attachEvent("onresize",t)}(this);
define("libs/respond/respond.min", function(){});

define('jQuery',["libs/jquery/jquery","libs/jquery/jquery.isSupport","libs/jquery/jquery.ui","libs/jquery/jquery.ui.sortable.performance","libs/jquery/jquery.tmpl","libs/jquery/jquery.mousewheel","libs/jquery/jquery.ui.timepicker","libs/jquery/jquery.ui.confirmation","libs/jquery/jquery.notifyBar","libs/jqote2/jquery.jqote2","libs/jquery/jquery.ui.animateWithCss","libs/jquery/jquery.ui.sprite","libs/jquery/jquery.ui.tauBubble","libs/jquery/jquery.ui.position","libs/jquery/jquery.textEditor","libs/jquery/jquery.ui.tauSortable","libs/jquery/jquery.ui.tauSelectable","libs/jquery/jquery.ui.tauProgressIndicator","libs/jquery/jquery.scrollTo","libs/jquery/jquery.gid","libs/jquery/jquery.ui.pscroller","libs/jquery/jquery.ui.rselect","libs/jquery/rhinoslider","libs/respond/respond.min"],function($){return $});