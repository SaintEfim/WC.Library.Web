using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WC.Library.Domain.Services;
using WC.Library.Shared.Exceptions;

namespace WC.Library.Web.Controllers;

public abstract class CrudApiControllerBase<TCategoryName, TManager, TProvider, TDomain,
    TDto> : ApiControllerBase<TCategoryName>
    where TManager : IDataManager<TDomain>
    where TProvider : IDataProvider<TDomain>
    where TDomain : class
    where TDto : class
{
    protected CrudApiControllerBase(
        IMapper mapper,
        ILogger<TCategoryName> logger,
        IEnumerable<IValidator> validators,
        TManager manager,
        TProvider provider)
        : base(mapper, logger, validators)
    {
        Manager = manager;
        Provider = provider;
    }

    protected TManager Manager { get; }

    protected TProvider Provider { get; }

    protected async Task<ICollection<TDto>> GetMany(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Mapper.Map<ICollection<TDto>>(await Provider.Get(cancellationToken));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving entities: {Message}", ex.Message);
            throw;
        }
    }

    protected async Task<TDto> GetOneById(
        Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        try
        {
            var entity = await Provider.GetOneById(id, cancellationToken);

            ArgumentNullException.ThrowIfNull(entity);

            return Mapper.Map<TDto>(entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving entity by ID: {Message}", ex.Message);
            throw;
        }
    }

    protected async Task<IActionResult> Create<TCreateDto>(TCreateDto payload,
        CancellationToken cancellationToken = default)
    {
        Validate(payload);
        try
        {
            await Manager.Create(Mapper.Map<TDomain>(payload), cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating entity: {Message}", ex.Message);
            throw;
        }
    }

    protected async Task<IActionResult> Update<TUpdateDto>(Guid id,
        JsonPatchDocument<TUpdateDto> patchDocument,
        CancellationToken cancellationToken = default) where TUpdateDto : class
    {
        try
        {
            var record = await Provider.GetOneById(id, cancellationToken: cancellationToken);
            if (record == null)
            {
                throw new NotFoundException();
            }

            var updateDto = Mapper.Map<TUpdateDto>(record);
            patchDocument.ApplyTo(updateDto);
            Validate(updateDto);
            Mapper.Map(updateDto, record);
            await Manager.Update(record, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating entity: {Message}", ex.Message);
            throw;
        }
    }

    protected async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(id.ToString());
        try
        {
            await Manager.Delete(id, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting entity: {Message}", ex.Message);
            throw;
        }
    }

    protected void Validate<T>(T model)
    {
        var errors = Validators.OfType<IValidator<T>>().Select(validator => validator.Validate(model))
            .SelectMany(result => result.Errors).ToList();

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}