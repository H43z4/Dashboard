﻿namespace Reports.Services
{
    public class EPayHttpClient
    {
        static readonly HttpClient httpClient;
        
        static EPayHttpClient()
        {
            httpClient = new();
        }

        public static async Task<bool> GeneratePSIDRequest(long epayTaskId)
        {
            try
            {
                var httpResponseMessage = await httpClient.GetAsync($"http://10.50.126.65:6060/api/payments/EnqueueEpayTask?epayTaskId={epayTaskId}");
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
