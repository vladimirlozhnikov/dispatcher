using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Http;
using Dispatcher.Test;

namespace Dispatcher.UI.Forms
{
    public partial class MainForm
    {
        private readonly List<DriverSimulator> _drivers  = new List<DriverSimulator>();
        private readonly List<CustomerSimulator> _customers = new List<CustomerSimulator>();
        private Job _averageJob;
        private Job _addJob;
        private Job _addCustomerJob;
        private bool _inProgress;
        private int _id1;
        private int _id2;
        private int _internalCrash;
        private int _serverError;
        readonly List<int> _phones = new List<int>();
        private readonly JobsQuery _testDriversJobsQuery = new JobsQuery(300);
        private readonly JobsQuery _testCustomersJobsQuery = new JobsQuery(300);
        private readonly JobsQuery _testUpdate = new JobsQuery(200);

#if (DEBUG_LOCAL_DB)
        private readonly HttpManager _httpManager = new HttpManager("http://api.litetaxi.com/alpha/taxiofworld/");
#else
        private readonly HttpManager _httpManager = new HttpManager("https://r.litetaxi.com/alpha/taxiofworld/");
#endif

        private void startTestButton_Click(object sender, EventArgs e)
        {
            _internalCrash = 0;
            _serverError = 0;

            _drivers.Clear();

            _addJob = _testDriversJobsQuery.AddJob();
            _addJob.Name = "Add Driver Job";
            _addJob.FuncEvent += job =>
            {
                AddDriverSimulator();
                return true;
            };
            _addJob.Ready = true;

            _addCustomerJob = _testCustomersJobsQuery.AddJob();
            _addCustomerJob.Name = "Add Customer Job";
            _addCustomerJob.FuncEvent += job =>
            {
                AddCustomerSimulator();
                return true;
            };
            _addCustomerJob.Ready = true;

            startTestButton.Enabled = false;
            stopTestButton.Enabled = true;

            _averageJob = _testUpdate.AddJob();
            _averageJob.Name = "Average Job";
            try
            {
                _averageJob.FuncEvent += job =>
                {
                    Invoke((MethodInvoker)(UpdateDriverTestTable));
                    Invoke((MethodInvoker)(UpdateCustomerTestTable));

                    _inProgress = true;

                    // получаем общее количество ошибок и отображаем
                    _internalCrash = 0;
                    _serverError = 0;
                    foreach (DriverSimulator ds in _drivers)
                    {
                        _internalCrash += ds.InternalCrash;
                        _serverError += ds.ServerError;
                    }

                    foreach (CustomerSimulator cs in _customers)
                    {
                        _internalCrash += cs.InternalCrash;
                        _serverError += cs.ServerError;
                    }

                    Invoke((MethodInvoker)(() =>
                    {
                        internalCrashTextBox.Text = _internalCrash.ToString();
                        serverErrorTextBox.Text = _serverError.ToString();
                    }));

                    _inProgress = false;

                    return true;
                };
            }
            catch (Exception)
            {
            }

            _averageJob.Ready = true;

            Job j = _testUpdate.AddJob();
            j.IntervalInMillis = 1000;
            try
            {
                j.FuncEvent += job =>
                {
                    Invoke((MethodInvoker) (UpdateAverage));

                    _httpManager.RequestCount = 0;
                    return true;
                };
            }
            catch (Exception)
            {
            }
            j.Ready = true;

            _testDriversJobsQuery.Run();
            _testCustomersJobsQuery.Run();
            _testUpdate.Run();
        }

        private void UpdateAverage()
        {
            // отображаем среднее число запросов за прошедший интервал
            averagerequestTextBox.Text = _httpManager.RequestCount.ToString(CultureInfo.InvariantCulture);
        }

        private void UpdateDriverTestTable()
        {
            if (_inProgress)
                return;

            _inProgress = true;

            // 1. найти симуляторы, которые надо удалить из таблицы
            List<DataGridViewRow> deleted = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in testCarsDataGridView.Rows)
            {
                int jobId = (int)row.Cells["JobId"].Value;
                DriverSimulator simulator = _drivers.FirstOrDefault(o => o.Id == jobId);

                if (simulator == null)
                {
                    deleted.Add(row);
                }
            }

            foreach (DataGridViewRow row in deleted)
            {
                testCarsDataGridView.Rows.Remove(row);
            }

