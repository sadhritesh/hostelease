using HostelEase.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace HostelEase.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }
        public async Task SendAccountCreatedEmailAsync(string toEmail, string firstName, string loginLink)
        {
            string body = $@"
                <html><body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin:0; padding:20px;'>
                  <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                    <h2 style='color:#333;'>Hello, {firstName}!</h2>
                    <p style='font-size:16px; color:#555;'>Your account has been successfully created and your email is confirmed.</p>
                    <p style='text-align:center;'>
                      <a href='{loginLink}' style='background:#198754; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Login to Your Account</a>
                    </p>
                    <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} HostelEase. All rights reserved.</p>
                  </div>
                </body></html>";
            await SendEmailAsync(toEmail, "Account Created - HostelEase", body, true);
        }

        public async Task SendRegistrationConfirmationEmailAsync(string toEmail, string firstName, string confirmationLink)
        {
            string body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin:0; padding:20px;'>
                        <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                            <h2 style='color:#333;'>Welcome, {firstName}!</h2>
                            <p style='font-size:16px; color:#555;'>Thank you for registering. Please confirm your email by clicking the button below.</p>
                            <p style='text-align:center;'>
                                <a href='{confirmationLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Confirm Your Email</a>
                            </p>
                            <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} HostelEase. All rights reserved.
                            </p>
                        </div>
                    </body>
               </html>";
            await SendEmailAsync(toEmail, "Email Confirmation - HostelEase", body, true);

        }

        public async Task ResendConfirmationEmailAsync(string toEmail, string firstName, string confirmationLink)
        {
            string body = $@"
                <html><body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin:0; padding:20px;'>
                  <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                    <h2 style='color:#333;'>Hello, {firstName}!</h2>
                    <p style='font-size:16px; color:#555;'>You requested a new email confirmation link. Please confirm your email by clicking the button below.</p>
                    <p style='text-align:center;'>
                      <a href='{confirmationLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Confirm Your Email</a>
                    </p>
                    <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year}HostelEase. All rights reserved.</p>
                  </div>
                </body></html>";
            await SendEmailAsync(toEmail, "Email Confirmation - HostelEase", body, true);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:Password"];

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink)
        {
            string html = $@"
            <html><body style='font-family: Arial, sans-serif; background:#f4f6f8; margin:0; padding:20px;'>
              <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                <h2 style='color:#333;'>Password Reset Request</h2>
                <p style='font-size:16px; color:#555;'>Hi {firstName},</p>
                <p style='font-size:16px; color:#555;'>We received a request to reset your password. Click the button below to choose a new one.</p>
                <p style='text-align:center;'>
                  <a href='{resetLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Reset Password</a>
                </p>
                <p style='font-size:13px; color:#777;'>If you didn't request this, you can ignore this email.</p>
                <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} HostelEase. All rights reserved.</p>
              </div>
            </body></html>";
            await SendEmailAsync(toEmail, "Reset Your Password - HostelEase", html, true);
        }
    }
}
