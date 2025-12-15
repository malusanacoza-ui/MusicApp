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

        // Ensure folder exists
        if (!Directory.Exists(musicPath))
        {
            return View(new List<Music>());
        }

        var songs = Directory.GetFiles(musicPath, "*.mp3")
            .Select(file => new Music
            {
                Title = Path.GetFileNameWithoutExtension(file),
                FileName = Path.GetFileName(file)
            })
            .ToList();

        return View(songs);
    }

    // ✅ FORCE DOWNLOAD ACTION
    public IActionResult Download(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();

        // 🔒 Security: prevent path traversal
        fileName = Path.GetFileName(fileName);

        var filePath = Path.Combine(_env.WebRootPath, "music", fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        // ✅ application/octet-stream forces browser download
        return PhysicalFile(
            filePath,
            "application/octet-stream",
            fileName
        );
    }
}
