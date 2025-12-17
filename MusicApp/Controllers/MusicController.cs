using Microsoft.AspNetCore.Mvc;
using MusicApp.Models;

public class MusicController : Controller
{
    private readonly IWebHostEnvironment _env;

    // ❤️ In-memory favorites
    private static HashSet<string> _favorites = new();

    public MusicController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // 🎵 MUSIC LIST
    public IActionResult Index()
    {
        var musicPath = Path.Combine(_env.WebRootPath, "music");

        if (!Directory.Exists(musicPath))
        {
            return View(new List<Music>());
        }

        var songs = Directory.GetFiles(musicPath, "*.mp3")
            .Select(file =>
            {
                var fileName = Path.GetFileName(file);

                return new Music
                {
                    Title = Path.GetFileNameWithoutExtension(file),
                    FileName = fileName,
                    IsFavorite = _favorites.Contains(fileName)
                };
            })
            .ToList();

        return View(songs);
    }

    // ❤️ TOGGLE FAVORITE (MUST BE OUTSIDE Index)
    [HttpPost]
    public IActionResult ToggleFavorite(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();

        fileName = Path.GetFileName(fileName);

        if (_favorites.Contains(fileName))
            _favorites.Remove(fileName);
        else
            _favorites.Add(fileName);

        return Ok();
    }

    // ⬇ DOWNLOAD
    public IActionResult Download(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();

        fileName = Path.GetFileName(fileName);

        var filePath = Path.Combine(_env.WebRootPath, "music", fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        return PhysicalFile(
            filePath,
            "application/octet-stream",
            fileName
        );

    }
    public IActionResult LikedSongs()
    {
        var musicPath = Path.Combine(_env.WebRootPath, "music");

        if (!Directory.Exists(musicPath))
            return View(new List<Music>());

        var songs = Directory.GetFiles(musicPath, "*.mp3")
            .Select(file =>
            {
                var fileName = Path.GetFileName(file);

                return new Music
                {
                    Title = Path.GetFileNameWithoutExtension(file),
                    FileName = fileName,
                    IsFavorite = _favorites.Contains(fileName)
                };
            })
            .Where(song => song.IsFavorite) // ❤️ ONLY FAVORITES
            .ToList();

        return View(songs);
    }

}
