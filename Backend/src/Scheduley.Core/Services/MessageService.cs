using System;
using System.Linq.Expressions;
using System.Xml;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Domain.RepositoryContracts;
using Scheduley.Core.DTOs;

namespace Scheduley.Core.Services;

public class MessageService<T, DTO>(IGenericRepository<T> messageRepository)
    : IMessageService<T, DTO>
    where T : Message
    where DTO : MessageRequest
{
    public async Task<T> CreateMessage(Guid messageSenderID, DTO messageRequest)
    {
        //!1) Construct the message
        T message = (T)messageRequest.ToMessage(messageSenderID);

        //!2) Save to database
        await messageRepository.Create(message);
        await messageRepository.SaveChangesAsync();

        return message;
    }

    public async Task<bool> DeleteMessage(Guid messageID)
    {
        T? message = await messageRepository.GetOne(messageID);

        if (message == null)
            return false;

        messageRepository.Delete(message);
        await messageRepository.SaveChangesAsync();
        return true;
    }

    public Task<T?> GetMessageByID(Guid messageID) => messageRepository.GetOne(messageID);

    public async Task<List<T>> GetMessages() => [.. await messageRepository.GetAll()];

    public async Task<List<T>> GetMessagesFiltered(Expression<Func<T, bool>> predicate) =>
        [.. await messageRepository.Find(predicate)];

    public Task<bool> SendMessage(T message)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateMessage(Guid messageID, DTO message)
    {
        T? oldMessage = await messageRepository.GetOne(messageID);
        if (oldMessage == null)
            return false;

        messageRepository.Update((T)message.ToMessage(oldMessage.UserId));
        await messageRepository.SaveChangesAsync();
        return true;
    }
}
