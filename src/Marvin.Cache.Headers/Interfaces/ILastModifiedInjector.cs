using Marvin.Cache.Headers.Domain;
using System;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Interfaces
{
    /// <summary>
    /// Contract for a LastModifiedInjector, which can be used to inject custom last modified dates for resources
    /// of which you know when they were last modified (eg: a DB timestamp, custom logic, ...)
    /// </summary>
    public interface ILastModifiedInjector
    {
        Task<DateTimeOffset> CalculateLastModified(
            ResourceContext context);

        Task<DateTimeOffset> CalculateLastModified(
            ResourceContext context, 
            ValidatorValue validatorValue);
    }
}
