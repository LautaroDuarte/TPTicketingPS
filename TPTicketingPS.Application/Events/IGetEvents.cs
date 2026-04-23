using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Application.Events;

public interface IGetEvents
{
    List<string> Execute();
}
