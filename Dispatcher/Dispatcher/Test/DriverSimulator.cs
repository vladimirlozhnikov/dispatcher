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
    // симуляция работы водителя
    public class DriverSimulator 
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public int TotalTime { get; set; }
        public Profile Profile { get; set; }
        public int InternalCrash { get; set; }
        public int ServerError { get; set; }
        public long LastMillis { get; set; }

        private string PromoCode { get; set; }
        private Token Token { get; set; }
        private List<Order> Orders { get; set; }
        private Order CurrentOrder { get; set; }
        private Position CurrentPosition { get; set; }
        private Position FirstPosition { get; set; }
        private Position AddressTo { get; set; }

        private readonly JobsQuery _jobsQuery = new JobsQuery(200);
        private readonly int _getSubmittedOrdersInMillis;
        private readonly int _reserveInMillis;
        private readonly int _updatePositionInillis;
        private readonly int _getDriversInMillis;
        private readonly int _checkStatusInMillis;
        private int _stepsCount;
        private bool _busy;
        private bool _inProgress;

        public HttpManager HttpManager { get; set; }

        public double AverageResponse { get; set; }

        public string CurrentJob { get; set; }
        public string ErrorMessage { get; set; }

        public DriverSimulator(int getSubmittedOrdersInMillis = 20000, int getDriversInMillis = 30000, int updatePositionInillis = 20000, int checkStatusInMillis = 20000)
        {
            _getSubmittedOrdersInMillis = getSubmittedOrdersInMillis;
            _reserveInMillis = 2000;
            _getDriversInMillis = getDriversInMillis;
            _updatePositionInillis = updatePositionInillis;
            _checkStatusInMillis = checkStatusInMillis;

            // вычисляем случайную координату в пределах минска
            // координата левого верхнего угла 53.954453, 27.414858
            // координата правого верхнего угла 53.963745, 27.681277
            // координата правого нижнего угла 53.836306, 27.689517
            // координата левого нижнего угла 53.844004, 27.418292

            double difftTopBottom = Math.Abs(53.963745 - 53.836306);
            double diffLeftRigth = Math.Abs(27.689517 - 27.414858);

            Random r2 = new Random();
            double randomLeft = r2.NextDouble()*diffLeftRigth;
            double randomTop = r2.NextDouble()*difftTopBottom;
            FirstPosition = new Position
            {
                Latitude = (53.836306 + randomTop).ToString(CultureInfo.InvariantCulture),
                Longtitude = (27.414858 + randomLeft).ToString(CultureInfo.InvariantCulture)
            };
            CurrentPosition = FirstPosition;

            // создаем задачу для отсылки координат
            Job updateJob = _jobsQuery.AddJob();
            updateJob.Name = "UpdatePosition";
            updateJob.IntervalInMillis = _updatePositionInillis;
            updateJob.FuncEvent += job =>
            {
                UpdatePosition(job);
                return true;
            };
            updateJob.Ready = true;

            // создаем задачу для получения списка водителей
            Job getDriversJob = _jobsQuery.AddJob();
            getDriversJob.Name = "FindProfiles";
            getDriversJob.IntervalInMillis = _getDriversInMillis;
            getDriversJob.FuncEvent += job =>
            {
                FindProfiles(job);
                return true;
            };
            getDriversJob.Ready = true;

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

        private void UpdatePosition(Job j)
        {
            if (Token == null)
                return;

            if (_inProgress)
                return;

            _inProgress = true;

            CurrentJob = "Обновление координат";
            LastMillis = j.IntervalInMillis;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task2 = HttpManager.UpdatePosition(Token, CurrentPosition, _busy ? ServiceType.Busy : ServiceType.Free);
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

                        if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                        {
                            InternalCrash++;
                        }

                        ErrorMessage = response.StatusCode == HttpStatusCode.OK ? typedRestore.ErrorMessage : String.Format("response status: {0}", response.StatusCode);
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void FindProfiles(Job j)
        {
            if (Token == null)
                return;

            if (_inProgress)
                return;

            _inProgress = true;

            CurrentJob = "Получение списка водителей";
            LastMillis = j.IntervalInMillis;

            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task2 = HttpManager.FindProfiles(Token, CurrentPosition);
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

        public void Run()
        {
            CurrentJob = "Запуск симулятора водителя";

            // получаем промо-код от сервера
            Job j = _jobsQuery.SingleShot();
            j.Name = Phone;
            j.IntervalInMillis = 2000;
            j.FuncEvent += job =>
            {
                GetPromoCode();
                return true;
            };
            j.Ready = true;

            _jobsQuery.Run();
        }

        public void Stop()
        {
            CurrentJob = "Остановка симулятора водителя";
            _jobsQuery.Stop();
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
            Task<HttpResponseMessage> task = HttpManager.Restore(Phone, PromoCode, ServiceType.Taxists, "development");
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

                                    // получаем список доступных заказов
                                    Job j2 = _jobsQuery.AddJob();
                                    j2.Name = String.Format("{0}, GetSubmittedOrders", Phone);
                                    j2.IntervalInMillis = _getSubmittedOrdersInMillis;
                                    j2.FuncEvent += job =>
                                    {
                                        GetSubmittedOrders(j2);
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

        private void GetSubmittedOrders(Job j)
        {
            if (_inProgress)
                return;

            CurrentJob = "Поиск доступных заказов";
            LastMillis = j.IntervalInMillis;
            _busy = false;

            _inProgress = true;

            // получаем список доступных заказов и пытаемся их захватить
            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.GetSubmittedOrders(Token);
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
                                Orders = typedRestore.ParsedResult<List<Order>>();
                                foreach (Order o in Orders)
                                {
                                    if (o.Status == ServiceType.Submitted)
                                    {
                                        _jobsQuery.DeleteJob(j);

                                        // сохраняем адрес клиента
                                        AddressTo = o.From.Position;

                                        // резервируем заказ
                                        Job j1 = _jobsQuery.AddJob();
                                        j1.Name = String.Format("{0}, ReserveOrder", Phone);
                                        j1.IntervalInMillis = _reserveInMillis;
                                        var o1 = o;
                                        j1.FuncEvent += job =>
                                        {
                                            ReserveOrder(o1, j1);
                                            return true;
                                        };
                                        j1.Ready = true;

                                        return;

                                        //RejectOrder(o);

                                        // получаем список доступных заказов
                                        //Job j2 = _jobsQuery.AddJob();
                                        //j2.Name = String.Format("{0}, GetSubmittedOrders", Phone);
                                        //j2.IntervalInMillis = _getSubmittedOrdersInMillis;
                                        //j2.FuncEvent += job =>
                                        //{
                                        //    GetSubmittedOrders(j2);
                                        //    return true;
                                        //};
                                        //j2.Ready = true;
                                    }
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

        private void RejectOrder(Order order)
        {
            CurrentJob = "Отмена заказа";

            if (_inProgress)
                return;

            _inProgress = true;

            // пытаемся захватить заказ
            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.RejectOrder(Token, order);
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
                        }
                    }
                    else
                    {
                        ErrorMessage = String.Format("task status: {0}", t.Status);
                    }
                });
        }

        private void ReserveOrder(Order order, Job j)
        {
            if (_inProgress)
                return;

            CurrentJob = "Захват заказа";
            LastMillis = j.IntervalInMillis;

            _inProgress = true;

            // пытаемся захватить заказ
            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.ReserveOrder(Token, order, "dont send push notification");
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
                            _jobsQuery.DeleteJob(j);

                            ErrorMessage = typedRestore.ErrorMessage;
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                Job j1 = _jobsQuery.AddJob();
                                j1.Name = String.Format("{0}, CheckOrderStatus", Phone);
                                j1.IntervalInMillis = _checkStatusInMillis;
                                j1.FuncEvent += job =>
                                {
                                    CheckOrderStatus(j1, order);
                                    return true;
                                };
                                j1.Ready = true;

                                return;
                            }

                            // если заказ уже был захвачен, то начинаем заново
                            // получаем список доступных заказов
                            Job j2 = _jobsQuery.AddJob();
                            j2.Name = String.Format("{0}, GetSubmittedOrders", Phone);
                            j2.IntervalInMillis = _getSubmittedOrdersInMillis;
                            j2.FuncEvent += job =>
                            {
                                GetSubmittedOrders(j2);
                                return true;
                            };
                            j2.Ready = true;
                            
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

        private void CheckOrderStatus(Job j, Order order)
        {
            if (_inProgress)
                return;

            CurrentJob = "Проверка статуса захваченного заказа";
            LastMillis = j.IntervalInMillis;

            _busy = true;
            _inProgress = true;

            // проверяем статус заказа
            long ticks = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.CheckOrderStatus(Token, order);
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
                                Status s = typedRestore.ParsedData<Status>();
                                if (s.status == ServiceType.Agree)
                                {
                                    _jobsQuery.DeleteJob(j);
                                    CurrentOrder = order;

                                    // начинаем маршрут от начальной до конечной точки (был вычислен заранее)
                                    // выполняем в отдельной задаче
                                    Random r = new Random((int)DateTime.Now.Ticks);
                                    _stepsCount = r.Next(10, 40);
                                    Job j1 = _jobsQuery.AddJob();
                                    j1.Name = String.Format("{0}, MoveToClient", Phone);
                                    j1.IntervalInMillis = _updatePositionInillis;
                                    j1.FuncEvent += job =>
                                    {
                                        MoveToClient(j1);
                                        return true;
                                    };
                                    j1.Ready = true;
                                }
                                else if (s.status == ServiceType.Canceled || s.status == ServiceType.Rejected || s.status == ServiceType.Completed)
                                {
                                    CurrentOrder = null;
                                    _jobsQuery.DeleteJob(j);

                                    // получаем список доступных заказов
                                    Job j2 = _jobsQuery.AddJob();
                                    j2.Name = String.Format("{0}, GetSubmittedOrders", Phone);
                                    j2.IntervalInMillis = _getSubmittedOrdersInMillis;
                                    j2.FuncEvent += job =>
                                    {
                                        GetSubmittedOrders(j2);
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

        private void MoveToClient(Job j)
        {
            if (_inProgress)
                return;

            CurrentJob = "Едет к клиенту";
            LastMillis = j.IntervalInMillis;
            _busy = true;
            _inProgress = true;

            // смотрим дистанцию между текущей позицией и конечной
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            double lat1 = double.Parse(CurrentPosition.Latitude, NumberStyles.Any, ci);
            double lon1 = double.Parse(CurrentPosition.Longtitude, NumberStyles.Any, ci);
            double lat2 = double.Parse(AddressTo.Latitude, NumberStyles.Any, ci);
            double lon2 = double.Parse(AddressTo.Longtitude, NumberStyles.Any, ci);

            const double diff = 500;

            double distanse = Utils.GetDistance(lat1, lon1, lat2, lon2);
            if (Math.Abs(distanse) <= diff)
            {
                // если дистанция меньше минимальной, то считае, что машина прибыла на место

                // останавливаем текущую задачу
                _jobsQuery.DeleteJob(j);

                // создаем новую, в которой машина будет двигаться в обратном направлении
                Random r = new Random((int)DateTime.Now.Ticks);
                _stepsCount = r.Next(10, 40);

                Job j1 = _jobsQuery.AddJob();
                j1.Name = String.Format("{0}, MoveToFinish", Phone);
                j1.IntervalInMillis = _updatePositionInillis;
                j1.FuncEvent += job =>
                {
                    _inProgress = false;
                    MoveToFinish(j1);
                    return true;
                };
                j1.Ready = true;

                // посылаем сигнал, что машина прибыла
                long ticks = DateTime.Now.Ticks;
                Task<HttpResponseMessage> task1 = HttpManager.ArrivedOrder(Token, "dont send push notification");
                task1.ContinueWith(
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
                            if (typedRestore.ErrorCode == ErrorCodeEnum.InternalCrash)
                            {
                                InternalCrash++;
                                return;
                            }

                            ErrorMessage = response.StatusCode == HttpStatusCode.OK ? typedRestore.ErrorMessage : String.Format("response status: {0}", response.StatusCode);
                        }
                        else
                        {
                            ErrorMessage = String.Format("task status: {0}", t.Status);
                        }
                    });

                return;
            }

            // вычисляем следующую точку в направлении адреса клиента
            double nextLatDiff = Math.Abs(lat1 - lat2) / _stepsCount;
            double nextLonDiff = Math.Abs(lon1 - lon2) / _stepsCount--;

            if (lat1 < lat2)
                lat1 += nextLatDiff;
            else
                lat1 -= nextLatDiff;

            if (lon1 < lon2)
                lon1 += nextLonDiff;
            else
                lon1 -= nextLonDiff;

            if (double.IsPositiveInfinity(lat1) ||
                double.IsNegativeInfinity(lat1) ||
                double.IsPositiveInfinity(lon1) ||
                double.IsNegativeInfinity(lon1) ||
                double.IsInfinity(lat1) ||
                double.IsInfinity(lon1) ||
                double.IsNaN(lat1) ||
                double.IsNaN(lon1))
            {
                int a = 10;
            }

            CurrentPosition = new Position
            {
                Latitude = lat1.ToString(CultureInfo.InvariantCulture),
                Longtitude = lon1.ToString(CultureInfo.InvariantCulture)
            };

            // проверить статус заказа. если он был отменен, то начинаем все с начала
            long ticks1 = DateTime.Now.Ticks;
            Task<HttpResponseMessage> task = HttpManager.CheckOrderStatus(Token, CurrentOrder);
            task.ContinueWith(
                t =>
                {
                    _inProgress = false;
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        double average = (DateTime.Now.Ticks - ticks1);
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
                                Status s = typedRestore.ParsedData<Status>();
                                if (s.status == ServiceType.Canceled)
                                {
                                    _jobsQuery.DeleteJob(j);
                                    CurrentOrder = null;

                                    // получаем список доступных заказов
                                    Job j2 = _jobsQuery.AddJob();
                                    j2.Name = String.Format("{0}, GetSubmittedOrders", Phone);
                                    j2.IntervalInMillis = _getSubmittedOrdersInMillis;
                                    j2.FuncEvent += job =>
                                    {
                                        GetSubmittedOrders(j2);
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

        private void MoveToFinish(Job j)
        {
            if (_inProgress)
                return;

            CurrentJob = "Едет к точке назначения";
            LastMillis = j.IntervalInMillis;

            _inProgress = true;

            // смотрим дистанцию между текущей позицией и конечной
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            double lat1 = double.Parse(CurrentPosition.Latitude, NumberStyles.Any, ci);
            double lon1 = double.Parse(CurrentPosition.Longtitude, NumberStyles.Any, ci);
            double lat2 = double.Parse(FirstPosition.Latitude, NumberStyles.Any, ci);
            double lon2 = double.Parse(FirstPosition.Longtitude, NumberStyles.Any, ci);

            const double diff = 500;

            double distanse = Utils.GetDistance(lat1, lon1, lat2, lon2);
            if (Math.Abs(distanse) <= diff)
            {
                // посылаем сигнал, что машина прибыла
                Random r = new Random((int)DateTime.Now.Ticks);
                decimal cost = (decimal)(r.NextDouble() * 200000);
                long ticks = DateTime.Now.Ticks;
                Task<HttpResponseMessage> task1 = HttpManager.CompleteOrder(Token, cost);
                task1.ContinueWith(
                    t =>
                    {
                        _inProgress = false;
                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            double average = (DateTime.Now.Ticks - ticks);
                            TimeSpan elapsedSpan = new TimeSpan((long)average);
                            AverageResponse = elapsedSpan.TotalMilliseconds;
                        }
                        else
                        {
                            ErrorMessage = String.Format("task status: {0}", t.Status);
                        }
                    });

                // начинаем все с начала
                _jobsQuery.DeleteJob(j);
                CurrentOrder = null;

                // получаем список доступных заказов
                Job j2 = _jobsQuery.AddJob();
                j2.Name = String.Format("{0}, GetSubmittedOrders", Phone);
                j2.IntervalInMillis = _getSubmittedOrdersInMillis;
                j2.FuncEvent += job =>
                {
                    GetSubmittedOrders(j2);
                    return true;
                };
                j2.Ready = true;

                return;
            }

            // вычисляем следующую точку в направлении адреса клиента
            double nextLatDiff = Math.Abs(lat1 - lat2) / _stepsCount;
            double nextLonDiff = Math.Abs(lon1 - lon2) / _stepsCount--;

            if (lat1 < lat2)
                lat1 += nextLatDiff;
            else
                lat1 -= nextLatDiff;

            if (lon1 < lon2)
                lon1 += nextLonDiff;
            else
                lon1 -= nextLonDiff;

            if (double.IsPositiveInfinity(lat1) ||
                double.IsNegativeInfinity(lat1) ||
                double.IsPositiveInfinity(lon1) ||
                double.IsNegativeInfinity(lon1) ||
                double.IsInfinity(lat1) ||
                double.IsInfinity(lon1) ||
                double.IsNaN(lat1) ||
                double.IsNaN(lon1))
            {
                int a = 10;
            }

            CurrentPosition = new Position
            {
                Latitude = lat1.ToString(CultureInfo.InvariantCulture),
                Longtitude = lon1.ToString(CultureInfo.InvariantCulture)
            };

            _inProgress = false;
        }
    }
}
