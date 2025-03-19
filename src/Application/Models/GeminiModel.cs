namespace Application.Models;
public record GeminiModel(IEnumerable<Content> Contents);
public record GeminiResponse(IEnumerable<Candidate> Candidates);
public record Candidate(Content Content);
public record Content(IEnumerable<Part> Parts);
public record Part(string Text);