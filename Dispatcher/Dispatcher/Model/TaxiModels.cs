using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dispatcher.Model
{
    public enum ServiceType
    {
        // car types
        Passenger = 0,
        Miniven = 1,
        Wagon = 2,
        Minibus = 3,
        BusinessClass = 4,
        Electric = 5,

        // image types
        Url = 6,
        Image = 7,
        Data = 8,

        // order type
        Submitted = 9,
        Arrived = 10,
        InProgress = 11,
        Canceled = 12,
        Completed = 13,
        Rip = 14,
        Reserved = 15,
        Rejected = 16,
        Agree = 17,

        // status type
        Free = 18,
        Busy = 19,

        // tariff type
        Order = 20,
        City = 21,
        Outcity = 22,
        Wait = 23,

        // customer type
        Customer = 24,
        Taxists = 25,
        Unknown = 26,

        // sms type
        Waiting = 27,
        Delivered = 28,
        Failured = 29,

        // additional statuses
        // offline
        Offline = 30,

        // profile
        Dispatcher = 31
    }

    public enum ErrorCodeEnum
    {
        [Description("")]
        Ok = 0,
        [Description("Произошла внутренняя ошибка сервера. Смотри Data для подробностей.")]
        InternalCrash = 666,
        [Description("Запрос должен содержать токен.")]
        TokenIsRequired = 100,
        [Description("Профиль не был найден.")]
        ProfileWasntFound = 101,
        [Description("Запрос должен содержать номер телефона.")]
        PhoneIsRequired = 102,
        [Description("Запрос должен содержать промо-код.")]
        PromoCodeIsRequired = 103,
        [Description("Запрос должен содержать профиль.")]
        ProfileIsRequired = 104,
        [Description("Запрос должен содержать идентификатор устройста.")]
        UdidIsRequired = 105,
        [Description("Запрос должен содержать адрес 'Откуда'.")]
        FromIsRequired = 106,
        [Description("Запрос должен содержать заказ.")]
        OrderIsRequired = 107,
        [Description("Заказ не имеет статуса 'Создан клиентом'.")]
        OrderIsNotSubmitted = 108,
        [Description("У заказа нет никакого статуса.")]
        OrderHasNotAnyStatus = 109,
        [Description("Заказ не был найден.")]
        OrderWasntFound = 110,
        [Description("Токен не был найден.")]
        TokenWasntFound = 111,
        [Description("Статус не был найден.")]
        StatusWasntFound = 112,
        [Description("Запрос должен содержать позицию на карте.")]
        PositionIsRequired = 113,
        [Description("Запрос должен содержать изображение.")]
        ImageIsRequired = 114,
        [Description("Профиль с таким e-mail уже существует.")]
        EmailExists = 115,
        [Description("Такой e-mail уже содержится в базе.")]
        EmailHasBeenRegistered = 116,
        [Description("Тариф не правильный.")]
        TarifIsNotValid = 117,

        // new errors
        [Description("Последний заказ не был завершен")]
        LastOrderHasNotBeenCompleted = 118,
        [Description("Sms коммутатор вернул ошибку 'Введены неправильные параметры'")]
        SmsCommutatorsErrorIncorrectParameters = 119,
        [Description("Номер телефона должен быть записан в международном формате")]
        PhoneMustBeWrittenInternationalFormat = 120,
        [Description("Заказ отменен.")]
        OrderIsCancelled = 121,
        [Description("Профиль заблокирован.")]
        ProfileIsBlocked = 122,
        [Description("У профиля нет соответствующих прав.")]
        ProfileHasNotPermissions = 123,
        [Description("Заказ не был зарезервирован водителем.")]
        OrderIsNotReserved = 124,
        [Description("Запрос должен содержать имя.")]
        NameIsRequired = 125,

        // dispatcher errors
        [Description("Запрос должен содержать данные о водителе.")]
        RequestMustContainTaxist = 126,
        [Description("Диспетчер не имеет лицензии")]
        DispatcherHasNotLicense = 127,
        [Description("Лицензия не была найдена")]
        LicensesDoNotMatch = 128,
        [Description("Запрос должен содержать данные о диспетчере")]
        RequestMustContainDispatcher = 129,
        [Description("Профиль с таким номером телефона уже существует.")]
        ProfileWithThisPhoneExists = 130,
        [Description("Возникла ошибка при логине администратора.")]
        AdminLoginFailed = 131,
        [Description("Токен не был найден.")]
        TokenIsNotFind = 132,
        [Description("Не было найдено ни одного доступного водителя.")]
        NoFoundAnyAvailableDrivers = 133,
        [Description("Запрос должен содержать дату 'С какого числа'.")]
        RequestMustContainTheDateFrom = 134,
        [Description("Запрос должен содержать дату 'По какое число'.")]
        RequestMustContainTheDateTo = 135,
        [Description("Время жизни профиля истекло.")]
        TokenIsExpired = 136,
        [Description("Не указано время прибытия машины.")]
        NoArrivalTime = 137,
        [Description("Не валидный тип.")]
        ServiceTypeIsNotValid = 138,
        [Description("Запрос должен содаржать настройки.")]
        RequestMustSettings = 139,
        [Description("Недопустимая операция.")]
        IllegalOperation = 140,
        [Description("Запрос должен содержать лицензию.")]
        RequestMustContainLicense = 141,

        [Description("Запрос должен содержать сообщение.")]
        RequestMustContainMessage = 142,
        [Description("Запрос должен содержать жалобу.")]
        RequestMustContainComplain = 143,
        [Description("Такая жалоба в базе не найдена")]
        ComplainWasntFound = 144,
        [Description("Диспетчер с таким именем или лицензией уже существует")]
        DispatcherExist = 145,
        [Description("Город с таким именем уже существует")]
        CityExists = 146,

        [Description("Запрос должен содержать имя социальной сети")]
        SocialNetworkIsRequired = 147,
        [Description("Социальная сеть не поддерживается")]
        SocialNetworkIsCorrupt = 148,

        [Description("Профиль деактивировн.")]
        ProfileIsNotActive = 149,
        [Description("Профиль уже в системе.")]
        ProfileIsOnline = 150,

        [Description("Кампания должна содержать имя.")]
        RequestMustContainsCampaignName = 151,
        [Description("Кампания должна содержать описание.")]
        RequestMustContainsCampaignDescription = 152,
        [Description("Кампания должна содержать дату начала.")]
        RequestMustContainsCampaignCreatedDate = 153,
        [Description("Кампания должна содержать дату окончания.")]
        RequestMustContainsCampaignExpiresDate = 154,
        [Description("Кампания должна содержать профили.")]
        RequestMustContainsCampaignProfiles = 155,
        [Description("Кампания должна содержать бенефиты.")]
        RequestMustContainsCampaignBenefit = 156,

        [Description("На сервере возникла не известная ошибка.")]
        UnknownError = 199
    }

    public class Token
    {
        public string Ticket { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Id { get; set; }

        public new string ToString()
        {
            return string.Format("[token:(Id = {0}, Ticket = {1}, BeginDate = {2}, EndDate = {3})]",
                                 Id, Ticket, BeginDate, EndDate);
        }
    }

    public class Tariff
    {
        public float City { get; set; }
        public float Outcity { get; set; }
        public float Wait { get; set; }
        public float Reservation { get; set; }

        public new string ToString()
        {
            return string.Format("[tariff:(Reservation = {0}, City = {1}, Outcity = {2}, Wait = {3})]",
                                 Reservation, City, Outcity, Wait);
        }
    }

    public class Profile
    {
        public int Id { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public int? Status { get; set; }
        public List<Address> Addresses { get; set; }
        public List<ServiceImage> ServiceImage { get; set; }
        public Car Car { get; set; }
        public Position Position { get; set; }
        public int DistanceToClient { get; set; }
        public bool Active { get; set; }
        public bool DriverActive { get; set; }
        public string License { get; set; }
        public DateTime? ExpiresWith { get; set; }
        public string PromoCode { get; set; }
        public string ServiceName { get; set; }

        public string Token { get; set; }

        public string DisplayName
        {
            get { return String.Format("{0} {1} {2}", Name, SurName, LastName); }
        }

        public new string ToString()
        {
            return string.Format("[profile:(Id = {0}, ServiceType = {1}, Email = {2}, Phone = {3}, Name = {4}, LastName = {5}, SurName = {6}, Status = {7}, Car = {8}, Position = {9}, Verified = {10}]",
                Id, ServiceType, Email, Phone, Name, LastName, SurName, Status,
                Car != null ? (object)Car.ToString() : Car,
                Position != null ? (object)Position.ToString() : Position,
                Active);
        }
    }

    public class Dispatcher
    {
        public int Id { get; set; }
        public Token Token { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string License { get; set; }
        public List<City> Cities { get; set; }
        public List<Operator> Operators { get; set; }
        public DateTime? ExpiresDate { get; set; }
    }

    public class Operator
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Tariff Tariff { get; set; }
        public Tariff CargoTariff { get; set; }
        public Tariff VipTariff { get; set; }
        public Position Center { get; set; }
        public List<Position> Region { get; set; }
        public List<Position> Border { get; set; }
        public bool Active { get; set; }
    }

    public class Address
    {
        public Position Position { get; set; }
        public string Description { get; set; }

        public new string ToString()
        {
            return string.Format("[address:(Position = {0}, Description = {1})]",
                                 Position.ToString(), Description);
        }
    }

    public class Sms
    {
        public int Id { get; set; }
        public Profile Profile { get; set; }
        public Profile From { get; set; }
        public string Message { get; set; }
        public ServiceType Status { get; set; }
        public string Response { get; set; }
        public DateTime Created { get; set; }

        public object Convert()
        {
            var o = new
            {
                recipient = "375336246930",
                message = "sfsfsfs",
                sender = "ssdfsdfs"
            };
            return o;
        }
    }

    public class Promo
    {
        public string Code { get; set; }
        public int Id { get; set; }
    }

    public class Device
    {
        public string UDID { get; set; }
        public int Id { get; set; }
    }

    public class Position
    {
        public int Id { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }

        public new string ToString()
        {
            return string.Format("[position:(Id = {0}, Latitude = {1}, Longitude = {2})]",
                                 Id, Latitude, Longtitude);
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public List<ServiceImage> ServiceImage { get; set; }
        public string Number { get; set; }
        //public List<Tariff> Tariffs { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public ServiceType ServiceType { get; set; }
        public ServiceType? ServiceType2 { get; set; }
        public ServiceType? ServiceType3 { get; set; }

        public new string ToString()
        {
            return string.Format("[car:(Id = {0}, Number = {1}, Model = {2}, Color = {3}, ServiceType = {4})]",
                                 Id, Number, Model, Color, ServiceType);
        }
    }

    /*public class Tariff
    {
        public ServiceType ServiceType { get; set; }
        public float Cost { get; set; }
    }*/

    public class Status2
    {
        public int Id { get; set; }
        public ServiceType ServiceType { get; set; }
        public DateTime? Date { get; set; }
        public Order Order { get; set; }
        public Profile Profile { get; set; }
    }

    public class Status
    {
        public int Id { get; set; }
        public ServiceType status { get; set; }
        public int ProfileId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GarbageCollector
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TaxistId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TaxistPhone { get; set; }
        public string TaxistName { get; set; }
        public ServiceType StatusServiceType { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public Profile Customer { get; set; }
        public Profile Taxist { get; set; }
        public ServiceType Status { get; set; }
        public DateTime? StatusCreatedDate { get; set; }
        public Address From { get; set; }
        public Address To { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal? Cost { get; set; }
        public string ArriveIn { get; set; }
        public int Reason { get; set; }
        public float? Discount { get; set; }

        [JsonIgnore]
        public double TotalSeconds { get; set; }

        public string DisplayName
        {
            get
            {
                return String.Format("Заказчик: {0}, Откуда: {1}, Водитель: {2}, Статус: {3}",
                    Customer.Phone,
                    From.Description,
                    Taxist != null ? Taxist.Name : "Не Указан",
                    Status);
            }
        }

        public new string ToString()
        {
            return string.Format("[order:(Id = {0}, Customer = {1}, Taxist = {2}, Status = {3}, FromId = {4}, ToId = {5})]",
                Id, Customer, Taxist, Status, From.ToString(), To != null ? (object)To.ToString() : To);
        }
    }

    public class Criteria
    {
    }

    public class ServiceImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Stream { get; set; }
    }

    public class DispatcherSettings
    {
        public int DispatcherId { get; set; }
        public int RadiusInMeters { get; set; }
        public int OrdersPeriodInHours { get; set; }
        public int? ClearOrdersInMinutes { get; set; }
        public int? ForceCarsOfflineInMinutes { get; set; }
        public int? CheckClientActivitiesInMinutes { get; set; }
        public int? CheckDriverActivitiesInMinutes { get; set; }
        public int? ClearReservedOrdersInMinutes { get; set; }
        public int? ClearAgreeOrdersInMinutes { get; set; }
        public int? ClearArrivedOrdersInMinutes { get; set; }
        public int? CarsInMap { get; set; }
        public int? FindCarsInDistance { get; set; }
        public bool EnableQueue { get; set; }
        public int? TaxistResponseInSeconds { get; set; }
        public int? AllowIgnoreOrders { get; set; }
    }

    public class Complain
    {
        public int Id { get; set; }
        public Profile Owner { get; set; }
        public Profile Respondent { get; set; }
        public string Message { get; set; }
        public int? Rate { get; set; }
        public DateTime Created { get; set; }
        public bool Approved { get; set; }
        public bool UnderConsideration { get; set; }
        public bool? Like { get; set; }
        public Order Order { get; set; }
    }

    // responses

    public class RestoreResponse
    {
        public Profile Profile { get; set; }
        public Token Token { get; set; }
    }

    public class LogionDispatcherResponse
    {
        public Dispatcher Dispatcher { get; set; }
        public Token Token { get; set; }
    }

    public class FindProfilesResponse
    {
        public List<Profile> Profiles { get; set; }
    }
    
    public class TypedResponse
    {
        public ErrorCodeEnum ErrorCode { get; set; }
        public string ErrorMessage
        {
            get
            {
                FieldInfo fi = ErrorCode.GetType().GetField(ErrorCode.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }

                return ErrorCode.ToString();
            }
        }
        public JToken Data { get; set; }

        public T ParsedData<T>()
        {
            return Data.ToObject<T>();
        }

        public T ParsedTemplate<T>(string template)
        {
            return Data[template].ToObject<T>();
        }

        public T ParsedResult<T>()
        {
            return Data["result"].ToObject<T>();
        }
    }

    public class OrderHistoreResponse
    {
        public List<GarbageCollector> Collectors { get; set; }
        public List<Status> Statuses { get; set; } 
    }

    public class Response
    {
        public ErrorCodeEnum ErrorCode = ErrorCodeEnum.Ok;
        public object Data
        {
            get
            {
                if (ErrorCode == ErrorCodeEnum.Ok && _data == null)
                {
                    return new { Status = "ok" };
                }

                return _data;
            }
            set
            {
                _data = value;
            }
        }
        public string ErrorMessage
        {
            get
            {
                FieldInfo fi = ErrorCode.GetType().GetField(ErrorCode.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }

                return ErrorCode.ToString();
            }
        }

        protected object _data;
    }

    public class GoogleGeoCodeResponse
    {
        public results[] results { get; set; }
        public string status { get; set; }

    }

    public class results
    {
        public address_component[] address_components { get; set; }
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }

        public string DisplayStreetName
        {
            get
            {
                return formatted_address;
            }
        }

        public string Id
        {
            get { return place_id; }
        }
    }

    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

    public class geometry
    {
        public bounds bounds { get; set; }
        public location location { get; set; }
        public string location_type { get; set; }
        public viewport viewport { get; set; }
    }

    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class viewport
    {
        public northeast northeast { get; set; }
        public southwest southwest { get; set; }
    }

    public class bounds
    {
        public northeast northeast { get; set; }
    }

    public class northeast
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class southwest
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    // regions
    public class Region
    {
        public string Name { get; set; }
        public Position Position { get; set; }
    }

    public class SmsResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }

    public class PaymentBalance
    {
        public int Id { get; set; }
        public Profile Profile { get; set; }
        public decimal Money { get; set; }
        public Contract Contract { get; set; }
        public List<PaymentBalance> Payments { get; set; }
    }

    public class Contract
    {
        public int Id { get; set; }
    }

    public class PaymentHistory
    {
        public int Id { get; set; }
        public decimal Diff { get; set; }
        public Contract From { get; set; }
        public Contract To { get; set; }
        public bool Direction { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Bonus
    {
        public int Id { get; set; }
        public Profile Customer { get; set; }
        public bool Active { get; set; }
        public bool? vk { get; set; }
        public bool? fb { get; set; }
        public bool? ok { get; set; }
        public bool? use { get; set; }
    }

    public class DiscountCampaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiresTo { get; set; }
        public List<Bonus> Bonuses { get; set; }
        public bool vk { get; set; }
        public bool fb { get; set; }
        public bool ok { get; set; }
        public bool use { get; set; }
        public bool Active { get; set; }
        public double Discount { get; set; } // in percents
    }
}
