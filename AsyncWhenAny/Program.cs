using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncWhenAny {
    class Program {
        static async Task Main(string[] args) {
            // Needs to await or else Main finished and program ends
            await SumPageSizeAsync();
        }

        static async Task SumPageSizeAsync() {
            var stopwatch = Stopwatch.StartNew();

            IEnumerable<Task<int>> download_tasks_query =
                s_urlList.Select(url => ProcessUrlAsync(url, s_client));

            // LINQ is deffered execution, call ToList() so each task gets started
            // async calls started here
            List<Task<int>> download_tasks = download_tasks_query.ToList();

            int total = 0;
            // This is ok for a small number of tasks. There are more efficient approaches for a large # of tasks.
            while (download_tasks.Any()) {
                // await a call to WhenAny to find first task that has finished its download
                Task<int> finished_task = await Task.WhenAny(download_tasks);
                download_tasks.Remove(finished_task);
                // await to retrieve the length of the downloaded site
                total += await finished_task;
            }
            stopwatch.Stop();
            Console.WriteLine($"\nTotal bytes returned: {total:#,#}");
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}\n");
        }

        static async Task<int> ProcessUrlAsync(string url, HttpClient client) {
            // waiting for it
            byte[] content = await client.GetByteArrayAsync(url);
            Console.WriteLine($"{url, -60} {content.Length,10:#,#}");
            return content.Length;
        }

        static readonly HttpClient s_client = new HttpClient {
            MaxResponseContentBufferSize = 1_000_000
         };

        static readonly IEnumerable<string> s_urlList = new string[] {
                    "https://youtube.com",
                    "https://docs.microsoft.com/aspnet/core",
                    "https://docs.microsoft.com/azure",
                    "https://docs.microsoft.com/azure/devops",
                    "https://docs.microsoft.com/dotnet",
                    "https://docs.microsoft.com/dynamics365",
                    "https://docs.microsoft.com/education",
                    "https://docs.microsoft.com/enterprise-mobility-security",
                    "https://docs.microsoft.com/gaming",
                    "https://docs.microsoft.com/graph",
                    "https://docs.microsoft.com/microsoft-365",
                    "https://docs.microsoft.com/office",
                    "https://docs.microsoft.com/powershell",
                    "https://docs.microsoft.com/sql",
                    "https://docs.microsoft.com/surface",
                    "https://docs.microsoft.com/system-center",
                    "https://docs.microsoft.com/visualstudio",
                    "https://docs.microsoft.com/windows",
                    "https://docs.microsoft.com/xamarin"
        };
    }
}
