using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Dispatcher.Model;
using Newtonsoft.Json.Linq;

namespace Dispatcher.Http
{

    public class LiteTaxiPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            //Return True to force the certificate to be accepted.
            return true;
        }
    }

    public class HttpManager
    {
        public int RequestCount { get; set; }

        private readonly HttpClient _httpClient;

        private HttpClient HttpClient
        {
            get
            {
                //HttpClient hc = new HttpClient { BaseAddress = _httpClient.BaseAddress };
                //hc.DefaultRequestHeaders.Accept.Clear();
                //hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //return hc;

                return _httpClient;
            }
        }

        public HttpManager()
        {
            _httpClient = new HttpClient
            {
#if (DEBUG_LOCAL_DB)
                BaseAddress = new Uri("http://api.litetaxi.com/d/taxiofworld/")
#else
                BaseAddress = new Uri("https://r.litetaxi.com/d/taxiofworld/")
#endif
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.CertificatePolicy = new LiteTaxiPolicy();
        }

        public HttpManager(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.CertificatePolicy = new LiteTaxiPolicy();
        }

        public Task<HttpResponseMessage> Restore(string phone, string promo, ServiceType ServiceType, string License)
        {
            RequestCount++;

            var request = new { phone, promo, ServiceType, License };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("Restore", content);
            return task;
        }

        public Task<HttpResponseMessage> LoginDispatcher(string name, string code, string license)
        {
            RequestCount++;

            var request = new { name, code, license };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("LoginDispatcher", content);
            return task;
        }

        public Task<HttpResponseMessage> GetDispatcherDrivers(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("GetDispatcherDrivers", content);
            return task;
        }

        public Task<HttpResponseMessage> FindAddresses(string addressPrefix)
        {
            RequestCount++;

            // http://habrahabr.ru/post/110460/

            string template = String.Format("http://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false&language=ru", addressPrefix);
            //var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.GetAsync(template);
            return task;
        }

        public Task<HttpResponseMessage> CreateDispatcherOrder(Token token, string name, string phone, Address from, Address to, Profile taxist)
        {
            RequestCount++;

            var request = new { token, name, phone, from, to, taxist };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("CreateDispatcherOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> CreateOrder(Token token, string name, string phone, Address from, Address to, Profile taxist)
        {
            RequestCount++;

            var request = new { token, name, phone, from, to, taxist };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("CreateOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> CheckCurrentOrder(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("CheckCurrentOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> LoginAdmin(string name, string password)
        {
            RequestCount++;

            var request = new {name, password};
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("LoginAdmin", content);
            return task;
        }

        public Task<HttpResponseMessage> GetDispatchers(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = HttpClient.PostAsync("GetDispatchers", content);
            return task;
        }

        public Task<HttpResponseMessage> SaveDispatcher(Token token, Model.Dispatcher dispatcher)
        {
            RequestCount++;

            var request = new { token, dispatcher };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync(dispatcher.Id > 0 ? "UpdateDispatcher" : "CreateDispatcher", content);
            return task;
        }

        public Task<HttpResponseMessage> DeleteDispatcher(Token token, Model.Dispatcher dispatcher)
        {
            RequestCount++;

            var request = new { token, dispatcher };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("DeleteDispatcher", content);
            return task;
        }

        public Task<HttpResponseMessage> SaveDriverWithDispatcher(Token token, Profile driver)
        {
            RequestCount++;

            var request = new { token, driver };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("SaveDriverWithDispatcher", content);
            return task;
        }

        public Task<HttpResponseMessage> DeleteDriverWithDispatcher(Token token, Profile driver)
        {
            RequestCount++;

            var request = new { token, driver };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("DeleteDriverWithDispatcher", content);
            return task;
        }

        public Task<HttpResponseMessage> GetDispatcherOrders(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetDispatcherOrders", content);
            return task;
        }

        public Task<HttpResponseMessage> GetDispatcherOrdersFromTo(Token token, DateTime dateFrom, DateTime dateTo)
        {
            RequestCount++;

            long from = dateFrom.ToBinary();
            long to = dateTo.ToBinary();

            var request = new { token, from, to };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetDispatcherOrdersFromTo", content);
            return task;
        }

        public Task<HttpResponseMessage> CancelOrder(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("CancelOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> RejectOrder(Token token, Order order)
        {
            RequestCount++;

            var request = new { token, order };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("RejectOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> ApproveOrder(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("ApproveOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> GetOrderHistory(Token token, Order order)
        {
            RequestCount++;

            var request = new { token, order };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetOrderHistory", content);
            return task;
        }

        public Task<HttpResponseMessage> SaveDispatcherSettings(Token token, DispatcherSettings settings)
        {
            RequestCount++;

            var request = new { token, settings };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("SaveDispatcherSettings", content);
            return task;
        }

        public Task<HttpResponseMessage> GetDispatcherSettings(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetDispatcherSettings", content);
            return task;
        }

        public Task<HttpResponseMessage> ClearOrders(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = _httpClient.PostAsync("ClearOrders", content);
            return task;
        }

        public Task<HttpResponseMessage> ForceDriversOffline()
        {
            RequestCount++;

            var task = _httpClient.PostAsync("ForceDriversOffline", null);
            return task;
        }

        public Task<HttpResponseMessage> GetComplains(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetComplains", content);
            return task;
        }

        public Task<HttpResponseMessage> ApproveComplain(Token token, Complain complain)
        {
            RequestCount++;

            var request = new { token, complain };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("ApproveComplain", content);
            return task;
        }

        public Task<HttpResponseMessage> GetPromoCode(string phone, string testMode)
        {
            RequestCount++;

            var request = new { phone, testMode };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetPromoCode", content);
            return task;
        }

        public Task<HttpResponseMessage> GetSubmittedOrders(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetSubmittedOrders", content);
            return task;
        }

        public Task<HttpResponseMessage> ReserveOrder(Token token, Order order, string testMode)
        {
            RequestCount++;

            var request = new { token, order, testMode };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("ReserveOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> UpdatePosition(Token token, Position position, ServiceType? status = null)
        {
            RequestCount++;

            var request = new { token, position, status };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PutAsync("UpdatePosition", content);
            return task;
        }

        public Task<HttpResponseMessage> ArrivedOrder(Token token, string testMode)
        {
            RequestCount++;

            var request = new { token, testMode };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("ArrivedOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> CompleteOrder(Token token, decimal Cost)
        {
            RequestCount++;

            var request = new { token, Cost };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("CompleteOrder", content);
            return task;
        }

        public Task<HttpResponseMessage> CheckOrderStatus(Token token, Order order)
        {
            RequestCount++;

            var request = new { token, order };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("CheckOrderStatus", content);
            return task;
        }

        public Task<HttpResponseMessage> FindProfiles(Token token, Position position)
        {
            RequestCount++;

            var request = new { token, position };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("FindProfiles", content);
            return task;
        }

        public Task<HttpResponseMessage> CreateOrUpdateCity(Token token, int dispatcherId, string name, string description,
            float tariffCity, float tariffWaiting, float tariffOutCity, float tariffReservation,
            float cargoTariffCity, float cargoTariffWaiting, float cargoTariffOutCity, float cargoTariffReservation,
            float vipTariffCity, float vipTariffWaiting, float vipTariffOutCity, float vipTariffReservation,
            bool active, float centerLatitude, float centerLongtitude,
            float[] region, float[] border)
        {
            RequestCount++;

            var request = new
            {
                token,
                dispatcherId,
                name,
                description,
                tariffCity,
                tariffWaiting,
                tariffOutCity,
                tariffReservation,
                cargoTariffCity,
                cargoTariffWaiting,
                cargoTariffOutCity,
                cargoTariffReservation,
                vipTariffCity,
                vipTariffWaiting,
                vipTariffOutCity,
                vipTariffReservation,
                active,
                centerLatitude,
                centerLongtitude,
                region,
                border
            };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("CreateOrUpdateCity", content);
            return task;
        }

        public Task<HttpResponseMessage> GetCities(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetCities", content);
            return task;
        }

        public Task<HttpResponseMessage> NewPromoCode(Token token, Profile profile)
        {
            RequestCount++;

            var request = new { token, profile };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("NewPromoCode", content);
            return task;
        }

        public Task<HttpResponseMessage> GetDiscountCampaigns(Token token)
        {
            RequestCount++;

            var request = new { token };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("GetDiscountCampaigns", content);
            return task;
        }

        public Task<HttpResponseMessage> CreateOrUpdateCampaign(Token token, DiscountCampaign campaign)
        {
            RequestCount++;

            List<Profile> profiles = new List<Profile>();
            foreach (Bonus bonus in campaign.Bonuses)
            {
                profiles.Add(bonus.Customer);
            }
            var request = new { token, campaign };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("CreateOrUpdateCampaign", content);
            return task;
        }

        public Task<HttpResponseMessage> DeleteCampaign(Token token, DiscountCampaign campaign)
        {
            RequestCount++;

            var request = new { token, campaignId = campaign.Id };
            JObject j = JObject.FromObject(request);
            var content = new StringContent(j.ToString(), Encoding.UTF8, "application/json");

            var task = HttpClient.PostAsync("DeleteCampaign", content);
            return task;
        }
    }
}
