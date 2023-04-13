using DigitalDiary;
using Microsoft.AspNetCore.Mvc;

namespace MyDigitalDiary.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiaryController : ControllerBase
{
    private readonly IDigitalDiaryRepository _repository;

    public DiaryController(IDigitalDiaryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public Task<IActionResult> GetRepositoryProjectName() => Task.FromResult((IActionResult)new OkObjectResult(_repository.RepositoryProjectName));

    [HttpPost]
    public Task<IActionResult> Post([FromBody] string message)
    {
        _repository.Write(message);
        return Task.FromResult((IActionResult)new OkResult());
    }
}