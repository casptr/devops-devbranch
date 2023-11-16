using System.Collections.Generic;
using System.Net.Http.Json;
using Client.Extensions;
using Foodtruck.Shared.Supplements;

namespace Foodtruck.Client.Supplements;

public class SupplementService : ISupplementService
{
    private readonly HttpClient client;
    private const string endpoint = "api/supplement";
    public SupplementService(HttpClient client)
    {
        this.client = client;
    }
    public async Task<SupplementResult.Index> GetAllAsync()
    {
        var response = await client.GetFromJsonAsync<SupplementResult.Index>($"{endpoint}/all");
        return response!;
    }


    public async Task<SupplementResult.Index> GetIndexAsync(SupplementRequest.Index request)
    {
        var response = await client.GetFromJsonAsync<SupplementResult.Index>($"{endpoint}?{request.AsQueryString()}");
        return response!;
    }


    public async Task<SupplementDto.Detail> GetDetailAsync(int supplementId)
    {
        var response = await client.GetFromJsonAsync<SupplementDto.Detail>($"{endpoint}/{supplementId}");
        return response!;
    }

    public async Task<SupplementResult.Mutate> CreateAsync(SupplementDto.Mutate request)
    {   
        var response = await client.PostAsJsonAsync(endpoint, request);
        return await response.Content.ReadFromJsonAsync<SupplementResult.Mutate>() ?? default!;
    }

    public async Task<SupplementResult.Mutate> EditAsync(int supplementId, SupplementDto.Mutate model)
    {
        var response =  await client.PutAsJsonAsync($"{endpoint}/{supplementId}", model);
        return await response.Content.ReadFromJsonAsync<SupplementResult.Mutate>() ?? default!;
    }

    public async Task DeleteAsync(int supplementId)
    {
        await client.DeleteAsync($"{endpoint}/{supplementId}");
    }

    //public async Task AddImage(int supplementId, SupplementDto.Mutate model)
    //{
    //    await client.PostAsJsonAsync($"{endpoint}/{supplementId}/addimage", model);
    //}

}
