using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Media.Service
{
    public class LocationService
    {
        private readonly HttpClient _httpClient;
        private const string API_BASE = "https://provinces.open-api.vn/api";

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SelectListItem>> GetProvincesAsync()
        {
            var response = await _httpClient.GetStringAsync($"{API_BASE}/p/");
            var provinces = JsonConvert.DeserializeObject<List<dynamic>>(response);

            var list = new List<SelectListItem>();
            foreach (var p in provinces)
            {
                list.Add(new SelectListItem
                {
                    Value = p.code.ToString(),
                    Text = p.name.ToString()
                });
            }
            return list;
        }

        public async Task<List<SelectListItem>> GetDistrictsAsync(int provinceCode)
        {
            var response = await _httpClient.GetStringAsync($"{API_BASE}/p/{provinceCode}?depth=2");
            dynamic province = JsonConvert.DeserializeObject(response);

            var list = new List<SelectListItem>();
            foreach (var d in province.districts)
            {
                list.Add(new SelectListItem
                {
                    Value = d.code.ToString(),
                    Text = d.name.ToString()
                });
            }
            return list;
        }

        public async Task<List<SelectListItem>> GetWardsAsync(int districtCode)
        {
            var response = await _httpClient.GetStringAsync($"{API_BASE}/d/{districtCode}?depth=2");
            dynamic district = JsonConvert.DeserializeObject(response);

            var list = new List<SelectListItem>();
            foreach (var w in district.wards)
            {
                list.Add(new SelectListItem
                {
                    Value = w.code.ToString(),
                    Text = w.name.ToString()
                });
            }
            return list;
        }
    }
}
