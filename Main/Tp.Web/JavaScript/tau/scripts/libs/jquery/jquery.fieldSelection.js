/**
     * jQuery plugin for getting selection or replace a text in input field and textarea
     *
     * Dual licensed under the MIT or GPL Version 2 licenses.
     *
     * @version 0.1
     * @author Oleg Slobodskoi (jsui.de)
     */

define(["jQuery"],function(jQuery){return function($){function get(elem){var data={start:0,end:elem.value.length,length:0};if(elem.selectionStart>=0)data.start=elem.selectionStart,data.end=elem.selectionEnd,data.length=data.end-data.start,data.text=elem.value.substr(data.start,data.length);else if(elem.ownerDocument.selection){var r=elem.ownerDocument.selection.createRange();if(!r)return data;var tr=elem.createTextRange(),ctr=tr.duplicate();tr.moveToBookmark(r.getBookmark()),ctr.setEndPoint("EndToStart",tr),data.start=ctr.text.length,data.end=ctr.text.length+r.text.length,data.text=r.text,data.length=r.text.length}return data}function replace(elem,text){if(elem.selectionStart>=0){var start=elem.selectionStart,end=elem.selectionEnd,pos,scrollTop=elem.scrollTop,scrollLeft=elem.scrollLeft;elem.value=elem.value.substr(0,start)+text+elem.value.substr(end),pos=start+text.length,elem.selectionStart=pos,elem.selectionEnd=pos,elem.scrollTop=scrollTop,elem.scrollLeft=scrollLeft}else if(elem.ownerDocument.selection){var range=elem.ownerDocument.selection.createRange();range.text=text,range.move("character",0),range.select()}else elem.value+=text,elem.scrollTop=1e5}$.fn.fieldSelection=function(text){var ret;return this.each(function(){this.focus(),ret=text?replace(this,text||""):get(this)}),ret||this}}(jQuery),define("MashupManager/jquery/fieldSelection",[],function(){return $}),$})