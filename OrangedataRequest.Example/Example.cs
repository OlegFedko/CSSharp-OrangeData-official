using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OrangedataRequest;
using OrangedataRequest.DataService;
using OrangedataRequest.Models;
using OrangedataRequest.Enums;

namespace TestLauncher
{
    public class Example
    {
        static async Task Main(string[] args)
        {
            /* Для тестовых целей скачать private_key_test.xml и client.pfx из
               https://github.com/orangedata-official/API/tree/master/files_for_test

               Для боевого использования:

               Перейти в https://lk.orangedata.ru/lk/integrations/direct
               
               Выбрать "Прямое подключение"
               
               Нажать "скачать программу", скачается Nebula.KeysGenerator.exe
               
               Запустить Nebula.KeysGenerator.exe, нажать "Сгенерировать RSA-ключи"
               Появится "rsa_2048_private_key.xml" и "rsa_2048_public_key.xml"
               
               В разделе "Введите публичную часть ключа" выбрать файл "rsa_2048_public_key.xml", нажать "Сохранить".
               
               В разделе "Получите сертификат" нажать "Скачать сертификат", скачается файл "<ВАШ ИНН>.zip".
               Внутри него нужен "<ВАШ ИНН>.pfx" и пароль из "readme_v2.txt" (всегда "1234")
               
               Нужны "rsa_2048_private_key.xml", "<ВАШ ИНН>.pfx", пароль из "readme_v2.txt"
            */
            var prKeyPath = ".\\private_key_test.xml";
            var certPath = ".\\client.pfx";
            var certPass = "1234";

            var apiUrl = OrangeRequest.TestEnvironment1ApiUrl;
            var inn = OrangeRequest.TestEnvironment1Inn;
            var key = OrangeRequest.TestEnvironment1Key;

            var dummyOrangeRequest = new OrangeRequest(prKeyPath, certPath, certPass, apiUrl);

            var documentId1 = Guid.NewGuid().ToString();
            var dummyCreateCheckRequest = new ReqCreateCheck
            {
                INN = inn,
                Key = key,
                Id = documentId1,
                Content = new Content
                {
                    Type = DocTypeEnum.In,
                    AgentType = AgentTypeEnum.PayingAgent,
                    TotalSum = 123.45m + 2 * 4.45m,
                    Vat7Sum = Math.Round((2 * 4.45m) * 5m / 105m, 2),
                    CheckClose = new CheckClose
                    {
                        Payments =
                        [
                            new Payment
                            {
                                Amount = 123.45m + 4.45m * 2,
                                Type = PaymentTypeEnum.Cash
                            }
                        ],
                        TaxationSystem = TaxationSystemEnum.Simplified
                    },
                    Positions =
                    [
                        new Position
                        {
                            Price = 123.45m,
                            Quantity = 1m,
                            Tax = VATRateEnum.NONE,
                            Text = "Булка",
                            PaymentMethodType = PaymentMethodTypeEnum.Full,
                            PaymentSubjectType = PaymentSubjectTypeEnum.Product
                        },
                        new Position
                        {
                            Price = 4.45m,
                            Quantity = 2m,
                            Tax = VATRateEnum.VAT5,
                            Text = "Спички",
                            PaymentMethodType = PaymentMethodTypeEnum.Full,
                            PaymentSubjectType = PaymentSubjectTypeEnum.Product
                        }
                    ],
                    CustomerContact = "foo@example.com"
                }
            };

            var documentId2 = Guid.NewGuid().ToString();
            var dummyCreateCorrectionCheckRequest = new ReqCreateCorrectionCheck
            {
                INN = inn,
                Key = key,
                Id = documentId2,
                Content = new CorrectionContent
                {
                    Type = DocTypeEnum.In,
                    CashSum = 2000,
                    TaxationSystem = TaxationSystemEnum.Common,
                    Tax4Sum = 2000,
                    CauseDocumentDate = DateTime.UtcNow.Date,
                    CauseDocumentNumber = "21"
                }
            };
            try
            {
                var res1 = await dummyOrangeRequest.CreateCheckAsync(dummyCreateCheckRequest);
                if (res1.StatusCode != HttpStatusCode.Created)
                {
                    if (res1.ResponseObject is Exception exception)
                        throw new ApplicationException("CreateCheckAsync failed", exception);
                    var e = (RespCreateCheck)res1.ResponseObject;
                    throw new Exception($"{nameof(dummyOrangeRequest.CreateCheckAsync)} failed with code {res1.StatusCode}: {string.Join(";", e.Errors)}");
                }

                ODResponse res2;
                while (true)
                {
                    res2 = await dummyOrangeRequest.GetCheckStateAsync(inn, documentId1);
                    if (res2.StatusCode != HttpStatusCode.Accepted)
                        break;
                    Console.WriteLine("Pending...");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                if (res2.StatusCode != HttpStatusCode.OK) 
                    throw new Exception($"{nameof(dummyOrangeRequest.GetCheckStateAsync)} failed with code {res2.StatusCode}: {res2.Response}");
                var r = (RespCheckStatus)res2.ResponseObject;
                Console.WriteLine($"FP: {r.FP}, FS: {r.FSNumber}, FD: {r.DocumentNumber}\n{res2.Response}");
                
                var res3 = await dummyOrangeRequest.CreateCorrectionCheckAsync(dummyCreateCorrectionCheckRequest);
                if (res3.StatusCode != HttpStatusCode.Created) 
                    throw new Exception($"{nameof(dummyOrangeRequest.CreateCorrectionCheckAsync)} failed with code {res3.StatusCode}: {res3.Response}");
                
                ODResponse res4;
                while (true)
                {
                    res4 = await dummyOrangeRequest.GetCorrectionCheckStateAsync(inn, documentId2);
                    if (res4.StatusCode != HttpStatusCode.Accepted)
                        break;
                    Console.WriteLine("Pending...");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                if (res4.StatusCode != HttpStatusCode.OK) 
                    throw new Exception($"{nameof(dummyOrangeRequest.GetCorrectionCheckStateAsync)} failed with code {res4.StatusCode}: {res4.Response}");
                var r2 = (RespCorrectionCheckStatus)res4.ResponseObject;
                Console.WriteLine($"FP: {r2.FP}, FS: {r2.FSNumber}, FD: {r2.DocumentNumber}\n{res4.Response}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}