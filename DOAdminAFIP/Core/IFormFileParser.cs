using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DOAdminAFIP.Core
{
    public interface IFormFileParser
    {
        WorkBookGenerationResult GenerateWorkBook(string content, string nombre_workbook);
    }
}
