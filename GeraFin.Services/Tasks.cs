using System;
using System.Linq;
using GeraFin.DAL.Helpers;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using GeraFin.InterFaces.Factory;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GeraFin.Models.ViewModels.Dashboard;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Services
{
    public class Tasks : ITasks
    {
        public int UserId;
        public string ConString;

        private readonly ApplicationDbContext _context;
        
        public Tasks(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            ConString = configuration.GetConnectionString("DevEnvR");
        }

        public async Task GetCurrentUserId()
        {
            ApplicationUser usr = await GetCurrentUserAsync();

            UserId = (from br in _context.UserProfile.Where(x => x.Email == usr.UserName) select br.UserProfileId).FirstOrDefault();
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            await GetCurrentUserId();

            return null;
        }

        public async Task GetUserTaskList()
        {
            await GetCurrentUserId();

            List<UserTasks> Items = await _context.UserTasks.Where(x => x.UserId.Equals(UserId)).ToListAsync();
        }

        public int AddUserTask(int UserID, string TaskName, DateTime DueDate, DateTime CompletionDate, bool IsOverDue, bool MarkCompleted)
        {
            SqlHelper helper = new SqlHelper(ConString);
            SqlParameter[] Parameters = new SqlParameter[6];
            Parameters.SetValue(helper.CreateParameter("@UserID", UserID, 0), 0);
            Parameters.SetValue(helper.CreateParameter("@TaskName", TaskName, 0), 1);
            Parameters.SetValue(helper.CreateParameter("@DueDate", DueDate, 0), 2);
            Parameters.SetValue(helper.CreateParameter("@CompletionDate", CompletionDate, 0), 3);
            Parameters.SetValue(helper.CreateParameter("@IsOverDue", IsOverDue, 0), 4);
            Parameters.SetValue(helper.CreateParameter("@IsOverDue", MarkCompleted, 0), 5);

            helper.Insert("GeraFin.stp_InsertUserTasks", System.Data.CommandType.StoredProcedure, Parameters);

            return (UserID);
        }

        Task ITasks.AddUserTask(int UserID, string TaskName, DateTime DueDate, DateTime CompletionDate, bool IsOverDue, bool MarkCompleted)
        {
            throw new NotImplementedException();
        }
    }
}
