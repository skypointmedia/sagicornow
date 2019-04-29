using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sagicor.Core.Common.Contracts;

namespace SagicorNow.Core
{
    public class ViewControllerBase : Controller
    {
        private List<IServiceContract> _disposableServices;

        protected virtual void RegisterServices(List<IServiceContract> disposableServices)
        {
        }

        private List<IServiceContract> DisposableServices => _disposableServices ?? (_disposableServices = new List<IServiceContract>());

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            RegisterServices(DisposableServices);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            foreach (var service in DisposableServices)
            {
                if (service is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}