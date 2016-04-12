//this file contains native extensions to first class objects
//consider this file as holder of such kind extensions 

Array.findOne = function(array, finderPredicate) {
    for (var i = 0; i < array.length; i++) {
        if (finderPredicate(array[i]) === true)
            return array[i];
    }
    return null;
}


String.prototype.contains = function(str) {
    return this.indexOf(str) != -1;
}

String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, '');
};

String.prototype.isBlank = function() {
    if (this.trim().length == 0)
        return true;
    else
        return false;
};

/**
 * Subtract hours from UTC time to preserve time after time zone
 * conversion when sending it to the server in AJAX request.
 *
 * Here is the problem: script service proxy converts times
 * from UTC to local time zone after receining JSON response.
 * But we want to it to stay the same time as on the server.
 * Applying this function will revert effect of time zone conversion.
 */

Date.prototype.toLocalDate = function() {
    var localTime = this.getTime();
    var localOffset = this.getTimezoneOffset() * 60000;
    return new Date(localTime - localOffset);
};

/**
 * Add hours to local time to preserve time after time zone
 * conversion when receiving it from the server in AJAX response.
 */

Date.prototype.toUniversalDate = function() {
    var localTime = this.getTime();
    var localOffset = this.getTimezoneOffset() * 60000;
    return new Date(localTime + localOffset);
};

