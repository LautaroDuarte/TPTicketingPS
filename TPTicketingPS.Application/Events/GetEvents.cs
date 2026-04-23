using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Application.Events;

public class GetEvents : IGetEvents
{
    public List<string> Execute()
    {
        return new List<string>
        {
            "Concierto Rock",
            "Partido de Fútbol"
        };
    }
}
