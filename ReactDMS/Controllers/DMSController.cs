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

            await DoPeriodicWorkAsync(interval, model.Url);

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

        private async Task DoPeriodicWorkAsync(TimeSpan interval, string Url)
        {
            // Repeat this loop until cancelled.
            while (!cancellationToken.IsCancellationRequested)
            {
                PostData(Url);

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval);
            }
        }

        private void PostData(string Url)
        {
            Random random = new Random();

            var model = new DMS
            {
                Timestamp = DateTime.UtcNow,
                DrillingFluidPressure = random.Next(100, 300),
                DrillingFluidFlow = random.Next(200, 500),
                HydraulicOilPressure1 = random.Next(1000, 5000),
                HydraulicOilPressure2 = random.Next(0, 1000),
                HydraulicOilFlow = random.Next(200, 500),
                TraverseRamPressure1 = random.Next(0, 1000),
                TraverseRamPressure2 = random.Next(100, 300),
                Rpm = random.Next(200, 500),
                AirPressure = random.Next(1000, 5000),
                AirVolume = random.Next(0, 1000),
                WaterVolume = random.Next(200, 500),
                DrillTravelDistance = random.Next(0, 1000),
                RotaryTorque1 = random.Next(100, 300),
                RotaryTorque2 = random.Next(200, 500),
                WeightOnBit = random.Next(1000, 5000),
                RateOfPenetration = random.Next(0, 1000),
                DduId = 1,
                Id = DateTime.UtcNow.Ticks
            };

            var webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            //var username = "steven@bsm";
            //var password = "aA!1234";
            ////webRequest.Credentials = new NetworkCredential(username, password);

            //string authInfo = username + ":" + password;
            //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            //webRequest.Headers["Authorization"] = "Basic " + authInfo;

            webRequest.Headers["AuthenticationToken"] = "AED97B8E-8774-4373-AF2E-4B1B45456405";

            var deptSerialized = JsonConvert.SerializeObject(new[] { model });
            using (StreamWriter sw = new StreamWriter(webRequest.GetRequestStream()))
            {
                sw.Write(deptSerialized);
            }

            try
            {
                HttpWebResponse httpWebResponse = webRequest.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    Debug.WriteLine(String.Format("StatusCode == {0}", httpWebResponse.StatusCode));
                    Debug.WriteLine(sr.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                var a = e;
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
        public long Id { get; set; }
        public int DduId { get; set; }
        public DateTime Timestamp { get; set; }

        // Sensor data
        public float DrillingFluidPressure { get; set; }
        public float DrillingFluidFlow { get; set; }
        public float HydraulicOilPressure1 { get; set; }
        public float HydraulicOilPressure2 { get; set; }
        public float HydraulicOilFlow { get; set; }
        public float TraverseRamPressure1 { get; set; }
        public float TraverseRamPressure2 { get; set; }
        public float AirPressure { get; set; }
        public float AirVolume { get; set; }
        public float WaterVolume { get; set; }
        public float DrillTravelDistance { get; set; }
        public long RpmPulseAccumulator { get; set; }
        public long HydraulicOilFlowPulseAccumulator { get; set; }
        public long DrillingFluidFlowPulseAccumulator { get; set; }

        // Calculated fields from PLC
        public float RateOfPenetration { get; set; }
        public float Rpm { get; set; }
        public float WeightOnBit { get; set; }
        public float RotaryTorque1 { get; set; }
        public float RotaryTorque2 { get; set; }
    }
}
