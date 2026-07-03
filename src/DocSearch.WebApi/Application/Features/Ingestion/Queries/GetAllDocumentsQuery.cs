using DocSearch.WebApi.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace DocSearch.WebApi.Application.Features.Ingestion.Queries;

public class GetAllDocumentsQuery : IRequest<IList<Document>>
{
}
