using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SportsStoreLogin
{
    class Gui
    {
        public static async Task loadAnimation(Button buttonForAnimation, CancellationToken token)
        {
            string originalContent = buttonForAnimation.Content.ToString();
            buttonForAnimation.IsEnabled = false;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    buttonForAnimation.Content = $"{originalContent} .";
                    await Task.Delay(400, token);
                    buttonForAnimation.Content = $"{originalContent} . .";
                    await Task.Delay(400, token);
                    buttonForAnimation.Content = $"{originalContent} . . .";
                    await Task.Delay(400, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                buttonForAnimation.Content = originalContent;
                buttonForAnimation.IsEnabled = true;
            }
        }
    }

    class Message
    {
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarn(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    class Validation
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();

                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
