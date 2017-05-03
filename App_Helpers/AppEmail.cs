using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using RentACar.ViewModels;
using System.Net;
using System.Threading.Tasks;

namespace RentACar.App_Helpers
{
    // Class that holds methods for sending emails through official company's email account
    public class AppEmail
    {
        private const string _from = ""; // Email address for Rent a car company
        private const string _password = ""; // Email password for Rent a car company
        private const string _rentSubject = "Rent a Car";
        private const string _msgFooter = "<p>Best Regards</p>" +
                                          "<p>Your Rent a Car Team!</p>";
        private const string _gmailHost = "smtp.gmail.com";
        private const int _port = 587;


        public AppEmail()
        {
            _message = new MailMessage();
        }

        private string _body;
        private string _subject;
        private MailMessage _message;
        private string _emailAddress;

        public string Body { get { return _body; } set { _body = value; } }
        public string Subject { get { return _subject; } set { _subject = value; } }
        public MailMessage Message { get { return _message; } }
        public string EmailAddress { get { return _emailAddress; } set { _emailAddress = value; } }

        public int SendOrder(OrderSummaryViewModel model)
        {
            string destEmail = model.User.Email;

            _message.To.Add(new MailAddress(destEmail));
            _message.From = new MailAddress(_from);
            _message.Subject = _rentSubject;

            string mainMsg = model.Html ?? "<h3> Order Number: " + model.Order.OrderNumber.ToString() + "</h3>" +
                            "<p> Pickup Location: " + model.Order.ReceiveLocation.DisplayLocation + "</p>" +
                            "<p> Pickup Time: " + model.Order.RecvTime.ToString() + "</p>" +
                            "<p> Drop Off Location: " + model.Order.DestinationLocation.DisplayLocation + "</p>" +
                            "<p> Drop Off Time: " + model.Order.DestTime.ToString() + "</p>" +
                            "<p> Vehicle Type: " + model.VehicleModel.VehicleType + "</p>" +
                            "<p> Vehicle Name: " + String.Format("{0} {1}", model.VehicleModel.Brand, model.VehicleModel.BrandType) + "</p>" +
                            "<p> Price for selected period: " + String.Format("{0:N2}", model.Order.TotalPrice) + " kn</p>" +
                            "<p> Order Creation Date And Time: " + DateTime.Now.ToString() + "</p>";

            string body = model.Html == null ? mainMsg : String.Format("<p>{0}</p><p>{1}</p>", mainMsg, _msgFooter);

            _message.Body = body;
            _message.IsBodyHtml = true;

            SendEmail(_message);

            return 1;
        }

        private int SendEmail(MailMessage message)
        {
            Task t = Task.Run(async () =>
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = _gmailHost;
                    smtp.Port = _port;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential
                    {
                        UserName = _from,
                        Password = _password
                    };

                    await smtp.SendMailAsync(message);
                }
            });


            //t.Wait();
            return 1;
        }


        private int Send(MailMessage message)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Host = _gmailHost;
                smtp.Port = _port;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential
                {
                    UserName = _from,
                    Password = _password
                };

                smtp.Send(message);
            }
            return 1;
        }
    }
}