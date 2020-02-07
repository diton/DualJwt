using System;
using System.Collections.Generic;
using System.Linq;

namespace DualJwt.Security
{
    public class ApiKeyService : IApiKeyService
    {
        private static List<ExternalService> _externalServices;

        public ApiKeyService()
        {
            _externalServices = new List<ExternalService>()
            {
                new ExternalService()
                {
                    Id = Guid.NewGuid(),
                    Name = "first",
                    ApiKey = "1234567890abcdefghij",
                    IsActive = true,
                    IsDeleted = false,
                    AllowedActions = new List<AllowedControllerAction>()
                    {
                        new AllowedControllerAction()
                        {
                            Controller = "values",
                            Method = "GET",
                            Action = "*"
                        }
                    }
                },
                new ExternalService()
                {
                    Id = Guid.NewGuid(),
                    Name = "first",
                    ApiKey = "abcdefghijklmnopqrs",
                    IsActive = true,
                    IsDeleted = false,
                    AllowedActions = new List<AllowedControllerAction>()
                    {
                        new AllowedControllerAction()
                        {
                            Controller = "values",
                            Method = "GET",
                            Action = "*"
                        }
                    }
                }
            };
        }

        public ExternalService GetExternalServiceByApiKey(string apiKey)
        {
            return _externalServices.SingleOrDefault(x => x.IsActive && !x.IsDeleted && x.ApiKey.Equals(apiKey));
        }

        public ExternalService GetRandom()
        {
            var rnd = new Random();
            var next = rnd.Next(0, _externalServices.Count - 1);
            return _externalServices[next];
        }
    }
}
