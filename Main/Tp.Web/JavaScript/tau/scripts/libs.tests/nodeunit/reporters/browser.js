/*!
 * Nodeunit
 * Copyright (c) 2010 Caolan McMahon
 * MIT Licensed
 *
 * THIS FILE SHOULD BE BROWSER-COMPATIBLE JS!
 * You can use @REMOVE_LINE_FOR_BROWSER to remove code from the browser build.
 * Only code on that line will be removed, its mostly to avoid requiring code
 * that is node specific
 */
define([],function(){return function(a){var exports={};return exports.info="Browser-based test reporter",exports.run=function(b,c){var d=(new Date).getTime();c=c||{};var e=c.bus||null;a.runModules(b,{filter:c.filter||!1,execution:c.execution,data:c.data,shuffle:c.shuffle,moduleStart:function(module){e.fire("module.start",{module:module})},testStart:function(a,b){e.fire("test.start",{names:a,moduleId:b})},testDone:function(a,b,c){e.fire("test.done",{names:a,moduleId:c,assertions:b})},moduleDone:function(module,a){e.fire("module.done",{module:module,assertions:a})},done:function(a){e.fire("modules.done",{assertions:a})}})},exports}})