using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Infrastructure;

namespace BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(UserDTO userDto);
        Task<OperationDetails> Update(UserDTO userDto);
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task SetInitialData(UserDTO adminDto, List<string> roles);
        UserDTO FindById(string user_id);
        UserDTO Find(string login);
        List<UserDTO> GetAll();
        void ChangeRole(string user_id, string newrole);
        void DeleteUser(string user_id);
    }
}
