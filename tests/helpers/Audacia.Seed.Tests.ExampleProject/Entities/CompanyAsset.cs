namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class CompanyAsset
{
    public int Id { get; set; }

    //public ICollection<>
}

public class RoomAsset : CompanyAsset
{
}

public class PoolAsset : CompanyAsset
{
}

public class EmployeeAsset : CompanyAsset
{
}