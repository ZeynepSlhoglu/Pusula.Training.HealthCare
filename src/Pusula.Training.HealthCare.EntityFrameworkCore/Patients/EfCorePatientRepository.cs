using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Addresses;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Districts;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Patients;

public class EfCorePatientRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Patient, Guid>(dbContextProvider), IPatientRepository
{
    public async Task<PatientWithAddressAndCountry> GetWithAddressAndCountryAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryForAddressAndCountryAsync())
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<List<Patient>> GetListAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? identityNumber = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumGender gender = EnumGender.None,
        EnumBloodType bloodType = EnumBloodType.None,
        EnumMaritalStatus maritalStatus = EnumMaritalStatus.None,
        Guid? countryId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilter(await GetQueryableAsync(), filterText, firstName, lastName, birthDateMin, birthDateMax,
                         identityNumber,
                         emailAddress, mobilePhoneNumber, homePhoneNumber, gender, bloodType, maritalStatus,
                         countryId)
                     .OrderBy(string.IsNullOrWhiteSpace(sorting) ? PatientConsts.GetDefaultSorting(false) : sorting)
                     .PageBy(skipCount, maxResultCount)
                     .ToListAsync(cancellationToken);
    }

    public async Task<List<PatientWithAddressAndCountry>> GetListWithAddressAndCountryAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? identityNumber = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumGender gender = EnumGender.None,
        EnumBloodType bloodType = EnumBloodType.None,
        EnumMaritalStatus maritalStatus = EnumMaritalStatus.None,
        Guid? countryId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilter(await GetQueryForAddressAndCountryAsync(), filterText, firstName, lastName,
                         birthDateMin, birthDateMax,
                         identityNumber,
                         emailAddress, mobilePhoneNumber, homePhoneNumber, gender, bloodType, maritalStatus,
                         countryId)
                     .OrderBy(string.IsNullOrWhiteSpace(sorting) ? PatientConsts.GetDefaultSorting(false) : sorting)
                     .PageBy(skipCount, maxResultCount)
                     .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? identityNumber = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumGender gender = EnumGender.None,
        EnumBloodType bloodType = EnumBloodType.None,
        EnumMaritalStatus maritalStatus = EnumMaritalStatus.None,
        Guid? countryId = null,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilter(await GetQueryableAsync(), filterText, firstName, lastName, birthDateMin, birthDateMax,
                identityNumber, emailAddress, mobilePhoneNumber, homePhoneNumber, gender, bloodType,
                maritalStatus,
                countryId)
            .LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual async Task<IQueryable<PatientWithAddressAndCountry>> GetQueryForAddressAndCountryAsync()
    {
        return from patient in await GetDbSetAsync()
               join patient_country in (await GetDbContextAsync()).Set<Country>() on patient.CountryId equals
                   patient_country.Id into
                   patient_countries
               from patient_country in patient_countries.DefaultIfEmpty()
               join address in (await GetDbContextAsync()).Set<Address>() on patient.Id equals address.PatientId into
                   addresses
               from address in addresses.DefaultIfEmpty()
               join district in (await GetDbContextAsync()).Set<District>() on address.DistrictId equals district.Id
                   into
                   districts
               from district in districts.DefaultIfEmpty()
               join city in (await GetDbContextAsync()).Set<City>() on district.CityId equals city.Id into cities
               from city in cities.DefaultIfEmpty()
               join address_country in (await GetDbContextAsync()).Set<Country>() on city.CountryId equals
                   address_country
                       .Id into address_countries
               from address_country in address_countries.DefaultIfEmpty()
               select new PatientWithAddressAndCountry
               {
                   Id = patient.Id,
                   EmailAddress = patient.EmailAddress,
                   FirstName = patient.FirstName,
                   IdentityNumber = patient.IdentityNumber,
                   HomePhoneNumber = patient.HomePhoneNumber,
                   BirthDate = patient.BirthDate,
                   Gender = patient.Gender,
                   BloodType = patient.BloodType,
                   LastName = patient.LastName,
                   MaritalStatus = patient.MaritalStatus,
                   MobilePhoneNumber = patient.MobilePhoneNumber,
                   CreationTime = patient.CreationTime,
                   Country = patient_country.Name,
                   CountryId = patient_country.Id,
                   Address = address != null
                       ? new AddressWithRelations()
                       {
                           Id = address.Id,
                           PatientId = patient.Id,
                           DistrictId = address.DistrictId,
                           District = district.Name,
                           CountryId = address_country.Id,
                           Country = address_country.Name,
                           CityId = city.Id,
                           City = city.Name,
                           AddressLine = address.AddressLine,
                           CreationTime = address.CreationTime
                       }
                       : null
               };
    }

