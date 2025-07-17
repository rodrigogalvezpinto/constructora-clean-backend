using Bogus;
using ConstructoraClean.Api.DTOs;
using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Api.Tests.Helpers
{
    public static class TestDataBuilder
    {
        public static Faker<Project> ProjectFaker => new Faker<Project>()
            .RuleFor(p => p.Id, f => f.Random.Int(1, 1000))
            .RuleFor(p => p.Name, f => f.Company.CompanyName())
            .RuleFor(p => p.Budget, f => f.Random.Decimal(10000, 500000))
            .RuleFor(p => p.RegionId, f => f.Random.Int(1, 10));

        public static Faker<Material> MaterialFaker => new Faker<Material>()
            .RuleFor(m => m.Id, f => f.Random.Int(1, 1000))
            .RuleFor(m => m.Name, f => f.Commerce.ProductName());

        public static Faker<Purchase> PurchaseFaker => new Faker<Purchase>()
            .RuleFor(p => p.Id, f => f.Random.Int(1, 1000))
            .RuleFor(p => p.ProjectId, f => f.Random.Int(1, 100))
            .RuleFor(p => p.MaterialId, f => f.Random.Int(1, 100))
            .RuleFor(p => p.SupplierId, f => f.Random.Int(1, 50))
            .RuleFor(p => p.TotalCost, f => f.Random.Decimal(100, 10000));

        public static Faker<TopMaterialDto> TopMaterialDtoFaker => new Faker<TopMaterialDto>()
            .RuleFor(t => t.Material, f => f.Commerce.ProductName())
            .RuleFor(t => t.TotalCost, f => f.Random.Decimal(1000, 50000));

        public static Faker<MonthlyBreakdownDto> MonthlyBreakdownDtoFaker => new Faker<MonthlyBreakdownDto>()
            .RuleFor(m => m.Month, f => f.Date.Recent(365).ToString("yyyy-MM"))
            .RuleFor(m => m.TotalCost, f => f.Random.Decimal(5000, 100000));

        public static Faker<RegionOverrunDto> RegionOverrunDtoFaker => new Faker<RegionOverrunDto>()
            .RuleFor(r => r.ProjectId, f => f.Random.Int(1, 1000))
            .RuleFor(r => r.Name, f => f.Company.CompanyName())
            .RuleFor(r => r.Budget, f => f.Random.Decimal(50000, 500000))
            .RuleFor(r => r.TotalCost, f => f.Random.Decimal(55000, 600000))
            .RuleFor(r => r.OverrunPct, (f, r) => r.Budget > 0 ? (r.TotalCost / r.Budget - 1) : null);

        public static ProjectCostsDto CreateProjectCostsDto(
            decimal totalCost = 100000m,
            int materialsCount = 5,
            int monthsCount = 6)
        {
            return new ProjectCostsDto
            {
                TotalCost = totalCost,
                TopMaterials = TopMaterialDtoFaker.Generate(materialsCount),
                MonthlyBreakdown = MonthlyBreakdownDtoFaker.Generate(monthsCount)
            };
        }

        public static List<RegionOverrunDto> CreateRegionOverrunList(int count = 5)
        {
            return RegionOverrunDtoFaker.Generate(count);
        }
    }
} 