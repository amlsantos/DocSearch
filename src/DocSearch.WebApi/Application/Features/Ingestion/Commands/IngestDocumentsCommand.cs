using System.Collections.Generic;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Ingestion.Commands;

public class IngestDocumentsCommand : IRequest<IList<Document>>
{
    public string DirectoryPath { get; }

    public IngestDocumentsCommand(string directoryPath)
    {
        DirectoryPath = directoryPath;
    }
}
