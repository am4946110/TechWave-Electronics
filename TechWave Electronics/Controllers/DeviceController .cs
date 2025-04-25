using TechWave_Electronics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;


namespace TechWave_Electronics.Controllers
{

    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SaveDeviceInfo([FromBody] Device deviceInfo)
        {
            if (deviceInfo == null)
            {
                return BadRequest("The device data is invalid.");
            }

            var device = new Device
            {
                UserAgent = deviceInfo.UserAgent,
                Platform = deviceInfo.Platform,
                Language = deviceInfo.Language,
                ScreenWidth = deviceInfo.ScreenWidth,
                ScreenHeight = deviceInfo.ScreenHeight,
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                Port = HttpContext.Connection.LocalPort.ToString(),

                Timestamp = DateTime.Now.AddHours(12)
            };

            string formattedTime = device.Timestamp.ToString("dd/MMM/yyyy   hh:mm   t"); ;


            _context.Devices.Add(device);
            _context.SaveChanges();

            return Ok("Device data saved successfully");
        }

        [Authorize(Roles = "Administrators,It Management")]
        public IActionResult index()
        {
            var device = _context.Devices.ToList();
            return View(device);
        }

        [Authorize(Roles = "Administrators,It Management")]
        public IActionResult ActivityLog()
        {
            var device = _context.ActivityLogs.ToList();
            return View(device);
        }


        public async Task<IActionResult> DeviceInfo()
        {
            var devices = await GetConnectedDevicesAsync();
            return View(devices);
        }

        private string GetLocalIPAddress()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint?.Address.ToString() ?? throw new Exception("لا يمكن تحديد عنوان IP المحلي.");
            }
        }

        private async Task<List<DeviceInfo>> GetConnectedDevicesAsync()
        {
            var deviceList = new List<DeviceInfo>();
            string localIP = GetLocalIPAddress();
            string[] ipParts = localIP.Split('.');
            string baseIP = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.";

            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(20); // تحديد عدد المهام المتزامنة

            for (int i = 1; i <= 254; i++)
            {
                string ip = baseIP + i.ToString();
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var reply = await ping.SendPingAsync(ip, 100);
                            if (reply.Status == IPStatus.Success)
                            {
                                string hostName = ip;
                                try
                                {
                                    var entry = await Dns.GetHostEntryAsync(ip);
                                    hostName = entry.HostName;
                                }
                                catch
                                {
                                    // تجاهل الأخطاء في الحصول على اسم الجهاز
                                }

                                lock (deviceList)
                                {
                                    deviceList.Add(new DeviceInfo
                                    {
                                        IPAddress = ip,
                                        HostName = hostName,
                                        Status = "Online"
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        // تجاهل الأخطاء في عملية الـ Ping
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return deviceList;
        }
    }


}

