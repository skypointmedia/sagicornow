/// <reference path="../../scripts/knockout-3.5.0.debug.js" />
/// <reference path="../../scripts/app.js" />


(function (sn) {
    var ProductSliderModel = function () {

        var self = this;

        self.CoverageAmount = ko.observable(250000);

        self.TenYearTermPerMonthCost = ko.observable(0);
        self.TenYearTerm = ko.observable(false);

        self.FifteenYearTermPerMonthCost = ko.observable(0);
        self.FifteenYearTerm = ko.observable(false);
       
        self.TwentyYearTermPerMonthCost = ko.observable(0);
        self.TwentyYearTerm = ko.observable(false);

        self.WholeLifePerMonthCost = ko.observable(0);
        self.WholeLife = ko.observable(false);

        self.WaiverPremium = ko.observable(false);

        self.AccidentalDeath = ko.observable(false);

        self.ChildrenCoverage = ko.observable(false);
    }
    sn.ProductSliderModel = ProductSliderModel;
}(window.SagicorNow))