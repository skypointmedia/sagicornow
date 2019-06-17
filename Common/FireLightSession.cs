using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SagicorNow.Common
{
    internal  static class FireLightSession
    {
        internal const string EmbedTokenKeyName = "FireLightToken";
        internal const string EmbedTokenDateTimeCreatedKeyName = "TokenCreatedDateTime";
#if DEBUG
        internal const string BaseUrl = "https://uat.firelighteapp.com/EGApp/"; //UAT
        //internal const string BaseUrl = "https://firelight.insurancetechnologies.com/EGApp/"; //QE
        internal const string ForeSightUrl = "https://illustration.test.sagicorlifeusa.com/sli6/Core/Acord/TXLifeService.svc";
#else
        internal const string BaseUrl = "https://www.firelighteapp.com/EGApp/";                       
#endif
        internal const string SagApiSecret = "b6c99d41902e46ff8dee144589bfe846"; //UAT
        //internal const string SagApiSecret = "43983160a16d4f0996f98d04fe5ea36d"; //QE
        internal const string SagOrgId = "D2C";
        internal const string SagCarrierCode = "SAG";
        internal const string CertSerialNum = "24000001c5e9e39d3274150b7b0002000001c5";
        internal const string AgentPartyId = "14529e88-60ed-4877-847a-3f682862c14f";
        internal const string ProducerId = "SAG0301";
        internal const string DefaultCoverage = "250000";

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