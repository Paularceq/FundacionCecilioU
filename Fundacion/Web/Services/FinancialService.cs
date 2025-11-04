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

        public async Task<Result<FinancialDashboardDto>> GetDashboardAsync()
        {
            return await _apiClient.GetAsync<FinancialDashboardDto>("financial/dashboard");
        }

        public async Task<Result<List<FinancialMovementDto>>> GetMovementsByDateRangeAsync(DateTime from, DateTime to)
        {
            var url = $"financial/movements?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
            return await _apiClient.GetAsync<List<FinancialMovementDto>>(url);
        }

        public async Task<Result<List<ScholarshipWithPaymentStatusDto>>> GetScholarshipsWithPaymentStatusAsync()
        {
            return await _apiClient.GetAsync<List<ScholarshipWithPaymentStatusDto>>("financial/scholarships");
        }

        public async Task<Result<string>> ProcessPendingScholarshipsAsync(int userId)
        {
            var url = $"financial/scholarships/process-pending?userId={userId}";
            return await _apiClient.PostAsync<object, string>(url, null);
        }

        public async Task<Result> ProcessScholarshipPaymentAsync(int scholarshipId, int userId)
        {
            var url = $"financial/scholarships/process-payment?scholarshipId={scholarshipId}&userId={userId}";
            return await _apiClient.PostAsync(url);
        }

        public async Task<Result> SetScholarshipActiveStatusAsync(int scholarshipId, bool isActive)
        {
            var url = $"financial/scholarships/set-active-status?scholarshipId={scholarshipId}&isActive={isActive.ToString().ToLower()}";
            return await _apiClient.PostAsync(url);
        }

        public async Task<Result<List<BudgetDto>>> GetAllBudgetsAsync()
        {
            return await _apiClient.GetAsync<List<BudgetDto>>("financial/budgets");
        }

        public async Task<Result> AddBudgetAsync(AddBudgetViewModel viewModel)
        {
            var dto = new AddBudgetDto
            {
                Amount = viewModel.Amount,
                Currency = viewModel.Currency,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate
            };

            return await _apiClient.PostAsync("financial/budgets", dto);
        }
    }
}
