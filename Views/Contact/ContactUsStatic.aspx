﻿<!DOCTYPE html>
<html>
<head>

<!-- Google Tag Manager -->
<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window,document,'script','dataLayer','GTM-T28B3LV');</script>
<!-- End Google Tag Manager -->
        
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css" integrity="sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ" crossorigin="anonymous">        
<link rel="stylesheet" href="~/Content/Site.css">
<link href="https://fonts.googleapis.com/css?family=Lato" rel="stylesheet">     
<link rel="stylesheet" href="~/Content/static_templates.css">
<link rel="icon" href="~/content/favicon.ico">
<meta name="google-site-verification" content="59vu63BIU87MeG2sdyBpVlrTVM4CNbitN05W87nZR50" />      
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta charset="utf-8">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
        
<!-- TrustBox script -->
<script type="text/javascript" src="https://widget.trustpilot.com/bootstrap/v5/tp.widget.bootstrap.min.js" async></script>
<!-- End Trustbox script -->        
<title>@ViewBag.Title</title>

</head>
    
<body id="statictemplate1">
<!-- Google Tag Manager (noscript) -->
<noscript><iframe src="https://www.googletagmanager.com/ns.html?id=GTM-T28B3LV" height="0" width="0" style="display:none;visibility:hidden"></iframe>
</noscript>
<!-- End Google Tag Manager (noscript) -->
        
<script src="~/Scripts/jquery-3.2.1.min.js" type="text/javascript"></script>

@Html.Partial("Header")
@RenderBody()
@Html.Partial("Footer")

<script type="text/javascript" src="~/Content/js/tether.min.js"></script>
<script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js"></script>
<script type="text/javascript" src="~/Content/js/formValidation.min.js"></script>
<script type="text/javascript" src="~/Content/js/plugins/Recaptcha.js"></script>
<script type="text/javascript" src="~/Content/js/framework/bootstrap4.min.js"></script>
<script type="text/javascript" src="~/Scripts/typeahead.bundle.min.js"></script>
@*<script type="text/javascript" src="~/Content/js/TypeAhead.js"></script>*@
<script type="text/javascript" src="~/Scripts/jquery.moneymask.js"></script>
<script type="text/javascript" src="~/Content/js/jquery.mask.min.js"></script>
<script type="text/javascript" src="~/Content/js/page-functions.js"></script>
@RenderSection("scripts", required: false)
<link rel="stylesheet" type="text/css" href="//cdnjs.cloudflare.com/ajax/libs/cookieconsent2/3.0.3/cookieconsent.min.css" />
<script src="//cdnjs.cloudflare.com/ajax/libs/cookieconsent2/3.0.3/cookieconsent.min.js"></script>
<script>

window.addEventListener("load", function(){
window.cookieconsent.initialise({
  "palette": {
    "popup": {
      "background": "#333333",
      "text": "#ffffff"
    },
    "button": {
      "background": "#82c341",
      "text": "#ffffff"
    }
  },
  "content": {
    "message": "By continuing to use this site, or by clicking \"I Accept\", you acknowledge and agree to this website's use of cookies to track performance and analytics, and that it may use cookies for other purposes as described in Sagicor's Privacy Statement. ",
    "dismiss": "I Accept",
    "href": "/pages/privacy-statement/"
  },

onPopupOpen: function() {
    var h = $('.cc-window').css( "height" );
    document.getElementById("legal_links").style.paddingBottom = h;
},

onPopupClose: function() {
    document.getElementById("legal_links").style.paddingBottom = '0px';
}


})});

    </script>
    <!--Chat Script-->
    <div id="comm100-button-1051"></div>
    <script type="text/javascript">var Comm100API=Comm100API||{}; (function(t) {function e(e) { var a = document.createElement("script"), c = document.getElementsByTagName("script")[0]; a.type = "text/javascript", a.async = !0, a.src = e + t.site_id, c.parentNode.insertBefore(a, c) }t.chat_buttons=t.chat_buttons||[], t.chat_buttons.push({code_plan:1051, div_id: "comm100-button-1051"}), t.site_id=1000157, t.main_code_plan=1051, e("https://ent.comm100.com/chatserver/livechat.ashx?siteId="), setTimeout(function() {t.loaded || e("https://entmax.comm100.com/chatserver/livechat.ashx?siteId=") }, 5e3) }) (Comm100API||{})</script>
</body>
</html>