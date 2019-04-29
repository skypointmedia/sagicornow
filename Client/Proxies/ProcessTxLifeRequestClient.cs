using System.ComponentModel.Composition;
using SagicorNow.Client.Contracts;

// This namespace here is off because partial classes needs to be within same namespace.
namespace SagicorNow.Foresight
{
    [Export(typeof(IProcessTXLifeRequestClient))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ProcessTXLifeRequestClient : IProcessTXLifeRequestClient
    {

    }
}