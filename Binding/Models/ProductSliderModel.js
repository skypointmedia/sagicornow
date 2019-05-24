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
        self.AccidentalDeathRiderAmount = ko.observable(0).extend({
            required: {
                onlyIf: function() { return self.AccidentalDeath(); }
            },
            number: true,
            min: 50000
        });

        self.ChildrenCoverage = ko.observable(false);
        self.ChildrenCoverageRiderAmount = ko.observable(2000).extend({
            min: 2000,
            max: 20000,
            message: "You entered an invalid amount. See tool-tip for more information.",
            required: {
                onlyIf: function () { return self.ChildrenCoverage(); }
            },
            number: true
        });
        self.AgeOfYoungest = ko.observable(1).extend({
            max: 19, message: "Child's age cannot exceed 19",
            required: {
                onlyIf: function () { return self.ChildrenCoverage(); }
            },
            number: true
        });
    }
    sn.ProductSliderModel = ProductSliderModel;
}(window.SagicorNow))