/// <reference path="../../scripts/app.js" />
/// <reference path="../../scripts/knockout-3.5.0.js" />

(function(sn) {
    var FraudWarningViewModel = function (pm) {
        // this may become lost based on context.
        var self = this;

        self.viewModelHelper = new SagicorNow.ViewModelHelper();
        self.fraudWarningModel = new SagicorNow.FraudWarningModel();

        // properties
        self.enableSave = ko.observable(false);
        self.passwordCreated = ko.observable(false);
        self.agreeToTerms = ko.observable(false);
        self.proposalModel = pm;
        // --

        self.initialize = function() {
            self.passwordCreated(false);
        }

        self.disableCheckbox = ko.pureComputed(function() {
            return self.passwordCreated();
        });

        self.enableSaveCheckStatus = ko.pureComputed(function () {
            return self.enableSave() ? "fa fa-check checkStyle" : "";
        });

        self.agreeToTermsCheckStatus = ko.pureComputed(function () {
            return self.agreeToTerms() ? "fa fa-check checkStyle" : "";
        });

        self.checkUncheckEnableSave = function () {
            self.enableSave(!self.enableSave());
        };

        self.checkUncheckAgreeToTerms = function () {
            self.agreeToTerms(!self.agreeToTerms());
        };

        // Need to know when the save quote checkbox is clicked /checked.
        self.enableSave.subscribe(function () {

            // forms validation variable needed globally in this event.
            var fv;

            if (self.enableSave()) {
                

            } else {
                self.fraudWarningModel.Password("");
                self.fraudWarningModel.ConfirmPassword("");
            }
        });

        self.submit = function(model) {
            var unmappedModel;
            var fraudWarningModel = ko.mapping.toJS(model);

            unmappedModel = $.extend(unmappedModel, self.proposalModel);
            unmappedModel = $.extend(unmappedModel, fraudWarningModel);

            if (self.enableSave()) {
                self.viewModelHelper.apiPostSync('api/quote/createPassword',
                    unmappedModel,
                    function(result) {
                        if (result) {
                            self.passwordCreated(true);
                        } else
                            self.passwordCreated(false);
                    });

                if (self.passwordCreated())
                    ko.utils.postJson("Quote", self.proposalModel);
            }
            else
                ko.utils.postJson("Quote", self.proposalModel);
        };

        self.goBack = function(model) {
            ko.utils.postJson("ReturnToProductSlider", self.proposalModel);
        }

        self.createPassword = function (model) {

            
        };

        self.initialize();
    };

    sn.FraudWarningViewModel = FraudWarningViewModel;
}(window.SagicorNow));