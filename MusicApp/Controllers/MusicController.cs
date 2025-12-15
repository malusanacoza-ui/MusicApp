using Microsoft.AspNetCore.Mvc;
using MusicApp.Models;


public class MusicController : Controller
{
    private readonly IWebHostEnvironment _env;

    public MusicController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public IActionResult Index()
    {
        var musicPath = Path.Combine(_env.WebRootPath, "music");

        var songs = Directory.GetFiles(musicPath, "*.mp3")
            .Select(file => new Music
            {
                Title = Path.GetFileNameWithoutExtension(file),
                FileName = Path.GetFileName(file)
            }).ToList();

        return View(songs);
    }

    public IActionResult Download(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();

        var filePath = Path.Combine(_env.WebRootPath, "music", fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        return PhysicalFile(filePath, "audio/mpeg", fileName);
    }

}
