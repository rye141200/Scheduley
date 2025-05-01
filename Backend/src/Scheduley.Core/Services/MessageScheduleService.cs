using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.DTOs;
using Scheduley.Core.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Scheduley.Core.Services;

public class MessageScheduleService(
    ILogger<MessageScheduleService> logger,
    IConfiguration config,
    IOptions<TwilioOptions> twilioConfig,
    IServiceProvider serviceProvider
) : BackgroundService, IMessageScheduleService
{
    public async Task<bool> SendWhatsAppMessage(WhatsAppMessage message)
    {
        //!1) Implement the logic to send the real whatsapp message using Twilio
        TwilioClient.Init(twilioConfig.Value.AccountSID, twilioConfig.Value.AuthToken);

        var senderWhatsAppNumber = new PhoneNumber("whatsapp:+14155238886");

        var recipientWhatsAppNumber = new PhoneNumber("whatsapp:+201092536110");
        var recipientTwo = new PhoneNumber("whatsapp:+201065065401");

        var sendedMessage = await MessageResource.CreateAsync(
            body: "Hello from Scheduley, this automated message is sent for you because you are a big توتة 🧑🏿‍🦲 can you guess the sender",
            from: senderWhatsAppNumber,
            to: recipientWhatsAppNumber
        );
        var sendedMessageTwo = await MessageResource.CreateAsync(
            body: "Hello from Scheduley, this automated message is sent for you because you are a big توتة 🧑🏿‍🦲 can you guess the sender",
            from: senderWhatsAppNumber,
            to: recipientTwo
        );

        return true;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Background service working");
            using var scope = serviceProvider.CreateScope();

            var whatsAppMessageService = scope.ServiceProvider.GetRequiredService<
                IMessageService<WhatsAppMessage, WhatsAppMessageRequest>
            >();

            var messages = await whatsAppMessageService.GetMessagesFiltered(message =>
                message.ScheduledDate.CompareTo(DateTime.UtcNow) == -1
            );

            messages.ForEach(async message => await SendWhatsAppMessage(message));

            await Task.Delay(
                TimeSpan.FromMinutes(Convert.ToInt64(config["Task:DelayMinutes"])),
                stoppingToken
            );
        }
    }
}
