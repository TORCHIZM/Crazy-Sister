using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.API
{
    public class ImageService
    {

        public string ApiPath = "https://launchwares.com/api";

        public async Task<string> UploadImage(byte[] bytes)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("POST"), $"{ApiPath}/misc/upload");

            var multipartContent = new MultipartFormDataContent();

            multipartContent.Add(new ByteArrayContent(bytes), "image", "xfile.png");
            request.Content = multipartContent;

            var response = await httpClient.SendAsync(request);
            var message = response.Content.ReadAsStringAsync().Result;

            var Image = JsonConvert.DeserializeObject<Models.ImageModel>(message);
            return $"{ApiPath.Substring(0, ApiPath.Length - 3)}storage/{Image.Path}";
        }
    }
}
