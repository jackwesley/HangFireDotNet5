using StartWithHangFire.Data.Repository.Interfaces;
using StartWithHangFire.Models;
using System;
using System.Threading.Tasks;

namespace StartWithHangFire.Service
{
    public class JobToProcess : IJobToProcess
    {
        private readonly IUserRepository _userRepository;
        public JobToProcess(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task InsertUser(string message)
        {

            await Task.Run(() =>
            {
                var id = Guid.NewGuid();
                var user = new User
                {
                    Id = id,
                    Name = $"{message}",
                    Description = $"Usuario salvo pelo método: {message} às {DateTime.Now}",
                    CreatedAt = DateTime.Now
                };

                _userRepository.Add(user);
            });

        }

        public void JobException(string message)
        {
            throw new Exception("Erro Ao Tentar Rodar este Job");
        }
    }
}
