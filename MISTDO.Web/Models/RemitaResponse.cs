using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class RemitaResponse
    {

        [JsonProperty("paymentReference")]
        public string PaymentReference { get; set; }
        [JsonProperty("processorId")]

        public string ProcessorId { get; set; }
        [JsonProperty("transactionId")]

        public string TransactionId { get; set; }
        [JsonProperty("message")]

        public string Message { get; set; }
        [JsonProperty("amount")]

        public int Amount { get; set; }

    }

    public class FailureResponse
    {
        [JsonProperty("code")]

        public string Code { get; set; }
        [JsonProperty("message")]

        public string Message { get; set; }
        [JsonProperty("dismissible")]

        public bool Dismissible { get; set; }
    }

}
