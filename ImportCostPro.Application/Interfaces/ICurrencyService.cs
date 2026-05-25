using ImportCostPro.Application.DTOs.Currency.Requests;
using ImportCostPro.Application.DTOs.Currency.Responses;

namespace ImportCostPro.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<CurrencyResponse>> GetAllAsync();
        Task<CurrencyResponse?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateCurrencyRequest request);
        Task UpdateAsync(UpdateCurrencyRequest request);
        Task ToggleStatusAsync(int id);
        Task DeleteAsync(int id);
    }
}