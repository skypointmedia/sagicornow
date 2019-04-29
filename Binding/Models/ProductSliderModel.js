/// <reference path="../../scripts/knockout-3.5.0.debug.js" />
/// <reference path="../../scripts/app.js" />


(function (sn) {
    var ProductSliderModel = function () {

        var self = this;

        self.CoverageAmount = ko.observable(250000);

        self.TenYearTermPerMonthCost = ko.observable(0);
        //self.TenYearTermPerMonthCostText = ko.pureComputed(function() {
        //        return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.TenYearTermPerMonthCost());
        //    });
        self.TenYearTerm = ko.observable(false);
        

        self.FifteenYearTermPerMonthCost = ko.observable(0);
        //self.FifteenYearTermPerMonthCostText = ko.pureComputed(function () {
        //    return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.FifteenYearTermPerMonthCost());
        //});
        self.FifteenYearTerm = ko.observable(false);
       
        self.TwentyYearTermPerMonthCost = ko.observable(0);
        //self.TwentyYearTermPerMonthCostText = ko.pureComputed(function () {
        //    return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.TwentyYearTermPerMonthCost());
        //});
        self.TwentyYearTerm = ko.observable(false);
        

        self.WholelifePerMonthCost = ko.observable(0);
        //self.WholeLifePerMonthCostText = ko.pureComputed(function () {
        //    return "<sup style='font-size: 28px; font-weight: bold'>$</sup>" + Math.ceil(self.WholeLifePerMonthCost());
        //});
        self.Wholelife = ko.observable(false);

        self.WaiverPremium = ko.observable(false);

        self.AccidentalDeath = ko.observable(false);

        self.ChildrenCoverage = ko.observable(false);

        self.FirstName = ko.observable("");

        self.EmailAddress = ko.observable("");

        self.PhoneNumber = ko.observable("");
    }
    sn.ProductSliderModel = ProductSliderModel;
}(window.SagicorNow))