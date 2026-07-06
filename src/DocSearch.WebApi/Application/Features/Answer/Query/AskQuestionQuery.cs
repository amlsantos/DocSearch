using MediatR;

namespace DocSearch.WebApi.Application.Features.Answer.Query;

public record AskQuestionQuery(string Question) : IRequest<AskQuestionResult>;
