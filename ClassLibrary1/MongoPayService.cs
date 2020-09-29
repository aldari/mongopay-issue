using MangoPay.SDK;
using MangoPay.SDK.Core;
using MangoPay.SDK.Core.Enumerations;
using MangoPay.SDK.Entities;
using MangoPay.SDK.Entities.GET;
using MangoPay.SDK.Entities.POST;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class MongoPayService
    {
        private static UserNaturalDTO _john;
        protected MangoPayApi Api;
        private static WalletDTO _johnsWallet;

        public MongoPayService()
        {
            this.Api = BuildNewMangoPayApi();
            //_johnsReports = new Dictionary<ReportType, ReportRequestDTO>();
        }

        public async Task GetTransactionsAsync()
        {
            UserNaturalDTO john = await GetJohn();

            WalletDTO wallet = await CreateJohnsWallet();
            PayInDTO payIn = await CreateJohnsPayInCardWeb(wallet.Id);

            var api = Api;
            Sort mpSort = new Sort();
            mpSort.AddField("CreationDate", SortDirection.desc);
            Pagination pagination = new Pagination(1, 99);

            var mpTransactions = await api.Wallets.GetTransactionsAsync(wallet.Id, pagination, mpSort);
        }

        protected MangoPayApi BuildNewMangoPayApi()
        {
            MangoPayApi api = new MangoPayApi();

            // use test client credentails
            api.Config.ClientId = "xxxxx";
            api.Config.ClientPassword = "xxxxxx";
            api.Config.BaseUrl = "https://api.sandbox.mangopay.com";
            api.Config.ApiVersion = "v2.01";

            return api;
        }

        protected async Task<UserNaturalDTO> GetJohn(bool recreate = false)
        {
            if (_john == null || recreate)
            {
                UserNaturalPostDTO user = new UserNaturalPostDTO("john.doe@sample.org", "John", "Doe", new DateTime(1975, 12, 21, 0, 0, 0), CountryIso.FR, CountryIso.FR);
                user.Occupation = "programmer";
                user.IncomeRange = 3;
                user.Address = new Address { AddressLine1 = "Address line 1", AddressLine2 = "Address line 2", City = "City", Country = CountryIso.PL, PostalCode = "11222", Region = "Region" };
                user.Capacity = CapacityType.DECLARATIVE;

                _john = await this.Api.Users.CreateAsync(user);

                _johnsWallet = null;
            }
            return _john;
        }


        protected async Task<WalletDTO> CreateJohnsWallet()
        {

            UserNaturalDTO john = await this.GetJohn();

            WalletPostDTO wallet = new WalletPostDTO(new List<string> { john.Id }, "WALLET IN EUR", CurrencyIso.EUR);

            return await Api.Wallets.CreateAsync(wallet);
        }

        protected async Task<PayInCardWebDTO> CreateJohnsPayInCardWeb(string walletId)
        {

            UserNaturalDTO user = await this.GetJohn();

            PayInCardWebPostDTO payIn = new PayInCardWebPostDTO(user.Id, new Money { Amount = 1000, Currency = CurrencyIso.EUR }, new Money { Amount = 0, Currency = CurrencyIso.EUR }, walletId, "https://test.com", CultureCode.FR, CardType.CB_VISA_MASTERCARD);

            return await this.Api.PayIns.CreateCardWebAsync(payIn);
        }
    }
}
