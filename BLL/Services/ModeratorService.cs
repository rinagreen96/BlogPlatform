using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Entities;
using BLL.Interfaces;
using BLL.Infrastructure;
using BLL.DTO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace BLL.Services
{
    public class ModeratorService: IModeratorService
    {
        IUnitOfWork Database { get; set; }
        public ModeratorService(IUnitOfWork uow)
        {
            Database = uow;
        }
        public void Send(string UserAdress, string message_text)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("katya_dance.96@mail.ru");
            message.To.Add(new MailAddress(UserAdress)); // кому отправлять  
            message.Subject = "BYL_administration"; // тема письма  
            message.Body = message_text;
            SmtpClient client = new SmtpClient("smtp.mail.ru");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Port = 587; // указываем порт  
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials =
            new NetworkCredential("katya_dance.96@mail.ru", "00ohazug1234");
            client.Send(message);  // отправить  
        }
        public void SendingEmailComment(string what_happened, int comment_id)
        {
            string UserAdress = Database.Comments.Get(comment_id).ApplicationUser.Email;
            string message_text = "";
            if (what_happened == "rejected")
            {
                message_text = "Your comment: "+Database.Comments.Get(comment_id).text+ " was rejected. Please, follow the rules of decency!";
            }
            if (what_happened == "approved")
            {
                message_text = "Your comment: " + Database.Comments.Get(comment_id).text + " was approved.";
            }
            Send(UserAdress, message_text);  
        }
        public void SendingEmailArticle(string what_happened, int article_id)
        {
            string UserAdress = Database.Articles.Get(article_id).Blog.ApplicationUser.Email;
            string message_text = "";
            if (what_happened == "rejected")
            {
                message_text = "Your article: " + Database.Comments.Get(article_id).text + " was rejected. Please, follow the rules of decency!";
            }
            if (what_happened == "approved")
            {
                message_text = "Your article: " + Database.Comments.Get(article_id).text + " was approved.";
            }
            Send(UserAdress, message_text);
        }
    }
}
