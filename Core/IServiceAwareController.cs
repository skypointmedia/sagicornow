using System.Collections.Generic;
using Sagicor.Core.Common.Contracts;

namespace SagicorNow.Core
{
    public interface IServiceAwareController
    {
        void RegisterDisposableServices(List<IServiceContract> disposableServices);

        List<IServiceContract> DisposableServices { get; }
    }
}