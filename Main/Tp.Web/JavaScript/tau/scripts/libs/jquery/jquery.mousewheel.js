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
define(["libs/jquery/jquery.ui"],function(jQuery){(function($){function c(a){var b=a||window.event,c=[].slice.call(arguments,1),d=0,e=!0,f=0,g=0;return a=$.event.fix(b),a.type="mousewheel",b.wheelDelta&&(d=b.wheelDelta/120),b.detail&&(d=-b.detail/3),g=d,b.axis!==undefined&&b.axis===b.HORIZONTAL_AXIS&&(g=0,f=-1*d),b.wheelDeltaY!==undefined&&(g=b.wheelDeltaY/120),b.wheelDeltaX!==undefined&&(f=-1*b.wheelDeltaX/120),c.unshift(a,d,f,g),($.event.dispatch||$.event.handle).apply(this,c)}var a=["DOMMouseScroll","mousewheel"];if($.event.fixHooks)for(var b=a.length;b;)$.event.fixHooks[a[--b]]=$.event.mouseHooks;$.event.special.mousewheel={setup:function(){if(this.addEventListener)for(var b=a.length;b;)this.addEventListener(a[--b],c,!1);else this.onmousewheel=c},teardown:function(){if(this.removeEventListener)for(var b=a.length;b;)this.removeEventListener(a[--b],c,!1);else this.onmousewheel=null}},$.fn.extend({mousewheel:function(a){return a?this.bind("mousewheel",a):this.trigger("mousewheel")},unmousewheel:function(a){return this.unbind("mousewheel",a)}})})(jQuery)})