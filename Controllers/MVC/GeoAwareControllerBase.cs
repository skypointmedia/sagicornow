using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SagicorNow.Controllers
{
    public class GeoAwareControllerBase : Controller
    {
        public string CurrentUserIPAddress
        {
            get
            {
                var ip = HttpContext.Request.UserHostAddress;
                                
                if ((ip == null || ip == "127.0.0.1" || ip == "::1") && Debugger.IsAttached)
                    return "104.244.227.244"; //test Jamaica IP 
                                              //return "205.214.200.108"; //test Barbados IP
                else                
                    return HttpContext.Request.UserHostAddress;
            }
        }

        /*
        public Location GetLocationFromIP(string ipAddress)
        {
            Location loc = null;
            string databasePath = HttpContext.Server.MapPath("~/app_data/GeoLite2-Country.mmdb");

            using (var reader = new DatabaseReader(databasePath))
            {
                var response = reader.Country(ipAddress);
                if (null != response)
                {
                    loc = new Location();
                    loc.countryCode = response.Country.IsoCode;
                }
             }

            return loc;
        }

        public Location CurrentLocation
        {
            get
            {
                if (Request.Cookies["sagicornow_loc_pref"] == null)
                {
                    var loc = this.GetLocationFromIP(CurrentUserIPAddress);

                    if (loc != null)
                    {
                        Response.Cookies["sagicornow_loc_pref"]["country"] = loc.countryCode;
                        Response.Cookies["sagicornow_loc_pref"].Expires = DateTime.Now.AddDays(1);
                    }

                    return loc;
                }
                else
                {
                    var loc = new Location
                    {
                        countryCode = Request.Cookies["sagicornow_loc_pref"]["country"]
                    };

                    return loc;
                }
            }
        }
        */
        
    }

    /// <summary>
    /// reference data class
    /// </summary>
    public class Location
    {
        public string city { get; set; }
        public string countryCode { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
}