using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WC.Library.Web.Controllers;

[ApiController]
public abstract class ApiControllerBase<TCategoryName> : ControllerBase
{
    protected ApiControllerBase(
        IMapper mapper,
        ILogger<TCategoryName> logger)
    {
        Mapper = mapper;
        Logger = logger;
    }

    protected IMapper Mapper { get; }

    protected ILogger<TCategoryName> Logger { get; }
}