using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using Team9.Models;


public class EmailMessage
{
    public static void confirmEmail(String toEmailAddress, Purchase p)
    {

    }
    public static void giftEmail(String toEmailAddress, Purchase p)
    {

    }
    public static void SendEmail(String toEmailAddress, String emailSubject, String emailBody)
    {
        //create an email client to send emails
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("longhornmusic0@gmail.com", "shanebuechele"),
            EnableSsl = true
        };

        //add anything you need to the body of the message
        // /n is a new line -  this will add some white space after the main body of the message
        String finalMessage = emailBody + "\n\n This is a disclaimer that will be on all messages";

        //create an email address object for the sender address
        MailAddress senderEmail = new MailAddress("mydummyemail@gmail.com", "Team 9");

        MailMessage mm = new MailMessage();
        mm.Subject = emailSubject;
        mm.From = senderEmail;
        mm.To.Add(new MailAddress(toEmailAddress));
        mm.Body = finalMessage;
        client.Send(mm);
    }
}