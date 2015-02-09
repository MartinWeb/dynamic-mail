using dynamic_mail.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Web.Http;

namespace dynamic_mail.Controllers
{
    public class MailController : ApiController
    {
        public String Get()
        {
            // Création d'un objet DynamicValues
            DynamicValues dyn = new DynamicValues();
            dyn.lastName = "Martin";
            dyn.job = "Developer";
            dyn.email = "test@test.com";
            dyn.age = 45;

            // Création d'un objet Mail
            Mail mail = new Mail();
            mail.to = "stievenart.m@gmail.com";
            mail.subject = "Mail from Dynamic-mail project";
            mail.content = "Bonjour M. {{lastName}}. Vous avez {{age}} ans et vous êtes {{job}}. Pouvez-vous nous confirmer que votre adresse email est bien la {{email}} ?";
            mail.dynamicValues = dyn;

            mail.content = dynamicMailValue(mail.content, dyn);

            if (sendMail(mail))
            {
                return "Mail envoyé";
            }
            else
            {
                return "Erreur lors de l'envoi du mail";
            }
        }

        // Function de remplacement des valeurs dynamiques définis dans le Web.config
        // @dyn : Values to replace in content
        private String dynamicMailValue(String content, dynamic dyn)
        {
            String dynamicValues = ConfigurationManager.AppSettings["dynamic_mail_values"].ToString();
            String[] dynamicValuesArray = dynamicValues.Split(',');

            foreach (String dynamic in dynamicValuesArray)
            {
                PropertyInfo prop = typeof(DynamicValues).GetProperty(dynamic.Replace("{{", "").Replace("}}", ""));

                if (prop != null)
                {
                    object value = prop.GetValue(dyn);

                    if (value != null)
                    {
                        content = content.Replace(dynamic, value.ToString());
                    }
                }
            }

            return content;
        }

        // Function d'envoi de mail
        private bool sendMail(Mail mail)
        {
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["smtp_server"]);

            if (ConfigurationManager.AppSettings["smtp_port"] != "")
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]);

            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtp_user"], ConfigurationManager.AppSettings["smtp_password"]);
            smtp.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["smtp_from"]);
            message.To.Add(mail.to);
            message.Subject = mail.subject;
            message.Body = mail.content;
            message.IsBodyHtml = true;

            try
            {
                smtp.Send(message);
                return true;
            }
            catch (Exception e)
            {
                String log_file = ConfigurationManager.AppSettings["log_file"];
                using (StreamWriter testData = new StreamWriter(log_file, true))
                {
                    testData.WriteLine("==== " + DateTime.Now + " ERREUR SEND MAIL ===");
                    testData.WriteLine(e.Message);
                    testData.WriteLine(e.StackTrace);
                    testData.WriteLine(e.InnerException);
                }
                return false;
            }
        }
    }
}
