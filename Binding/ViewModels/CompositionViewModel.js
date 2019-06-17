/// <reference path="../../scripts/app.js" />
/// <reference path="../../scripts/knockout-3.5.0.js" />
/// <reference path="../models/productSliderModel.js" />
/// <reference path="../models/proposalHistoryModel.js" />
/// <reference path="../models/fraudWarningModel.js" />


(function (sn) {
    var compositionViewModel = function (data, qvm) {

        var self = this;

        var initialState = 'composition'; // represents the first view mode - in other VMs, can come through an argument

        // Properties
        self.quoteViewModel = qvm;
        self.submissionRequested = ko.observable(false);
        self.ccRiderAmountFormatted = ko.observable("2,000");
        self.adRiderAmountFormatted = ko.observable("250,000");
        self.viewMode = ko.observable();

        self.enableSave = ko.observable(false);
        self.passwordCreated = ko.observable(false);
        self.agreeToTerms = ko.observable(false);
        self.submissionRequested2 = ko.observable(false);
        self.phoneNumberFormatted = ko.observable("");
        self.phoneNumberMask = null;
        self.ccRiderAmountMask = null;
        self.adRiderAmountMask = null;

        self.viewModelHelper = new SagicorNow.ViewModelHelper();
        self.productSliderModel = new SagicorNow.ProductSliderModel();
        self.fraudWarningModel = new SagicorNow.FraudWarningModel(self);

        // Methods
        self.initialize = function () {
            if (data[0].IllustrationResult)
                self.productSliderModel.TenYearTermPerMonthCost(data[0].IllustrationResult.ResultBasis.Vector.V[0]);
            if (data[1].IllustrationResult)
                self.productSliderModel.FifteenYearTermPerMonthCost(data[1].IllustrationResult.ResultBasis.Vector.V[0]);
            if (data[2].IllustrationResult)
                self.productSliderModel.TwentyYearTermPerMonthCost(data[2].IllustrationResult.ResultBasis.Vector.V[0]);
            if (data[3].IllustrationResult)
                self.productSliderModel.WholeLifePerMonthCost(data[3].IllustrationResult.ResultBasis.Vector.V[0]);

            self.viewMode('step1'); // this will perform the first url-state handling round.

            self.passwordCreated(false);
        };

        self.goBack = function (model) {
            self.viewModelHelper.modelIsValid(true);
            if (self.viewMode() == 'step2') {
                self.viewMode('step1');
            }
        };

        self.applyMask = function () {
            var ccRiderAmountInputElement = document.getElementById("CHILD_RIDER_AMOUNT_FORMATTED");
            var adRiderAmountInputElement = document.getElementById("ADB_AMOUNT");

            self.ccRiderAmountMask = window.IMask(ccRiderAmountInputElement, {
                mask: Number,
                scale: 2,  // digits after point, 0 for integers
                signed: false,  // disallow negative
                thousandsSeparator: ","
            });

            self.adRiderAmountMask = window.IMask(adRiderAmountInputElement, {
                mask: Number,
                scale: 2,  // digits after point, 0 for integers
                signed: false,  // disallow negative
                thousandsSeparator: ","
            });
        };

        self.applyMask2 = function () {
            var phoneNumberInputElement = document.getElementById("PROPOSED_INSURED_HOME_PHONE");

            self.phoneNumberMask = window.IMask(phoneNumberInputElement,
                {
                    mask: '+{1}(000)000-0000'
                });
        };

        self.viewMode.subscribe(function () {
            switch (self.viewMode()) {
                case 'step1':
                    self.viewModelHelper.pushUrlState('step1', null, null, 'quote/composition');
                    break;
                case 'step2':
                    self.viewModelHelper.pushUrlState('step2', null, null, 'quote/composition');
                    break;
            }

            initialState = self.viewModelHelper.handleUrlState(initialState);
        });

        if (window.Modernizr.history) {
            window.onpopstate = function (arg) {
                if (arg.state != null) {
                    self.viewModelHelper.statePopped = true;
                    self.viewMode(arg.state.Code);
                }
            };
        }

        self.applyNow = function (model) {

            // Validation requires to know when a submission is made.
            self.submissionRequested(true);

            // we should not move fwd unless product selection is validated.           
            if (!self.isValidProductSelection())
                return;

            // we should not move fwd unless product slider model is validated.    
            if (!ko.validatedObservable(model).isValid())
                return;

            self.viewMode('step2');
        };

        self.getRevisedIllustrationAsync = function (coverage) {

            // we should not move fwd unless product slider model is validated.    
            if (!ko.validatedObservable(self.productSliderModel).isValid())
                return;

            self.quoteViewModel.CoverageAmount = coverage;

            // Must keep both models in sync.
            self.quoteViewModel.AccidentalDeath = self.productSliderModel.AccidentalDeath();
            self.quoteViewModel.WaiverPremium = self.productSliderModel.WaiverPremium();
            self.quoteViewModel.ChildrenCoverage = self.productSliderModel.ChildrenCoverage();
            self.quoteViewModel.AccidentalDeathRiderAmount = self.productSliderModel.AccidentalDeathRiderAmount();
            self.quoteViewModel.ChildrenCoverageRiderAmount = self.productSliderModel.ChildrenCoverageRiderAmount();
            self.quoteViewModel.AgeOfYoungest = self.productSliderModel.AgeOfYoungest();

            self.viewModelHelper.apiPost('api/quote/getIllustration', self.quoteViewModel,
                function (result) {
                    if (result[0].IllustrationResult)
                        self.productSliderModel.TenYearTermPerMonthCost(result[0].IllustrationResult.ResultBasis.Vector
                            .V[0]);
                    else
                        self.productSliderModel.TenYearTermPerMonthCost(0);

                    if (result[1].IllustrationResult)
                        self.productSliderModel.FifteenYearTermPerMonthCost(result[1].IllustrationResult.ResultBasis
                            .Vector.V[0]);
                    else
                        self.productSliderModel.FifteenYearTermPerMonthCost(0);

                    if (result[2].IllustrationResult)
                        self.productSliderModel.TwentyYearTermPerMonthCost(result[2].IllustrationResult.ResultBasis
                            .Vector.V[0]);
                    else
                        self.productSliderModel.TwentyYearTermPerMonthCost(0);

                    if (result[3].IllustrationResult)
                        self.productSliderModel.WholeLifePerMonthCost(
                            result[3].IllustrationResult.ResultBasis.Vector.V[0]);
                    else
                        self.productSliderModel.WholeLifePerMonthCost(0);

                    self.productSliderModel.CoverageAmount(coverage);

                    if (self.productSliderModel.AccidentalDeath())
                        self.productSliderModel.AccidentalDeathRiderAmount(coverage);
                });
        };

        self.checkStatus = function (c) {
            self.productSliderModel.TenYearTerm(c === 10 && !self.disable10YrTermProduct());
            self.productSliderModel.FifteenYearTerm(c === 15 && !self.disable15YrTermProduct());
            self.productSliderModel.TwentyYearTerm(c === 20 && !self.disable20YrTermProduct());
            self.productSliderModel.WholeLife(c === 100 && !self.disableWholeLifeProduct());

            // Must keep both models in sync.
            self.quoteViewModel.TenYearTerm = (c === 10 && !self.disable10YrTermProduct());
            self.quoteViewModel.FifteenYearTerm = (c === 15 && !self.disable15YrTermProduct());
            self.quoteViewModel.TwentyYearTerm = (c === 20 && !self.disable20YrTermProduct());
            self.quoteViewModel.WholeLife = (c === 100 && !self.disableWholeLifeProduct());
        };

        self.checkUncheck10Yr = function () {
            self.checkStatus(10);
        };

        self.checkUncheck15Yr = function () {
            self.checkStatus(15);
        };

        self.checkUncheck20Yr = function () {
            self.checkStatus(20);
        };

        self.checkUncheckWL = function () {
            self.checkStatus(100);
        };

        self.checkUncheckWavierPremium = function () {
            if (self.quoteViewModel.Age > 55 || self.quoteViewModel.Age < 18)
                self.productSliderModel.WaiverPremium(false);
            else
                self.productSliderModel.WaiverPremium(!self.productSliderModel.WaiverPremium());
        };

        self.checkUncheckAccidentalDeath = function () {
            if (self.quoteViewModel.Age > 60)
                self.productSliderModel.AccidentalDeath(false);
            else
                self.productSliderModel.AccidentalDeath(!self.productSliderModel.AccidentalDeath());
        };

        self.checkUncheckChildrenCoverage = function () {
            self.productSliderModel.ChildrenCoverage(!self.productSliderModel.ChildrenCoverage());
        };

        //-----

        self.getHealthStatus = function (rc) {
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

        self.phoneNumberFormatted.subscribe(function () {
            self.fraudWarningModel.PhoneNumber(self.phoneNumberMask.unmaskedValue);
        });

        self.submit = function (model) {
            // Validation requires to know when a submission is made.
            self.submissionRequested2(true);

            // we should not move fwd unless product slider model is validated.    
            if (!ko.validatedObservable(model).isValid())
                return;

            var unmappedModel;
            var fraudWarningModel = ko.mapping.toJS(model);

            unmappedModel = $.extend(unmappedModel, self.quoteViewModel);
            unmappedModel = $.extend(unmappedModel, fraudWarningModel);

            // the objects are currently in an KO Observable form; need them in plain JS.
            var proposalHistoryModel = ko.mapping.toJS(new SagicorNow.ProposalHistoryModel());
            var productSliderModel = ko.mapping.toJS(self.productSliderModel);

            // not all the values needed are in the product slider model.
            proposalHistoryModel.Birthday = self.quoteViewModel.birthday;
            proposalHistoryModel.Gender = self.quoteViewModel.gender;
            proposalHistoryModel.GenderTc = self.quoteViewModel.genderInfo.TC;
            proposalHistoryModel.Health = self.quoteViewModel.health;
            proposalHistoryModel.StateName = self.quoteViewModel.stateInfo.Name;
            proposalHistoryModel.StateCode = self.quoteViewModel.state;
            proposalHistoryModel.StateTc = self.quoteViewModel.stateInfo.TC;
            proposalHistoryModel.Age = self.quoteViewModel.Age;
            proposalHistoryModel.AgeOfYoungest = self.quoteViewModel.AgeOfYoungest;
            proposalHistoryModel.Tobacco = self.quoteViewModel.tobacco;
            proposalHistoryModel.SmokerStatusTc = self.quoteViewModel.smokerStatusInfo.TC;
            proposalHistoryModel.RiskClassTc = self.quoteViewModel.riskClass.TC;

            var data = $.extend(true, proposalHistoryModel, productSliderModel);

            if (self.enableSave()) {
                self.viewModelHelper.apiPostSync('api/quote/createPassword',
                    unmappedModel,
                    function (result) {
                        if (result) {
                            self.passwordCreated(true);
                        } else
                            self.passwordCreated(false);
                    });

                if (self.passwordCreated())
                    ko.utils.postJson("EmbeddedApp", { vm: data });
            } else
                ko.utils.postJson("EmbeddedApp", { vm: data });
        };


        // Computed Properties.
        self.isCoverageGreaterThan250K = ko.pureComputed(function () {
            return self.productSliderModel.CoverageAmount() > 250000;
        });

        self.enableWholeLifeCssClass = ko.pureComputed(function () {
            return self.disableWholeLifeProduct() ? "groupField OuterGroup_WL OuterGroup_WL_Disabled" : "groupField OuterGroup_WL";
        });

        self.coverageAmountText = ko.pureComputed(function () {
            return window.numeral(self.productSliderModel.CoverageAmount()).format("$0,0");
        });

        self.tenYearTermPerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.TenYearTermPerMonthCost());
        });

        self.fifteenYearTermPerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.FifteenYearTermPerMonthCost());
        });

        self.twentyYearTermPerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.TwentyYearTermPerMonthCost());
        });

        self.wholeLifePerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.WholeLifePerMonthCost());
        });

        self.isValidProductSelection = ko.pureComputed(function () {
            return self.productSliderModel.TenYearTerm() ||
                self.productSliderModel.FifteenYearTerm() ||
                self.productSliderModel.TwentyYearTerm() ||
                self.productSliderModel.WholeLife();
        });

        self.tenYearTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.TenYearTerm() ? "fa fa-check checkStyle" : "";
        });

        self.fifteenYearTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.FifteenYearTerm() ? "fa fa-check checkStyle" : "";
        });

        self.twentyYearTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.TwentyYearTerm() ? "fa fa-check checkStyle" : "";
        });

        self.wholeLifeTermCheckStatus = ko.pureComputed(function () {
            if (self.isCoverageGreaterThan250K()) {
                return "fa fa-check checkStyle checkStyle--hidden";
            }
            return self.productSliderModel.WholeLife() ? "fa fa-check checkStyle" : "";
        });

        self.wavierOfPremiumCheckStatus = ko.pureComputed(function () {
            if (self.quoteViewModel.Age > 55 || self.quoteViewModel.Age < 18)
                return "fa fa-check checkStyle checkStyle--hidden";
            return self.productSliderModel.WaiverPremium() ? "fa fa-check checkStyle" : "";
        });

        self.accidentalDeathCheckStatus = ko.pureComputed(function () {
            if (self.quoteViewModel.Age > 60)
                return "fa fa-check checkStyle checkStyle--hidden";
            return self.productSliderModel.AccidentalDeath() ? "fa fa-check checkStyle" : "";
        });

        self.childrenCoverageCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.ChildrenCoverage() ? "fa fa-check checkStyle" : "";
        });

        self.disable10YrTermProduct = ko.pureComputed(function () {
            var coverage = self.productSliderModel.CoverageAmount();
            var age = self.quoteViewModel.Age;
            if (coverage < 50000 || (age > 55 && coverage > 500000) || (age > 45 && age < 56 && coverage > 750000))
                return true;
            else
                return false;
        });

        self.disable15YrTermProduct = ko.pureComputed(function () {
            var coverage = self.productSliderModel.CoverageAmount();
            var age = self.quoteViewModel.Age;
            if (coverage < 50000 || (age > 55 && coverage > 500000) || (age > 45 && age < 56 && coverage > 750000))
                return true;
            else
                return false;
        });

        self.disable20YrTermProduct = ko.pureComputed(function () {
            var coverage = self.productSliderModel.CoverageAmount();
            var age = self.quoteViewModel.Age;
            if (coverage < 50000 || (age > 55 && coverage > 500000) || (age > 45 && age < 56 && coverage > 750000))
                return true;
            else
                return false;
        });

        self.disableWholeLifeProduct = ko.pureComputed(function () {
            var coverage = self.productSliderModel.CoverageAmount();
            var age = self.quoteViewModel.Age;
            var riskClass = self.quoteViewModel.riskClass.Value;
            if (riskClass === "OLI_UNWRITE_SUPERB" ||
                riskClass === "OLI_UNWRITE_POOR" ||
                coverage < 25000 ||
                coverage > 250000 ||
                (age > 55 && coverage > 500000) ||
                (age > 45 && age < 56 && coverage > 750000))
                return true;
            else
                return false;
        });

        self.ageOfYoungestValidationMessage = ko.pureComputed(function () {
            if (self.productSliderModel.AgeOfYoungest() > 19) {
                return "Child's age cannot exceed 19.";
            }
            return "This is a required field.";
        });

        self.hasADBRider = ko.pureComputed(function() {
            return self.productSliderModel.AccidentalDeath();
        });

        self.hasWOPRider = ko.pureComputed(function() {
            return self.productSliderModel.WaiverPremium();
        });

        self.hasCCRider = ko.pureComputed(function() {
            return self.productSliderModel.ChildrenCoverage();
        });

        //---

        self.disableCheckbox = ko.pureComputed(function () {
            return self.passwordCreated();
        });

        self.enableSaveCheckStatus = ko.pureComputed(function () {
            return self.enableSave() ? "fa fa-check checkStyle" : "";
        });

        self.agreeToTermsCheckStatus = ko.pureComputed(function () {
            return self.agreeToTerms() ? "fa fa-check checkStyle" : "";
        });

        self.ageGenderStatement = ko.pureComputed(function () {
            var text = "You are a " + self.quoteViewModel.Age + " year old ";
            if (self.quoteViewModel.Gender === "OLI_GENDER_MALE")
                text += "man";
            else
                text += "woman";
            return text;
        });

        self.locationStatement = ko.pureComputed(function () {
            return "You are completing and signing this application in " + self.quoteViewModel.StateName;
        });

        self.tobaccoUseStatement = ko.pureComputed(function () {
            var text = "You ";
            if (self.quoteViewModel.Tobacco === "OLI_TOBACCO_NEVER")
                text += "don't ";
            text += "use tobacco / nicotine products";
            return text;
        });

        self.healthStatement = ko.pureComputed(function () {
            return "You are in " + self.getHealthStatus(self.quoteViewModel.Health) + " health";
        });

        self.coverageStatement = ko.pureComputed(function () {
            var text = "You are buying " +
                window.numeral(self.productSliderModel.CoverageAmount()).format("$0,0") +
                " of coverage for ";
            if (self.productSliderModel.TenYearTerm())
                text += "10 years";
            else if (self.productSliderModel.FifteenYearTerm())
                text += "15 years";
            else if (self.productSliderModel.TwentyYearTerm())
                text += "20 years";
            else
                text += "for Life";
            return text;
        });

        self.AccidentalDeathStatement = ko.pureComputed(function () {
            var text = "You are adding a " +
                window.numeral(self.productSliderModel.AccidentalDeathRiderAmount()).format("$0,0") +
                " Accidental Death Benefit Rider to your policy";
            return text;
        });

        self.childrenCoverageStatement = ko.pureComputed(function () {
            var text = "You are adding a " +
                window.numeral(self.productSliderModel.ChildrenCoverageRiderAmount()).format("$0,0") +
                " Child Benefit Rider to your policy";
            return text;
        });

        // Event Handlers 
        self.productSliderModel.AccidentalDeath.subscribe(function (value) {
            if (value)
                self.productSliderModel.AccidentalDeathRiderAmount(self.productSliderModel.CoverageAmount());

            self.getRevisedIllustrationAsync(self.quoteViewModel.CoverageAmount);
        });

        self.productSliderModel.AgeOfYoungest.subscribe(function (value) {
            self.getRevisedIllustrationAsync(self.quoteViewModel.CoverageAmount);
        });

        self.productSliderModel.AccidentalDeathRiderAmount.subscribe(function (value) {
            self.getRevisedIllustrationAsync(self.quoteViewModel.CoverageAmount);
        });

        self.productSliderModel.ChildrenCoverage.subscribe(function () {
            self.getRevisedIllustrationAsync(self.quoteViewModel.CoverageAmount);
        });

        self.productSliderModel.ChildrenCoverageRiderAmount.subscribe(function () {
            self.getRevisedIllustrationAsync(self.quoteViewModel.CoverageAmount);
        });

        self.productSliderModel.WaiverPremium.subscribe(function () {
            self.getRevisedIllustrationAsync(self.quoteViewModel.CoverageAmount);
        });

        self.ccRiderAmountFormatted.subscribe(function () {
            self.productSliderModel.ChildrenCoverageRiderAmount(self.ccRiderAmountMask.unmaskedValue);
        });

        self.adRiderAmountFormatted.subscribe(function () {
            self.productSliderModel.AccidentalDeathRiderAmount(self.adRiderAmountMask.unmaskedValue);
        });

        self.initialize();
    }

    sn.CompositionViewModel = compositionViewModel;
}(window.SagicorNow))