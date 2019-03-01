// Set page focus to zipcode input on pageload
/*
// Mask all dollar inputs to look pretty.
$(function() { $('#final-expenses, #mortgages, #other-debts, #total-annual-income, #current-savings, #current-retirement-savings, #existing-life-insurance-value, #spouses-annual-income').mask("#,##0.00", {reverse: true});
});
*/
// Validate the form inputs before submiting to the server
$(document).ready(function () {

    //Calculate and display age based on birthday
    var d = new Date();
    d.setDate(d.getDate() - 1);
    var strDate = (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear();

    function getAge(dateString) {
        var today = new Date();
        var birthDate = new Date(dateString);
        var age = today.getFullYear() - birthDate.getFullYear();
        var m = today.getMonth() - birthDate.getMonth();
        if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }
        return age;
    }

    // Window resize 
    function resize() {
        $('#resize').css('height', window.innerHeight - '360');
    }
    resize();

    window.onresize = function () {
        resize();
    };


    // Tool tips
    $('[data-toggle="tooltip"]').tooltip();


    //$('#code').focus();
    //mask input
    $('#birthday').mask('99/99/9999');
    $('#txt-ssn').mask('999-99-9999');
    $('input[placeholder="12345"]').mask('99999');
    $('.two-digits').mask('99');
    $('input[placeholder="Phone Number"]').mask('999-999-9999');
    $('input[placeholder="___-___-____"]').mask('999-999-9999');

    //quote page functions
    var stateSelected = $('select[name="state"]').find('option:selected').val();

    //quote page functions

    if (stateSelected == "FL" || stateSelected == "IL" || stateSelected == "IN" || stateSelected == "NV" || stateSelected == "WA" || stateSelected == "WY") {
        $('.replacement-policy-box').show();
    } else {
        $('.replacement-policy-box').hide();

    }

    $('select[name="state"]').on('change', function () {

        var stateSelected = $(this).find('option:selected').val();

        if (stateSelected != "FL" &&
            stateSelected != "IL" &&
            stateSelected != "IN" &&
            stateSelected != "NV" &&
            stateSelected != "WA" &&
            stateSelected != "WY"
        ) {
            $('.replacement-policy-box').hide();
        } else {
            $('.replacement-policy-box').show();
        }
    });

    //Get all textareas that have a "maxlength" property. Now, and when later adding HTML using jQuery-scripting:
    $('textarea[maxlength]').on('keyup blur', function () {
        // Store the maxlength and value of the field.
        var maxlength = $(this).attr('maxlength');
        var val = $(this).val();

        // Trim the field if it has content over the maxlength.
        if (val.length > maxlength) {
            $(this).val(val.slice(0, maxlength));
        }
    });

    var moneyfieldsValidators = {
        notEmpty: {
            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> This is a required field"
        },
        stringLength: {
            max: 9,
            message: 'Number too large',
            transformer: function ($field, validatorName, validator) {
                var value = $field.val();
                return value.split(',').join('').replace('$', '');
            }
        },
        digits: {
            message: 'The value must be a whole number',
            transformer: function ($field, validatorName, validator) {
                var value = $field.val();
                return value.split(',').join('').replace('$', '');
            }
        }
    };
    var ssnfieldsValidators = {
        notEmpty: {
            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Social Security Number is required."
        },
        stringLength: {
            max: 9,
            message: 'Number too large',
            transformer: function ($field, validatorName, validator) {
                var value = $field.val();
                return value.split('-').join('');
            }
        },
        digits: {
            message: 'Only numbers are allowed.',
            transformer: function ($field, validatorName, validator) {
                var value = $field.val();
                return value.split('-').join('');
            }
        }
    };

    var requiredFieldValidators = {
        notEmpty: {
            message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> This is a required field"
        }
    };

    //initialize the form validation
    $('#quote-form').formValidation({
        framework: 'Bootstrap4',
        // Feedback icons
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        // List of fields and their validation rules
        fields: {
            state: {
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Your State is required"
                    }
                },
            },
            SocialSecurityNumber: {
                validators: ssnfieldsValidators
            },
            birthday: {
                threshold: 10,
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Date of Birth is required"
                    },
                    date: {
                        max: strDate,
                        format: 'MM/DD/YYYY',
                        message: "<span class='error_img'> </span> Enter a valid date in MM/DD/YYYY format"
                    }
                },
                onSuccess: function () {
                    var dob = $("#birthday").val();
                    $("#birthday_message").empty();
                    $("#birthday_message").append("<small><strong><i>You are " + getAge(dob) + " years old.</i></strong></small>");
                },
                onError: function () {
                    $("#birthday_message").empty();
                }
            },
            gender: {
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please confirm Gender"
                    }
                }
            },
            tobacco: {
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please confirm Tobacco Use"
                    }
                }
            },
            health: {
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please confirm Overall Health"
                    }
                }
            },
            zipcode: {
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please enter your Zip Code"
                    }
                }
            },
            finalExpenses: {
                validators: moneyfieldsValidators
            },
            mortgageExpenses: {
                validators: moneyfieldsValidators
            },
            otherDebtExpenses: {
                validators: moneyfieldsValidators
            },
            totalAnnualIncome: {
                validators: moneyfieldsValidators
            },
            currentSavings: {
                validators: moneyfieldsValidators
            },
            currentRetirementSavings: {
                validators: moneyfieldsValidators
            },
            existingLifeInsuranceValue: {
                validators: moneyfieldsValidators
            },
            spouseTotalAnnualIncome: {
                validators: moneyfieldsValidators
            },
            yearsOfIncomeLeft: {
                validators: requiredFieldValidators
            },
            spouseYearsOfIncomeLeft: {
                validators: requiredFieldValidators
            },
            ReplacementPolicy: {
                validators: {
                    notEmpty: {
                        message: "<i class='fa fa-exclamation-circle' aria-hidden='true'></i> Please confirm if this is a replacement"
                    }
                }
            },
            familyMakeup: {
                validators: requiredFieldValidators
            },
            numKidsNeedFunding: {
                validators: requiredFieldValidators
            },
            schoolType: {
                validators: requiredFieldValidators
            }
        }
    })
        // Revalidate replacement policy question when submitting the form
        .on('submit', function (e) {
            $('#quote-form').formValidation('revalidateField', 'ReplacementPolicy');

            $('.money').each(function () {
                var elemName = $(this).attr('name');
                $('#quote-form').data('formValidation').revalidateField(elemName);
                var value = $(this).val();
                $(this).val(value.split(',').join('').replace('$', ''));
            });
        })
        .on('blur', '.money', function (e) {
            var fv = $('#quote-form').data('formValidation');
            var fn = $(this).attr('name');
            var currFieldValid = true;
            fv.$invalidFields.each(function () {
                if ($(this).attr('name') === fn)
                    currFieldValid = false;
            });
            if (!currFieldValid) //revalidate field if not valid on blur 
                fv.revalidateField(fn);
        })
        .on('err.field.fv', function (e, data) {
            //form has errors 
            //if (data.fv.getSubmitButton()) {
            //    data.fv.disableSubmitButtons(false);
            //}
            $('.money').each(function () { // function to apply mask
                $(this).maskMoney('mask', $(this).val());
            });
        })
        .find('.money').maskMoney({
            thousands: ',',
            prefix: '$',
            precision: 0,
            allowZero: true,
            allowEmpty: true
        })
        .find('.money').each(function () { // function to apply mask on load!
            $(this).maskMoney('mask', $(this).val());
        });

    $('#contactform').formValidation({
        framework: 'Bootstrap4',
        live: 'submitted',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
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
                        transformer: function ($field, validatorName, validator) {
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
        }
    })
        .on('submit', function () {
            //$('#contactform').formValidation('revalidateField', 'email');
            //if ($('#phone').val().length < 10) {
            //}
        });
});






