using System;
using System.Linq.Expressions;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.DTOs;

namespace Scheduley.Core.Contracts;

public interface IMessageService<T, DTO>
    where T : Message
    where DTO : MessageRequest
{
    public Task<List<T>> GetMessages();
    public Task<T?> GetMessageByID(Guid messageID);
    public Task<T> CreateMessage(Guid messageSenderID, DTO messageRequest);
    public Task<bool> UpdateMessage(Guid messageID, DTO message);
    public Task<bool> DeleteMessage(Guid messageID);
    public Task<bool> SendMessage(T message);
    public Task<List<T>> GetMessagesFiltered(Expression<Func<T, bool>> predicate);
}
