function CommandGatewayMock(config){
    this._create(config);
}

CommandGatewayMock.prototype = {
    _stubs: null,
    _paused: null,
    _lastParams:null,


    _create: function(config){
        this._paused = false;
        this._stubs = [];
    },

    shouldReturn: function(command, result){
        this._stubs[command] = result;
    },

    pause: function(){
        this._paused = true;
    },

    execute: function(command, data, success){
        this._lastParams = {command: command, data: data, success: success};
        if (!this._paused){
            this.resume();
        }
    },

    resume: function(){
        this._proceed(this._lastParams.success, this._lastParams.command);
        this._paused = false;
        this._lastParams = null;
    },

    _proceed: function(success, command) {
        var result = this._stubs[command];
        if(this._lastParams.data != undefined && result.args != undefined){
            if(JSON.stringify(this._lastParams.data) == JSON.stringify(result.args)){
                success(result.result);
            }
            return;
        }

        success(result);
    }
}