using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;

namespace ReactDMS.Controllers
{
    [RoutePrefix("api/dms")]
    public class DMSController : ApiController
    {
        private static CancellationTokenSource cancellationToken = new CancellationTokenSource();

        [Route("")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Post(TenantAPI model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            cancellationToken = new CancellationTokenSource();

            var interval = TimeSpan.FromSeconds(1);

            await DoPeriodicWorkAsync(interval, model.Url, model.TenantId);

            return Ok();
        }

        [Route("cancel")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Cancel(TenantAPI model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            cancellationToken.Cancel();

            return Ok();
        }

        private async Task DoPeriodicWorkAsync(TimeSpan interval, string Url, Guid tenantId)
        {
            // Repeat this loop until cancelled.
            while (!cancellationToken.IsCancellationRequested)
            {
                PostData(Url, tenantId);

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval);
            }
        }

        private void PostData(string Url, Guid tenantId)
        {
            Random random = new Random();

            var model = new DMS();
            model.TenantId = tenantId;
            model.TimeStamp = new DateTimeOffset(DateTime.Now);
            model.DFP   =  random.Next(100, 300);
            model.DFF   =  random.Next(200, 500);
            model.HOP1  = random.Next(1000, 5000);
            model.HOP2  = random.Next(0, 1000);
            model.HOF   =  random.Next(200, 500);
            model.TRP1  = random.Next(0, 1000);
            model.TRP2  = random.Next(100, 300);
            model.RPM   =  random.Next(200, 500);
            model.AP    =   random.Next(1000, 5000);
            model.AV    =   random.Next(0, 1000);
            model.WV    =   random.Next(200, 500);
            model.DTD   =  random.Next(0, 1000);
            model.RT1   =  random.Next(100, 300);
            model.RT2   =  random.Next(200, 500);
            model.WOB   =  random.Next(1000, 5000);
            model.ROP   =  random.Next(0, 1000);

            var webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            //var username = "tony.tang@imdexlimited.com";
            //var password = "aA!1234";
            //webRequest.Credentials = new NetworkCredential(username, password);

            //string authInfo = username + ":" + password;
            //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            //webRequest.Headers["Authorization"] = "Basic " + authInfo;

            var deptSerialized = JsonConvert.SerializeObject(model);
            using (StreamWriter sw = new StreamWriter(webRequest.GetRequestStream()))
            {
                sw.Write(deptSerialized);
            }

            HttpWebResponse httpWebResponse = webRequest.GetResponse() as HttpWebResponse;
            using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                Debug.WriteLine(String.Format("StatusCode == {0}", httpWebResponse.StatusCode));
                Debug.WriteLine(sr.ReadToEnd());
            }
        }
    }


    public class TenantAPI
    {
        public Guid TenantId { get; set; }
        public string Url { get; set; }
    }

    public class DMS
    {
        public Guid TenantId { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Drilling Fluid Pressure
        /// </summary>
        public double DFP { get; set; }

        /// <summary>
        /// Drilling Fluid Flow
        /// </summary>
        public double DFF { get; set; }

        /// <summary>
        /// Hydraulic Oil Pressure 1
        /// </summary>
        public double HOP1 { get; set; }

        /// <summary>
        /// Hydraulic Oil Pressure 2
        /// </summary>
        public double HOP2 { get; set; }

        /// <summary>
        /// Hydraulic Oil Flow
        /// </summary>
        public double HOF { get; set; }

        /// <summary>
        /// Traverse Ram Pressure 1
        /// </summary>
        public double TRP1 { get; set; }

        /// <summary>
        /// Traverse Ram Pressure 2
        /// </summary>
        public double TRP2 { get; set; }

        /// <summary>
        /// Air Pressure
        /// </summary>
        public double AP { get; set; }

        /// <summary>
        /// Air Volume
        /// </summary>
        public double AV { get; set; }

        /// <summary>
        /// Water Volume
        /// </summary>
        public double WV { get; set; }

        /// <summary>
        /// Drill Travel Distance
        /// </summary>
        public double DTD { get; set; }

        /// <summary>
        /// Rotary Torque 1
        /// </summary>
        public double RT1 { get; set; }

        /// <summary>
        /// Rotary Torque 2
        /// </summary>
        public double RT2 { get; set; }

        /// <summary>
        /// Weight On Bit
        /// </summary>
        public double WOB { get; set; }

        /// <summary>
        /// Revolutions per minute
        /// </summary>
        public double RPM { get; set; }

        /// <summary>
        /// Rate Of Penetration
        /// </summary>
        public double ROP { get; set; }
    }
}
