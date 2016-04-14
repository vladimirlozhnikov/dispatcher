using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;

namespace Dispatcher.UI.Forms
{
    public partial class OdrerHistoryForm : Form
    {
        private List<GarbageCollector> Collectors { get; set; } 
        private List<Status> Statuses { get; set; }
        private Order Order { get; set; }

        public OdrerHistoryForm(Order order, List<GarbageCollector> collectors, List<Status> statuses)
        {
            Order = order;
            Collectors = collectors;
            Statuses = statuses;

            InitializeComponent();

            orderNumberTextBox.Text = order.Id.ToString();
            customerPhoneTextBox.Text = order.Customer.Phone;
            customerNameTextBox.Text = order.Customer.Name;
            fromTextBox.Text = order.From.Description;
            toTextBox.Text = order.To != null ? order.To.Description : String.Empty;
            taxistPhoneTextBox.Text = order.Taxist != null ? order.Taxist.Phone : String.Empty;
            taxistNameTextBox.Text = order.Taxist != null ? order.Taxist.DisplayName : String.Empty;

            // показываем дату создания заказа и клиента
            object[] displayRow1 = { 0,
                                    Utils.ServiceTypeToString(ServiceType.Submitted),
                                    order.Customer.Phone,
                                    "-",
                                    order.CreatedDate };
            orderHistoryDataGridView.Rows.Add(displayRow1);

            // показываем список водителей, которым был предложен заказ
            foreach (GarbageCollector gc in collectors)
            {
                Profile taxist = Document.Profiles.FirstOrDefault(p1 => p1.Id == gc.TaxistId);
                object[] displayRow2 = { gc.StatusId,
                                        Utils.ServiceTypeToString(gc.StatusServiceType),
                                        "-",
                                        taxist != null ? taxist.Phone : "-",
                                        gc.CreatedDate };
                orderHistoryDataGridView.Rows.Add(displayRow2);
            }

            // показываем историю заказа (за исключением даты создания и захватат водителем)
            foreach (Status s in Statuses)
            {
                if (s.status == ServiceType.Submitted ||
                    s.status == ServiceType.Reserved)
                {
                    continue;
                }

                Profile p = Document.Profiles.FirstOrDefault(p1 => p1.Id == s.ProfileId);

                object[] displayRow3 = { s.Id,
                                        Utils.ServiceTypeToString(s.status),
                                        (p != null && p.ServiceType == ServiceType.Customer) ? p.Phone : "-",
                                        (p != null && p.ServiceType == ServiceType.Taxists) ? p.Phone : "-",
                                        s.Date };
                orderHistoryDataGridView.Rows.Add(displayRow3);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
