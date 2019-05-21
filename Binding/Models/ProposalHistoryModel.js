/// <reference path="../../scripts/knockout-3.5.0.debug.js" />
/// <reference path="../../scripts/app.js" />

(function (sn) {
    var ProposalHistoryModel = function () {

        var self = this;

        self.StateName = ko.observable("");
        self.StateCode = ko.observable("");
        self.StateTc = ko.observable("");
        self.Gender = ko.observable("");
        self.GenderTc = ko.observable("");
        self.Birthday = ko.observable("");
        self.Age = ko.observable("");
        self.Tobacco = ko.observable("");
        self.SmokerStatusTc = ko.observable("");
        self.Health = ko.observable("");
        self.RiskClassTc = ko.observable("");
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
        self.FirstName = ko.observable("");
        self.PhoneNumber = ko.observable("");
    }
    sn.ProposalHistoryModel = ProposalHistoryModel;
}(window.SagicorNow))