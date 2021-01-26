using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Scrima.OData.AspNetCore
{
    internal class RemoveQueryOptionsActionDescriptorProvider : IActionDescriptorProvider
    {
        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var actionDescriptor in context.Results)
            {
                foreach (var param in actionDescriptor.Parameters.Where(p => p.ParameterType.IsODataQuery()))
                {
                    actionDescriptor.Properties["scrima-odata-query"] = param.ParameterType;
                }
            }
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }

        public int Order => 0;
    }
}
