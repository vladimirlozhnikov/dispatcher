using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Dispatcher.Model;
using Dispatcher.UI.Forms;

namespace Dispatcher.Business
{
    static class Utils
    {
        private static readonly WaitingForm WaitingForm = new WaitingForm();

        public static string ServiceTypeToString(ServiceType type)
        {
            string status = "Не определен";
            switch (type)
            {
                case ServiceType.Passenger:
                    status = "Легковая машина";
                    break;
                case ServiceType.Miniven:
                    status = "Минивен";
                    break;
                case ServiceType.Wagon:
                    status = "Универсал";
                    break;
                case ServiceType.Minibus:
                    status = "Микроавтобус";
                    break;
                case ServiceType.BusinessClass:
                    status = "Машина бизнес-класса";
                    break;
                case ServiceType.Electric:
                    status = "Электрокар";
                    break;
                case ServiceType.Url:
                    status = "Ссылка";
                    break;
                case ServiceType.Image:
                    status = "Изображение";
                    break;
                case ServiceType.Data:
                    status = "Данные";
                    break;

                case ServiceType.Submitted:
                    status = "Создан";
                    break;
                case ServiceType.Arrived:
                    status = "Машина прибыла";
                    break;
                case ServiceType.InProgress:
                    status = "Машина в пути";
                    break;
                case ServiceType.Canceled:
                    status = "Отменен клиентом";
                    break;
                case ServiceType.Completed:
                    status = "Завершен";
                    break;
                case ServiceType.Rip:
                    status = "Заказ протух";
                    break;
                case ServiceType.Reserved:
                    status = "Зарезервирован водителем";
                    break;
                case ServiceType.Rejected:
                    status = "Отменен водителем";
                    break;
                case ServiceType.Agree:
                    status = "Подтвержден клиентом";
                    break;

                case ServiceType.Free:
                    status = "Свободен";
                    break;

                case ServiceType.Busy:
                    status = "Занят";
                    break;

                case ServiceType.Order:
                    status = "Дневной тариф";
                    break;
                case ServiceType.City:
                    status = "В городе";
                    break;
                case ServiceType.Outcity:
                    status = "За городом";
                    break;
                case ServiceType.Wait:
                    status = "Простой машины";
                    break;

                case ServiceType.Customer:
                    status = "Клиент";
                    break;
                case ServiceType.Taxists:
                    status = "Водитель";
                    break;
                case ServiceType.Unknown:
                    status = "Тип не определен";
                    break;

                case ServiceType.Waiting:
                    status = "Ожидание";
                    break;
                case ServiceType.Delivered:
                    status = "Доставлено";
                    break;
                case ServiceType.Failured:
                    status = "Ошибка";
                    break;

                case ServiceType.Offline:
                    status = "Не в сети";
                    break;

                case ServiceType.Dispatcher:
                    status = "Диспетчер";
                    break;
            }

            return status;
        }

        public static SizeF GetListItemSize(string text, int width, Font font, Graphics graphics)
        {
            string s = text;
            Font f = new Font(font.Name, 13, font.Style, font.Unit);

            SizeF sf = graphics.MeasureString(s, f, width);
            return sf;
        }

        public static void DrawListboxItem(string text, float y, DrawItemEventArgs e, Font font, Color color)
        {
            // Draw the background of the ListBox control for each item.
            Font f = new Font(font.Name, 12, font.Style, font.Unit);
            Rectangle r = new Rectangle(e.Bounds.Left, (int)(e.Bounds.Top + y), e.Bounds.Width, e.Bounds.Height);
            e.Graphics.DrawString(text, f, new SolidBrush(color), /*e.Bounds*/r);
        }

        public static void ShowWaitingForm(string waitingText)
        {
            WaitingForm.WaitingText = waitingText;
            WaitingForm.Show();
        }

        public static void HideWaitingForm()
        {
            WaitingForm.Hide();
        }

        public static Task<Image> GetImageFromUrl(string url, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    WebRequest req = WebRequest.Create(url);
                    WebResponse response = req.GetResponse();
                    Stream stream = response.GetResponseStream();

                    if (stream != null) return Image.FromStream(stream);
                }
                catch (Exception)
                {
                    return null;
                }
                return null;
            }, cancellationToken);
        }

        public static string ImageToBase64(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, ImageFormat.Gif);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        // Deg2Rad.
        // Конвертировать градусы в радианы
        /// <param name="deg">Градусы.</param>
        private static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        // Rad2Deg.
        // Конвертировать радианы в градусы
        /// <param name="rad">Радианы.</param>
        private static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        // GetDistance.
        // Вычислить дистанцию между двумя точками
        /// <param name="lat1">Широта 1.</param>
        /// <param name="lon1">Долгота 1.</param>
        /// <param name="lat2">Широта 2.</param>
        /// <param name="lon2">Долгота 2.</param>
        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            //code for Distance in Kilo Meter
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Abs(Math.Round(Rad2Deg(Math.Acos(dist)) * 60 * 1.1515 * 1.609344 * 1000, 0));
            return (dist);
        }

        // Определить, находится ли точка внутри многоугольника
        public static bool IsPointInPolygon(PointF[] poly, PointF pnt)
        {
            // http://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
            int i, j;
            int nvert = poly.Length;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((poly[i].Y > pnt.Y) != (poly[j].Y > pnt.Y)) &&
                 (pnt.X < (poly[j].X - poly[i].X) * (pnt.Y - poly[i].Y) / (poly[j].Y - poly[i].Y) + poly[i].X))
                    c = !c;
            }
            return c;
        }

        public static bool SendSms(Sms sms)
        {
            // используем сервис http://websms.by/pages/doc/ru

            string baseUr = string.Format(
                "http://cp.websms.by/?r=api/msg_send&user={0}&apikey={1}&urgent=1&recipients={2}&message={3}&sender={4}",
                "vladimir.lozhnikov@gmail.com",
                "qE0PBXsnnV",
                sms.Profile.Phone,
                sms.Message,
                "LiteTaxi"
                );
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUr);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            if (response == null)
            {
                return false;
            }

            Stream stream = response.GetResponseStream();
            if (stream == null)
            {
                return false;
            }

            using (var reader = new StreamReader(stream))
            {
                string objText = reader.ReadToEnd();

                JavaScriptSerializer js = new JavaScriptSerializer();
                SmsResponse r = js.Deserialize<SmsResponse>(objText);
                if (r != null && r.status.ToLower() == "error")
                {
                    return false;
                }
            }

            return true;
        }
    }
}
