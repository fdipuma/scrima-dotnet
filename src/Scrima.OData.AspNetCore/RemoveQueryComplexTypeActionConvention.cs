using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Scrima.OData.AspNetCore
{
    internal class RemoveQueryOptionsActionDescriptorProvider : IActionDescriptorProvider
    {
        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var actionDescriptor in context.Results)
            {
                foreach (var param in actionDescriptor.Parameters.Where(p => p.ParameterType.IsQueryOptions()).ToList())
                {
                    actionDescriptor.Parameters.Add(new ParameterDescriptor
                    {
                        Name = param.Name,
                        ParameterType = typeof(ODataQueryParams<>).MakeGenericType(param.ParameterType.GetGenericArguments()[0]),
                        BindingInfo = new BindingInfo { BindingSource = BindingSource.Special },
                    });
                    actionDescriptor.Properties["scrima-odata-query"] = param.ParameterType;
                    actionDescriptor.Parameters.Remove(param);
                }
            }
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }

        public int Order => 0;
    }
}
