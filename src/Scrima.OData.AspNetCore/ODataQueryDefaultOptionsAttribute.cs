using System;

namespace Scrima.OData.AspNetCore;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ODataQueryDefaultOptionsAttribute : Attribute
{
    public const int None = -1;
    public const int Inherit = -2;
    
    private bool? _alwaysShowCount;
    public int MaxTop { get; set; } = Inherit;
    public int DefaultTop { get; set; } = Inherit;

    public bool AlwaysShowCount
    {
        get => _alwaysShowCount.GetValueOrDefault();
        set => _alwaysShowCount = value;
    }

    public ODataQueryDefaultOptions BuildOptions(ODataQueryDefaultOptions inheritFrom = null)
    {
        return new ODataQueryDefaultOptions
        {
            MaxTop = MaxTop switch
            {
                None => null,
                Inherit => inheritFrom?.MaxTop,
                _ => MaxTop
            },
            DefaultTop = DefaultTop switch
            {
                None => null,
                Inherit => inheritFrom?.MaxTop,
                _ => DefaultTop
            },
            AlwaysShowCount = _alwaysShowCount ?? inheritFrom?.AlwaysShowCount ?? false
        };
    }
}
