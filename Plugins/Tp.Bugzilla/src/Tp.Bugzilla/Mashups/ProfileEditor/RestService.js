/**
 * Created by .
 * User: shnyukova
 * Date: 30.06.11
 * Time: 15:47
 * To change this template use File | Settings | File Templates.
 */

tau.mashups.addDependency("libs/jquery/jquery")
    .addModule("Bugzilla/RestService", function() {
        function restService() {
            this._ctor();
        }

        restService.prototype = {
            _requestUrlBase:'/api/v1/',
            _requestsInProgress:0,
            _cache:{},

            _ctor:function() {
            },

            _getUrl:function(entityName) {
                return new Tp.WebServiceURL(this._requestUrlBase + entityName).url;
            },

            //region getUsers

            getUsers:function(projectId, success) {
                // TODO: error processing
                if (parseInt(projectId) <= 0) {
                    success({ "Items":[] });
                }

                $.getJSON(this._getUrl('Projects.asmx/' + projectId + '/ProjectMembers/?include=[User[Id]]&take=1000'),
                    $.proxy(this._onProjectUsersRecieved(success), this));
            },

            _onProjectUsersRecieved:function(onSuccess) {
                return function(users) {
                    this._cache.Users = users.Items;
                    $.getJSON(this._getUrl('Users.asmx?include=[Id,FirstName,LastName,Role,IsActive]&take=1000'), $.proxy(this._onAllUsersRecieved(onSuccess), this));
                };
            },

            _onAllUsersRecieved:function(onSuccess) {
                var that = this;
                return function(allUsers) {
                    var validUsers = $.grep(allUsers.Items, function(element) {
                        var usersInTeam = $.grep(that._cache.Users, function(item) {
                            return element.Id === item.User.Id;
                        });

                        return usersInTeam.length > 0;
                    });

                    if (onSuccess) {
                        onSuccess({ "Items":validUsers });
                    }
                };
            },

            getAvatarUrl:function(id) {
                var url = new Tp.WebServiceURL('/avatar.ashx');
                url.setArgumentValue('size', 24);
                url.setArgumentValue('UserId', id);
                url.setArgumentValue('tick', 0);
                return url.toString();
            },

            //region getBugInfo

            getBugInfo:function(projectId, success) {
                if (parseInt(projectId) <= 0) {
                    success({ "Items":[] });

                    return;
                }

                this._requestsInProgress = 3;

                $.getJSON(this._getUrl('Projects.asmx/' + projectId + '?include=[process[id]]'),
                    $.proxy(this._onProcessRecieved(success), this));

                $.getJSON(this._getUrl('Priorities.asmx?take=1000'),
                    $.proxy(this._onPrioritiesRecieved(success), this));

                $.getJSON(this._getUrl('Severities.asmx?take=1000'),
                    $.proxy(this._onSeveritiesRecieved(success), this));
            },

            _onProcessRecieved: function(success){
                return function(process){
                    $.getJSON(this._getUrl('Processes.asmx/' + process.Process.Id + '/EntityStates/?include=[Id,Name,EntityType[Id,Name]]&take=1000'),
                    $.proxy(this._onEntityStatesRecieved(success), this));
                }
            },

            _onEntityStatesRecieved:function(onSuccess) {
                var that = this;
                return function(states) {
                    that._requestsInProgress--;
                    var source = $(states.Items)
                        .filter(
                        function(index, element) {
                            return element.EntityType.Name == 'Bug';
                        }).map(function(index, element) {
                            return { Id:element.Id, Name:element.Name };
                        });

                    that._cache.states = $.makeArray(source);

                    if (that._requestsInProgress == 0 && onSuccess)
                        onSuccess(that._cache);
                };
            },

            _onPrioritiesRecieved:function(onSuccess) {
                var that = this;
                return function(priorities) {
                    that._requestsInProgress--;
                    var source = $(priorities.Items)
                        .filter(
                        function(index, element) {
                            return element.EntityType.Name == 'Bug';
                        }).map(function(index, element) {
                            return { Id:element.Id, Name:element.Name };
                        });

                    that._cache.priorities = $.makeArray(source);

                    if (that._requestsInProgress == 0 && onSuccess)
                        onSuccess(that._cache);
                };
            },

            _onSeveritiesRecieved:function(onSuccess) {
                var that = this;
                return function(severities) {
                    that._requestsInProgress--;
                    var source = $(severities.Items)
                        .map(function(index, element) {
                            return { Id:element.Id, Name:element.Name };
                        });

                    that._cache.severities = $.makeArray(source);

                    if (that._requestsInProgress == 0 && onSuccess)
                        onSuccess(that._cache);
                };
            },

            //region getProjects

            getContext:function(success) {
                $.getJSON(this._getUrl('/Context.asmx'), $.proxy(this._onGetContext(success), this));
            },

            _onGetContext:function(success) {
                return function(context) {
                    var processes = $.grep(context.Processes.Items, function(process) {
                        var bugTrackingPractice = $.grep(process.Practices.Items, function(practice) {
                            return practice.Name === 'Bug Tracking';
                        });

                        return bugTrackingPractice.length === 1;
                    });

                    var projects = $.grep(context.SelectedProjects.Items, function(project) {
                        var isValid = false;
                        $.each(processes, function(index, process) {
                            if (project.Process.Id === process.Id) {
                                isValid = true;
                            }
                        });

                        return isValid;
                    });

                    success({projects:projects, processes:processes});
                };
            },

            //region getRoles

            getRoles:function(processes, success) {

                var processList = $.map(processes,
                    function(item) {
                        return item.Id;
                    }).join(',');
                $.getJSON(this._getUrl('/EntityStates.asmx?where=(Entitytype.name eq "Bug") and (Process.ID in (' + processList + ')) and (Role is not null)&take=1000'), $.proxy(this._onRolesReceived(success), this));
            },

            _onRolesReceived:function(success) {
                var that = this;
                return function(data) {
                    var items = $.map(data.Items, function(item) {
                        return item.Role;
                    });
                    that._cache.roles = [];
                    for (var i = 0; i < items.length; i++) {
                        if ($.grep(that._cache.roles,
                            function(item) {
                                return item.Id == items[i].Id;
                            }).length == 0) {
                            that._cache.roles.push(items[i]);
                        }
                    }
                    success(that._cache.roles);
                };
            }
        };

        return restService;
    });
