/// <reference path="../../scripts/app.js" />
/// <reference path="../../scripts/knockout-3.5.0.js" />

(function(sn) {
    var FraudWarningViewModel = function (qvm) {
        var self = this;

        self.viewModelHelper = new SagicorNow.viewModelHelper();
        self.fraudWarningModel = new SagicorNow.FraudWarningModel();
        self.enableSave = ko.observable(false);
        self.passwordCreated = ko.observable(false);
        self.agreeToTerms = ko.observable(false);
        self.quoteViewModel = qvm;

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

        self.enableSave.subscribe(function () {
            if(self.enableSave())
                $("#create-password-modal").modal();
        });

       
    };

    sn.FraudWarningViewModel = FraudWarningViewModel;
}(window.SagicorNow));