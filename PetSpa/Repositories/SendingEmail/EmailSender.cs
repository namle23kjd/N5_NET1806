

using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
namespace PetSpa.Repositories.SendingEmail
{

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emialConfig;
        public EmailSender(EmailSettings emialConfig) => _emialConfig = emialConfig;

        public async void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendEmailAsync(emailMessage);
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emialConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;

        }

        private async Task SendEmailAsync(MimeMessage message)
        {
            using var client = new SmtpClient();
            try
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true;

                    // Bỏ qua kiểm tra trạng thái thu hồi chứng chỉ
                    if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                    {
                        foreach (var chainStatus in chain.ChainStatus)
                        {
                            if (chainStatus.Status == X509ChainStatusFlags.RevocationStatusUnknown)
                                continue;

                            if (chainStatus.Status != X509ChainStatusFlags.NoError)
                                return false;
                        }
                    }

                    return true;
                };

                await client.ConnectAsync(_emialConfig.SmtpServer, _emialConfig.Port, true).ConfigureAwait(false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emialConfig.Username, _emialConfig.Password).ConfigureAwait(false);
                await client.SendAsync(message).ConfigureAwait(false);
            }
            catch (SmtpCommandException ex)
            {
                Console.WriteLine($"Lỗi Lệnh SMTP: {ex.Message}");
                throw new Exception("Lỗi lệnh SMTP khi gửi email", ex);
            }
            catch (SmtpProtocolException ex)
            {
                Console.WriteLine($"Lỗi Giao thức SMTP: {ex.Message}");
                throw new Exception("Lỗi giao thức SMTP khi gửi email", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi chung: {ex.Message}");
                throw new Exception("Xảy ra lỗi khi gửi email", ex);
            }
            finally
            {
                await client.DisconnectAsync(true).ConfigureAwait(false);
                client.Dispose();
            }
        }
    }
}
