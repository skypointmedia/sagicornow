using System;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SagicorNow.Common;
using SagicorNow.Models;

namespace SagicorNow.Controllers
{
    public class FirelightAwareControllerBase : Controller
    {
        public string FirelightAccessToken
        {
            get
            {
                _token = Session[FireLightSession.EmbedTokenKeyName] as string;
                var tokenReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<FirelightTokenReturn>(_token);
                return tokenReturn.access_token;
            }
        }

        /// <summary>
        /// Retrieves Firelight token and saves it with the users session.
        /// </summary>
        /// <param name="user"></param>
        public async Task<string> GetFirelightTokenAsync(string user)
        {
            var req = (HttpWebRequest)WebRequest.Create($"{FireLightSession.BaseUrl}api/Security/GetToken");
            req.Method = "POST";
            req.ContentType = "text/plain";

            var secretBinary = Encoding.UTF8.GetBytes(FireLightSession.SagApiSecret);
            var hashBinary = new SHA256Managed().ComputeHash(secretBinary);

            //convert to hex string
            var builder = new StringBuilder();

            foreach (var hash in hashBinary)
            {
                builder.Append(hash.ToString("X2"));
            }

            var hashValue = builder.ToString();
            var reqDate = DateTime.UtcNow;

            var xml1228 = String.Empty; //get 1228 XML

            if (!string.IsNullOrWhiteSpace(user))
                xml1228 = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));

            //please note, the user 1228 is optional
            var textToSign = FireLightSession.SagApiSecret + reqDate.ToString("MMddyyyyHHmmss") + FireLightSession.SagCarrierCode + xml1228;
            var nonSignedBinary = Encoding.UTF8.GetBytes(textToSign);

            //load up the test cert - in this example, it comes from a pfx file in a local directory - you
            // may need to get it from your machine's certificate store
            var cert = FireLightSession.FindClientCertificate(FireLightSession.CertSerialNum);
            var rsa = cert.GetRSAPrivateKey();
            var signedBinary = rsa.SignData(nonSignedBinary, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            //base64-encoded, then url-encoded
            var signed64 = HttpUtility.UrlEncode(Convert.ToBase64String(signedBinary));

            //optionally place the user's 1228 on the request if it has value (url-encode it)
            var body = $"grant_type=hashsig&id={FireLightSession.SagCarrierCode}&secret={hashValue}&sig={signed64}"
            + (string.IsNullOrWhiteSpace(xml1228) ? "" : $"&xml={HttpUtility.UrlEncode(xml1228)}");

            var bodyBinary = Encoding.UTF8.GetBytes(body);
            req.Date = reqDate;
            req.ContentLength = bodyBinary.LongLength;

            using (var post = await req.GetRequestStreamAsync())
            {
                post.Write(bodyBinary, 0, bodyBinary.Length);
            }

            using (var response = (HttpWebResponse)await req.GetResponseAsync())
            {
                var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
                _token = await reader.ReadToEndAsync();
            }

            Session[FireLightSession.EmbedTokenKeyName] = _token;
            Session[FireLightSession.EmbedTokenDateTimeCreatedKeyName] = DateTime.Now;

            return _token;
        }

        private string _token;

        protected bool FirelightTokeIsNoneExistentOrExpired() {
            var exist = FirelightTokenExist();

            if (exist && DateTime.TryParse(Session[FireLightSession.EmbedTokenDateTimeCreatedKeyName] as string, out var expirationDate))
            {
                return (DateTime.Now - expirationDate).Seconds >= 900;
            }

            return true;
        }
        protected bool FirelightTokenExist() {
            return string.IsNullOrEmpty(Session[FireLightSession.EmbedTokenKeyName] as string);
        }
    }
}