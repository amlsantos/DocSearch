using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public record AskQuestionQuery(string Question) : IRequest<AskQuestionResult>;
