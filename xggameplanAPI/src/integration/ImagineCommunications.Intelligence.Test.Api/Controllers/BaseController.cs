using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public abstract class BaseController<TCreate, TUpdate, TDelete, TBulkCreate, TBulkUpdate, TBulkDelete> : ApiController
        where TCreate : class, IEvent
        where TUpdate : class, IEvent
        where TDelete : class, IEvent
        where TBulkCreate : class, IBulkEvent<IEvent>
        where TBulkUpdate : class, IBulkEvent<IEvent>
        where TBulkDelete : class, IEvent
    {
        protected readonly IServiceBus ServiceBus;

        protected BaseController(IServiceBus serviceBus)
        {
            ServiceBus = serviceBus;
        }

        protected virtual async Task<IHttpActionResult> Create(TCreate createModel, Guid? groupTransactionId = null)
        {
            try
            {
                var messageId = await ServiceBus.PublishAsync(createModel, groupTransactionId);
                return Ok(messageId);
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    e.Message,
                    e.StackTrace,
                    InnerExceptionMessage = e.InnerException?.Message,
                    InnerExceptionStackTrace = e.InnerException?.StackTrace
                });
            }
        }

        protected virtual async Task<IHttpActionResult> Update(TUpdate updateModel, Guid? groupTransactionId = null)
        {
            try
            {
                _ = await ServiceBus.PublishAsync(updateModel, groupTransactionId);
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    e.Message,
                    e.StackTrace,
                    InnerExceptionMessage = e.InnerException?.Message,
                    InnerExceptionStackTrace = e.InnerException?.StackTrace
                });
            }
        }

        protected virtual async Task<IHttpActionResult> Delete(TDelete deleteModel, Guid? groupTransactionId = null)
        {
            try
            {
                _ = await ServiceBus.PublishAsync(deleteModel, groupTransactionId);
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    e.Message,
                    e.StackTrace,
                    InnerExceptionMessage = e.InnerException?.Message,
                    InnerExceptionStackTrace = e.InnerException?.StackTrace
                });
            }
        }

        protected virtual async Task<IHttpActionResult> BulkCreate(TBulkCreate bulkCreateModel, Guid? groupTransactionId = null)
        {
            try
            {
                _ = await ServiceBus.PublishAsync(bulkCreateModel, groupTransactionId);
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    e.Message,
                    e.StackTrace,
                    InnerExceptionMessage = e.InnerException?.Message,
                    InnerExceptionStackTrace = e.InnerException?.StackTrace
                });
            }
        }

        protected virtual async Task<IHttpActionResult> BulkUpdate(TBulkUpdate bulkUpdateModel, Guid? groupTransactionId = null)
        {
            try
            {
                _ = await ServiceBus.PublishAsync(bulkUpdateModel, groupTransactionId);
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    e.Message,
                    e.StackTrace,
                    InnerExceptionMessage = e.InnerException?.Message,
                    InnerExceptionStackTrace = e.InnerException?.StackTrace
                });
            }
        }

        protected virtual async Task<IHttpActionResult> BulkDelete(TBulkDelete bulkUpdateModel, Guid? groupTransactionId = null)
        {
            try
            {
                _ = await ServiceBus.PublishAsync(bulkUpdateModel, groupTransactionId);
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    e.Message,
                    e.StackTrace,
                    InnerExceptionMessage = e.InnerException?.Message,
                    InnerExceptionStackTrace = e.InnerException?.StackTrace
                });
            }
        }
    }
}
