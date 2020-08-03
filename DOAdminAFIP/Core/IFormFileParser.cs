using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DOAdminAFIP.Core
{
    public interface IRawInputParser
    {
        WorkBookGenerationResult GenerateWorkBook(string content, string nombre_workbook);
    }
}
