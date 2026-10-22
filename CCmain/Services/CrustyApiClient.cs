using System.Net.Http.Json;
using CrustyCrab.Models;
using Microsoft.AspNetCore.Http;



namespace CrustyCrab.Services
{
    public class CrustyApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<CrustyApiClient> _logger;

        public CrustyApiClient(HttpClient http, ILogger<CrustyApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public string ResolveApiUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
                return absolute.ToString();

            return new Uri(_http.BaseAddress!, url.TrimStart('/')).ToString();
        }


        public Task<List<ApiMenuItem>?> GetMenu()
            => Get<List<ApiMenuItem>>("api/menu");

        public Task<List<ApiMenuItem>?> GetPopularMenuItems()
            => Get<List<ApiMenuItem>>("api/menu/popular");

        public Task<ApiMenuItem?> GetMenuItem(int id)
            => Get<ApiMenuItem>($"api/menu/{id}");

        public Task<ApiMenuItem?> CreateMenuItem(ApiMenuItem item)
            => Post<ApiMenuItem>("api/menu", item);

        public Task<ApiMenuItem?> UpdateMenuItem(ApiMenuItem item)
            => Put<ApiMenuItem>($"api/menu/{item.MenuItemID}", item);

        public async Task<ApiMenuItem?> UploadMenuImage(
    int menuItemId,
    IFormFile image)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                await using var stream = image.OpenReadStream();

                using var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(
                        image.ContentType);

                content.Add(
                    fileContent,
                    "image",
                    image.FileName);

                var response = await _http.PostAsync(
                    $"api/menu/{menuItemId}/image",
                    content);

