/// <reference path="../../scripts/app.js" />
/// <reference path="../../scripts/knockout-3.5.0.js" />


(function (sn) {
    var ProductSliderViewModel = function(data, qvm) {

        var self = this;

        self.viewModelHelper = new SagicorNow.viewModelHelper();
        self.productSliderModel = new SagicorNow.ProductSliderModel();
        self.quoteViewModel = qvm;
        

        self.initialize = function () {

            self.productSliderModel.TenYearTermPerMonthCost(data[0].IllustrationResult.ResultBasis.Vector.V[0]);
            self.productSliderModel.FifteenYearTermPerMonthCost(data[1].IllustrationResult.ResultBasis.Vector.V[0]);
            self.productSliderModel.TwentyYearTermPerMonthCost(data[2].IllustrationResult.ResultBasis.Vector.V[0]);
        };

        self.applyNow = function (model) {
            
            var unmappedModel;
            var productSliderModel = ko.mapping.toJS(model);
           
            unmappedModel = $.extend(unmappedModel, self.quoteViewModel);
            unmappedModel = $.extend(unmappedModel, productSliderModel);

            ko.utils.postJson("ProductSlider", unmappedModel);
        };

        self.calculateNeeds = function(model) {
           
        };

        self.getRevisedIllustrationAsync = function (coverage) {
            self.quoteViewModel.CoverageAmount = coverage;
            self.viewModelHelper.apiPost('api/quote/getIllustration', self.quoteViewModel,
                function (result) {
                    self.productSliderModel.TenYearTermPerMonthCost(result[0].IllustrationResult.ResultBasis.Vector.V[0]);
                    self.productSliderModel.FifteenYearTermPerMonthCost(result[1].IllustrationResult.ResultBasis.Vector.V[0]);
                    self.productSliderModel.TwentyYearTermPerMonthCost(result[2].IllustrationResult.ResultBasis.Vector.V[0]);
                    self.productSliderModel.CoverageAmount(coverage);
                });
        };

        self.TenYearTermPerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.TenYearTermPerMonthCost());
        });

        self.FifteenYearTermPerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.FifteenYearTermPerMonthCost());
        });

        self.TwentyYearTermPerMonthCostText = ko.pureComputed(function () {
            return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.productSliderModel.TwentyYearTermPerMonthCost());
        });

        self.WholeLifePerMonthCostText = ko.pureComputed(function () {
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

        self.TenYearTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.TenYearTerm() ? "fa fa-check checkStyle" : "";
        });

        self.FifteenYearTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.FifteenYearTerm() ? "fa fa-check checkStyle" : "";
        });

        self.TwentyYearTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.TwentyYearTerm() ? "fa fa-check checkStyle" : "";
        });

        self.WholeLifeTermCheckStatus = ko.pureComputed(function () {
            return self.productSliderModel.WholeLife() ? "fa fa-check checkStyle" : "";
        });

        self.initialize();
    }
    sn.ProductSliderViewModel = ProductSliderViewModel;
}(window.SagicorNow))