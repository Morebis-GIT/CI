using System;
using System.Linq;
using xggameplan.Model;

namespace xggameplan.core.Validators
{
    public class DataManipulator : IDataManipulator
    {
        public CreatePassModel Manipulate(CreatePassModel command)
        {
            if (command !=null && command.PassSalesAreaPriorities != null)
            {
                command.PassSalesAreaPriorities.StartDate = null;
                command.PassSalesAreaPriorities.EndDate = null;
            }
            return command;
        }

        public CreateRunModel Manipulate(CreateRunModel command)
        {
            if (command != null)
            {
                command.StartDate = this.stripTimePart(command.StartDate);
                command.EndDate = this.stripTimePart(command.EndDate);
                if (command.Scenarios != null) {
                    command.Scenarios
                        .Where(scenario => scenario.Passes != null)
                        .AsParallel().ForAll(scenario =>scenario.Passes
                        .Where(pass => pass.PassSalesAreaPriorities != null)
                        .AsParallel().ForAll(pass => {
                            pass.PassSalesAreaPriorities.StartDate = this.stripTimePart(pass.PassSalesAreaPriorities.StartDate);
                            pass.PassSalesAreaPriorities.EndDate = this.stripTimePart(pass.PassSalesAreaPriorities.EndDate);
                        }));
                }
            }
            return command;
        }

        private DateTime stripTimePart(DateTime date)
        {
            return date.Date;
        }

        private DateTime? stripTimePart(DateTime? date)
        {
            if(date.HasValue)
            {
                return this.stripTimePart(date.Value);
            }
            return date;
        }

    }
}
