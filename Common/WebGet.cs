namespace Common
{
    public static class Web
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> Get(string url)
        {
            var a = await client.GetAsync(url);
            return await a.Content.ReadAsStringAsync();
        }
    }
}
