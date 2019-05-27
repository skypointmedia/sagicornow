window.SagicorNow = {};

(function(sn) {
    var rootPath;
    sn.rootPath = rootPath;
}(window.SagicorNow));

(function (sn) {
    var mustEqual = function (val, other) {
        return val === other;
    }
    sn.mustEqual = mustEqual;
}(window.SagicorNow));

(function (sn) {
    var ViewModelHelper = function () {

        var self = this;

        self.modelIsValid = ko.observable(true);
        self.modelErrors = ko.observableArray();
        self.isLoading = ko.observable(false);

        self.statePopped = false;
        self.stateInfo = {};

        self.apiGetSync = function (uri, data, success, failure, always) {
            self.isLoading(true);
            self.modelIsValid(true);
            $.ajax({ type: 'Get', async: false, url: SagicorNow.rootPath + uri, data: data })
                .done(success)
                .fail(function (result) {
                    if (failure == null) {
                        if (result.status !== 400)
                            self.modelErrors([result.status + ':' + result.statusText + ' - ' + result.responseText]);
                        else
                            self.modelErrors(JSON.parse(result.responseText));
                        self.modelIsValid(false);
                    }
                    else
                        failure(result);
                })
                .always(function () {
                    if (always == null)
                        self.isLoading(false);
                    else
                        always();
                });
        }

        self.apiPostSync = function (uri, data, success, failure, always) {
            self.isLoading(true);
            self.modelIsValid(true);
            $.ajax({ type: 'Post', async: false, url: SagicorNow.rootPath + uri, data: data })
                .done(success)
                .fail(function (result) {
                    if (failure == null) {
                        if (result.status !== 400)
                            self.modelErrors([result.status + ':' + result.statusText + ' - ' + result.responseText]);
                        else
                            self.modelErrors(JSON.parse(result.responseText));
                        self.modelIsValid(false);
                    }
                    else
                        failure(result);
                })
                .always(function () {
                    if (always == null)
                        self.isLoading(false);
                    else
                        always();
                });
        };

        self.apiGet = function (uri, data, success, failure, always) {
            self.isLoading(true);
            self.modelIsValid(true);
            $.get(SagicorNow.rootPath + uri, data)
                .done(success)
                .fail(function (result) {
                    if (failure == null) {
                        if (result.status != 400)
                            self.modelErrors([result.status + ":" + result.statusText + " - " + result.responseText]);
                        else
                            self.modelErrors(JSON.parse(result.responseText));
                        self.modelIsValid(false);
                    }
                    else
                        failure(result);
                })
                .always(function () {
                    if (always == null)
                        self.isLoading(false);
                    else
                        always();
                });
        };

        self.apiPost = function (uri, data, success, failure, always) {
            self.isLoading(true);
            self.modelIsValid(true);
            $.post(SagicorNow.rootPath + uri, data)
                .done(success)
                .fail(function (result) {
                    if (failure == null) {
                        if (result.status !== 400)
                            self.modelErrors([result.status + ":" + result.statusText + " - " + result.responseText]);
                        else
                            self.modelErrors(JSON.parse(result.responseText));
                        self.modelIsValid(false);
                    }
                    else
                        failure(result);
                })
                .always(function () {
                    if (always == null)
                        self.isLoading(false);
                    else
                        always();
                });
        };

        self.pushUrlState = function (code, title, id, url) {
            self.stateInfo = { State: { Code: code, Id: id }, Title: title, Url: SagicorNow.rootPath + url };
        }

        self.handleUrlState = function (initialState) {
            if (!self.statePopped) {
                if (initialState !== "") {
                    history.replaceState(self.stateInfo.State, self.stateInfo.Title, self.stateInfo.Url);
                    // we're past the initial nav state so from here on everything should push
                    initialState = "";
                }
                else {
                    history.pushState(self.stateInfo.State, self.stateInfo.Title, self.stateInfo.Url);
                }
            }
            else
                self.statePopped = false; // only actual popping of state should set this to true

            return initialState;
        }
    }
    sn.ViewModelHelper = ViewModelHelper;
}(window.SagicorNow));

ko.bindingHandlers.loadingWhen = {
    // any ViewModel using this extension needs a property called isLoading
    // the div tag to contain the loaded content uses a data-bind="loadingWhen: isLoading"
    init: function (element) {
        var
            $element = $(element),
            currentPosition = $element.css("position");
        $loader = $("<div>").addClass("loading-loader").hide();

        //add the loader
        $element.append($loader);

        //make sure that we can absolutely position the loader against the original element
        if (currentPosition == "auto" || currentPosition == "static")
            $element.css("position", "relative");

        //center the loader
        $loader.css({
            position: "absolute",
            top: "50%",
            left: "50%",
            "margin-left": -($loader.width() / 2) + "px",
            "margin-top": -($loader.height() / 2) + "px"
        });
    },
    update: function (element, valueAccessor) {
        var isLoading = ko.utils.unwrapObservable(valueAccessor()),
            $element = $(element),
            $childrenToHide = $element.children(":not(div.loading-loader)"),
            $loader = $element.find("div.loading-loader");

        if (isLoading) {
            $childrenToHide.css("visibility", "hidden").attr("disabled", "disabled");
            $loader.show();
        }
        else {
            $loader.fadeOut("fast");
            $childrenToHide.css("visibility", "visible").removeAttr("disabled");
        }
    }
};