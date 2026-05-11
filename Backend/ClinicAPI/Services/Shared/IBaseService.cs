using System;

namespace ClinicAPI.Services.Shared;

public interface IBaseService<T, ID>
{
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(ID id);
}
