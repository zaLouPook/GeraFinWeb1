using System.Linq;
using System.Threading.Tasks;
using GeraFin.InterFaces.Factory;

namespace GeraFin.DAL.DataAccess
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, IFunctional functional)
        {
            context.Database.EnsureCreated();

            if (context.ApplicationUser.Any())
            {
                return;
            }
            await functional.CreateDefaultSuperAdmin();
            await functional.InitAppData();
        }
    }
}