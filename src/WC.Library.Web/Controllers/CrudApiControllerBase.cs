using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WC.Library.Data.Services;
using WC.Library.Domain.Models;
using WC.Library.Domain.Services;
using WC.Library.Web.Models;

namespace WC.Library.Web.Controllers;

public abstract class CrudApiControllerBase<TCategoryName, TManager, TProvider, TDomain, TDto, TDtoDetail>
    : ApiControllerBase<TCategoryName>
    where TManager : IDataManager<TDomain>
    where TProvider : IDataProvider<TDomain>
    where TDomain : class, IModel
    where TDto : class, IDto
    where TDtoDetail : class, IDto
{
    protected CrudApiControllerBase(
        IMapper mapper,
        ILogger<TCategoryName> logger,
        TManager manager,
        TProvider provider)
        : base(mapper, logger)
    {
        Manager = manager;
        Provider = provider;
    }

    protected TManager Manager { get; }

    protected TProvider Provider { get; }

    protected async Task<ICollection<TDto>> GetMany(
        bool withIncludes = false,
        IWcTransaction? transaction = default,
        CancellationToken cancellationToken = default)
    {
        return Mapper.Map<ICollection<TDto>>(await Provider.Get(withIncludes, transaction, cancellationToken));
    }

    protected async Task<TDtoDetail> GetOneById(
        Guid id,
        bool withIncludes = false,
        IWcTransaction? transaction = default,
        CancellationToken cancellationToken = default)
    {
        return Mapper.Map<TDtoDetail>(await Provider.GetOneById(id, withIncludes, transaction, cancellationToken));
    }

    protected async Task<IActionResult> Create<TCreateDto, TCreatedResultDto>(
        TCreateDto payload,
        string createAtRouteName,
        CancellationToken cancellationToken = default)
    {
        var createdItem = await Manager.Create(Mapper.Map<TDomain>(payload), default, cancellationToken);

        return CreatedAtRoute(createAtRouteName, new { id = createdItem.Id },
            Mapper.Map<TCreatedResultDto>(createdItem));
    }

    protected async Task<IActionResult> Update<TUpdateDto>(
        Guid id,
        JsonPatchDocument<TUpdateDto> patchDocument,
        IWcTransaction? transaction = default,
        CancellationToken cancellationToken = default)
        where TUpdateDto : class
    {
        var record = await Provider.GetOneById(id, true, transaction, cancellationToken);
        var updateDto = Mapper.Map<TUpdateDto>(record);

        patchDocument.ApplyTo(updateDto);

        if (!TryValidateModel(updateDto))
        {
            return BadRequest(ModelState);
        }

        Mapper.Map(updateDto, record);
        _ = await Manager.Update(record!, transaction, cancellationToken);

        return Ok();
    }

    protected async Task<IActionResult> Delete(
        Guid id,
        IWcTransaction? transaction = default,
        CancellationToken cancellationToken = default)
    {
        _ = await Manager.Delete(id, transaction, cancellationToken);

        return NoContent();
    }
}

public abstract class CrudApiControllerBase<TCategoryName, TManager, TProvider, TDomain, TDto>
    : CrudApiControllerBase<TCategoryName, TManager, TProvider, TDomain, TDto, TDto>
    where TManager : IDataManager<TDomain>
    where TProvider : IDataProvider<TDomain>
    where TDomain : class, IModel
    where TDto : class, IDto
{
    protected CrudApiControllerBase(
        IMapper mapper,
        ILogger<TCategoryName> logger,
        TManager manager,
        TProvider provider)
        : base(mapper, logger, manager, provider)
    {
    }
}
