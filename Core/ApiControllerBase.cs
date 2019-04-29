using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Http;
using Sagicor.Core.Common.Contracts;

namespace SagicorNow.Core
{
    public class ApiControllerBase : ApiController, IServiceAwareController
    {
        private List<IServiceContract> _disposableServices;

        protected virtual void RegisterServices(List<IServiceContract> disposableServices)
        {
        }

        void IServiceAwareController.RegisterDisposableServices(List<IServiceContract> disposableServices)
        {
            RegisterServices(disposableServices);
        }

        List<IServiceContract> IServiceAwareController.DisposableServices => _disposableServices ?? (_disposableServices = new List<IServiceContract>());

        protected HttpResponseMessage GetHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> codeToExecute)
        {
            HttpResponseMessage response = null;

            try
            {
                response = codeToExecute.Invoke();
            }
            catch (SecurityException ex)
            {
                response = request.CreateResponse(HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (FaultException ex)
            {
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        protected Task<HttpResponseMessage> GetHttpResponse(HttpRequestMessage request, Func<Task<HttpResponseMessage>> codeToExecute)
        {
            Task<HttpResponseMessage> response = null;

            try
            {
                response = codeToExecute.Invoke();
            }
            catch (SecurityException ex)
            {
                response = Task.FromResult(request.CreateResponse(HttpStatusCode.Unauthorized, ex.Message));
            }
            catch (FaultException ex)
            {
                response = Task.FromResult(request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
            catch (Exception ex)
            {
                response = Task.FromResult(request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }

            return response;
        }
    }
}