(function ($) {
    FormValidation.Validator.email = {
        validate: function (validator, $field, options) {
            var value = $field.val();
            if (value === '') {
                return true;
            }
            // Check the password strength
            if (value.length < 8) {
                return false;
            }
            // The password doesn't contain any uppercase character
            if (value === value.toLowerCase()) {
                return false;
            }
            // The password doesn't contain any uppercase character
            if (value === value.toUpperCase()) {
                return false;
            }
            // The password doesn't contain any digit
            if (value.search(/[0-9]/) < 0) {
                return false;
            }
            return true;
        }
    };
}(window.jQuery));


// Make nav dropdown on hover
$(document).ready(function () {
    var navbarToggle = '.navbar-toggler';
    $('.dropdown, .dropup').each(function () {
        var dropdown = $(this),
            dropdownToggle = $('[data-toggle="dropdown"]', dropdown),
            dropdownHoverAll = dropdownToggle.data('dropdown-hover-all') || false;
        dropdown.hover(function () {
            var notMobileMenu = $(navbarToggle).length > 0 && $(navbarToggle).css('display') === 'none';
            if ((dropdownHoverAll == true || (dropdownHoverAll == false && notMobileMenu))) {
                dropdownToggle.trigger('click');
            }
        })
    });
});

/*Grab the Google Client ID and send to Controller menthod*/
$(function () {
    var serviceURL = '/quote/SetGoogleCid';
    ga('create', 'UA-97044577-1', 'auto', 'sagTracker'); //use Sagicor provider ID to create tracker
    ga(function () {
        var clientId = ga.getByName('sagTracker').get('clientId'); //get client id from tracker
        $.ajax({
            type: "POST",
            url: serviceURL,
            data: {
                gcid: clientId
            },
            dataType: "json",
            /*success: successFunc,*/
            /*error: errorFunc*/
        });
    });

    function successFunc(data, status) { }

    function errorFunc() { }
});

