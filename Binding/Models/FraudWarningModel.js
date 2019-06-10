/// <reference path="../../scripts/knockout-3.5.0.debug.js" />
/// <reference path="../../scripts/app.js" />

(function (sn) {
    var FraudWarningModel = function (vm) {

        var self = this;

        self.checkPassword = function (val, other) {
            return val == other;
        }


        self.FirstName = ko.observable("").extend({
            required: { message: "First name is required" }
        });

        self.EmailAddress = ko.observable("").extend({
            required: { message: "Login email is required" },
            email: { message: "Login is not a valid email" }
        });

        self.PhoneNumber = ko.observable("").extend({
            required: { message: "A contact number is required" }
        });

        self.Password = ko.observable("").extend({
            required: {
                onlyIf: function () {
                    return vm.enableSave();
                },
                message: "Password is required"
            },
            minLength: { message: "Password must be at least 6 characters", params: 6 }
        });

        self.ConfirmPassword = ko.observable("").extend({
            validation: {
                validator: function (val, other) {
                    return val === self.Password();
                },
                message: 'Passwords do not match!'
            }  
        });

        self.EnableSave = ko.observable(false);

    };
    sn.FraudWarningModel = FraudWarningModel;
}(window.SagicorNow));