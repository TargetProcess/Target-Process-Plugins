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

define([],function(){return function(nodeunit){var exports={};return exports.info="Browser-based test reporter",exports.run=function(modules,options){var start=(new Date).getTime();options=options||{};var bus=options.bus||null;nodeunit.runModules(modules,{filter:options.filter||!1,execution:options.execution,data:options.data,shuffle:options.shuffle,moduleStart:function(module){bus.fire("module.start",{module:module})},testStart:function(names,moduleId){bus.fire("test.start",{names:names,moduleId:moduleId})},testDone:function(names,assertions,moduleId){bus.fire("test.done",{names:names,moduleId:moduleId,assertions:assertions})},moduleDone:function(module,assertions){bus.fire("module.done",{module:module,assertions:assertions})},done:function(assertions){bus.fire("modules.done",{assertions:assertions})}})},exports}})