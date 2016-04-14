using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Newtonsoft.Json;

namespace Dispatcher.UI.Forms
{
    public partial class CampaignsForm : Form
    {
        private Token Token { get; set; }
        private List<DiscountCampaign> Campaigns { get; set; }

        public CampaignsForm(Token token)
        {
            Token = token;
            InitializeComponent();

            campaignGridView.MouseDoubleClick += CampaignGridViewOnMouseDoubleClick;
            campaignGridView.MouseClick += CampaignGridViewOnMouseClick;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте...");
            Task<HttpResponseMessage> task = Document.HttpManager.GetDiscountCampaigns(Token);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

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
                                List<DiscountCampaign> campaigns = typedRestore.ParsedData<List<DiscountCampaign>>();
                                Invoke((MethodInvoker)(() => BindCampaigns(campaigns)));
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

        private void BindCampaigns(List<DiscountCampaign> campaigns)
        {
            Campaigns = campaigns;

            campaignGridView.Rows.Clear();
            foreach (DiscountCampaign campaign in campaigns)
            {

                object[] displayRow = { campaign.Id,
                                        campaign.Name,
                                        campaign.CreatedDate,
                                        campaign.ExpiresTo,
                                        campaign.Active ? "Активна" : "Выключена"
                                      };
                campaignGridView.Rows.Add(displayRow);
            }
        }

        private void newCampaignButton_Click(object sender, EventArgs e)
        {
            NewCampaignForm newCampaignForm = new NewCampaignForm(Token);
            newCampaignForm.ShowDialog();
        }

        private void CampaignGridViewOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hti = campaignGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;

            campaignGridView.ClearSelection();
            campaignGridView.Rows[hti.RowIndex].Selected = true;

            // get selected profile
            int campaignId = (int)campaignGridView.Rows[hti.RowIndex].Cells["Id"].Value;
            DiscountCampaign campaign = Campaigns.FirstOrDefault(c => c.Id == campaignId);

            if (campaign == null)
                return;

            NewCampaignForm newCampaignForm = new NewCampaignForm(Token, campaign);
            newCampaignForm.ShowDialog();
        }

        private void CampaignGridViewOnMouseClick(object sender, MouseEventArgs e)
        {
            var hti = campaignGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                campaignGridView.ClearSelection();
                campaignGridView.Rows[hti.RowIndex].Selected = true;

                // get selected profile
                int campaignId = (int)campaignGridView.Rows[hti.RowIndex].Cells["Id"].Value;
                DiscountCampaign campaign = Campaigns.FirstOrDefault(c => c.Id == campaignId);

                if (campaign == null)
                    return;

                ContextMenu m = new ContextMenu();
                MenuItem mi = new MenuItem("Удалить кампанию");
                mi.Click += (o, args) => { DeleteCampaign(campaign); };
                m.MenuItems.Add(mi);
                m.Show(campaignGridView, new Point(e.X, e.Y));
            }
        }

        private void DeleteCampaign(DiscountCampaign campaign)
        {
            Utils.ShowWaitingForm("Ожидайте...");
            Task<HttpResponseMessage> task = Document.HttpManager.DeleteCampaign(Token, campaign);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

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
                                Campaigns.Remove(campaign);
                                Invoke((MethodInvoker)(() => BindCampaigns(Campaigns)));
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
    }
}