            // 2. найти симуляторы, которые надо добавить в таблицу
            List<DriverSimulator> added = new List<DriverSimulator>();
            foreach (DriverSimulator ds in _drivers)
            {
                bool exists = false;
                foreach (DataGridViewRow row in testCarsDataGridView.Rows)
                {
                    int jobId = (int)row.Cells["JobId"].Value;
                    if (jobId == ds.Id)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    added.Add(ds);
                }
            }

            foreach (DriverSimulator ds in added)
            {
                object[] displayRow =
                    {
                        ds.Id,
                        ds.Phone,
                        ds.CurrentJob,
                        ds.ErrorMessage,
                        ds.AverageResponse,
                        ds.TotalTime
                    };
                testCarsDataGridView.Rows.Add(displayRow);
            }

            // 3. обновляем таблицу
            foreach (DataGridViewRow row in testCarsDataGridView.Rows)
            {
                int jobId = (int)row.Cells["JobId"].Value;
                DriverSimulator simulator = _drivers.FirstOrDefault(o => o.Id == jobId);

                if (simulator == null)
                    continue;

                row.Cells["DriverPhone"].Value = simulator.Phone;
                row.Cells["CurrentOperation"].Value = string.Format("{0} - ({1} Сек)", simulator.CurrentJob, simulator.LastMillis >= 0 ? ((float)simulator.LastMillis / 1000) : 0);
                row.Cells["CurrentMessage"].Value = simulator.ErrorMessage;
                row.Cells["Average"].Value = simulator.AverageResponse;
                row.Cells["Time"].Value = simulator.TotalTime;
            }

            _inProgress = false;
        }

        private void UpdateCustomerTestTable()
        {
            if (_inProgress)
                return;

            _inProgress = true;

            // 1. найти симуляторы, которые надо удалить из таблицы
            List<DataGridViewRow> deleted = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in testCustomersDataGridView.Rows)
            {
                int jobId = (int)row.Cells["CustomerJobId"].Value;
                CustomerSimulator simulator = _customers.FirstOrDefault(o => o.Id == jobId);

                if (simulator == null)
                {
                    deleted.Add(row);
                }
            }

            foreach (DataGridViewRow row in deleted)
            {
                testCustomersDataGridView.Rows.Remove(row);
            }

            // 2. найти симуляторы, которые надо добавить в таблицу
            List<CustomerSimulator> added = new List<CustomerSimulator>();
            foreach (CustomerSimulator cs in _customers)
            {
                bool exists = false;
                foreach (DataGridViewRow row in testCustomersDataGridView.Rows)
                {
                    int jobId = (int)row.Cells["CustomerJobId"].Value;
                    if (jobId == cs.Id)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    added.Add(cs);
                }
            }

            foreach (CustomerSimulator cs in added)
            {
                object[] displayRow =
                    {
                        cs.Id,
                        cs.Phone,
                        cs.CurrentJob,
                        cs.ErrorMessage,
                        cs.AverageResponse,
                        cs.TotalTime
                    };
                testCustomersDataGridView.Rows.Add(displayRow);
            }

            // 3. обновляем таблицу
            foreach (DataGridViewRow row in testCustomersDataGridView.Rows)
            {
                int jobId = (int)row.Cells["CustomerJobId"].Value;
                CustomerSimulator simulator = _customers.FirstOrDefault(o => o.Id == jobId);

                if (simulator == null)
                    continue;

                row.Cells["TestCustomerPhone"].Value = simulator.Phone;
                row.Cells["TestCustomerCurrentOperation"].Value = string.Format("{0} - ({1} Сек)", simulator.CurrentJob, simulator.LastMillis >= 0 ? ((float)simulator.LastMillis / 1000) : 0);
                row.Cells["TestCustomerCurrentMessage"].Value = simulator.ErrorMessage;
                row.Cells["TestCustomerAverage"].Value = simulator.AverageResponse;
                row.Cells["TestCustomerTotalTime"].Value = simulator.TotalTime;
            }

