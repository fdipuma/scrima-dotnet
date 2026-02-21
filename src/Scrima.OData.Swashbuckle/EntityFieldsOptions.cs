using System;

namespace Scrima.OData.Swashbuckle;

public class EntityFieldsOptions
{
    public ShowEntityFieldsOptions Show { get; set; } = ShowEntityFieldsOptions.None;
    public bool ExposeAsExtensions { get; set; } = true;
}


[Flags]
public enum ShowEntityFieldsOptions
{
    None = 0b0000,
    OnFilter = 0b0001,
    OnOrder = 0b0010,
    OnAllODataFields = OnFilter | OnOrder,
}
