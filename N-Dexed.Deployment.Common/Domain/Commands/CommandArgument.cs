using System;
using System.Linq;
using System.Collections.Generic;
using N_Dexed.Deployment.Common.Resources;

namespace N_Dexed.Deployment.Common.Domain
{
    public class CommandArgument : IItemInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public int Ordinal { get; set; }

    }

    public static class CommandArgumentExtensions
    {

        public static CommandArgument GetArgument(this List<CommandArgument> commandArguments, string argumentName)
        {
            CommandArgument returnValue = (
                                            from
                                                arguments
                                            in
                                                commandArguments
                                            where
                                                arguments.Name == argumentName
                                            select
                                                arguments
                                          ).FirstOrDefault();

            return returnValue;
        }


        public static CommandArgument GetRequiredArgument(this List<CommandArgument> commandArguments, string argumentName)
        {
            CommandArgument returnValue = GetArgument(commandArguments, argumentName);

            if (returnValue == null)
            {
                string errorMessage = string.Format(ErrorMessages.MissingRequiredArgument, argumentName);
                throw new MissingMemberException(errorMessage);
            }

            if (returnValue.Value == null)
            {
                string errorMessage = string.Format(ErrorMessages.MissingRequiredArgumentValue, argumentName);
                throw new MissingMemberException(errorMessage);
            }

            return returnValue;
        }
    }
}
