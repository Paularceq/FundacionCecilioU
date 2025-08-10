using Shared.Dtos.Financial;
using Shared.Models;
using Web.Http;
using Web.Models.Financial;

namespace Web.Services
{
    public class FinancialService
    {
        private readonly ApiClient _apiClient;

        public FinancialService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result> AddLeaseIncomeAsync(int userId, LeaseIncomeViewModel viewModel)
        {
            var leaseIncomeDto = new LeaseIncomeDto
            {
                TenantName = viewModel.TenantName,
                TenantIdentification = viewModel.TenantIdentification,
                Usage = viewModel.Usage,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Amount = viewModel.Amount,
                Currency = viewModel.Currency,
                CreatedById = userId
            };

            if (viewModel.ReceiptFile != null && viewModel.ReceiptFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await viewModel.ReceiptFile.CopyToAsync(memoryStream);

                leaseIncomeDto.ReceiptBytes = Convert.ToBase64String(memoryStream.ToArray());
                leaseIncomeDto.ReceiptContentType = viewModel.ReceiptFile.ContentType;
                leaseIncomeDto.ReceiptFileName = viewModel.ReceiptFile.FileName;
            }

            return await _apiClient.PostAsync("financial/lease-income", leaseIncomeDto);
        }

        public async Task<Result> AddExpenseAsync(int userId, AdministrativeExpenseViewModel viewModel)
        {
            var expenseDto = new AdministrativeExpenseDto
            {
                Description = viewModel.Description,
                Amount = viewModel.Amount,
                Currency = viewModel.Currency,
                Date = viewModel.Date,
                CreatedById = userId
            };
            if (viewModel.ReceiptFile != null && viewModel.ReceiptFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await viewModel.ReceiptFile.CopyToAsync(memoryStream);
                expenseDto.ReceiptBytes = Convert.ToBase64String(memoryStream.ToArray());
                expenseDto.ReceiptContentType = viewModel.ReceiptFile.ContentType;
                expenseDto.ReceiptFileName = viewModel.ReceiptFile.FileName;
            }
            return await _apiClient.PostAsync("financial/expense", expenseDto);
        }
    }
}
