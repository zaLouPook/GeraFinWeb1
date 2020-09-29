using System.Threading.Tasks;

namespace GeraFin.InterFaces.Factory
{
    public interface IRoles
    {
        Task GenerateRolesFromPagesAsync();

        Task AddToRoles(string applicationUserId);
    }
}
