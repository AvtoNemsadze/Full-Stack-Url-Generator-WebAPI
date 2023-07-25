using Backend.Context;
using Backend.Dtos;
using Backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VideoController(ApplicationDbContext context)
        {
            _context = context;
        }


        // CRUD
        [HttpPost]
        public async Task<ActionResult<VideoEntity>> CreateNewVideo([FromBody]CreateVideoDto dto)
        {
            var newVideo = new VideoEntity()
            {
                Title = dto.Title,
                Url = CreateUniqueUrl(),
            };

            await _context.Videos.AddAsync(newVideo);
            await _context.SaveChangesAsync();

            return Ok(newVideo);
        }

        [HttpGet]
        public async Task<ActionResult<List<VideoEntity>>> GetAllVideos()
        {
            var allVideos = await _context.Videos.OrderByDescending(q => q.CreatedAt).ToListAsync();
            return Ok(allVideos);
        }

        [HttpGet]
        [Route("{videoId}")]
        public async Task<ActionResult<VideoEntity>> GetVideoById([FromRoute]long videoId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(v => v.Id == videoId);

            if(video is null)
            {
                return NotFound("Video Not Found");
            }

            return Ok(video);
        }

        [HttpPatch]
        [Route("{videoId}")]
        public async Task<IActionResult> UpdateVideo([FromRoute] long videoId, CreateVideoDto dto)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(v => v.Id == videoId);

            if (video is null)
            {
                return NotFound("Video Not Found");
            }

            video.Title = dto.Title;
            await _context.SaveChangesAsync();
            return Ok("video updated successfully");
        }

        [HttpDelete]
        [Route("{videoId}")]
        public async Task<IActionResult> DeleteVideo([FromRoute,] long videoId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(v => v.Id == videoId);

            if (video is null)
            {
                return NotFound("Video Not Found");
            }

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return Ok("Video deleted successfully");
        }


        // create unique URL
        private string CreateUniqueUrl()
        {
            var newRandomUrl = string.Empty;
            Random rand = new Random();

            var boolFlag = true;

            while (boolFlag)
            {
                newRandomUrl = "";

                for (var i = 0; i < 8; i++)
                {
                    var randomNum = rand.Next(1,9);
                    var randomChar1 = (char)rand.Next('a', 'z');
                    var randomChar2 = (char)rand.Next('A', 'Z');

                    newRandomUrl += randomNum.ToString();
                    newRandomUrl += randomChar1.ToString();
                    newRandomUrl += randomChar2.ToString();
                }

                bool isDuplicate = _context.Videos.Any(q => q.Url == newRandomUrl);

                if(!isDuplicate)
                {
                    boolFlag = false;
                }
            }
            return newRandomUrl;
        }
    }
}
