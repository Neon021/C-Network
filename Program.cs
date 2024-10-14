using System.Net.NetworkInformation;

namespace Program
{
    class Program
    {
        public static void Main(string[] args)
        {
            string[] resources = Downloader.Downloader.ResourcesToDownload();
            Task download = Downloader.Downloader.Download(resources);
            Thread.Sleep(1000);
            download.GetAwaiter().GetResult();
            Console.WriteLine("Successfully downloaded resources");
        }
    }
}

namespace Downloader
{
    class Downloader
    {
        public static string[] ResourcesToDownload()
        {
            Console.WriteLine("Please enter the resources' name you'd like to download\r\nFor example: google.com, 16bpp.net");
            string[] resources = new string[10];
            while (true)
            {
                string input = Console.ReadLine();
                char[] charArray = input.ToCharArray();
                int i = 0;
                while (i < charArray.Length)
                {
                    if (charArray[i] == '.')
                    {
                        AddInput(input);
                        goto DotCheck;
                    }
                    i++;
                }
                Console.WriteLine("Input format was wrong.");
                return resources;

            DotCheck:
                Console.WriteLine("Do you want to finish?\r\nY/N");
                string answer = Console.ReadLine();
                if (string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
                    return resources;
            }

            void AddInput(string input)
            {
                int totalElements = resources.Count(x => x != null);
                int capacity = resources.Length;
                if (totalElements + 1 >= capacity)
                {
                    string[] newArray = new string[capacity * 2];
                    for (int i = 0; i < totalElements; i++)
                    {
                        newArray[i] = resources[i];
                    }
                    resources = newArray;
                }
                resources[totalElements] = input;
            }
        }
        public static async Task Download(string[] resources)
        {
            //TODO: Cut null parts of the array
            Queue<string> strings = new(resources);

            using (HttpClient httpClient = new())
            {
                while (strings.Count > 0)
                {
                    strings.TryDequeue(out string? currResource);
                    if (currResource != null)
                    {
                        HttpResponseMessage response = await httpClient.GetAsync($"https://{currResource}");

                        if (response.IsSuccessStatusCode)
                        {
                            byte[] data = await response.Content.ReadAsByteArrayAsync();

                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            string filename = currResource[..currResource.IndexOf('.')];
                            FileStream stream = File.Create($"{desktopPath}\\{filename}");
                            await stream.WriteAsync(data, 0, data.Length);
                            stream.Close();
                        }
                        else
                        {
                            Console.WriteLine($"Unable to download this resource: {currResource}");
                        }
                    }
                }
            }
        }
    }
}