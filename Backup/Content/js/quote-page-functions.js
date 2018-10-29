// Set page focus to zipcode input on pageload

/*
// Mask all dollar inputs to look pretty.
$(function() { $('#final-expenses, #mortgages, #other-debts, #total-annual-income, #current-savings, #current-retirement-savings, #existing-life-insurance-value, #spouses-annual-income').mask("#,##0.00", {reverse: true});
});
*/



// Validate the form inputs before submiting to the server
$(document).ready(function() {

    var d = new Date();
    d.setDate(d.getDate() - 1);

    var strDate = (d.getMonth() + 1) + '/' + d.getDate() + '/' +  d.getFullYear();


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

    // Tool tips
    $('[data-toggle="tooltip"]').tooltip();

    //$('#code').focus();
    //mask input
    $('#birthday').mask('99/99/9999');
    $('input[placeholder="12345"]').mask('99999');
    $('.two-digits').mask('99');
    $('input[placeholder="___-___-____"]').mask('999-999-9999');
    /*$('.money').maskMoney({thousands:',', prefix: '$', precision: 0});
    $('.money').each(function(){ // function to apply mask on load!
        $(this).maskMoney('mask', $(this).val());
    });
    */

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
                        message: 'Your State is required'
                    }
                },
            },
            birthday: {
                threshold: 10,
                validators: {
                    notEmpty: {
                        message: 'Please enter a valid date in MM/DD/YYYY format'
                    },
                    date: {
                        message: 'Date entered is invalid',
                        max: strDate,
                        format: 'MM/DD/YYYY',
                        message: 'Date entered is invalid'
                    }
                },
                onSuccess: function() {
                    var dob = $("#birthday").val();
                    $("#birthday_message").empty();
                    $("#birthday_message").append("<small><strong><i>You are " + getAge(dob) + " years old.</i></strong></small>");
                },
                onError: function() {
                    $("#birthday_message").empty();
                }
            },
            gender: {
                validators: {
                    notEmpty: {
                        message: 'Please confirm Gender'
                    }
                }
            },
            tobacco: {
                validators: {
                    notEmpty: {
                        message: 'Please confirm Tobacco Use'
                    }
                }
            },
            health: {
                validators: {
                    notEmpty: {
                        message: 'Please confirm Overall Health'
                    }
                }
            },
            zipcode: {
                validators: {
                    notEmpty: {
                        message: 'Please enter your Zip Code'
                    }
                }
            },/*
            moneyfields: {
                selector: ".money",
                validators: {
                    notEmpty: {
                        message: 'This is a required field'
                    },
                    stringLength: {
                        max: 9,
                        message: 'Number too large',
                        transformer: function($field, validatorName, validator) {
                            var value = $field.val();
                            return value.split(',').join('').replace('$','');
                            }
                    },
                    digits: {
                            message: 'The value must be a whole number',
                            transformer: function($field, validatorName, validator) {
                            var value = $field.val();
                            return value.split(',').join('').replace('$','');
                        }
                    }
                }
            },*/
            yearsOfIncomeLeft: {
                validators: {
                    notEmpty: {
                        message: 'This is a required field'
                    }
                }
            },
            spouseYearsOfIncomeLeft: {
                validators: {
                    notEmpty: {
                        message: 'This is a required field'
                    }
                }
            },
            ReplacementPolicy: {
                validators: {
                    notEmpty: {
                        message: 'Please confirm if this is a replacement'
                    }
                }
            }
            ,
            familyMakeup: {
                validators: {
                    notEmpty: {
                        message: 'This is a required field'
                    }
                }
            }
            ,
            numKidsNeedFunding: {
                validators: {
                    notEmpty: {
                        message: 'This is a required field'
                    }
                }
            },
            schoolType: {
                validators: {
                    notEmpty: {
                        message: 'This is a required field'
                    }
                }
            }
        }
    })
    // Revalidate replacement policy question when submitting the form
    .on('submit', function(e) {
        $('#quote-form').formValidation('revalidateField', 'ReplacementPolicy');
        $('.money').each(function() {
            //$('#quote-form').formValidation('revalidateField', $(this).attr('name'));
            var value = $(this).val();
            $(this).val(value.split(',').join('').replace('$',''));
        });
    })
    .on('blur', '.money', function(e) {
        $('#quote-form').formValidation('revalidateField', $(this).attr('name'));
    })
    .find('.money').maskMoney({thousands:',', prefix: '$', precision: 0})
    .find('.money').each(function(){ // function to apply mask on load!
        $(this).maskMoney('mask', $(this).val());
    });


    $('#contactform').formValidation({
        framework: 'Bootstrap4',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            firstname: {
                validators: {
                    notEmpty: {
                        message: 'The First Name is required.'
                    }
                }
            },
            phone: {
                validators: {
                    notEmpty: {
                        message: 'Please enter a valid phone number (###-###-####).'
                    },
                    stringLength: {
                        min: 10,
                        message: 'Please enter a valid phone number (###-###-####)',
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
                        message: 'The email address provided appears to be an invalid address.'
                    },
                    notEmpty:{
                        message:'The email address is required.'
                    }
                }
            }
        }
    })
    .on('submit', function() {
        $('#contactform').formValidation('revalidateField', 'email');

        if ($('#phone').val().length < 10)
        {

        }
    });
});

(function($) {
    FormValidation.Validator.email = {
        validate: function(validator, $field, options) {
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