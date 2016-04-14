using System.Collections.Generic;
using Dispatcher.Http;
using Dispatcher.Model;

namespace Dispatcher.Business
{
    public class Document
    {
        private static Document _document;

        private Model.Dispatcher PrivateMe { get; set; }
        private Region PrivateCurrentRegion { get; set; }
        private int PrivateCurrentMapZoom { get; set; }
        private List<Region> PrivateAvailabledRegions { get; set; }
        private List<Profile> PrivateProfiles { get; set; }
        private List<Order> PrivateOrders { get; set; }
        private List<Complain> PrivateComplains { get; set; }
        private HttpManager PrivateHttpManager { get; set; }

        private static Document Instance()
        {
            return _document ?? (_document = new Document());
        }

        public static Model.Dispatcher Me
        {
            get { return Instance().PrivateMe; }
            set { Instance().PrivateMe = value; }
        }

        public static Region CurrentRegion
        {
            get { return Instance().PrivateCurrentRegion; }
            set { Instance().PrivateCurrentRegion = value; }
        }

        public static int CurrentMapZoom
        {
            get { return Instance().PrivateCurrentMapZoom; }
            set { Instance().PrivateCurrentMapZoom = value; }
        }

        public static List<Region> AvailabledRegions
        {
            get { return Instance().PrivateAvailabledRegions; }
            set { Instance().PrivateAvailabledRegions = value; }
        }

        public static HttpManager HttpManager
        {
            get { return Instance().PrivateHttpManager; }
        }

        public static List<Profile> Profiles
        {
            get { return Instance().PrivateProfiles; }
            set
            {
                if (value == null)
                    return;

                foreach (Profile profile in value)
                {
                    Profile p = Instance().PrivateProfiles.Find(p1 => p1.Id == profile.Id);
                    if (p != null)
                    {
                        p.Status = profile.Status;
                        p.Position = profile.Position;
                    }
                    else
                    {
                        Instance().PrivateProfiles.Add(profile);
                    }
                }
            }
        }

        public static List<Order> Orders
        {
            get { return Instance().PrivateOrders; }
            set { Instance().PrivateOrders = value; }
        }

        public static List<Complain> Complains
        {
            get { return Instance().PrivateComplains; }
            set { Instance().PrivateComplains = value; }
        }

        private Document()
        {
            PrivateAvailabledRegions = new List<Region>();

            // initialize availabled regions
            Region minsk = new Region
            {
                Name = "Минск",
                Position = new Position
                {
                    Latitude = "53.902299",
                    Longtitude = "27.557209"
                }
            };
            Region stolbcy = new Region
            {
                Name = "Столбцы",
                Position = new Position
                {
                    Latitude = "53.4824731",
                    Longtitude = "26.7512798"
                }
            };

            PrivateAvailabledRegions.Add(minsk);
            PrivateAvailabledRegions.Add(stolbcy);
            PrivateCurrentRegion = minsk;
            PrivateCurrentMapZoom = 11;

            // initialize cars
            PrivateProfiles = new List<Profile>();

            // initialize hht manager
            PrivateHttpManager = new HttpManager();
        }
    }
}