                return await ReadResponse<ApiMenuItem>(
                    response,
                    $"api/menu/{menuItemId}/image",
                    "POST");
            }
            catch (Exception ex) when (
                ex is HttpRequestException or
                TaskCanceledException or
                System.Text.Json.JsonException)
            {
                _logger.LogWarning(
                    ex,
                    "POST api/menu/{Id}/image failed",
                    menuItemId);

                return null;
            }
        }
        public async Task<(bool Success, string? ErrorMessage)> DeleteMenuItem(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/menu/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

                return (false, error?.Message ?? "The menu item could not be deleted.");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "DELETE api/menu/{Id} failed", id);

                return (false, "The API could not be reached.");
            }
        }
        public class ApiErrorResponse
        {
            public string Message { get; set; } = string.Empty;
        }
        public async Task<List<ApiOrder>?> GetOrders() => await Get<List<ApiOrder>>("api/order");

        public Task<ApiOrder?> GetOrder(int id) => Get<ApiOrder>($"api/order/{id}");

        public Task<List<ApiOrder>?> GetCustomerOrders(int customerId) => Get<List<ApiOrder>>($"api/order/customer/{customerId}");

        public Task<ApiOrder?> CreateOrder(int customerId, string deliveryMethod, int menuItemId, int quantity, string? notes, int? deliveryAddressId = null)
        {
            var body = new
            {
                CustomerID = customerId,
                DeliveryAddressID = deliveryAddressId,
                DeliveryMethod = deliveryMethod,
                Items = new[] { new { MenuItemID = menuItemId, Quantity = quantity, Notes = notes } }
            };
            return Post<ApiOrder>("api/order", body);
        }

        public Task<ApiOrder?> CreateOrder(int customerId, int? deliveryAddressId, string deliveryMethod, IEnumerable<ApiOrderItemRequest> items)
        {
            var body = new
            {
                CustomerID = customerId,
                DeliveryAddressID = deliveryAddressId,
                DeliveryMethod = deliveryMethod,
                Items = items.Select(i => new { MenuItemID = i.MenuItemID, Quantity = i.Quantity, Notes = i.Notes }).ToList()
            };
            return Post<ApiOrder>("api/order", body);
        }

        public Task<ApiOrder?> CancelOrder(int orderId, int customerId) => Put<ApiOrder>($"api/order/{orderId}/cancel?customerId={customerId}", new { });

        public Task<ApiOrder?> UpdateOrderStatus(int orderId, string status) => Put<ApiOrder>($"api/order/{orderId}/status", new { OrderStatus = status });

        public Task<List<ApiCustomer>?> GetCustomers() => Get<List<ApiCustomer>>("api/customer");

        public Task<ApiCustomer?> GetCustomer(int id) => Get<ApiCustomer>($"api/customer/{id}");

        public Task<ApiCustomer?> CreateCustomer(string firstName, string lastName, string phone, string email, string password)
        {
            var body = new { FirstName = firstName, LastName = lastName, PhoneNumber = phone, EmailAddress = email, Password = password };
            return Post<ApiCustomer>("api/customer", body);
        }

        public Task<ApiCustomer?> UpdateCustomer(int id, string firstName, string lastName, string phone, string email)
        {
            var body = new { FirstName = firstName, LastName = lastName, PhoneNumber = phone, EmailAddress = email };
            return Put<ApiCustomer>($"api/customer/{id}", body);
        }

        public Task<bool> DeleteCustomer(int id) => Delete($"api/customer/{id}");

        public Task<ApiLoginResponse?> CustomerLogin(string identifier, string password) => Login("api/auth/customer/login", identifier, password);

        public Task<ApiLoginResponse?> AdminLogin(string identifier, string password) => Login("api/auth/admin/login", identifier, password);

        public Task<ApiCart?> GetCustomerCart(int customerId) => Get<ApiCart>($"api/cart/customer/{customerId}");

        public Task<ApiCart?> GetCart(int id) => Get<ApiCart>($"api/cart/{id}");

        public Task<ApiCart?> CreateCart(int customerId) => Post<ApiCart>("api/cart", new { CustomerID = customerId });

        public Task<ApiCart?> AddCartItem(int cartId, int menuItemId, int quantity) => Post<ApiCart>($"api/cart/{cartId}/items", new { MenuItemID = menuItemId, Quantity = quantity });

        public Task<ApiCart?> UpdateCartItem(int cartItemId, int quantity) => Put<ApiCart>($"api/cart/items/{cartItemId}", new { Quantity = quantity });

        public Task<ApiCart?> RemoveCartItem(int cartItemId) => Delete<ApiCart>($"api/cart/items/{cartItemId}");

        public Task<List<ApiDeliveryAddress>?> GetAddresses() => Get<List<ApiDeliveryAddress>>("api/deliveryaddress");

        public async Task<List<AddressSuggestion>?> SearchAddressSuggestions(string query)
        {
            try
            {
                var url = $"api/AddressAutocomplete?q={Uri.EscapeDataString(query)}";
                var response = await _http.GetAsync(url);

                return await ReadResponse<List<AddressSuggestion>>(
                    response,
                    url,
                    "GET");
            }
            catch (Exception ex) when (
                ex is HttpRequestException or
                TaskCanceledException or
                System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "Address autocomplete search failed");
                return null;
            }
        }


        public Task<ApiDeliveryAddress?> GetAddress(int id) => Get<ApiDeliveryAddress>($"api/deliveryaddress/{id}");

        public Task<List<ApiDeliveryAddress>?> GetCustomerAddresses(int customerId) => Get<List<ApiDeliveryAddress>>($"api/deliveryaddress/customer/{customerId}");

        public Task<ApiDeliveryAddress?> CreateAddress(int customerId, string suburb, string houseNumber, string streetName, string? buildingType)
        {
            var body = new { Suburb = suburb, HouseNumber = houseNumber, StreetName = streetName, BuildingType = buildingType, CustomerID = customerId };
            return Post<ApiDeliveryAddress>("api/deliveryaddress", body);
        }

        public Task<ApiDeliveryAddress?> UpdateAddress(int id, string suburb, string houseNumber, string streetName, string? buildingType)
        {
            var body = new { Suburb = suburb, HouseNumber = houseNumber, StreetName = streetName, BuildingType = buildingType };
            return Put<ApiDeliveryAddress>($"api/deliveryaddress/{id}", body);
        }


        public async Task<(bool Success, string? ErrorMessage)> DeleteAddress(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/deliveryaddress/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

                return (false, error?.Message ??
                    "The delivery address could not be deleted.");
            }
            catch (Exception ex) when (
                ex is HttpRequestException or
                TaskCanceledException or
                System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "DELETE api/deliveryaddress/{Id} failed", id);

                return (false, "The API could not be reached. Please try again.");
            }
        }


        public Task<List<ApiPaymentMethod>?> GetPaymentMethods() => Get<List<ApiPaymentMethod>>("api/payment/methods");

        public Task<ApiPaymentMethod?> GetPaymentMethod(int id) => Get<ApiPaymentMethod>($"api/payment/methods/{id}");

        public Task<ApiPaymentMethod?> CreatePaymentMethod(string name) => Post<ApiPaymentMethod>("api/payment/methods", new { MethodName = name });

        public Task<bool> DeletePaymentMethod(int id) => Delete($"api/payment/methods/{id}");

        public Task<List<ApiCustomerPaymentMethod>?> GetCustomerPaymentMethods(int customerId) => Get<List<ApiCustomerPaymentMethod>>($"api/payment/customer/{customerId}");

        public Task<ApiCustomerPaymentMethod?> SaveCustomerPaymentMethod(int customerId, int paymentMethodId, string cardholderName, string cardNumber, int expiryMonth, int expiryYear)
        {
            var body = new
            {
                CustomerID = customerId,
                PaymentMethodID = paymentMethodId,
                CardholderName = cardholderName,
                CardNumber = cardNumber,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear
            };
            return Post<ApiCustomerPaymentMethod>("api/payment/customer", body);
        }

        public Task<bool> DeleteCustomerPaymentMethod(int id, int customerId) => Delete($"api/payment/customer/{id}?customerId={customerId}");

        public Task<ApiPayment?> CreatePayment(int orderId, int paymentMethodId, decimal amount) => Post<ApiPayment>("api/payment", new { OrderID = orderId, PaymentMethodID = paymentMethodId, Amount = amount });

        public Task<ApiPayment?> GetPayment(int id) => Get<ApiPayment>($"api/payment/{id}");

        public Task<List<ApiPayment>?> GetOrderPayments(int orderId) => Get<List<ApiPayment>>($"api/payment/order/{orderId}");

        public Task<ApiPayment?> UpdatePaymentStatus(int paymentId, string status) => Put<ApiPayment>($"api/payment/{paymentId}/status", new { PaymentStatus = status });

        public Task<List<ApiStaff>?> GetStaff() => Get<List<ApiStaff>>("api/adminstaff/staff");

        public Task<ApiStaff?> GetStaff(int id) => Get<ApiStaff>($"api/adminstaff/staff/{id}");

        public Task<ApiStaff?> CreateStaff(string firstName, string lastName, string role, string phoneNumber)
        {
            var body = new { FirstName = firstName, LastName = lastName, Role = role, PhoneNumber = phoneNumber };
            return Post<ApiStaff>("api/adminstaff/staff", body);
        }

        public Task<ApiStaff?> UpdateStaff(int id, string firstName, string lastName, string role, string phoneNumber)
        {
            var body = new { FirstName = firstName, LastName = lastName, Role = role, PhoneNumber = phoneNumber };
            return Put<ApiStaff>($"api/adminstaff/staff/{id}", body);
        }

        public Task<bool> DeleteStaff(int id) => Delete($"api/adminstaff/staff/{id}");

        public Task<List<ApiAdmin>?> GetAdmins() => Get<List<ApiAdmin>>("api/adminstaff/admins");

        public Task<ApiAdmin?> GetAdmin(int id) => Get<ApiAdmin>($"api/adminstaff/admins/{id}");

        public Task<ApiAdmin?> CreateAdmin(string username, string email, string password, int? staffId)
        {
            var body = new { Username = username, EmailAddress = email, Password = password, StaffId = staffId };
            return Post<ApiAdmin>("api/adminstaff/admins", body);
        }

        public Task<ApiAdmin?> UpdateAdmin(int id, string username, string email, int? staffId)
        {
            var body = new { Username = username, EmailAddress = email, StaffId = staffId };
            return Put<ApiAdmin>($"api/adminstaff/admins/{id}", body);
        }

        public Task<bool> DeleteAdmin(int id) => Delete($"api/adminstaff/admins/{id}");

        public Task<ApiCrabbyBotResponse?> GetCrabbyBotResponse(string optionId) => Post<ApiCrabbyBotResponse>("api/crabbybot", new { OptionId = optionId });

        private Task<ApiLoginResponse?> Login(string url, string identifier, string password) => Post<ApiLoginResponse>(url, new { Identifier = identifier, Password = password });

        private async Task<T?> Get<T>(string url) where T : class
        {
            try
            {
                var response = await _http.GetAsync(url);
                return await ReadResponse<T>(response, url, "GET");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "GET {Url} failed", url);
                return null;
            }
        }

        private async Task<T?> Post<T>(string url, object body) where T : class
        {
            try
            {
                var response = await _http.PostAsJsonAsync(url, body);
                return await ReadResponse<T>(response, url, "POST");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "POST {Url} failed", url);
                return null;
            }
        }

        private async Task<T?> Put<T>(string url, object body) where T : class
        {
            try
            {
                var response = await _http.PutAsJsonAsync(url, body);
                return await ReadResponse<T>(response, url, "PUT");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "PUT {Url} failed", url);
                return null;
            }
        }

        private async Task<bool> Delete(string url)
        {
            try
            {
                var response = await _http.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("DELETE {Url} returned {StatusCode}: {Error}", url, response.StatusCode, error);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _logger.LogWarning(ex, "DELETE {Url} failed", url);
                return false;
            }
        }

        private async Task<T?> Delete<T>(string url) where T : class
        {
            try
            {
                var response = await _http.DeleteAsync(url);
                return await ReadResponse<T>(response, url, "DELETE");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "DELETE {Url} failed", url);
                return null;
            }
        }

        private async Task<T?> ReadResponse<T>(HttpResponseMessage response, string url, string method) where T : class
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("{Method} {Url} returned {StatusCode}: {Error}", method, url, response.StatusCode, error);
                return null;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<T>();
        }


        public Task<ApiCustomerReview?> CreateReview(
            int customerId,
            int rating,
            string comment)
        {
            var body = new
            {
                Rating = rating,
                Comment = comment
            };

            return Post<ApiCustomerReview>(
                $"api/review/customer/{customerId}",
                body);
        }

        public Task<List<ApiCustomerReview>?> GetReviews()
        {
            return Get<List<ApiCustomerReview>>("api/review");
        }

        public Task<ApiCustomerReview?> RespondToReview(
            int reviewId,
            int adminId,
            string adminResponse)
        {
            var body = new
            {
                AdminResponse = adminResponse
            };

            return Put<ApiCustomerReview>(
                $"api/review/{reviewId}/response/{adminId}",
                body);
        }
        public Task<List<ApiCustomerReview>?> GetCustomerReviews(int customerId)
        {
            return Get<List<ApiCustomerReview>>(
                $"api/review/customer/{customerId}");
        }

    }
}
public class ApiOrderItemRequest
{
    public int MenuItemID { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}