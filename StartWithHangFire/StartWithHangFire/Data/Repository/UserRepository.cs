using StartWithHangFire.Data.Context;
using StartWithHangFire.Data.Repository.Interfaces;
using StartWithHangFire.Models;

namespace StartWithHangFire.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
           : base(context)
        {

        }
    }
}
