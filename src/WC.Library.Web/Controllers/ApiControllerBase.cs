using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WC.Library.Web.Controllers;

[ApiController]
public abstract class ApiControllerBase<TCategoryName> : ControllerBase
{
    protected ApiControllerBase(
        IMapper mapper,
        ILogger<TCategoryName> logger,
        IEnumerable<IValidator> validators)
    {
        Mapper = mapper;
        Logger = logger;
        Validators = validators;
    }

    protected IMapper Mapper { get; }

    protected ILogger<TCategoryName> Logger { get; }

    protected IEnumerable<IValidator> Validators { get; }
}