using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public record GetAllDocumentsQuery : IRequest<IList<Document>>;
