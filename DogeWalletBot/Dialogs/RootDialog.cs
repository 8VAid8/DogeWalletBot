using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DogeWalletBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;


            if (activity.Text == "/setwallet") // if "/setwallet" command
            {
                context.Call(new SetWalletDialog(), SetWalletDialogResumeAfter);
            }
            else if (activity.Text.Contains("/setwallet")) // if "/setwallet [address]" command
            {
                var forvardedMsg = context.MakeMessage();
                forvardedMsg.Text = activity.Text;
                await context.Forward(new SetWalletDialog(), SetWalletDialogResumeAfter, forvardedMsg, CancellationToken.None);
            }
            else if (activity.Text.Contains("/balance")) // if "/balance [address]" command
            {
                var forvardedMsg = context.MakeMessage();
                forvardedMsg.Text = activity.Text;
                await context.Forward(new GetBalanceDialog(), GetBalanceDialogResumeAfter, forvardedMsg, CancellationToken.None);
            }
            else if (activity.Text.Contains("/received")) // if "/received [address]" command
            {
                var forvardedMsg = context.MakeMessage();
                forvardedMsg.Text = activity.Text;
                await context.Forward(new GetReceivedDialog(), GetReceivedDialogResumeAfter, forvardedMsg, CancellationToken.None);
            }
            else if (activity.Text.Contains("/sent")) // if "/sent [address]" command
            {
                var forvardedMsg = context.MakeMessage();
                forvardedMsg.Text = activity.Text;
                await context.Forward(new GetSentDialog(), GetSentDialogResumeAfterAsync, forvardedMsg, CancellationToken.None);
            }
            else if (activity.Text.Contains("/qrcode")) // if "/qrcode [address]" command
            {
                var forvardedMsg = context.MakeMessage();
                forvardedMsg.Text = activity.Text;
                await context.Forward(new GetQRCodeDialog(), GetQRCodeDialogResumeAfterAsync, forvardedMsg, CancellationToken.None);
            }
            else if (activity.Text.Contains("/report")) // if "/report [address]" command
            {
                //    var forvardedMsg = context.MakeMessage();
                //    forvardedMsg.Text = activity.Text;
                //    await context.Forward(new GetQRCodeDialog(), GetQRCodeDialogResumeAfterAsync, forvardedMsg, CancellationToken.None);
                await context.PostAsync("NotImplemented yet...");
            }
            else
            {
                if (activity.Text == "/start") //start conversation
                    GreetUser(context, result);
                else if (activity.Text == "/help") //show help
                    ShowHelp(context);

                context.Wait(MessageReceivedAsync);
            } 
        }

        private async Task GetQRCodeDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
        }

        private async Task GetSentDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
        }

        private async Task GetReceivedDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
        }

        private async Task GetBalanceDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
        }

        private async Task SetWalletDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            string address = await result;
            if(!string.IsNullOrEmpty(address))
                await context.PostAsync($"Saved DogeCoin wallet address: {address}.");
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
        }

        private void ShowHelp(IDialogContext context)
        {
            string helpStartText = "Attach your DogeCoin wallet (/setwallet [address]) and you will be able perform this commands:\r\n";
            string commands = "/balance - Returns DogeCoin wallet balance\r\n";
            context.PostAsync(helpStartText + commands);
        }

        private void GreetUser(IDialogContext context, IAwaitable<object> result)
        {
            context.PostAsync("Welcome, Young Shibe!\r\n");
            ShowHelp(context);
        }
    }
}