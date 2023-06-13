using System.Drawing.Imaging;
using CapturePicture;
using Microsoft.AspNetCore.Mvc;

namespace CapturePictureAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly PictureHolder _holder;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        PictureHolder holder 
        )
    {
        _logger = logger;
        _holder = holder;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet( Name ="capture")]
    [Route("capture")]
    public async Task<ActionResult> CapturePicture()
    {
        byte[] picture = _holder.Picture;
        if( picture != null)
        {
            return new FileContentResult(picture, "image/jpeg");
        }

        return new EmptyResult();

        /*
        //获取所有的摄像头
        string[] devices = UsbCamera.FindDevices();

        //获取摄像头支持的分辨率
        int cameraIndex = 0;
        UsbCamera.VideoFormat[] formats = UsbCamera.GetVideoFormat(cameraIndex);
        for (int i = 0; i < formats.Length; i++)
            Console.WriteLine("{0}:{1}", i, formats[i]);

        // create usb camera and start.
        using var camera = new UsbCamera(cameraIndex, formats[0]);
        camera.Start();

        //第一次截图不延迟的话，会出现黑屏
        await Task.Delay(100);

        var bmp = camera.GetBitmap();
        MemoryStream stream = new MemoryStream();
        bmp.Save(stream, ImageFormat.Jpeg);
        stream.Seek(0, SeekOrigin.Begin);
        */

        
    }
}
