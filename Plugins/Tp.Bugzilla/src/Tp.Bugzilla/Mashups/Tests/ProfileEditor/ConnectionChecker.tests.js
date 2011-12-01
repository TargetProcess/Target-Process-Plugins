require(["Bugzilla/ConnectionChecker"],
    function (connectionChecker) {
        module('test connection checker', {
            setup: function () {
                this.placeholder = $('<div class="svn-settings"><div class="pad-box"><p class="label">Profile Name&nbsp;<span name="NameErrorLabel" class="error"></span></p><input type="text" value="bz" style="width: 275px;" id="name" name="Name" class="input" disabled="disabled"></div><div class="separator"></div><div class="pad-box"><h3 class="h3">Bugzilla Connection</h3><p class="label">Bugzilla Url&nbsp;<span name="UrlErrorLabel" class="error"></span></p><input type="text" style="width: 100%;" value="http://new-bugzilla/bugzilla363" class="input" id="url" name="Url"><br><p class="label"><span class="small">&nbsp;Ex: http://bugzilla.mysite.com</span></p><p class="label pt-10">Login&nbsp;<span name="LoginErrorLabel" class="error"></span></p><input type="text" style="width: 275px;" value="bugzilla@targetprocess.com" id="login" name="Login" class="input"><p class="label pt-10">Password&nbsp;<span name="PasswordErrorLabel" class="error"></span></p><input type="password" value="bugzillaadmin" id="password" name="Password" style="width: 275px;" class="input"><table>   <tbody><tr><td> <p class="label pt-10">TargetProcess Project&nbsp;<span name="ProjectErrorLabel" class="error"></span></p> <select name="Project" id="project" class="select"> <option value="0">- Select project -</option>   <option value="280">#2</option><option value="1">Private Universe #1</option> </select></td><td> <p class="label pt-10">Bugzilla Queries&nbsp;<span name="QueriesErrorLabel" class="error"></span></p> <input type="text" value="alla plugin test" id="queries" name="Queries" style="width: 275px;" class="input"></td>   </tr>  </tbody></table></div> <div class="separator"></div> <div class="check-block"> <p style="display: block;" id="successfulConnection" class="message-ok"><span>Connection was established successfully</span></p> <p style="display:none" id="failedConnection" class="error-message"><span></span></p> <a class="button" id="checkConnection" href="javascript:void(0);">Check Connection</a>&nbsp;<a class="button" id="mapValues" href="javascript:void(0);">Map Values</a><span style="display: none;" class="preloader" id="checkConnectionPreloader"></span> </div> <div> <p class="label"><span name="ProfileErrorLabel" class="error"></span></p> </div></div>');
                this.profileRetriever = function () {
                    return {
                        "Name": "bz",
                        "Settings": {
                            "Login": "bugzilla@targetprocess.com",
                            "Password": "bugzillaadmin",
                            "PrioritiesMapping": null,
                            "Project": 1,
                            "Queries": "alla plugin test",
                            "Url": "http:\/\/new-bugzilla\/bugzilla363"
                        }
                    };
                };

                this.errorMessageContainer = new ErrorMessageContainerMock();
                this.connectionChecker = new connectionChecker({
                    placeholder: this.placeholder,
                    profileRetriever: this.profileRetriever,
                    loaderSelector: 'span#checkConnectionPreloader'
                });
                this.connectionChecker.errorMessageContainer = this.errorMessageContainer;
                this.connectionChecker.initialize();
                this.connectionChecker.commandGateway = {};
                this.successfulCheckResult = { "Statuses": ["UNCONFIRMED", "NEW", "In Development", "RESOLVED", "In Testing", "CLOSED"] };
                this.failedCheckResult = '[{ "FieldName": "Url", "Message": "The remote server returned an error: (404) Not Found."}]';
                this.errorCheckResult = 'error occured';
            },

            teardown: function () {
            }
        });

        test('should check connection with successful result', function () {
            var that = this;
            this.connectionChecker.commandGateway.execute = function (command, data, success) {
                notEqual(that.placeholder.find('span#checkConnectionPreloader').css('display'), 'none', 'preloader is shown during connection check');
                equal(that.placeholder.find('#successfulConnection').css('display'), 'none', 'success message is hidden during connection check');
                equal(that.placeholder.find('#failedConnection').css('display'), 'none', 'failed message is hidden during connection check');
                success(that.successfulCheckResult);
            };

            this.connectionChecker.checkConnection();

            equal(this.placeholder.find('span#checkConnectionPreloader').css('display'), 'none', 'preloader is hidden after connection check finished');
            notEqual(this.placeholder.find('#successfulConnection').css('display'), 'none', 'success message shown if connection check was successful');
            equal(this.placeholder.find('#failedConnection').css('display'), 'none', 'failed message shown if connection check was successful');
        });

        test('should check connection with failed result', function () {
            var that = this;
            this.connectionChecker.commandGateway.execute = function (command, data, success, fail) {
                notEqual(that.placeholder.find('span#checkConnectionPreloader').css('display'), 'none', 'preloader is shown during connection check');
                equal(that.placeholder.find('#successfulConnection').css('display'), 'none', 'success message is hidden during connection check');
                equal(that.placeholder.find('#failedConnection').css('display'), 'none', 'failed message is hidden during connection check');
                fail(that.failedCheckResult);
            };

            this.connectionChecker.checkConnection();

            equal(this.placeholder.find('span#checkConnectionPreloader').css('display'), 'none', 'preloader is hidden after connection check finished');
            equal(this.placeholder.find('#successfulConnection').css('display'), 'none', 'success message is hidden if connection check failed');
            equal(this.errorMessageContainer.errors.length, 1, 'invalid fields passed to error message container');
        });

        test('should clear errors if connection check failed and then succeed', function () {
            var that = this;
            this.connectionChecker.commandGateway.execute = function (command, data, success, fail) {
                fail(that.failedCheckResult);
            };
            this.connectionChecker.checkConnection();

            this.connectionChecker.commandGateway.execute = function (command, data, success) {
                success(that.successfulCheckResult);
            };

            this.connectionChecker.checkConnection();
            equal(this.placeholder.find('span#checkConnectionPreloader').css('display'), 'none', 'preloader is hidden after connection check finished');
            notEqual(this.placeholder.find('#successfulConnection').css('display'), 'none', 'success message shown if connection check was successful');
            equal(this.placeholder.find('#failedConnection').css('display'), 'none', 'error message is hidden if connection check was successful');
            equal(this.errorMessageContainer.errors.length, 0, 'errors has been cleared');
        });

        test('should check connection with error returned', function () {
            var that = this;
            this.connectionChecker.commandGateway.execute = function (command, data, success, fail, error) {
                error(that.errorCheckResult);
            };
            this.connectionChecker.checkConnection();
            equal(this.placeholder.find('span#checkConnectionPreloader').css('display'), 'none', 'preloader is hidden after connection check finished');
            equal(this.placeholder.find('#successfulConnection').css('display'), 'none', 'success message is hidden if connection check returned error');
        });
    });