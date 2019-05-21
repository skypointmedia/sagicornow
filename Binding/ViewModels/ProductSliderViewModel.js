/// <reference path="../../scripts/app.js" />
/// <reference path="../../scripts/knockout-3.5.0.js" />
/// <reference path="../models/productSliderModel.js" />


(function (sn) {
    var ProductSliderViewModel = function(data, qvm) {

        var self = this;

        self.viewModelHelper = new SagicorNow.ViewModelHelper();
        self.productSliderModel = new SagicorNow.ProductSliderModel();
        self.quoteViewModel = qvm;
        

        self.initialize = function () {
            self.productSliderModel.TenYearTermPerMonthCost(data[0].IllustrationResult.ResultBasis.Vector.V[0]);
            self.productSliderModel.FifteenYearTermPerMonthCost(data[1].IllustrationResult.ResultBasis.Vector.V[0]);
            self.productSliderModel.TwentyYearTermPerMonthCost(data[2].IllustrationResult.ResultBasis.Vector.V[0]);
            self.productSliderModel.WholeLifePerMonthCost(data[3].IllustrationResult.ResultBasis.Vector.V[0]);
        };

        self.applyNow = function (model) {
            
            var proposalHistoryModel = ko.mapping.toJS(new SagicorNow.ProposalHistoryModel());
            var productSliderModel = ko.mapping.toJS(model);

            proposalHistoryModel.Birthday = self.quoteViewModel.birthday;
            proposalHistoryModel.Gender = self.quoteViewModel.gender;
            proposalHistoryModel.GenderTc = self.quoteViewModel.genderInfo.TC;
            proposalHistoryModel.Health = self.quoteViewModel.health;
            proposalHistoryModel.StateName = self.quoteViewModel.stateInfo.Name;
            proposalHistoryModel.StateCode = self.quoteViewModel.state;
            proposalHistoryModel.StateTc = self.quoteViewModel.stateInfo.TC;
            proposalHistoryModel.Age = self.quoteViewModel.Age;
            proposalHistoryModel.Tobacco = self.quoteViewModel.tobacco;
            proposalHistoryModel.SmokerStatusTc = self.quoteViewModel.smokerStatusInfo.TC;
            proposalHistoryModel.RiskClassTc = self.quoteViewModel.riskClass.TC;
           
            //unmappedModel = $.extend(unmappedModel, self.quoteViewModel);
            //unmappedModel = $.extend(unmappedModel, productSliderModel);

            var data = $.extend(true, proposalHistoryModel, self.quoteViewModel, productSliderModel);

            ko.utils.postJson("FraudWarning", data);
        };

        self.getRevisedIllustrationAsync = function (coverage) {
            self.quoteViewModel.CoverageAmount = coverage;
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

        self.isCoverageGreaterThan250K = ko.pureComputed(function() {
            return self.productSliderModel.CoverageAmount() > 250000;
        });

        self.enableWholeLifeCssClass = ko.pureComputed(function() {
            return self.productSliderModel.CoverageAmount() <= 250000 ? "groupField OuterGroup_WL" :"groupField OuterGroup_WL OuterGroup_WL_Disabled";
        });

        self.coverageAmountText = ko.pureComputed(function() {
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

        self.checkUncheck10Yr = function() {
            self.productSliderModel.TenYearTerm(!self.productSliderModel.TenYearTerm());
        };

        self.checkUncheck15Yr = function () {
            self.productSliderModel.FifteenYearTerm(!self.productSliderModel.FifteenYearTerm());
        };

        self.checkUncheck20Yr = function () {
            self.productSliderModel.TwentyYearTerm(!self.productSliderModel.TwentyYearTerm());
        };

        self.checkUncheckWL = function () {
            self.productSliderModel.WholeLife(!self.productSliderModel.WholeLife());
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

        self.childrenCoverageCheckStatus = ko.pureComputed(function() {
            return self.productSliderModel.ChildrenCoverage() ? "fa fa-check checkStyle" : "";
        });

        self.productSliderModel.AccidentalDeath.subscribe(function(value) {
            if (value)
                self.productSliderModel.AccidentalDeathRiderAmount(self.productSliderModel.CoverageAmount());
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
            if (coverage < 50000 || (age> 55 && coverage > 500000) || (age > 45 && age < 56 && coverage > 750000))
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
        });



        self.initialize();
    }
    sn.ProductSliderViewModel = ProductSliderViewModel;
}(window.SagicorNow))