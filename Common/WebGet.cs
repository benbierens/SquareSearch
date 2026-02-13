namespace Common
{
    public static class Web
    {
        private static readonly HttpClient client = new HttpClient();

        public static string Get(string url)
        {
            var task1 = client.GetAsync(url);
            task1.Wait();
            var a = task1.Result;
            var task2 = a.Content.ReadAsStringAsync();
            task2.Wait();
            return task2.Result;
        }
    }
}
