using System;

namespace GracePointeSecurity.Library;

public sealed class ProductKey
{
    public Guid InstallGuid { get; set; }

    public string OrgName { get; set; }
}