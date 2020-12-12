using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core
{
    public static class Commands
    {
        public static Dictionary<int, string> MessageBoxes = new Dictionary<int, string>() {
            { 0, "{\"Title\":\"Windows\",\"Description\":\"Güvenlik duvarı aşıldı\",\"BoxIcon\":1,\"BoxButtons\":1}" },
            { 1, "{\"Title\":\"Windows\",\"Description\":\"Windows dosyaları ciddi şekilde zarar gördü. Şimdi düzeltmek ister misiniz?\",\"BoxIcon\":3,\"BoxButtons\":2}" },
            { 2, "{\"Title\":\"Hacker\",\"Description\":\"Kaçmaya çalışma bilgisayarını ele geçirdim!\",\"BoxIcon\":2,\"BoxButtons\":3}" },
            { 3, "{\"Title\":\"Windows\",\"Description\":\"Güvenlik duvarı dış kaynaklar tarafından devre dışı bırakıldı.\",\"BoxIcon\":1,\"BoxButtons\":1}" }
        };

        public static Dictionary<int, string> Links = new Dictionary<int, string>() {
            { 0, "start chrome http://192.168.1.2" },
            { 1, "start chrome https://www.youtube.com/watch?v=j9V78UbdzWI" }
        };

        public static string MessageBox() => MessageBoxes[new Random().Next(0, MessageBoxes.Count)];
        public static string OpenLink() => Links[new Random().Next(0, Links.Count)];
    }
}
