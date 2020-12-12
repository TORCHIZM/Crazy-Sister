using Client.Core;
using Client.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        public static int Heartrate = 0;
        public static int LastHeartrate = 0;
        public static Socket _Sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static IPAddress _Target;
        public static string _IP;
        public static IPEndPoint _EndPoint;
        public static byte[] _bytes = new byte[1048576];
        public static bool InRandomizer = false;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Write("Sunucu IP Adresi: ");
                StartClient(IPAddress.Parse(Console.ReadLine()));
            }
            else
            {
                StartClient(IPAddress.Parse(args[0]));
            }
        }
        public static void StartClient(IPAddress ipAddress)
        {
            _Target = ipAddress;
            byte[] bytes = new byte[1048576];

            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 16860);
                _EndPoint = remoteEP;

                while (true)
                {
                    try
                    {
                        Socket sender = new Socket(ipAddress.AddressFamily,
                            SocketType.Stream, ProtocolType.Tcp);

                        _Sender = sender;
                        sender.Connect(remoteEP);
                        _IP = sender.RemoteEndPoint.ToString();
                        Console.WriteLine($"{sender.RemoteEndPoint} sunucusu ile bağlantı sağlandı");

                        Console.WriteLine("Yaptırmak istediğiniz komutu girin: ");

                        var input = "t";
                        if (!InRandomizer) input = Console.ReadLine();

                        if (input.Split().Length == 1) input += " t";
                        var message = ProcessRequest(input, sender);
                        byte[] msg = Encoding.UTF8.GetBytes($"{message}<EOF>");

                        int bytesSent = sender.Send(msg);

                        int bytesRec = sender.Receive(bytes);
                        Console.WriteLine($"Yanıt: {Encoding.UTF8.GetString(bytes, 0, bytesRec)}");

                        if (InRandomizer)
                        {
                            string Resp = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                            if (Resp.StartsWith("https://launchwares.com/storage/publicImages/"))
                            {
                                var message2 = ProcessRequest($"CMD start chrome {Resp}", sender);
                                byte[] msg2 = Encoding.UTF8.GetBytes($"{message2}<EOF>");
                                int bytesSent2 = sender.Send(msg2);
                            }
                        }

                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine($"ArgumentNullException: {ane.ToString()}");
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine($"SoketHatası: {se}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Beklenmeyen Hata: {e}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void SendRequest(string Type, string Request)
        {
            byte[] bytes = new byte[1048576];
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(_Target, 16860);

                Socket sender = new Socket(_Target.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                sender.Connect(remoteEP);

                Console.WriteLine($"{sender.RemoteEndPoint} sunucusu ile bağlantı sağlandı");

                var message = "";
                byte[] msg;
                if (Type != "-Messagebox")
                {
                    message = ProcessRequest($"{Type} {Request}", sender);
                    msg = Encoding.UTF8.GetBytes($"{message}<EOF>");
                }
                else
                {
                    Data data = new Data() {
                        Type = "MessageBox",
                        Contents = Request
                    };

                    message = JsonConvert.SerializeObject(data);
                    msg = Encoding.UTF8.GetBytes($"{message}<EOF>");
                }

                int bytesSent = sender.Send(msg);
                int bytesRec = sender.Receive(_bytes);
                string Resp = Encoding.UTF8.GetString(_bytes, 0, bytesRec);
                Console.WriteLine($"Yanıt: {Encoding.UTF8.GetString(_bytes, 0, bytesRec)}");

                if (InRandomizer)
                {
                    if (Resp.StartsWith("https://launchwares.com/storage/publicImages/"))
                    {
                        var message2 = ProcessRequest($"CMD start chrome {Resp}", sender);
                        byte[] msg2 = Encoding.UTF8.GetBytes($"{message2}<EOF>");
                        int bytesSent2 = sender.Send(msg2);
                    }
                }

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine($"ArgumentNullException: {ane.ToString()}");
            }
            catch (SocketException se)
            {
                Console.WriteLine($"SoketHatası: {se}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Beklenmeyen Hata: {e}");
            }
        }

        public static string ProcessRequest(string Request, Socket sender)
        {
            Data data = new Data();

            data.Type = Request.Split(new char[0])[0];
            data.Contents = Request;

            for (int i = 0; i < data.Type.Length + 1; i++)
                data.Contents = data.Contents.Substring(1, data.Contents.Length - 1);

            if (data.Type == "MessageBox")
            {
                MessageBoxModel MessageBoxObj = new MessageBoxModel();
                Console.Write("Başlık Girin: ");
                MessageBoxObj.Title = Console.ReadLine();
                Console.Write("Açıklama Girin: ");
                MessageBoxObj.Description = Console.ReadLine();
                Console.WriteLine("\n" +
                    "1 - Tamam\n" +
                    "2 - Evet/Hayır\n" +
                    "3 - Tamam/İptal\n" +
                    "4 - İptal/Yeniden Dene/Yoksay");
                Console.Write("Buton Tipi: ");
                MessageBoxObj.BoxButtons = int.Parse(Console.ReadLine());
                Console.WriteLine("\n" +
                    "1 - Hata\n" +
                    "2 - Bilgi\n" +
                    "3 - Soru İşareti\n" +
                    "4 - Uyarı");
                Console.Write("Buton Tipi: ");
                MessageBoxObj.BoxIcon = int.Parse(Console.ReadLine());

                data.Contents = JsonConvert.SerializeObject(MessageBoxObj);
            }
            if (data.Type == "-MessageBox")
            {
                data.Type = "MessageBox";
            }
            else if (data.Type == "Heartrate")
            {
                InRandomizer = true;

                //sender.Shutdown(SocketShutdown.Both);
                //sender.Close();

                SubprocessController sb = new SubprocessController(@"C:\Users\Torchizm\Desktop\MiBand-HeartrateOutput.exe");
                sb.OutputDataReceived += Sb_OutputDataReceived;
            }

            return JsonConvert.SerializeObject(data);
        }

        private static void Sb_OutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            try
            {
                Console.WriteLine($"InComing heartrate: {args.Data}");
                LastHeartrate = Heartrate;
                Heartrate = int.Parse(args.Data);

                if (Heartrate != 0 && Heartrate != LastHeartrate)
                {
                    Randomizer(Heartrate);
                }
            }
            catch (Exception)
            {
                LastHeartrate = Heartrate;
                Heartrate = 0;
            }
        }

        public static void Randomizer(int Heartrate)
        {
            if(Heartrate < 50)
            {
                SendRequest("CMD", "start chrome");
            }
            if (Heartrate > 55 && Heartrate < 60)
            {
                SendRequest("Screenshot", "t");
            }
            else if (Heartrate > 60 && Heartrate < 70)
            {
                SendRequest("-MessageBox", Commands.MessageBox());
            }
            else if (Heartrate > 70)
            {
                SendRequest("CMD", Commands.OpenLink());
            }
        }
    }
}
