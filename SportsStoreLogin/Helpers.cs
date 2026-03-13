using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
}
