

using System.Drawing.Imaging;
using CapturePicture;

public class HostedCameraService : IHostedService, IDisposable
{
    private readonly ILogger<HostedCameraService> _logger;
    private Timer? _timer = null;

    private UsbCamera _camera = null;
    private PictureHolder _holder = null;

    public HostedCameraService(
        ILogger<HostedCameraService> logger,
        PictureHolder holder)
    {
        _logger = logger;
        _holder = holder;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        //获取所有的摄像头
        string[] devices = UsbCamera.FindDevices();

        //获取摄像头支持的分辨率
        int cameraIndex = 0;
        UsbCamera.VideoFormat[] formats = UsbCamera.GetVideoFormat(cameraIndex);
        for (int i = 0; i < formats.Length; i++)
            Console.WriteLine("{0}:{1}", i, formats[i]);

        // create usb camera and start.
        _camera = new UsbCamera(cameraIndex, formats[0]);
        _camera.Start();

        //第一次截图不延迟的话，会出现黑屏
        Thread.Sleep(1000);

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        if( _camera != null) {
            _logger.LogInformation("taking picture......");
            var bmp = _camera.GetBitmap();
            using( MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Jpeg);
                stream.Seek(0, SeekOrigin.Begin);

                _holder.Picture = stream.ToArray();
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _camera.Dispose();
    }
}