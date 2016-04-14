using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dispatcher.Business;
using Dispatcher.Http;
using Dispatcher.Model;
using Newtonsoft.Json;

namespace Dispatcher.Test
{
    // симуляция работы клиента
    public class CustomerSimulator
    {
        public int Id { get; set; }
        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set
            {
                AddressFrom.Description = String.Format("Test address, {0}", value);
                _phone = value;
            }
        }
        public string Name { get; set; }
        public int TotalTime { get; set; }
        public Profile Profile { get; set; }
        public int InternalCrash { get; set; }
        public int ServerError { get; set; }
        public long LastMillis { get; set; }

        private string PromoCode { get; set; }
        private Token Token { get; set; }
        private Order CurrentOrder { get; set; }
        private Address AddressFrom { get; set; }
        private Address AddressTo { get; set; }
        private readonly JobsQuery _jobsQuery = new JobsQuery(200);

        private readonly int _createOrderInMillis;
        private readonly int _checkCurrentOrderInMillis;
        private bool _inProgress;

        public HttpManager HttpManager { get; set; }

        public double AverageResponse { get; set; }

        public string CurrentJob { get; set; }
        public string ErrorMessage { get; set; }

        public CustomerSimulator(int checkCurrentOrderInMillis)
        {
            _checkCurrentOrderInMillis = checkCurrentOrderInMillis;

            // вычисляем случайную координату в пределах минска
            // координата левого верхнего угла 53.954453, 27.414858
            // координата правого верхнего угла 53.963745, 27.681277
            // координата правого нижнего угла 53.836306, 27.689517
            // координата левого нижнего угла 53.844004, 27.418292

            double difftTopBottom = Math.Abs(53.963745 - 53.836306);
            double diffLeftRigth = Math.Abs(27.689517 - 27.414858);

            Random r2 = new Random();
            _createOrderInMillis = r2.Next(10000, 30000) / 1000 * 1000;
            double randomLeft = r2.NextDouble() * diffLeftRigth;
            double randomTop = r2.NextDouble() * difftTopBottom;
            Position position = new Position
            {
                Latitude = (53.836306 + randomTop).ToString(CultureInfo.InvariantCulture),
                Longtitude = (27.414858 + randomLeft).ToString(CultureInfo.InvariantCulture)
            };

            if (double.IsPositiveInfinity((53.836306 + randomTop)) ||
                double.IsNegativeInfinity((53.836306 + randomTop)) ||
                double.IsPositiveInfinity(27.414858 + randomLeft) ||
                double.IsNegativeInfinity(27.414858 + randomLeft) ||
                double.IsInfinity((53.836306 + randomTop)) ||
                double.IsInfinity(27.414858 + randomLeft) ||
                double.IsNaN((53.836306 + randomTop)) ||
                double.IsNaN(27.414858 + randomLeft))
            {
                int a = 10;
            }

            AddressFrom = new Address
            {
                Position = position,
                Description = String.Format("Test address, {0}", Phone)
            };

            // создаем задачу для получения списка машин
            Job updateJob = _jobsQuery.AddJob();
            updateJob.Name = "FindProfiles";
            updateJob.IntervalInMillis = r2.Next(10000, 20000);
            updateJob.FuncEvent += job =>
            {
                FindProfiles(job);
                return true;
            };
            updateJob.Ready = true;

            // создаем задачу для обновления общего времени
            Job totalTimeJob = _jobsQuery.AddJob();
            totalTimeJob.Name = "totalTimeJob";
            totalTimeJob.IntervalInMillis = 1000;
            totalTimeJob.FuncEvent += job =>
            {
                TotalTime++;
                if (LastMillis > 0)
                    LastMillis -= 1000;
                return true;
            };
            totalTimeJob.Ready = true;
        }

        public void Run()
        {
            CurrentJob = "Запуск симулятора клиента";

            GetPromoCode();
            _jobsQuery.Run();
        }

        public void Stop()
        {
            CurrentJob = "Остановка симулятора клиента";
            _jobsQuery.Stop();
        }

