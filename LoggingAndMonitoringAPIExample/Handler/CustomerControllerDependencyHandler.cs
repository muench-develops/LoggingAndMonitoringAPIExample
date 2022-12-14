using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.Extensions.Caching.Memory;

namespace LoggingAndMonitoringAPIExample.Handler
{
    public class CustomerDependencyHandler
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMemoryCache _cache;

        public CustomerDependencyHandler() { }

        public CustomerDependencyHandler(
            ICustomerService customerService,
            IMapper mapper,
            ILoggerFactory loggerFactory,
            IMemoryCache cache)
        {
            _customerService = customerService;
            _mapper = mapper;
            _loggerFactory = loggerFactory;
            _cache = cache;
        }

        public virtual IMemoryCache GetCache()
        {
            return _cache;
        }

        public virtual ICustomerService GetCustomerService()
        {
            return _customerService;
        }

        public virtual IMapper GetMapper()
        {
            return _mapper;
        }

        public virtual ILoggerFactory GetLoggerFactory()
        {
            return _loggerFactory;
        }
    }
}