#region Delete

    public virtual async Task DeleteAllAsync(
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? identityNumber = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumGender gender = EnumGender.None,
        EnumBloodType bloodType = EnumBloodType.None,
        EnumMaritalStatus maritalStatus = EnumMaritalStatus.None,
        Guid? countryId = null,
        CancellationToken cancellationToken = default)
    {
        var ids = ApplyFilter(await GetQueryableAsync(), filterText, firstName, lastName, birthDateMin, birthDateMax,
                identityNumber,
                emailAddress, mobilePhoneNumber, homePhoneNumber, gender, bloodType, maritalStatus,
                countryId)
            .Select(e => e.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

#endregion

#region Apply Filter

    protected virtual IQueryable<Patient> ApplyFilter(
        IQueryable<Patient> query,
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? identityNumber = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumGender gender = EnumGender.None,
        EnumBloodType bloodType = EnumBloodType.None,
        EnumMaritalStatus maritalStatus = EnumMaritalStatus.None,
        Guid? countryId = null)
    {
        return query
               .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                   e => e.FirstName!.Contains(filterText!) || e.LastName!.Contains(filterText!) ||
                       e.IdentityNumber!.Contains(filterText!) || e.EmailAddress!.Contains(filterText!) ||
                       e.MobilePhoneNumber!.Contains(filterText!) || e.HomePhoneNumber!.Contains(filterText!))
               .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.Contains(firstName!))
               .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.Contains(lastName!))
               .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
               .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
               .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber.Contains(identityNumber!))
               .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress.Contains(emailAddress!))
               .WhereIf(!string.IsNullOrWhiteSpace(mobilePhoneNumber),
                   e => e.MobilePhoneNumber.Contains(mobilePhoneNumber!))
               .WhereIf(!string.IsNullOrWhiteSpace(homePhoneNumber),
                   e => e.HomePhoneNumber != null && e.HomePhoneNumber.Contains(homePhoneNumber!))
               .WhereIf(gender != EnumGender.None, e => e.Gender == gender)
               .WhereIf(bloodType != EnumBloodType.None, e => e.BloodType == bloodType)
               .WhereIf(maritalStatus != EnumMaritalStatus.None, e => e.MaritalStatus == maritalStatus)
               .WhereIf(countryId.HasValue, e => e.CountryId == countryId!.Value);
    }

    protected virtual IQueryable<PatientWithAddressAndCountry> ApplyFilter(
        IQueryable<PatientWithAddressAndCountry> query,
        string? filterText = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? birthDateMin = null,
        DateTime? birthDateMax = null,
        string? identityNumber = null,
        string? emailAddress = null,
        string? mobilePhoneNumber = null,
        string? homePhoneNumber = null,
        EnumGender gender = EnumGender.None,
        EnumBloodType bloodType = EnumBloodType.None,
        EnumMaritalStatus maritalStatus = EnumMaritalStatus.None,
        Guid? countryId = null)
    {
        return query
               .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                   e => e.FirstName!.Contains(filterText!) || e.LastName!.Contains(filterText!) ||
                       e.IdentityNumber!.Contains(filterText!) || e.EmailAddress!.Contains(filterText!) ||
                       e.MobilePhoneNumber!.Contains(filterText!) ||
                       e.HomePhoneNumber!.Contains(filterText!))
               .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.Contains(firstName!))
               .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.Contains(lastName!))
               .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
               .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
               .WhereIf(!string.IsNullOrWhiteSpace(identityNumber),
                   e => e.IdentityNumber.Contains(identityNumber!))
               .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress.Contains(emailAddress!))
               .WhereIf(!string.IsNullOrWhiteSpace(mobilePhoneNumber),
                   e => e.MobilePhoneNumber.Contains(mobilePhoneNumber!))
               .WhereIf(!string.IsNullOrWhiteSpace(homePhoneNumber),
                   e => e.HomePhoneNumber != null && e.HomePhoneNumber.Contains(homePhoneNumber!))
               .WhereIf(gender != EnumGender.None, e => e.Gender == gender)
               .WhereIf(bloodType != EnumBloodType.None, e => e.BloodType == bloodType)
               .WhereIf(maritalStatus != EnumMaritalStatus.None, e => e.MaritalStatus == maritalStatus)
               .WhereIf(countryId.HasValue, e => e.CountryId == countryId!.Value);
    }

#endregion
}