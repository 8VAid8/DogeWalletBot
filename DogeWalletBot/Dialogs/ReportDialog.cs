using Budget.Bot.DAL;
using DogeWalletBot.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogeWalletBot.Dialogs
{
    [Serializable]
    internal class ReportDialog : IDialog<object>
    {
        private int attempts = 3;
        public string ExceptionMessage { get; set; } = "There was some errors, enter valid DogeCoin wallet address, please!";
        private string ExceptionFinalMessage { get; set; } = $"Wow, we can't get the qr code of specified address. May be address is wrong, check it, please and try again later.";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            await Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            string address = "";
            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                if (message.Text.Contains(" ")) //if "/qrcode [address]" command
                {
                    address = message.Text.Replace("/report ", "").Trim();
                }
                else
                    context.UserData.TryGetValue("wallet", out address);

                if (!string.IsNullOrEmpty(address))
                    try
                    {
                        var receivedTransactions = await Client.GetReceivedTransactions(address);
                        if (receivedTransactions != null)
                        {
                            await SendReport(context, address, receivedTransactions);
                            context.Done(0);
                        }
                        else
                            await ProcessErrors(context);
                    }
                    catch (Exception ex)
                    {
                        await context.PostAsync(ex.Message);
                        await ProcessErrors(context);
                    }
                else
                {
                    await context.PostAsync($"Set wallet addres first (/setwallet [addrss]) or call /qrcode [address] command!");
                    context.Fail(new Exception("DogeCoin wallet address wasn't saved!"));
                }
            }
        }

        private async Task SendReport(IDialogContext context, string address, List<ReceivedTransaction> transactions)
        {
            Reporter repr = new Reporter();
            //if (context.Activity.ChannelId != "telegram")
            //    return;

            using (MemoryStream pdfReport = repr.GetReceivedTransactionsPdf(address, transactions))
            {
                TelegramBotClient client = new TelegramBotClient("token");
                var me = await client.GetMeAsync();

                var chatId = context.Activity.From.Id;

                FileToSend fileToSend = new FileToSend
                {
                    Content = pdfReport,
                    Filename = "report.pdf"
                };

                await client.SendDocumentAsync(chatId, new FileToSend("Received transactions.pdf", pdfReport), $"Received by {address} address transactions");
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



        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    //var reportMsg = context.MakeMessage();
        //    //reportMsg.Attachments.Add(new Attachment()
        //    //{
        //    //    ContentUrl = "http://www.pdf995.com/samples/pdf.pdf",
        //    //    ContentType = "application/pdf",
        //    //    Name = "Report"
        //    //});
        //    //await context.PostAsync(reportMsg);
        //}
    }
}