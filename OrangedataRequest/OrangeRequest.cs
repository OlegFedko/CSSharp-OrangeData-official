using OrangedataRequest.DataService;
using System.Threading.Tasks;

namespace OrangedataRequest
{
    public sealed class OrangeRequest
    {
        /// <summary>
        /// This test environment provides API functionality without a web cabinet. Device emulators are used instead of physical devices.
        /// Any tester can use the OrangeData 1 test environment for free.
        /// 
        /// Test environment 1 can be used according to the INN of the test organization without a long configuration period*.
        /// 
        /// Test Environment 1 is free and recommended for customer testing.
        /// </summary>
        public const string TestEnvironment1Inn = "7727401209";
        public const string TestEnvironment1Key = "7727401209";

        /// <summary>
        /// Web office: Not available<br/>
        /// Browser Accessibility Check URL: https://apip.orangedata.ru:2443/api/v2/<br/>
        /// Swagger: https://apip.orangedata.ru:2443/swagger<br/>
        /// Swagger JSON specification: https://apip.orangedata.ru:2443/swagger/v2/swagger.json
        /// </summary>
        public const string TestEnvironment1ApiUrl = "https://apip.orangedata.ru:2443/api/v2";

        /// <summary>
        /// Web office: https://test.orangedata.ru/lk/<br/>
        /// Browser Accessibility Check URL: https://apip.orangedata.ru:12001/api/v2/<br/>
        /// Swagger: https://apip.orangedata.ru:12001/swagger<br/>
        /// Swagger JSON specification: https://apip.orangedata.ru:12001/swagger/v2/swagger.json
        /// </summary>
        public const string TestEnvironment2ApiUrl = "https://apip.orangedata.ru:12001/api/v2";

        /// <summary>
        /// Web office: https://lk.orangedata.ru<br/>
        /// Browser Accessibility Check URL: https://api.orangedata.ru:12003/api/v2/<br/>
        /// Swagger: https://api.orangedata.ru:12003/swagger<br/>
        /// Swagger JSON specification: https://api.orangedata.ru:12003/swagger/v2/swagger.json
        /// </summary>
        public const string ApiUrl = "https://api.orangedata.ru:12003/api/v2";

        private readonly ODDataService _dataService;

        /// <summary>
        /// </summary>
        /// <param name="keyPath">Путь к xml-файлу ключа для подписи клиентских сообщений</param>
        /// <param name="certPath">Путь к клиентскому сертификату</param>
        /// <param name="certPassword">Пароль клиентского сертификата</param>
        /// <param name="apiUrl">URL-адрес к продуктовому или тестовому API</param>
        public OrangeRequest(string keyPath, string certPath, string certPassword, string apiUrl = ApiUrl)
        {
            _dataService = new ODDataService(keyPath, certPath, certPassword, apiUrl);
        }

        /// <summary>
        ///     Отправка запроса создания чека
        /// </summary>
        /// <param name="check">Чек</param>
        /// <returns></returns>
        public async Task<ODResponse> CreateCheckAsync(ReqCreateCheck check)
        {
            return await _dataService.SendCheckAsync(check);
        }

        /// <summary>
        ///     Отправка запроса состояния чека
        /// </summary>
        /// <param name="INN">ИНН организации, для которой пробивается чек</param>
        /// <param name="documentId">Идентификатор документа, который был указан при его создании</param>
        /// <returns></returns>
        public async Task<ODResponse> GetCheckStateAsync(string INN, string documentId)
        {
            return await _dataService.GetCheckStateAsync(INN, documentId);
        }

        /// <summary>
        ///     Отправка запроса создания чека коррекции
        /// </summary>
        /// <param name="correctionCheck">Чек коррекции</param>
        /// <returns></returns>
        public async Task<ODResponse> CreateCorrectionCheckAsync(ReqCreateCorrectionCheck correctionCheck)
        {
            return await _dataService.CreateCorrectionsCheckAsync(correctionCheck);
        }

        /// <summary>
        ///     Отправка запроса состояния чека коррекции
        /// </summary>
        /// <param name="INN">ИНН организации, для которой пробивается чек</param>
        /// <param name="documentId">Идентификатор документа, который был указан при его создании</param>
        /// <returns></returns>
        public async Task<ODResponse> GetCorrectionCheckStateAsync(string INN, string documentId)
        {
            return await _dataService.GetCorrectionCheckStateAsync(INN, documentId);
        }
    }
}
