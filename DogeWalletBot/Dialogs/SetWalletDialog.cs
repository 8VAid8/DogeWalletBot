using System;
using System.Threading.Tasks;
using Budget.Bot.DAL;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DogeWalletBot.Dialogs
{
    [Serializable]
    internal class SetWalletDialog : IDialog<string>
    {
        private int attempts = 3;
        public string ExceptionMessage { get; set; } = "There was some errors, enter valid DogeCoin wallet address, please!";
        private string ExceptionFinalMessage { get; set; } = $"Wow, we can't save your address. May be it is wrong, check it, please and try again later.";

        public async Task StartAsync(IDialogContext context)
        {
            var msg = context.Activity.AsMessageActivity();
            if (msg.Text == "/setwallet")
                await context.PostAsync("Enter DogeCoin wallet address, please!");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            string address = "";
            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                if (message.Text.Contains(" ")) //if "/setwallet [address]" command
                {
                    address = message.Text.Replace("/setwallet ", "").Trim();
                }
                else
                    address = message.Text;
                try
                {
                    var balance = await Client.GetBalanceAsync(address);
                    if (balance.Success == 1)
                    {
                        context.UserData.SetValue("wallet", address);
                        context.Done(address);
                    }
                    else
                        await ProcessErrors(context);
                }
                catch(Exception ex)
                {
                    await context.PostAsync(ex.Message);
                    await ProcessErrors(context);
                }
            }
        }

        private async Task ProcessErrors(IDialogContext context)
        {
            --attempts;
            if (attempts > 0)
            {
                await context.PostAsync(ExceptionMessage);
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                    parent/calling dialog. */
                context.Fail(new TooManyAttemptsException(ExceptionFinalMessage));
            }
        }
    }
}