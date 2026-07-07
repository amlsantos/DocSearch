using MediatR;

namespace DocSearch.WebApi.Application.Features.Answer.Query;

public record AskQuestionQuery(string Question, int Limit = 5) : IRequest<AskQuestionResult>;
