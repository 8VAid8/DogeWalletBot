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
                var forvardedMsg = context.MakeMessage();
                forvardedMsg.Text = activity.Text;
                await context.Forward(new ReportDialog(), ReportDialogResumeAfterAsync, forvardedMsg, CancellationToken.None);
                //await context.PostAsync("NotImplemented yet...");
                //context.Wait(MessageReceivedAsync);
            }
            else
            {
                if (activity.Text == "/start") //start conversation
                    await GreetUser(context, result);
                else if (activity.Text == "/help") //show help
                    await ShowHelp(context);

                context.Wait(MessageReceivedAsync);
            } 
        }

        private async Task ReportDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
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

        private async Task ShowHelp(IDialogContext context)
        {
            string helpStartText = "Attach your DogeCoin wallet (/setwallet [address]) and you will be able perform this commands:\n\n";
            string commands = "/balance [address] - Returns DogeCoin wallet balance from specified or default address\n\n" +
                "/received [address] - Returns the received DogeCoins by specified or default address\n\n" +
                "/sent [address] - Returns the sent from specified or default address DogeCoins\n\n" +
                "/qrcode [address] - Returns the qrcode of specified or default wallet address\n\n" +
                "/report [address] - Returns specified or default wallet address report";
            await context.PostAsync(helpStartText + commands);
        }

        private async Task GreetUser(IDialogContext context, IAwaitable<object> result)
        {
            await SendGreetMessage(context);
            await ShowHelp(context);
        }

        private async Task SendGreetMessage(IDialogContext context)
        {
            var qrMsg = context.MakeMessage();
            qrMsg.Text = "Welcome, Young Shibe!\r\n";
            qrMsg.Attachments.Add(new Attachment()
            {
                ContentUrl = "http://www.stickpng.com/assets/images/5845e608fb0b0755fa99d7e7.png",
                ContentType = "image/png",
				Name = " "
            });
            await context.PostAsync(qrMsg);
        }
    }
}