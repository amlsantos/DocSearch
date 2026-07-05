namespace DocSearch.WebApi.Application.Common.Interfaces;

public interface IQuestionAnswerer
{
    Task<string> AnswerAsync(string question, IEnumerable<string> contextChunks, CancellationToken cancellationToken = default);
}