using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Models.Messages;

namespace User.API.Infrastructure.Services.AzureServiceBusSender.Interface
{
    public interface IDeleteUserMessageSender
    {
        Task PublishMessage(BaseMessage baseMessage);
    }
}
