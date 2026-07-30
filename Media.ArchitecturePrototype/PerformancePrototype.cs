using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.ArchitecturePrototype
{
    public class PerformancePrototype
    {
        private static readonly string BaseUrl = "https://localhost:7014";
        private static readonly HttpClient client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        })
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private static readonly string[] TestPaths =
        {
            "sach-trong-nuoc",
            "foreign-books",
            "foreign-books/fiction",
            "sach-trong-nuoc/tam-ly-ky-nang-song"
        };

        private static readonly string[] SortOptions = { "price-asc", "price-desc", "newest", "name-asc" };
        private static readonly string[] PriceRanges = { "range1", "range2", "range3", "range4", "range5" };
        
        public async Task CheckingPerformance()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            int totalRequests = 50;
            int maxDegreeOfParallelism = 5;
            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
            var tasks = new List<Task>();

            Console.WriteLine($"Target: {BaseUrl}");
            Console.WriteLine($"Bắt đầu bắn {totalRequests} requests, xử lý song song {maxDegreeOfParallelism} luồng...");
            Console.WriteLine("------------------------------------------------------------------");

            for (int i = 1; i <= totalRequests; i++)
            {
                int requestId = i;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        await ExecuteRequest(requestId);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Hoàn tất quá trình Prototype. Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
        static async Task ExecuteRequest(int requestId)
        {
            string path = TestPaths[Random.Shared.Next(TestPaths.Length)];
            string sort = SortOptions[Random.Shared.Next(SortOptions.Length)];
            string price = PriceRanges[Random.Shared.Next(PriceRanges.Length)];
            int page = Random.Shared.Next(1, 3);
            string requestUrl = $"{BaseUrl}/chude/{path}?sortBy={sort}&priceRanges={price}&page={page}&pageSize=48";

            Stopwatch sw = Stopwatch.StartNew();
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            try
            {
                var response = await client.GetAsync(requestUrl);
                sw.Stop();

                string status = $"{(int)response.StatusCode} {response.ReasonPhrase}";
                long elapsedMs = sw.ElapsedMilliseconds;

                Console.WriteLine($"[{timestamp}] [REQ #{requestId:D2}] | Path: {path,-20} | Status: {status,-10} | Time: {elapsedMs}ms");
            }
            catch (Exception ex)
            {
                sw.Stop();
                Console.WriteLine($"[{timestamp}] [REQ #{requestId:D2}] | Path: {path,-20} | LỖI: {ex.Message}");
            }
        }
    }
}
