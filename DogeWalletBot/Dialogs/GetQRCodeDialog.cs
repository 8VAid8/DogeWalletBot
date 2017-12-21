using Budget.Bot.DAL;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace DogeWalletBot.Dialogs
{
    [Serializable]
    internal class GetQRCodeDialog : IDialog<object>
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
                    address = message.Text.Replace("/qrcode ", "").Trim();
                }
                else
                    context.UserData.TryGetValue("wallet", out address);

                if (!string.IsNullOrEmpty(address))
                    try
                    {
                        var qrCodeLink = await Client.GetQRCode(address);
                        if (qrCodeLink != null)
                        {
                            await SendQrCode(context, address, qrCodeLink);
                            context.Done(0);
                        }
                        else
                            await ProcessErrors(context);
                    }
                    catch (Exception ex)
                    {
                        //await context.PostAsync(ex.Message);
                        await ProcessErrors(context);
                    }
                else
                {
                    await context.PostAsync($"Set wallet addres first (/setwallet [addrss]) or call /qrcode [address] command!");
                    context.Fail(new Exception("DogeCoin wallet address wasn't saved!"));
                }
            }
        }

        private async Task SendQrCode(IDialogContext context, string address, string qrCodeLink)
        {
            var qrMsg = context.MakeMessage();
            qrMsg.Text = $"QR code of {address} address:";
            qrMsg.Attachments.Add(new Attachment()
            {
                ContentUrl = qrCodeLink,
                ContentType = "image/png",
                Name = "DogeWalletAddressQRCode.png"
            });
            await context.PostAsync(qrMsg);
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