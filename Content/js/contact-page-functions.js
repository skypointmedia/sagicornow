
document.addEventListener('DOMContentLoaded', function(e) {
    FormValidation.formValidation(
        document.getElementById('contactform'),
        {
            fields: {
            firstName: {
                    validators: {
                        notEmpty: {
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> The First Name is required."
                        }
                    }
                },
                comment: {
                    validators: {
                        notEmpty: {
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please enter a comment."
                        },
                        stringLength: {
                            max: 500,
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> You have exceeded the maximum number of characters allowed."
                        }
                    }
                },
                phone: {
                    validators: {
                        notEmpty: {
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please enter a valid phone number (###-###-####)."
                        },
                        stringLength: {
                            min: 10,
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please enter a valid phone number (###-###-####)",
                            transformer: function($field, validatorName, validator) {
                                var value = $field.val();
                                return value.split('-').join('');
                            }
                        }
                    }
                },
                email: {
                    validators: {
                        emailAddress: {
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> The email address provided appears to be an invalid address."
                        },
                        notEmpty: {
                            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> The email address is required."
                        }
                    }
                }
                
            },

            plugins: {
                bootstrap: new FormValidation.plugins.Bootstrap(),
                recaptcha: new FormValidation.plugins.Recaptcha({
                    element: 'captchaContainer',
                    language: 'en',
                    message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Are you sure you're not a robot?",
                    siteKey: '6LdkmaAUAAAAACTnLRZBQVreocf83sLpENXK4PuO',
                    theme: 'light',
                }),
                defaultSubmit: new FormValidation.plugins.DefaultSubmit(),
                submitButton: new FormValidation.plugins.SubmitButton(),
                trigger: new FormValidation.plugins.Trigger(),
                icon: new FormValidation.plugins.Icon({
                    valid: 'fa fa-check',
                    invalid: 'fa fa-times',
                    validating: 'fa fa-refresh'
                }),

            },
        }
    )
});