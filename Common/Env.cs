using System;
using System.Collections.Generic;
namespace Common
{
    public static class Env
    {
        public static string Var(string key)
        {
            var variables = GetAllEnvironmentVariables();
            if (!variables.ContainsKey(key))
            {
                throw new ArgumentException($"No environment variable '{key}' was found");
            } 

            return variables[key];
        }

        public static int VarInt(string key)
        {
            var variables = GetAllEnvironmentVariables();
            if (!variables.ContainsKey(key))
            {
                throw new ArgumentException($"No environment variable '{key}' was found");
            }

            return int.Parse(variables[key]);
        }
        public static Dictionary<string, string> GetAllEnvironmentVariables()
        {
            var dict = new Dictionary<string, string>();
            var userVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
            var machineVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            foreach (dynamic variable in userVariables)
            {
                if (!dict.ContainsKey(variable.Key))
                {
                    dict.Add(variable.Key, variable.Value);
                }
            }

            foreach (dynamic variable in machineVariables)
            {
                if (!dict.ContainsKey(variable.Key))
                {
                    dict.Add(variable.Key, variable.Value);
                }
            }

            return dict;
        }
    }
}