        private void FindProfiles(Job j)
        {
            if (Token == null)
                return;

            if (_inProgress)
                return;

            CurrentJob = "Получение списка водителей";
            LastMillis = j.IntervalInMillis;

            _inProgress = true;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task2 = HttpManager.FindProfiles(Token, AddressFrom.Position);
            task2.ContinueWith(
                t =>
                {
                    _inProgress = false;
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                List<Profile> profiles = typedRestore.ParsedTemplate<List<Profile>>("profiles");
                            }
                            else if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void GetPromoCode()
        {
            CurrentJob = "Получение промо-кода";

            // логинимся на сервер
            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.GetPromoCode(Phone, "return promo code");
            task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                PromoCode = typedRestore.ParsedData<string>();
                                if (!string.IsNullOrEmpty(PromoCode))
                                {
                                    // если промо-код получен, то логинимся
                                    Restore();
                                }
                            }
                            else if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void Restore()
        {
            CurrentJob = "Вход в систему";

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.Restore(Phone, PromoCode, ServiceType.Customer, null);
            task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                RestoreResponse data = typedRestore.ParsedData<RestoreResponse>();
                                if (data != null)
                                {
                                    Token = data.Token;
                                    Profile = data.Profile;

                                    // создаем новый заказ
                                    Job j2 = _jobsQuery.AddJob();
                                    j2.Name = "CreateOrder";
                                    j2.IntervalInMillis = _createOrderInMillis;
                                    j2.FuncEvent += job =>
                                    {
                                        CreateOrder(j2);
                                        return true;
                                    };
                                    j2.Ready = true;
                                }
                            }
                            else if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void CreateOrder(Job j)
        {
            if (_inProgress)
                return;

            CurrentJob = "Создаем заказ";
            LastMillis = j.IntervalInMillis;

            _inProgress = true;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.CreateOrder(Token, Name, Phone, AddressFrom, AddressTo, null);
            task.ContinueWith(
                t =>
                {
                    _inProgress = false;
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                CurrentOrder = typedRestore.ParsedData<Order>();
                                _jobsQuery.DeleteJob(j);

                                // проверяем статус заказа
                                Job j2 = _jobsQuery.AddJob();
                                j2.Name = "CheckCurrentOrder";
                                j2.IntervalInMillis = _checkCurrentOrderInMillis;
                                j2.FuncEvent += job =>
                                {
                                    CheckCurrentOrder(j2);
                                    return true;
                                };
                                j2.Ready = true;
                            }
                            /*else if (typedRestore.ErrorCode != ErrorCodeEnum.NoFoundAnyAvailableDrivers)
                            {
                                // отклоняем
                                //CancelOrder();

                                // начинаем все заново
                                Job j3 = _jobsQuery.AddJob();
                                j3.Name = "CreateOrder";
                                j3.IntervalInMillis = _createOrderInMillis;
                                j3.FuncEvent += job =>
                                {
                                    CreateOrder(j3);
                                    return true;
                                };
                                j3.Ready = true;
                            }*/
                            
                            if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void CheckCurrentOrder(Job j)
        {
            if (_inProgress)
                return;

            CurrentJob = "Проверяем статус заказа";
            LastMillis = j.IntervalInMillis;

            _inProgress = true;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.CheckCurrentOrder(Token);
            task.ContinueWith(
                t =>
                {
                    _inProgress = false;
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                CurrentOrder = typedRestore.ParsedTemplate<Order>("order");

                                // проверить статус.
                                if (CurrentOrder.Status == ServiceType.Reserved)
                                {
                                    // если заказ был захвачен, то случайным образом соглашаемся или отклоняем
                                    _jobsQuery.DeleteJob(j);

                                    Random r = new Random();
                                    int i = r.Next(0, 100);
                                    bool b = (i >= 10 || i <= 90);
                                    //if (b)
                                    {
                                        // соглашаемся
                                        ApproveOrder();
                                    }
                                    /*else
                                    {
                                        // отклоняем
                                        CancelOrder();

                                        // начинаем все заново
                                        Job j3 = _jobsQuery.AddJob();
                                        j3.Name = "CreateOrder";
                                        j3.IntervalInMillis = _createOrderInMillis;
                                        j3.FuncEvent += job =>
                                        {
                                            CreateOrder(j3);
                                            return true;
                                        };
                                        j3.Ready = true;
                                    }*/
                                }
                                else if (CurrentOrder.Status == ServiceType.Canceled ||
                                         CurrentOrder.Status == ServiceType.Rejected)
                                {
                                    _jobsQuery.DeleteJob(j);

                                    Job dj = _jobsQuery.FindByName("CreateOrder");
                                    if (dj != null)
                                    {
                                        _jobsQuery.DeleteJob(dj);
                                    }
                                    
                                    // начинаем все заново
                                    Job j2 = _jobsQuery.AddJob();
                                    j2.Name = "CreateOrder";
                                    j2.IntervalInMillis = _createOrderInMillis;
                                    j2.FuncEvent += job =>
                                    {
                                        CreateOrder(j2);
                                        return true;
                                    };
                                    j2.Ready = true;
                                }
                                else if (CurrentOrder.Status == ServiceType.Arrived)
                                {
                                    // машина прибыла
                                    CurrentJob = "Машина прибыла";
                                }
                                else if (CurrentOrder.Status == ServiceType.Completed)
                                {
                                    // заказ завершен
                                    CurrentJob = "Заказ завершен";

                                    // начинаем все заново
                                    Job j2 = _jobsQuery.AddJob();
                                    j2.Name = "CreateOrder";
                                    j2.IntervalInMillis = _createOrderInMillis;
                                    j2.FuncEvent += job =>
                                    {
                                        CreateOrder(j2);
                                        return true;
                                    };
                                    j2.Ready = true;
                                }
                            }
                            else if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void ApproveOrder()
        {
            if (_inProgress)
                return;

            CurrentJob = "принимаем заказ";

            _inProgress = true;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.ApproveOrder(Token);
            task.ContinueWith(
                t =>
                {
                    _inProgress = false;
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                // проверяем статус заказа
                                Job j2 = _jobsQuery.AddJob();
                                j2.Name = "CheckCurrentOrder";
                                j2.IntervalInMillis = _checkCurrentOrderInMillis;
                                j2.FuncEvent += job =>
                                {
                                    CheckCurrentOrder(j2);
                                    return true;
                                };
                                j2.Ready = true;
                                return;
                            }

                            if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }  
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }

                    // начинаем все заново
                    Job j3 = _jobsQuery.AddJob();
                    j3.Name = "CreateOrder";
                    j3.IntervalInMillis = _createOrderInMillis;
                    j3.FuncEvent += job =>
                    {
                        CreateOrder(j3);
                        return true;
                    };
                    j3.Ready = true;
                });
        }

        private void CancelOrder()
        {
            if (_inProgress)
                return;

            CurrentJob = "отменяем заказ";

            _inProgress = true;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.CancelOrder(Token);
            task.ContinueWith(
                t =>
                {
                    _inProgress = false;
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks);
                        TimeSpan elapsedSpan = new TimeSpan((long)average);
                        AverageResponse = elapsedSpan.TotalMilliseconds;

                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            ServerError++;
                            ErrorMessage = response.StatusCode.ToString();
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                            }

                            if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }
    }
}
