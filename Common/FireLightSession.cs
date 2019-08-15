using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SagicorNow.Common
{
    internal  static class FireLightSession
    {
        internal static readonly string EmbedTokenKeyName = ConfigurationManager.AppSettings["EmbedTokenKeyName"];
        internal static readonly string EmbedTokenDateTimeCreatedKeyName = ConfigurationManager.AppSettings["EmbedTokenDateTimeCreatedKeyName"];

        // https://uat.firelighteapp.com/EGApp/ (UAT)
        // https://firelight.insurancetechnologies.com/EGApp/ (QE)
        // https://firelight.insurancetechnologies.com/EGAppNext/ (QE Next)
        internal static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        // https://illustration.test.sagicorlifeusa.com/sli6/Core/Acord/TXLifeService.svc"
        internal static readonly string ForeSightUrl = ConfigurationManager.AppSettings["ForeSightUrl"];

        // b6c99d41902e46ff8dee144589bfe846 - UAT
        // afb68d89cd474c5cabea892bae716cff - QE Next
        // 43983160a16d4f0996f98d04fe5ea36d - QE
        internal static readonly string SagApiSecret = ConfigurationManager.AppSettings["SagApiSecret"];

        internal static readonly string SagOrgId = ConfigurationManager.AppSettings["SagOrgId"];
        internal static readonly string SagCarrierCode = ConfigurationManager.AppSettings["SagCarrierCode"];
        internal static readonly string CertSerialNum = ConfigurationManager.AppSettings["CertSerialNum"];
        internal static readonly string AgentPartyId = ConfigurationManager.AppSettings["AgentPartyId"];
        internal static readonly string ProducerId = ConfigurationManager.AppSettings["ProducerId"];
        internal static readonly string DefaultCoverage = ConfigurationManager.AppSettings["DefaultCoverage"];

        /// <summary>
        /// find cert on machine
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public static X509Certificate2 FindClientCertificate(string serialNumber)
        {
            return
                FindCertificate(StoreLocation.CurrentUser) ??
                FindCertificate(StoreLocation.LocalMachine);

            X509Certificate2 FindCertificate(StoreLocation location)
            {
                X509Store store = new X509Store(location);
                store.Open(OpenFlags.OpenExistingOnly);
                IEnumerable certs = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, true);
                return certs.OfType<X509Certificate2>().FirstOrDefault();
            };
        }
    }
}