using VZOR.Images.Domain;

namespace VZOR.Images.Application.Database;

public interface IReadDbContext
{
    IQueryable<Image> Visitors { get; }
}