            _inProgress = false;
        }

        private void stopTestButton_Click(object sender, EventArgs e)
        {
            _testDriversJobsQuery.DeleteJob(_addJob);
            _testCustomersJobsQuery.DeleteJob(_addCustomerJob);

            foreach (DriverSimulator ds in _drivers)
            {
                ds.Stop();
            }

            foreach (CustomerSimulator cs in _customers)
            {
                cs.Stop();
            }

            Job j = _testDriversJobsQuery.AddJob();
            j.Name = "Delete Driver Job";
            j.IntervalInMillis = 300;
            j.FuncEvent += job =>
            {
                if (!_inProgress)
                    DeleteDriverSimulator(j);
                return true;
            };
            j.Ready = true;

            Job j2 = _testCustomersJobsQuery.AddJob();
            j2.Name = "Delete Customer Job";
            j2.IntervalInMillis = 300;
            j2.FuncEvent += job =>
            {
                if (!_inProgress)
                    DeleteCustomerSimulator(j2);
                return true;
            };
            j2.Ready = true;

            startTestButton.Enabled = true;
            stopTestButton.Enabled = false;
        }

        private void AddDriverSimulator()
        {
            if (_inProgress)
                return;

            _inProgress = true;

            if (carsNumericUpDown.Value == 0)
            {
                _id1 = 0;
                _phones.Clear();
                _testDriversJobsQuery.DeleteJob(_addJob);
                _inProgress = false;
                return;
            }

            Invoke((MethodInvoker)(() =>
            {
                carsNumericUpDown.Value--;
            }));

            DriverSimulator ds = new DriverSimulator((int)getOrdersNumericUpDown.Value * 1000,
                (int)getDriversNumericUpDow.Value * 1000,
                (int)updatePositionNumericUpDown.Value * 1000,
                (int)checkStatusNumericUpDown.Value * 1000)
            {
                Id = _id1++
            };
            ds.HttpManager = _httpManager;
            _drivers.Add(ds);

            // генерируем случайный номер телефона
            Random r = new Random();
            int ph = r.Next(100000000, 999999999);

            // проверяем, чтобы не было повторений
            bool exists;
            do
            {
                exists = false;
                foreach (int p in _phones)
                {
                    if (p == ph)
                    {
                        ph = r.Next(100000000, 999999999);
                        exists = true;
                    }
                }
            } while (exists);

            ds.Phone = String.Format("+375{0}", ph);
            _phones.Add(ph);

            ds.Run();
            _inProgress = false;
        }

        private void AddCustomerSimulator()
        {
            if (_inProgress)
                return;

            _inProgress = true;

            if (customerCountUpDown.Value == 0)
            {
                _id2 = 0;
                _phones.Clear();
                _testCustomersJobsQuery.DeleteJob(_addCustomerJob);
                _inProgress = false;
                return;
            }

            Invoke((MethodInvoker)(() =>
            {
                customerCountUpDown.Value--;
            }));

            CustomerSimulator cs = new CustomerSimulator((int)(ordersTimeUpDown.Value * 1000))
            {
                Id = _id2++
            };
            cs.HttpManager = _httpManager;
            _customers.Add(cs);

            // генерируем случайный номер телефона
            Random r = new Random();
            int ph = r.Next(100000000, 999999999);

            // проверяем, чтобы не было повторений
            bool exists;
            do
            {
                exists = false;
                foreach (int p in _phones)
                {
                    if (p == ph)
                    {
                        ph = r.Next(100000000, 999999999);
                        exists = true;
                    }
                }
            } while (exists);

            cs.Phone = String.Format("+375{0}", ph);
            _phones.Add(ph);

            cs.Run();
            _inProgress = false;
        }

        private void DeleteDriverSimulator(Job j)
        {
            _inProgress = true;

            if (_drivers.Count == 0)
            {
                _testDriversJobsQuery.DeleteJob(j);
                _inProgress = false;
                return;
            }

            DriverSimulator ds = _drivers[0];

            Task<HttpResponseMessage> task = Document.HttpManager.DeleteDriverWithDispatcher(Token, ds.Profile);
            task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        ds.Stop();
                        _drivers.Remove(ds);
                    }
                    else
                    {
                        ds.ErrorMessage =
                            "Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.";
                    }

                    _inProgress = false;
                });
        }

        private void DeleteCustomerSimulator(Job j)
        {
            _inProgress = true;

            if (_customers.Count == 0)
            {
                _testCustomersJobsQuery.DeleteJob(j);
                _inProgress = false;
                return;
            }

            CustomerSimulator cs = _customers[0];

            Task<HttpResponseMessage> task = Document.HttpManager.DeleteDriverWithDispatcher(Token, cs.Profile);
            task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        cs.Stop();
                        _customers.Remove(cs);
                    }
                    else
                    {
                        cs.ErrorMessage =
                            "Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.";
                    }

                    _inProgress = false;
                });
        }
    }
}
