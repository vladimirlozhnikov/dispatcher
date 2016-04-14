using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Newtonsoft.Json;

namespace Dispatcher.UI.Forms
{
    public partial class OrdersHistoryFromToForm : Form
    {
        private Token Token { get; set; }
        private DateTime DateFrom { get; set; }
        private DateTime DateTo { get; set; }
        private List<Order> Orders { get; set; }

        private readonly WaitingForm _waitingForm = new WaitingForm();

        public OrdersHistoryFromToForm(Token token)
        {
            Token = token;
            InitializeComponent();

            // инициализация дат для поиска
            DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-2);
            DateTo = DateTime.Now;

            fromButton.Text = DateFrom.ToLongDateString();
            toButton.Text = DateTo.ToLongDateString();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShowWaitingForm(string waitingText)
        {
            _waitingForm.WaitingText = waitingText;
            _waitingForm.Show();
        }

        private void HideWaitingForm()
        {
            _waitingForm.Hide();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.GetDispatcherOrdersFromTo(Token, DateFrom, DateTo);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker) (HideWaitingForm));

                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                Orders = typedRestore.ParsedData<List<Order>>();
                                Invoke((MethodInvoker) (() => OrdersBinding(Orders)));
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
        }

        private void OrdersBinding(List<Order> orders)
        {
            ordersListView.Items.Clear();

            List<ListViewItem> items = GetDataSource(Orders, namePhoneTextBox.Text);
            foreach (ListViewItem listViewItem in items)
            {
                ordersListView.Items.Add(listViewItem);
            }
        }

        private List<ListViewItem> GetDataSource(List<Order> orders, string prefix)
        {
            List<ListViewItem> items = new List<ListViewItem>();

            foreach (Order order in orders)
            {
                ListViewItem item = new ListViewItem(order.Id.ToString());
                item.SubItems.Add(order.Status.ToString());
                item.SubItems.Add(order.Customer.Phone);
                item.SubItems.Add(order.Taxist != null ? order.Taxist.Phone : "Не назначен");
                item.SubItems.Add(string.Format("{0}, {1}", order.CreatedDate.ToLongDateString(), order.CreatedDate.ToLongTimeString()));

                // проверить префикс для поиска
                if (!string.IsNullOrEmpty(prefix))
                {
                    if (order.Customer.Phone.Contains(prefix))
                        items.Add(item);
                    else if (order.Taxist != null &&
                        order.Taxist.Phone.Contains(prefix))
                        items.Add(item);
                }
                else
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private void fromButton_Click(object sender, EventArgs e)
        {
            fromButton.Text = monthCalendar.SelectionRange.Start.ToLongDateString();

            // установить дату from с 0 часов
            DateFrom = new DateTime(monthCalendar.SelectionRange.Start.Year, monthCalendar.SelectionRange.Start.Month, monthCalendar.SelectionRange.Start.Day, 0, 0, 0);
        }

        private void toButton_Click(object sender, EventArgs e)
        {
            toButton.Text = monthCalendar.SelectionRange.Start.ToLongDateString();

            // установить дату to в 23.59
            DateTo = new DateTime(monthCalendar.SelectionRange.Start.Year, monthCalendar.SelectionRange.Start.Month, monthCalendar.SelectionRange.Start.Day, 23, 59, 59);
        }

        private void namePhoneTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Orders == null)
                return;

            OrdersBinding(Orders);
        }
    }
}
