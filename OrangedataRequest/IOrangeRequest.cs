using System.Threading.Tasks;
using OrangedataRequest.DataService;

namespace OrangedataRequest
{
    public interface IOrangeRequest
    {
        /// <summary>
        ///     Отправка запроса создания чека
        /// </summary>
        /// <param name="check">Чек</param>
        /// <returns></returns>
        Task<ODResponse> CreateCheckAsync(ReqCreateCheck check);

        /// <summary>
        ///     Отправка запроса состояния чека
        /// </summary>
        /// <param name="INN">ИНН организации, для которой пробивается чек</param>
        /// <param name="documentId">Идентификатор документа, который был указан при его создании</param>
        /// <returns></returns>
        Task<ODResponse> GetCheckStateAsync(string INN, string documentId);

        /// <summary>
        ///     Отправка запроса создания чека коррекции
        /// </summary>
        /// <param name="correctionCheck">Чек коррекции</param>
        /// <returns></returns>
        Task<ODResponse> CreateCorrectionCheckAsync(ReqCreateCorrectionCheck correctionCheck);

        /// <summary>
        ///     Отправка запроса состояния чека коррекции
        /// </summary>
        /// <param name="INN">ИНН организации, для которой пробивается чек</param>
        /// <param name="documentId">Идентификатор документа, который был указан при его создании</param>
        /// <returns></returns>
        Task<ODResponse> GetCorrectionCheckStateAsync(string INN, string documentId);
    }
}