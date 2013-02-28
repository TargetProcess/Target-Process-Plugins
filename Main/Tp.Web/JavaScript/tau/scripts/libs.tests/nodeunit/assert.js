// http://wiki.commonjs.org/wiki/Unit_Testing/1.0
//
// THIS IS NOT TESTED NOR LIKELY TO WORK OUTSIDE V8!
//
// Originally from narwhal.js (http://narwhaljs.org)
// Copyright (c) 2009 Thomas Robinson <280north.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

define([],function(){function fail(actual,expected,message,operator,stackStartFunction){throw new assert.AssertionError({message:message,actual:actual,expected:expected,operator:operator,stackStartFunction:stackStartFunction})}function _deepEqual(actual,expected){return actual===expected?!0:actual instanceof Date&&expected instanceof Date?actual.getTime()===expected.getTime():actual instanceof RegExp&&expected instanceof RegExp?actual.source===expected.source&&actual.global===expected.global&&actual.ignoreCase===expected.ignoreCase&&actual.multiline===expected.multiline:typeof actual!="object"&&typeof expected!="object"?actual==expected:objEquiv(actual,expected)}function isUndefinedOrNull(value){return value===null||value===undefined}function isArguments(object){return Object.prototype.toString.call(object)=="[object Arguments]"}function objEquiv(a,b){if(isUndefinedOrNull(a)||isUndefinedOrNull(b))return!1;if(a.prototype!==b.prototype)return!1;if(isArguments(a))return isArguments(b)?(a=pSlice.call(a),b=pSlice.call(b),_deepEqual(a,b)):!1;try{var ka=_keys(a),kb=_keys(b),key,i}catch(e){return!1}if(ka.length!=kb.length)return!1;ka.sort(),kb.sort();for(i=ka.length-1;i>=0;i--)if(ka[i]!=kb[i])return!1;for(i=ka.length-1;i>=0;i--){key=ka[i];if(!_deepEqual(a[key],b[key]))return!1}return!0}function _throws(shouldThrow,block,err,message){var exception=null,threw=!1,typematters=!0;message=message||"",arguments.length==3?typeof err=="string"&&(message=err,typematters=!1):arguments.length==2&&(typematters=!1);try{block()}catch(e){threw=!0,exception=e}shouldThrow&&!threw&&fail("Missing expected exception"+(err&&err.name?" ("+err.name+").":".")+(message?" "+message:"")),!shouldThrow&&threw&&typematters&&exception instanceof err&&fail("Got unwanted exception"+(err&&err.name?" ("+err.name+").":".")+(message?" "+message:""));if(shouldThrow&&threw&&typematters&&!(exception instanceof err)||!shouldThrow&&threw)throw exception}var exports={},_keys=function(obj){if(Object.keys)return Object.keys(obj);if(typeof obj!="object"&&typeof obj!="function")throw new TypeError("-");var keys=[];for(var k in obj)obj.hasOwnProperty(k)&&keys.push(k);return keys},pSlice=Array.prototype.slice,assert=exports;assert.AssertionError=function(options){this.name="AssertionError",this.message=options.message,this.actual=options.actual,this.expected=options.expected,this.operator=options.operator;var stackStartFunction=options.stackStartFunction||fail;Error.captureStackTrace&&Error.captureStackTrace(this,stackStartFunction)},assert.AssertionError.super_=Error;var ctor=function(){this.constructor=assert.AssertionError};return ctor.prototype=Error.prototype,assert.AssertionError.prototype=new ctor,assert.AssertionError.prototype.toString=function(){return this.message?[this.name+":",this.message].join(" "):[this.name+":",JSON.stringify(this.expected),this.operator,JSON.stringify(this.actual)].join(" ")},assert.AssertionError.__proto__=Error.prototype,assert.fail=function(message){fail(null,null,message)},assert._fail=fail,assert.ok=function(value,message){value||fail(value,!0,message,"==",assert.ok)},assert.equal=function(actual,expected,message){actual!=expected&&fail(actual,expected,message,"==",assert.equal)},assert.notEqual=function(actual,expected,message){actual==expected&&fail(actual,expected,message,"!=",assert.notEqual)},assert.deepEqual=function(actual,expected,message){_deepEqual(actual,expected)||fail(actual,expected,message,"deepEqual",assert.deepEqual)},assert.notDeepEqual=function(actual,expected,message){_deepEqual(actual,expected)&&fail(actual,expected,message,"notDeepEqual",assert.notDeepEqual)},assert.strictEqual=function(actual,expected,message){actual!==expected&&fail(actual,expected,message,"===",assert.strictEqual)},assert.notStrictEqual=function(actual,expected,message){actual===expected&&fail(actual,expected,message,"!==",assert.notStrictEqual)},assert.throws=function(block,error,message){_throws.apply(this,[!0].concat(pSlice.call(arguments)))},assert.doesNotThrow=function(block,error,message){_throws.apply(this,[!1].concat(pSlice.call(arguments)))},assert.ifError=function(err){if(err)throw err},exports})