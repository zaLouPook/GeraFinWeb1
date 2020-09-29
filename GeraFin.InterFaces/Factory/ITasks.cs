using System;
using System.Threading.Tasks;

namespace GeraFin.InterFaces.Factory
{
    public interface ITasks
    {
        Task GetUserTaskList();

        Task AddUserTask(int UserID, string TaskName, DateTime DueDate, DateTime CompletionDate, bool IsOverDue, bool MarkCompleted);
    }
}
