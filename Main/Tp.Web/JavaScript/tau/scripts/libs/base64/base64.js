/*
Copyright (c) 2008 Fred Palmer fred.palmer_at_gmail.com

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

define([],function(){function StringBuffer(){this.buffer=[]}function Utf8EncodeEnumerator(input){this._input=input,this._index=-1,this._buffer=[]}function Base64DecodeEnumerator(input){this._input=input,this._index=-1,this._buffer=[]}StringBuffer.prototype.append=function(string){return this.buffer.push(string),this},StringBuffer.prototype.toString=function e(){return this.buffer.join("")};var Base64={codex:"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",encode:function(input){var output=new StringBuffer,enumerator=new Utf8EncodeEnumerator(input);while(enumerator.moveNext()){var chr1=enumerator.current;enumerator.moveNext();var chr2=enumerator.current;enumerator.moveNext();var chr3=enumerator.current,enc1=chr1>>2,enc2=(chr1&3)<<4|chr2>>4,enc3=(chr2&15)<<2|chr3>>6,enc4=chr3&63;isNaN(chr2)?enc3=enc4=64:isNaN(chr3)&&(enc4=64),output.append(this.codex.charAt(enc1)+this.codex.charAt(enc2)+this.codex.charAt(enc3)+this.codex.charAt(enc4))}return output.toString()},decode:function(input){var output=new StringBuffer,enumerator=new Base64DecodeEnumerator(input);while(enumerator.moveNext()){var charCode=enumerator.current;if(charCode<128)output.append(String.fromCharCode(charCode));else if(charCode>191&&charCode<224){enumerator.moveNext();var charCode2=enumerator.current;output.append(String.fromCharCode((charCode&31)<<6|charCode2&63))}else{enumerator.moveNext();var charCode2=enumerator.current;enumerator.moveNext();var charCode3=enumerator.current;output.append(String.fromCharCode((charCode&15)<<12|(charCode2&63)<<6|charCode3&63))}}return output.toString()}};return Utf8EncodeEnumerator.prototype={current:Number.NaN,moveNext:function(){if(this._buffer.length>0)return this.current=this._buffer.shift(),!0;if(this._index>=this._input.length-1)return this.current=Number.NaN,!1;var charCode=this._input.charCodeAt(++this._index);return charCode==13&&this._input.charCodeAt(this._index+1)==10&&(charCode=10,this._index+=2),charCode<128?this.current=charCode:charCode>127&&charCode<2048?(this.current=charCode>>6|192,this._buffer.push(charCode&63|128)):(this.current=charCode>>12|224,this._buffer.push(charCode>>6&63|128),this._buffer.push(charCode&63|128)),!0}},Base64DecodeEnumerator.prototype={current:64,moveNext:function(){if(this._buffer.length>0)return this.current=this._buffer.shift(),!0;if(this._index>=this._input.length-1)return this.current=64,!1;var enc1=Base64.codex.indexOf(this._input.charAt(++this._index)),enc2=Base64.codex.indexOf(this._input.charAt(++this._index)),enc3=Base64.codex.indexOf(this._input.charAt(++this._index)),enc4=Base64.codex.indexOf(this._input.charAt(++this._index)),chr1=enc1<<2|enc2>>4,chr2=(enc2&15)<<4|enc3>>2,chr3=(enc3&3)<<6|enc4;return this.current=chr1,enc3!=64&&this._buffer.push(chr2),enc4!=64&&this._buffer.push(chr3),!0}},Base64})