using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows;
using Microsoft.Win32;

namespace EmailSender
{
    public partial class MainWindow : Window
    {
        private string smtpServer = "smtp.gmail.com"; 
        private int smtpPort = 587; 
        private string emailAddress;
        private string emailPassword;
        private List<string> attachments = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            emailAddress = EmailTextBox.Text;
            emailPassword = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(emailAddress) || string.IsNullOrWhiteSpace(emailPassword))
            {
                MessageBox.Show("Please enter both email and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            SendEmailButton.IsEnabled = true;
            MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AttachFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select Files to Attach"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                attachments.Clear();
                attachments.AddRange(openFileDialog.FileNames);
                AttachmentsTextBlock.Text = $"{attachments.Count} files attached";
            }
        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ToTextBox.Text))
            {
                MessageBox.Show("Please enter a recipient email address.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(emailAddress);
                    message.To.Add(ToTextBox.Text);
                    message.Subject = SubjectTextBox.Text;
                    message.Body = MessageTextBox.Text;

                    if (ImportantCheckBox.IsChecked == true)
                    {
                        message.Headers.Add("X-Priority", "1"); // Важливе повідомлення
                    }

                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(new Attachment(attachment));
                    }

                    using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(emailAddress, emailPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(message);
                    }

                    MessageBox.Show("Email sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SmtpException smtpEx)
            {
                MessageBox.Show($"SMTP Error: {smtpEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending email: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}