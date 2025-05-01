using System;
using Microsoft.Extensions.Hosting;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.Contracts;

public interface IMessageScheduleService
{
    public Task<bool> SendWhatsAppMessage(WhatsAppMessage message);
}
