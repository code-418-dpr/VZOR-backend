using WhoCame.Core.Abstractions;
using WhoCame.Core.Dtos;

namespace WhoCame.Visitors.Application.Features.Commands.AddPhotosToVisitor;

public record AddPhotosToVisitorCommand(Guid VisitorId, IEnumerable<CreateFileDto> FileDtos) : ICommand;
