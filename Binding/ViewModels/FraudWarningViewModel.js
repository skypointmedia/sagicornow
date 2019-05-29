/// <reference path="../../scripts/app.js" />
/// <reference path="../../scripts/knockout-3.5.0.js" />

(function(sn) {
    var FraudWarningViewModel = function (pm) {
        // this may become lost based on context.
        var self = this;
        
        // properties
        self.enableSave = ko.observable(false);
        self.passwordCreated = ko.observable(false);
        self.agreeToTerms = ko.observable(false);
        self.proposalModel = pm;
        self.submissionRequested = ko.observable(false);
        self.viewModelHelper = new SagicorNow.ViewModelHelper();
        self.fraudWarningModel = new SagicorNow.FraudWarningModel(self);
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

        self.ageGenderStatement = ko.pureComputed(function() {
            var text = "You are a " + self.proposalModel.Age + " year old ";
            if (self.proposalModel.Gender === "OLI_GENDER_MALE")
                text += "man";
            else
                text += "woman";
            return text;
        });

        self.locationStatement = ko.pureComputed(function() {
            return "You are completing and signing this application in " + self.proposalModel.StateName;
        });

        self.tobaccoUseStatement = ko.pureComputed(function() {
            var text = "You ";
            if (self.proposalModel.Tobacco === "OLI_TOBACCO_NEVER")
                text += "don't ";
            text += "use tobacco / nicotine products";
            return text;
        });

        self.healthStatement = ko.pureComputed(function() {
            return "You are in "+self.getHealthStatus(self.proposalModel.Health)+" health";
        });

        self.coverageStatement = ko.pureComputed(function() {
            var text = "You are buying " +
                window.numeral(self.proposalModel.CoverageAmount).format("$0,0") +
                " of coverage for ";
            if (self.proposalModel.TenYearTerm)
                text += "10 years";
            else if (self.proposalModel.FifteenYearTerm)
                text += "15 years";
            else if (self.proposalModel.TwentyYearTerm)
                text += "20 years";
            else
                text += "for Life";
            return text;
        });

        self.getHealthStatus = function(rc) {
            switch (rc) {
                case "OLI_UNWRITE_SUPERB":
                    return "Superb";
                case "OLI_UNWRITE_PREFERRED":
                    return "Excellent";
                case "OLI_UNWRITE_STANDARD":
                    return "Good";
                case "OLI_UNWRITE_RATED":
                    return "Fair";
                case "OLI_UNWRITE_POOR":
                    return "Poor";
                
                default:
                    return "Unknown";
            }
        };

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

        self.submit = function (model) {
            // Validation requires to know when a submission is made.
            self.submissionRequested(true);

            // we should not move fwd unless product slider model is validated.    
            if (!ko.validatedObservable(model).isValid())
                return;

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